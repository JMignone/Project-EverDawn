using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillShot : MonoBehaviour, ICaster, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField]
    private Unit unit;

    [SerializeField]
    private List<GameObject> abilityPrefabs;

    [SerializeField]
    private List<float> abilityDelays;
    private float currentDelay;

    [SerializeField]
    private ApplyResistanceStats applyResistanceStats; //A list of effects than can be set to be resisted while casting

    private bool isFiring;
    private int currentProjectileIndex;
    private Vector3 fireStartPosition;
    private Vector3 fireMousePosition;
    private Vector3 fireDirection;

    [SerializeField]
    private AbilityUI abilityUI;

    [SerializeField]
    private Canvas abilityPreviewCanvas;

    [SerializeField]
    private List<GameObject> abilityPreviews;

    [SerializeField]
    private Sprite abilityPreviewLine;

    [SerializeField]
    private Sprite abilityPreviewLinear;

    [SerializeField]
    private Sprite abilityPreviewBomb;

    [SerializeField]
    private Sprite abilityPreviewRange;

    private bool isDragging;

    private bool abilityControl; //sets it so the skillshot doesnt unset unit casting, but rather the projectile will unset it. Usful for movements like dashes or maybe pulls
    private bool shootRaycast;
    private float maxRange;
    private int areaMask;

    //a flag set if an ability controls when the skillshot will continue. Usage: a unit fires a grappler and will not be pulled forward until the grappler has hit somthing
    private bool pauseFiring;
    //a flag set by an ability to stop continuing the skillshot regardless if there are more abilities to follow
    private bool exitOverride;
    //an amount of abilities to skip starting from the last ability
    private int skipOverride;
    //gives abilities the power to enable targets for the future abilities of the skill shot
    private Actor3D targetOverride;

    public Unit Unit
    {
        get { return unit; }
    }

    public List<GameObject> AbilityPrefabs
    {
        get { return abilityPrefabs; }
    }

    public List<float> AbilityDelays
    {
        get { return abilityDelays; }
        set { abilityDelays = value; }
    }

    public float CurrentDelay
    {
        get { return currentDelay; }
        set { currentDelay = value; }
    }

    public bool IsFiring
    {
        get { return isFiring; }
        set { isFiring = value; }
    }

    public int CurrentProjectileIndex
    {
        get { return currentProjectileIndex; }
        set { currentProjectileIndex = value; }
    }

    public AbilityUI AbilityUI
    {
        get { return abilityUI; }
        //set { abilityUI = value; }
    }

    public bool PauseFiring
    {
        get { return pauseFiring; }
        set { pauseFiring = value; }
    }

    public bool ExitOverride
    {
        get { return exitOverride; }
        set { exitOverride = value; }
    }

    public int SkipOverride
    {
        get { return skipOverride; }
        set { skipOverride = value; }
    }

    void Start() {
        abilityPreviews = new List<GameObject>();
        createAbilityPreviews();
        currentDelay = 0;

        isFiring = false;
        isDragging = false;
        fireMousePosition = new Vector3(-1, -1, -1);

        AbilityUI.StartStats();

        //Below sets up what will be needed for clicking the ability, such that we only need to calculate maxRange and areaMask once.
        maxRange = 0;
        areaMask = 1;
        foreach (GameObject ability in abilityPrefabs) { //this finds the largest range of all the abilitys shot by this skillshot
            Component component = ability.GetComponent(typeof(IAbility));
            if((component as IAbility).AbilityControl)
                abilityControl = true;
            if((component as IAbility).Range > maxRange)
                maxRange = (component as IAbility).Range;
            if((component as IAbility).AreaMask() > areaMask)
                areaMask = (component as IAbility).AreaMask();
            if(ability.GetComponent<Movement>())
                shootRaycast = !ability.GetComponent<Movement>().PassObstacles;
        }
    }

    private void LateUpdate()
    {
        abilityUI.LateUpdateStats();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if(!isDragging && unit.Stats.IsReady && abilityUI.CanDrag) {
            isDragging = true;

            unit.Stats.IsHoveringAbility = true;

            abilityUI.AbilitySprite.enabled = false;
            abilityUI.AbilityCancel.enabled = true;

            foreach(GameObject preview in abilityPreviews) {
                if(preview.CompareTag("Player")) { //this is a summon preview, as its more complicated
                    preview.transform.GetChild(1).GetChild(0).GetComponent<Image>().enabled = true;
                    //preview.transform.GetChild(1).GetChild(0).GetComponent<Collider>().enabled = true;
                }
                else {
                    preview.transform.GetChild(0).GetComponent<Image>().enabled = true;
                    if(preview.GetComponent<Collider>())
                        preview.GetComponent<Collider>().enabled = true;
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Update takes this functions place
    }
    /*
        At the moment, a boomerang that selfdestructs and lingers at the end does not have the preview that id like (the linger circle around the unit wont be there,
        as the selfdestruct location takes precidence). But this kind of projectile is probably unlikely to happen anyway
    */
    private void Update() {
        abilityUI.UpdateStats();
        if (isDragging) {
            Vector3 position = GameFunctions.getPosition(false);
            Vector3 direction = (abilityPreviewCanvas.transform.position - position);
            direction.y = 0;
            position.y = 0;

            Quaternion rotation = Quaternion.LookRotation(direction);
            abilityPreviewCanvas.transform.rotation = Quaternion.Lerp(rotation, abilityPreviewCanvas.transform.rotation, 0f);

            foreach(GameObject preview in abilityPreviews) {
                if(preview.GetComponent<SphereCollider>() || preview.GetComponent<BoxCollider>()) {
                    GameObject go = abilityPrefabs.Find(go => go.name == preview.name);
                    if(go.GetComponent<Movement>())
                        AdjustMovementPreview(preview, go.GetComponent<Movement>(), position, direction);
                    else if(go.GetComponent<Projectile>() && !preview.GetComponent<BoxCollider>() && !preview.GetComponent<MeshCollider>()) //if the preview corresponds to a projectile and doesnt have a box collider. Reason being, all projectiles with a box collider doesnt move away from the unit and only rotates. Projectiles will have other collider if it linngers of selfdestructs at the end
                        AdjustProjectilePreview(preview, go.GetComponent<Projectile>(), position, direction);
                    else if(go.GetComponent<CreateAtLocation>())
                        AdjustCALPreview(preview, go.GetComponent<CreateAtLocation>(), position, direction);
                }
            }
        }
        else if(isFiring)
            Fire();
        else
            fireMousePosition = new Vector3(-1, -1, -1); //this is to reset the mouse position, needed because of special summon location conditions
    }

    public void OnEndDrag(PointerEventData eventData) {
        if(isDragging) {
            isDragging = false;
            unit.Stats.IsHoveringAbility = false;

            abilityUI.AbilitySprite.enabled = true;
            abilityUI.AbilityCancel.enabled = false;

            foreach(GameObject preview in abilityPreviews) {
                if(preview.CompareTag("Player")) { //this is a summon preview, as its more complicated
                    fireMousePosition = preview.transform.GetChild(0).position;
                    preview.transform.GetChild(1).GetChild(0).GetComponent<Image>().enabled = false;
                    //preview.transform.GetChild(1).GetChild(0).GetComponent<Collider>().enabled = false;
                } 
                else {
                    preview.transform.GetChild(0).GetComponent<Image>().enabled = false;
                    if (preview.GetComponent<Collider>())
                        preview.GetComponent<Collider>().enabled = false;
                }
            }

            GameManager.removeAbililtyIndicators();

            if(abilityUI.CardCanvasDim.rect.height < Input.mousePosition.y && unit.Stats.CanAct) {
                fireStartPosition = abilityPreviewCanvas.transform.position;
                if(fireMousePosition == new Vector3(-1, -1, -1)) { //if the ability was not a summon or a movement, get the position
                    fireMousePosition = GameFunctions.getPosition(false);
                    fireMousePosition.y = 0;
                }
                fireStartPosition.y = 0;
                fireDirection = fireMousePosition - fireStartPosition;

                isFiring = true;
                unit.Stats.IsCastingAbility = true;
                unit.SetTarget(null);
                abilityUI.resetAbility();
                applyResistanceStats.ApplyResistance(unit);
            }
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        if(!isDragging && abilityUI.CanDrag && unit.Stats.IsReady) { //if the abililty can be dragged
            Collider[] colliders = Physics.OverlapSphere(unit.Agent.Agent.transform.position, maxRange);
            Component testComponent = abilityPrefabs[0].GetComponent(typeof(IAbility));

            Vector3 closestTargetPosition = new Vector3(-1, -1, -1);
            if(colliders.Length > 0) {
                float closestDistance = 9999;
                float distance;
                foreach(Collider collider in colliders) {
                    if(!collider.CompareTag(abilityPrefabs[0].tag) && collider.name == "Agent") {
                        Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                        if(GameFunctions.WillHit((testComponent as IAbility).HeightAttackable, (testComponent as IAbility).TypeAttackable, damageable)) {
                            distance = Vector3.Distance(unit.Agent.Agent.transform.position, collider.transform.position);
                            if(distance < closestDistance) {
                                closestDistance = distance;
                                closestTargetPosition = collider.transform.position;
                            }
                        }
                    }
                }
            }

            if(closestTargetPosition != new Vector3(-1, -1, -1) && unit.Stats.CanAct) { //if there is a valid target within the max range
                fireStartPosition = abilityPreviewCanvas.transform.position;
                fireStartPosition.y = 0;

                if(areaMask != 1) {
                    UnityEngine.AI.NavMeshHit hit;
                    if(UnityEngine.AI.NavMesh.SamplePosition(closestTargetPosition, out hit, 12f, areaMask))
                        fireMousePosition = hit.position;
                    else
                        fireMousePosition = closestTargetPosition;
                }
                else if(shootRaycast) {
                    UnityEngine.AI.NavMeshHit hit;
                    UnityEngine.AI.NavMesh.Raycast(unit.Agent.Agent.transform.position, closestTargetPosition, out hit, 1);
                    fireMousePosition = hit.position;
                }
                else
                    fireMousePosition = closestTargetPosition;

                fireDirection = fireMousePosition - fireStartPosition;

                isFiring = true;
                unit.Stats.IsCastingAbility = true;
                unit.SetTarget(null);
                abilityUI.resetAbility();
                applyResistanceStats.ApplyResistance(unit);
            }
        }
    }

    private void Fire() {
        if(!unit.Stats.CanAct || exitOverride) {
            isFiring = false;
            exitOverride = false;
            pauseFiring = false;
            targetOverride = null;
            skipOverride = 0;
            currentProjectileIndex = 0;
            currentDelay = 0;
            unit.Stats.IsCastingAbility = false;
        }
        else if(currentDelay < abilityDelays[currentProjectileIndex] || pauseFiring) //if we havnt reached the delay yet
            currentDelay += Time.deltaTime * unit.Stats.EffectStats.SlowedStats.CurrentSlowIntensity;
        else if(currentProjectileIndex == abilityPrefabs.Count - skipOverride) { //if we completed the last delay
            isFiring = false;
            exitOverride = false;
            pauseFiring = false;
            targetOverride = null;
            skipOverride = 0;
            currentProjectileIndex = 0;
            currentDelay = 0;
            if(!abilityControl)
                unit.Stats.IsCastingAbility = false;
        }
        else { //if we completed a delay
            float rangeIncrease = 0;
            Vector3 startPos = fireStartPosition;
            if(fireStartPosition != abilityPreviewCanvas.transform.position) { //if the unit has moved since starting its ability ei: movement abilities, increase/decrease the range of the abilities accordingly
                rangeIncrease = Vector3.Distance(abilityPreviewCanvas.transform.position, fireMousePosition) - Vector3.Distance(fireStartPosition, fireMousePosition);
                startPos = abilityPreviewCanvas.transform.position;
            }

            if(targetOverride != null) {
                if(abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                    GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], startPos, targetOverride, fireDirection, unit, transform.parent.tag, 1, this);
                else if(abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                    GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], startPos, targetOverride, fireDirection, unit, transform.parent.tag, 1, this);
            }
            else {
                if(abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                    GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], startPos, fireMousePosition, fireDirection, unit, transform.parent.tag, 1, rangeIncrease, this);
                else if(abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                    GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], startPos, fireMousePosition, fireDirection, unit, transform.parent.tag, 1, rangeIncrease, this);
            }
            currentDelay = 0;
            currentProjectileIndex++;
        }
    }

    private void AdjustProjectilePreview(GameObject preview, Projectile proj, Vector3 position, Vector3 direction) {
        float range = proj.Range;
        float radius = proj.Radius;

        float distance = Vector3.Distance(position, abilityPreviewCanvas.transform.position);

        bool previewCircleAtEnd = !proj.GrenadeStats.IsGrenade && !proj.SelfDestructStats.SelfDestructs &&
                                  (proj.LingeringStats.Lingering && proj.LingeringStats.LingerAtEnd);
        //if its not a grenade, doesnt selfdestruct, and lingers at the end its true        
        bool previewCircleAtBeginning = previewCircleAtEnd && proj.BoomerangStats.IsBoomerang;

        if(previewCircleAtEnd && !previewCircleAtBeginning)
            position = abilityPreviewCanvas.transform.position + (direction.normalized * (range - radius) * -1); //locks the circle at the furthest position
        else if(previewCircleAtBeginning)
            position = abilityPreviewCanvas.transform.position;
        else if(distance > range - radius) {
            Vector3 distFromRadius = position - abilityPreviewCanvas.transform.position;
            distFromRadius *= (range - radius) / distance;
            position = abilityPreviewCanvas.transform.position + distFromRadius;
        }
        position.y = .1f;
        preview.transform.position = position;
    }

    private void AdjustMovementPreview(GameObject preview, Movement move, Vector3 position, Vector3 direction) {
        float range = move.Range;
        float radius = move.Radius;

        RectTransform previewImageTransform = preview.transform.GetChild(0).GetComponent<RectTransform>();
        float distance = Vector3.Distance(position, abilityPreviewCanvas.transform.position);

        if (preview.GetComponent<BoxCollider>()) { //if the projectile is not a grenade
            if(distance > range - radius) {
                Vector3 distFromRadius = position - abilityPreviewCanvas.transform.position;
                distFromRadius *= (range - radius) / distance;
                position = abilityPreviewCanvas.transform.position + distFromRadius;
                position.y = 0;
            }

            UnityEngine.AI.NavMeshHit hit;
            if(unit.Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING) //if the unit is flying, disregard all obstacles
                position = GameFunctions.adjustForBoundary(position);
            else if(move.PassObstacles) { //if the movment is able to pass through obstacles
                position = GameFunctions.adjustForBoundary(position);
                if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 12f, 9))
                    position = hit.position;

                Vector3 newDirection = abilityPreviewCanvas.transform.position - position;
                newDirection.y = 0;
                Quaternion rotation = Quaternion.LookRotation(newDirection);
                abilityPreviewCanvas.transform.rotation = Quaternion.Lerp(rotation, abilityPreviewCanvas.transform.rotation, 0f);
            }
            else {
                UnityEngine.AI.NavMesh.Raycast(unit.Agent.Agent.transform.position, position, out hit, 1);
                //position = GameFunctions.adjustForTowers(hit.position, radius);
                position = hit.position;
            }
            distance = Vector3.Distance(position, abilityPreviewCanvas.transform.position);

            previewImageTransform.sizeDelta = new Vector2(previewImageTransform.sizeDelta.x, distance);
            previewImageTransform.localPosition = new Vector3(0, 0, -distance / 2);

            preview.GetComponent<BoxCollider>().size = new Vector3(move.Radius * 2, 1, distance);
            preview.GetComponent<BoxCollider>().center = new Vector3(0, 0, -distance / 2);
        }
        else { //else if the projectile is a grenade
            bool previewCircleAtEnd = !move.GrenadeStats.IsGrenade && !move.SelfDestructStats.SelfDestructs &&
                                  (move.LingeringStats.Lingering && move.LingeringStats.LingerAtEnd);
            //if its not a grenade, doesnt selfdestruct, and lingers at the end its true        
            bool previewCircleAtBeginning = previewCircleAtEnd && move.BoomerangStats.IsBoomerang;

            if(previewCircleAtEnd && !previewCircleAtBeginning)
                position = abilityPreviewCanvas.transform.position + (direction.normalized * (range - radius) * -1); //locks the circle at the furthest position
            else if(previewCircleAtBeginning)
                position = abilityPreviewCanvas.transform.position;
            else if(distance > range - radius) {
                Vector3 distFromRadius = position - abilityPreviewCanvas.transform.position;
                distFromRadius *= (range - radius) / distance;
                position = abilityPreviewCanvas.transform.position + distFromRadius;
            }
            position.y = .1f;

            UnityEngine.AI.NavMeshHit hit;
            if(move.PassObstacles) { //if the movment is able to pass through obstacles
                position = GameFunctions.adjustForBoundary(position);
                if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 12f, 9))
                    position = hit.position;
            }
            else {
                UnityEngine.AI.NavMesh.Raycast(unit.Agent.Agent.transform.position, position, out hit, 1);
                //position = GameFunctions.adjustForTowers(hit.position, radius);
                position = hit.position;
            }

            position.y = .1f;
            previewImageTransform.position = position;
        }
        fireMousePosition = position;
    }

    private void AdjustCALPreview(GameObject preview, CreateAtLocation cal, Vector3 position, Vector3 direction) {
        float range = cal.Range;
        float radius = cal.Radius;
        float distance = Vector3.Distance(position, abilityPreviewCanvas.transform.position);

        if(distance > range - radius) {
            Vector3 distFromRadius = position - abilityPreviewCanvas.transform.position;
            distFromRadius *= (range - radius) / distance;
            position = abilityPreviewCanvas.transform.position + distFromRadius;
        }
        position = GameFunctions.adjustForBoundary(position);

        if(cal.SummonStats.IsSummon) { //if its a summon, we dont want the preview to appear in places the summon cant spawn
            position.y = 0;

            int areaMask = cal.SummonStats.AreaMask();

            UnityEngine.AI.NavMeshHit hit;
            if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 12f, areaMask))
                position = hit.position;
            if(preview.transform.childCount > 1)
                preview.transform.GetChild(0).GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(position); //this moves the summon part of the preview
            else
                preview.GetComponent<RectTransform>().position = position; //this moves the other parts, perhaps an explosion around the summon
        }
        else {
            UnityEngine.AI.NavMeshHit hit;
            if(cal.TeleportStats.IsWarp && unit.Stats.MovementType == GameConstants.MOVEMENT_TYPE.GROUND) {
                if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 12f, 9))
                    position = hit.position;
            }

            if(preview.GetComponent<BoxCollider>()) {
                BoxCollider previewCollider = preview.GetComponent<BoxCollider>();
                if(previewCollider.size.x < previewCollider.size.y) //this is the vertical component for a linear cal
                    preview.transform.position = new Vector3(position.x, .1f, 0);
                else if(previewCollider.size.x > previewCollider.size.y) //this is the horizontal component for a linear cal
                    preview.transform.position = new Vector3(0, .1f, position.z);
            }
            else { //these are any other components
                //if the preview in question is NOT the ally radius part of the warp, which requires a sphere collider
                if (!(cal.TeleportStats.IsWarp && cal.TeleportStats.TeleportsAllies && preview.GetComponent<SphereCollider>().radius == cal.TeleportStats.AllyRadius)) {
                    position.y = .1f;
                    preview.transform.position = position;
                }
            }
            preview.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        fireMousePosition = position;
    }

    public void createAbilityPreviews()
    {

        List<GameObject> uniqueProjectiles = new List<GameObject>();

        foreach (GameObject goAbility in abilityPrefabs)
        {
            if (!uniqueProjectiles.Contains(goAbility) && !(goAbility.GetComponent(typeof(IAbility)) as IAbility).HidePreview)
            {
                uniqueProjectiles.Add(goAbility);
                if (goAbility.GetComponent<Projectile>())
                    createProjectilePreview(goAbility);
                else if (goAbility.GetComponent<CreateAtLocation>())
                    createCALPreview(goAbility);
            }
        }
    }

    private void createProjectilePreview(GameObject goProj)
    {
        Projectile projectile = goProj.GetComponent<Projectile>();
        if (!projectile.GrenadeStats.IsGrenade)
        {
            float range = projectile.Range;
            float width = projectile.Radius * 2;

            /* -- Create the base GameObject -- */
            GameObject go = new GameObject(); //Create the GameObject
            go.name = goProj.name;

            if(goProj.GetComponent<Movement>()) {
                if(goProj.GetComponent<Movement>().HighlightEnemies)
                    go.tag = "AbilityHighlight";
                //not all movement abilities need to highlight enemies, so we dont add the tag if it does not
            }
            else
                go.tag = "AbilityHighlight"; 

            /* -- Add the AbilityPreview component  -- */
            AbilityPreview aPrev = go.AddComponent<AbilityPreview>();
            aPrev.HeightAttackable = projectile.HeightAttackable;
            aPrev.TypeAttackable = projectile.TypeAttackable;

            /* -- Creates the Image GameObject and component -- */
            GameObject previewImageGo = new GameObject();
            previewImageGo.name = "Sprite";
            Image previewImage = previewImageGo.AddComponent<Image>(); //Add the Image Component script
            previewImage.color = new Color32(255, 255, 255, 100);
            previewImage.sprite = abilityPreviewLine; //Set the Sprite of the Image Component on the new GameObject
            previewImage.enabled = false;

            RectTransform imageTransform = previewImageGo.GetComponent<RectTransform>();
            imageTransform.anchorMin = new Vector2(.5f, 0);
            imageTransform.anchorMax = new Vector2(.5f, 0);
            imageTransform.pivot = new Vector2(.5f, .5f);
            imageTransform.SetParent(go.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            imageTransform.localPosition = new Vector3(0, 0, -1 * range / 2);
            imageTransform.localRotation = Quaternion.Euler(270, 0, 0);
            imageTransform.sizeDelta = new Vector2(width, range);

            /* -- Creates the Collider component -- */
            if(projectile.CustomPathStats.HasCustomPath) {
                previewImage.sprite = projectile.CustomPathStats.CustomImage; //Set the Sprite of the Image Component on the new GameObject
                previewImage.preserveAspect = true;
                imageTransform.localRotation = Quaternion.Euler(270, 180, 0);
                imageTransform.pivot = projectile.CustomPathStats.Pivot;
                imageTransform.sizeDelta = projectile.CustomPathStats.SizeDelta;
                //imageTransform.pivot = new Vector2(.92f, .525f);
                //imageTransform.sizeDelta = new Vector2(30f, 31f);

                MeshCollider previewHitBox = go.AddComponent<MeshCollider>();
                previewHitBox.sharedMesh = projectile.CustomPathStats.CustomCollider;
                previewHitBox.enabled = false;
            }
            else {
                BoxCollider previewHitBox = go.AddComponent<BoxCollider>();
                previewHitBox.size = new Vector3(width, 1, range);
                previewHitBox.center = new Vector3(0, 0, -1 * range / 2);
                previewHitBox.enabled = false;
            }

            /* -- Adjust the GameObjects transform, directly affecting the collider -- */
            go.transform.SetParent(abilityPreviewCanvas.transform); 
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);
            go.SetActive(true);
            abilityPreviews.Add(go);
        }

        if(projectile.SelfDestructStats.SelfDestructs || projectile.GrenadeStats.IsGrenade ||
          (projectile.LingeringStats.Lingering && projectile.LingeringStats.LingerAtEnd)) { //we must also add the blow up range
            float radius;
            if(projectile.SelfDestructStats.SelfDestructs)
                radius = projectile.SelfDestructStats.ExplosionRadius;
            else if(projectile.GrenadeStats.IsGrenade)
                radius = projectile.GrenadeStats.ExplosionRadius;
            else
                radius = projectile.LingeringStats.LingeringRadius;

            /* -- Create the base GameObject -- */
            GameObject goBoom = new GameObject();
            goBoom.name = goProj.name;
            goBoom.tag = "AbilityHighlight";

            /* -- Add the AbilityPreview component  -- */
            AbilityPreview aPrev = goBoom.AddComponent<AbilityPreview>();
            aPrev.HeightAttackable = projectile.HeightAttackable;
            aPrev.TypeAttackable = projectile.TypeAttackable;

            /* -- Creates the Image GameObject and component -- */
            GameObject previewImageGo = new GameObject();
            previewImageGo.name = "Sprite";
            Image previewImage = previewImageGo.AddComponent<Image>(); //Add the Image Component script
            previewImage.color = new Color32(255, 255, 255, 100);
            previewImage.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
            previewImage.enabled = false;

            RectTransform imageTransform = previewImageGo.GetComponent<RectTransform>();
            imageTransform.anchorMin = new Vector2(.5f, 0);
            imageTransform.anchorMax = new Vector2(.5f, 0);
            imageTransform.pivot = new Vector2(.5f, .5f);
            imageTransform.SetParent(goBoom.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            imageTransform.localPosition = new Vector3(0, 0, 0);
            imageTransform.localRotation = Quaternion.Euler(270, 0, 0);
            imageTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

            /* -- Creates the Collider component -- */
            SphereCollider previewHitBoxBoom = goBoom.AddComponent<SphereCollider>();
            previewHitBoxBoom.radius = radius;
            previewHitBoxBoom.center = new Vector3(0, 0, 0);
            previewHitBoxBoom.enabled = false;

             /* -- Adjust the GameObjects transform, directly affecting the collider -- */
            goBoom.transform.SetParent(abilityPreviewCanvas.transform); 
            goBoom.transform.localPosition = Vector3.zero;
            goBoom.transform.localRotation = Quaternion.Euler(Vector3.zero);
            goBoom.SetActive(true);
            abilityPreviews.Add(goBoom);

            /* ----- Add the range circle ----- */
            GameObject goBoomRange = new GameObject();
            goBoomRange.name = goProj.name;

            /* -- Creates the Image GameObject and component -- */
            GameObject previewRangeImageGo = new GameObject();
            previewRangeImageGo.name = "Sprite";
            Image previewRangeImage = previewRangeImageGo.AddComponent<Image>(); //Add the Image Component script
            previewRangeImage.color = new Color32(255, 255, 255, 100);
            previewRangeImage.sprite = abilityPreviewRange; //Set the Sprite of the Image Component on the new GameObject
            previewRangeImage.enabled = false;

            RectTransform imageRangeTransform = previewRangeImageGo.GetComponent<RectTransform>();
            imageRangeTransform.anchorMin = new Vector2(.5f, 0);
            imageRangeTransform.anchorMax = new Vector2(.5f, 0);
            imageRangeTransform.pivot = new Vector2(.5f, .5f);
            imageRangeTransform.SetParent(goBoomRange.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            imageRangeTransform.localPosition = Vector3.zero;
            imageRangeTransform.localRotation = Quaternion.Euler(270, 0, 0);
            imageRangeTransform.sizeDelta = new Vector2(projectile.Range * 2, projectile.Range * 2);

            goBoomRange.transform.SetParent(abilityPreviewCanvas.transform);
            goBoomRange.transform.localPosition = Vector3.zero;
            goBoomRange.transform.localRotation = Quaternion.Euler(Vector3.zero);
            goBoomRange.SetActive(true);
            abilityPreviews.Add(goBoomRange);
        }
    }

    private void createCALPreview(GameObject goCAL)
    {
        CreateAtLocation cal = goCAL.GetComponent<CreateAtLocation>();

        if(cal.SummonStats.IsSummon) {
            /* -- Create the base GameObject -- */
            GameObject go = Instantiate(cal.SummonStats.SummonPreview);
            go.name = goCAL.name;

            /* -- Obtain the preset components of the summon preview -- */
            Image previewImageBoom = go.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            RectTransform previewBoomTransform = go.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
            UnityEngine.AI.NavMeshAgent previewAgent = go.transform.GetChild(0).GetComponent<UnityEngine.AI.NavMeshAgent>();

            /* -- Determine the correct radius and give the agent type the corresponding id  -- */
            float radius;
            if(cal.SummonStats.Size == GameConstants.SUMMON_SIZE.BIG) {
                radius = 6;
                previewAgent.agentTypeID = 287145453;
            }
            else if(cal.SummonStats.Size == GameConstants.SUMMON_SIZE.SMALL) {
                radius = 3;
                previewAgent.agentTypeID = -902729914;
            }
            else {
                radius = 3;
                previewAgent.agentTypeID = 0;
            }

            /* -- Adjust the final values and add it to the scene  -- */
            previewImageBoom.enabled = false;
            previewBoomTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

            go.transform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.

            go.SetActive(true);
            abilityPreviews.Add(go);
        }
        if(cal.LinearStats.IsLinear) {
            float width = cal.LinearStats.ExplosionWidth;

            if(cal.LinearStats.IsVertical || (!cal.LinearStats.IsVertical && !cal.LinearStats.IsHorizontal)) { //if the linear attack has the vertical component
                /* -- Create the base GameObject -- */
                GameObject goLinearVert = new GameObject();
                goLinearVert.name = goCAL.name;
                goLinearVert.tag = "AbilityHighlight";

                /* -- Add the AbilityPreview component  -- */
                AbilityPreview aPrev = goLinearVert.AddComponent<AbilityPreview>();
                aPrev.HeightAttackable = cal.HeightAttackable;
                aPrev.TypeAttackable = cal.TypeAttackable;

                /* -- Creates the Image GameObject and component -- */
                GameObject previewImageLinearVertGo = new GameObject();
                previewImageLinearVertGo.name = "Sprite";
                Image previewLinearVertImage = previewImageLinearVertGo.AddComponent<Image>(); //Add the Image Component script
                previewLinearVertImage.color = new Color32(255, 255, 255, 100);
                previewLinearVertImage.sprite = abilityPreviewLinear; //Set the Sprite of the Image Component on the new GameObject
                previewLinearVertImage.enabled = false;

                RectTransform imageLinearVertTransform = previewImageLinearVertGo.GetComponent<RectTransform>();
                imageLinearVertTransform.anchorMin = new Vector2(.5f, 0);
                imageLinearVertTransform.anchorMax = new Vector2(.5f, 0);
                imageLinearVertTransform.pivot = new Vector2(.5f, .5f);
                imageLinearVertTransform.SetParent(goLinearVert.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
                imageLinearVertTransform.localPosition = Vector3.zero;
                imageLinearVertTransform.localRotation = Quaternion.Euler(Vector3.zero);
                imageLinearVertTransform.sizeDelta = new Vector2(width, GameManager.Instance.Ground.transform.localScale.z * 10);

                BoxCollider previewHitBoxLinearVert = goLinearVert.AddComponent<BoxCollider>();
                previewHitBoxLinearVert.size = new Vector3(width, GameManager.Instance.Ground.transform.localScale.z * 10, .5f);
                previewHitBoxLinearVert.center = Vector3.zero;
                previewHitBoxLinearVert.enabled = false;

                /* -- Adjust the GameObjects transform, directly affecting the collider -- */
                goLinearVert.transform.SetParent(abilityPreviewCanvas.transform); 
                goLinearVert.transform.localPosition = Vector3.zero;
                goLinearVert.transform.localRotation = Quaternion.Euler(Vector3.zero);
                goLinearVert.SetActive(true);
                abilityPreviews.Add(goLinearVert);
            }
            if(cal.LinearStats.IsHorizontal) { //if the linear attack has the horizontal component
                /* -- Create the base GameObject -- */
                GameObject goLinearHorz = new GameObject();
                goLinearHorz.name = goCAL.name;
                goLinearHorz.tag = "AbilityHighlight";

                /* -- Add the AbilityPreview component  -- */
                AbilityPreview aPrev = goLinearHorz.AddComponent<AbilityPreview>();
                aPrev.HeightAttackable = cal.HeightAttackable;
                aPrev.TypeAttackable = cal.TypeAttackable;

                /* -- Creates the Image GameObject and component -- */
                GameObject previewImageLinearHorzGo = new GameObject();
                previewImageLinearHorzGo.name = "Sprite";
                Image previewLinearHorzImage = previewImageLinearHorzGo.AddComponent<Image>(); //Add the Image Component script
                previewLinearHorzImage.color = new Color32(255, 255, 255, 100);
                previewLinearHorzImage.sprite = abilityPreviewLinear; //Set the Sprite of the Image Component on the new GameObject
                previewLinearHorzImage.enabled = false;

                RectTransform imageLinearHorzTransform = previewImageLinearHorzGo.GetComponent<RectTransform>();
                imageLinearHorzTransform.anchorMin = new Vector2(.5f, 0);
                imageLinearHorzTransform.anchorMax = new Vector2(.5f, 0);
                imageLinearHorzTransform.pivot = new Vector2(.5f, .5f);
                imageLinearHorzTransform.SetParent(goLinearHorz.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
                imageLinearHorzTransform.localPosition = Vector3.zero;
                imageLinearHorzTransform.localRotation = Quaternion.Euler(Vector3.zero);
                imageLinearHorzTransform.sizeDelta = new Vector2(GameManager.Instance.Ground.transform.localScale.x * 10, width);

                BoxCollider previewHitBoxLinearHorz = goLinearHorz.AddComponent<BoxCollider>();
                previewHitBoxLinearHorz.size = new Vector3(GameManager.Instance.Ground.transform.localScale.x * 10, width, .5f);
                previewHitBoxLinearHorz.center = Vector3.zero;
                previewHitBoxLinearHorz.enabled = false;

                /* -- Adjust the GameObjects transform, directly affecting the collider -- */
                goLinearHorz.transform.SetParent(abilityPreviewCanvas.transform); 
                goLinearHorz.transform.localPosition = Vector3.zero;
                goLinearHorz.transform.localRotation = Quaternion.Euler(Vector3.zero);
                goLinearHorz.SetActive(true);
                abilityPreviews.Add(goLinearHorz);
            }
        }

        if(cal.LingeringStats.Lingering || cal.SelfDestructStats.SelfDestructs) {
            float radius = Math.Max(cal.LingeringStats.LingeringRadius, cal.SelfDestructStats.ExplosionRadius);

            /* -- Create the base GameObject -- */
            GameObject goBoom = new GameObject();
            goBoom.name = goCAL.name;
            goBoom.tag = "AbilityHighlight";

            /* -- Add the AbilityPreview component  -- */
            AbilityPreview aPrev = goBoom.AddComponent<AbilityPreview>();
            aPrev.HeightAttackable = cal.HeightAttackable;
            aPrev.TypeAttackable = cal.TypeAttackable;

            /* -- Creates the Image GameObject and component -- */
            GameObject previewImageGo = new GameObject();
            previewImageGo.name = "Sprite";
            Image previewImage = previewImageGo.AddComponent<Image>(); //Add the Image Component script
            previewImage.color = new Color32(255, 255, 255, 100);
            previewImage.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
            previewImage.enabled = false;

            RectTransform imageTransform = previewImageGo.GetComponent<RectTransform>();
            imageTransform.anchorMin = new Vector2(.5f, 0);
            imageTransform.anchorMax = new Vector2(.5f, 0);
            imageTransform.pivot = new Vector2(.5f, .5f);
            imageTransform.SetParent(goBoom.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            imageTransform.localPosition = Vector3.zero;
            imageTransform.localRotation = Quaternion.Euler(Vector3.zero);
            imageTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

            /* -- Creates the Collider component -- */
            SphereCollider previewHitBoxBoom = goBoom.AddComponent<SphereCollider>();
            previewHitBoxBoom.radius = radius;
            previewHitBoxBoom.center = new Vector3(0, 0, 0);
            previewHitBoxBoom.enabled = false;

             /* -- Adjust the GameObjects transform, directly affecting the collider -- */
            goBoom.transform.SetParent(abilityPreviewCanvas.transform); 
            goBoom.transform.localPosition = Vector3.zero;
            goBoom.transform.localRotation = Quaternion.Euler(Vector3.zero);
            goBoom.SetActive(true);
            abilityPreviews.Add(goBoom);
        }

        if(cal.TeleportStats.IsWarp) {
            float radius = unit.Agent.Agent.radius;

            /* -- Create the base GameObject -- */
            GameObject goBoom = new GameObject();
            goBoom.name = goCAL.name;
            goBoom.tag = "AbilityHighlight";

            /* -- Add the AbilityPreview component  -- */
            AbilityPreview aPrev = goBoom.AddComponent<AbilityPreview>();
            aPrev.HeightAttackable = cal.HeightAttackable;
            aPrev.TypeAttackable = cal.TypeAttackable;

            /* -- Creates the Image GameObject and component -- */
            GameObject previewImageGo = new GameObject();
            previewImageGo.name = "Sprite";
            Image previewImage = previewImageGo.AddComponent<Image>(); //Add the Image Component script
            previewImage.color = new Color32(255, 255, 255, 100);
            previewImage.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
            previewImage.enabled = false;

            RectTransform imageTransform = previewImageGo.GetComponent<RectTransform>();
            imageTransform.anchorMin = new Vector2(.5f, 0);
            imageTransform.anchorMax = new Vector2(.5f, 0);
            imageTransform.pivot = new Vector2(.5f, .5f);
            imageTransform.SetParent(goBoom.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            imageTransform.localPosition = new Vector3(0, 0, 0);
            imageTransform.localRotation = Quaternion.Euler(Vector3.zero);
            imageTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

            /* -- Creates the Collider component -- */
            SphereCollider previewHitBoxBoom = goBoom.AddComponent<SphereCollider>();
            previewHitBoxBoom.radius = radius;
            previewHitBoxBoom.center = new Vector3(0, 0, 0);
            previewHitBoxBoom.enabled = false;

             /* -- Adjust the GameObjects transform, directly affecting the collider -- */
            goBoom.transform.SetParent(abilityPreviewCanvas.transform); 
            goBoom.transform.localPosition = Vector3.zero;
            goBoom.transform.localRotation = Quaternion.Euler(Vector3.zero);
            goBoom.SetActive(true);
            abilityPreviews.Add(goBoom);

            if(cal.TeleportStats.TeleportsAllies) {
                radius = cal.TeleportStats.AllyRadius;

                /* -- Create the base GameObject -- */
                GameObject goAWarp = new GameObject();
                goAWarp.name = goCAL.name;
                goAWarp.tag = "FriendlyAbilityHighlight";

                /* -- Add the AbilityPreview component  -- */
                AbilityPreview aAWarpPrev = goAWarp.AddComponent<AbilityPreview>();
                aAWarpPrev.HeightAttackable = cal.HeightAttackable;
                aAWarpPrev.TypeAttackable = cal.TypeAttackable;

                /* -- Creates the Image GameObject and component -- */
                GameObject previewAWarpImageGo = new GameObject();
                previewAWarpImageGo.name = "Sprite";
                Image previewAWarpImage = previewAWarpImageGo.AddComponent<Image>(); //Add the Image Component script
                previewAWarpImage.color = new Color32(255, 255, 255, 100);
                previewAWarpImage.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
                previewAWarpImage.enabled = false;

                RectTransform imageAWarpTransform = previewAWarpImageGo.GetComponent<RectTransform>();
                imageAWarpTransform.anchorMin = new Vector2(.5f, 0);
                imageAWarpTransform.anchorMax = new Vector2(.5f, 0);
                imageAWarpTransform.pivot = new Vector2(.5f, .5f);
                imageAWarpTransform.SetParent(goAWarp.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
                imageAWarpTransform.localPosition = new Vector3(0, 0, 0);
                imageAWarpTransform.localRotation = Quaternion.Euler(Vector3.zero);
                imageAWarpTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

                /* -- Creates the Collider component -- */
                SphereCollider previewHitBoxAWarp = goAWarp.AddComponent<SphereCollider>();
                previewHitBoxAWarp.radius = radius;
                previewHitBoxAWarp.center = new Vector3(0, 0, 0);
                previewHitBoxAWarp.enabled = false;

                /* -- Adjust the GameObjects transform, directly affecting the collider -- */
                goAWarp.transform.SetParent(abilityPreviewCanvas.transform); 
                goAWarp.transform.localPosition = Vector3.zero;
                goAWarp.transform.localRotation = Quaternion.Euler(Vector3.zero);
                goAWarp.SetActive(true);
                abilityPreviews.Add(goAWarp);
            }
        }

        /* ----- Add the range circle ----- */
        GameObject goBoomRange = new GameObject();
        goBoomRange.name = goCAL.name;

        /* -- Creates the Image GameObject and component -- */
        GameObject previewRangeImageGo = new GameObject();
        previewRangeImageGo.name = "Sprite";
        Image previewRangeImage = previewRangeImageGo.AddComponent<Image>(); //Add the Image Component script
        previewRangeImage.color = new Color32(255, 255, 255, 100);
        previewRangeImage.sprite = abilityPreviewRange; //Set the Sprite of the Image Component on the new GameObject
        previewRangeImage.enabled = false;

        RectTransform imageRangeTransform = previewRangeImageGo.GetComponent<RectTransform>();
        imageRangeTransform.anchorMin = new Vector2(.5f, 0);
        imageRangeTransform.anchorMax = new Vector2(.5f, 0);
        imageRangeTransform.pivot = new Vector2(.5f, .5f);
        imageRangeTransform.SetParent(goBoomRange.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
        imageRangeTransform.localPosition = Vector3.zero;
        imageRangeTransform.localRotation = Quaternion.Euler(270, 0, 0);
        imageRangeTransform.sizeDelta = new Vector2(cal.Range * 2, cal.Range * 2);

        goBoomRange.transform.SetParent(abilityPreviewCanvas.transform);
        goBoomRange.transform.localPosition = Vector3.zero;
        goBoomRange.transform.localRotation = Quaternion.Euler(Vector3.zero);
        goBoomRange.SetActive(true);
        abilityPreviews.Add(goBoomRange);
    }

    public void SetNewLocation(Vector3 newLocation, Vector3 newDirection) {
        fireMousePosition = newLocation;
        fireDirection = newDirection;
    }

    public void SetNewTarget(Actor3D newTarget) {
        targetOverride = newTarget;
    }

}
