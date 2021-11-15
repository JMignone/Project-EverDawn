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
    private bool exitOveride;
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

    public bool ExitOveride
    {
        get { return exitOveride; }
        set { exitOveride = value; }
    }

    void Start()
    {
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
        foreach (GameObject ability in abilityPrefabs)
        { //this finds the largest range of all the abilitys shot by this skillshot
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDragging && unit.Stats.IsReady && abilityUI.CanDrag)
        {
            isDragging = true;

            unit.Stats.IsHoveringAbility = true;

            abilityUI.AbilitySprite.enabled = false;
            abilityUI.AbilityCancel.enabled = true;

            foreach (GameObject preview in abilityPreviews)
            {
                if (preview.CompareTag("Player"))
                { //this is a summon preview, as its more complicated
                    preview.transform.GetChild(1).GetChild(0).GetComponent<Image>().enabled = true;
                    //preview.transform.GetChild(1).GetChild(0).GetComponent<Collider>().enabled = true;
                }
                else
                {
                    preview.GetComponent<Image>().enabled = true;
                    if (preview.GetComponent<Collider>())
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
    private void Update()
    {
        abilityUI.UpdateStats();
        if (isDragging)
        {
            Vector3 position = GameFunctions.getPosition(false);
            Vector3 direction = abilityPreviewCanvas.transform.position - position;
            direction.y = 0;
            position.y = 1;

            Quaternion rotation = Quaternion.LookRotation(direction);
            abilityPreviewCanvas.transform.rotation = Quaternion.Lerp(rotation, abilityPreviewCanvas.transform.rotation, 0f);

            foreach (GameObject preview in abilityPreviews)
            {
                if (preview.GetComponent<SphereCollider>() || preview.GetComponent<BoxCollider>())
                {
                    GameObject go = abilityPrefabs.Find(go => go.name == preview.name);
                    if (go.GetComponent<Movement>())
                        AdjustMovementPreview(preview, go.GetComponent<Movement>(), position, direction);
                    else if (go.GetComponent<Projectile>() && !preview.GetComponent<BoxCollider>()) //if the preview corresponds to a projectile and doesnt have a box collider. Reason being, all projectiles with a box collider doesnt move away from the unit and only rotates
                        AdjustProjectilePreview(preview, go.GetComponent<Projectile>(), position, direction);
                    else if (go.GetComponent<CreateAtLocation>())
                    {
                        AdjustCALPreview(preview, go.GetComponent<CreateAtLocation>(), position, direction);
                    }
                }
            }
        }
        else if (isFiring)
            Fire();
        else
            fireMousePosition = new Vector3(-1, -1, -1); //this is to reset the mouse position, needed because of special summon location conditions
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(isDragging) {
            isDragging = false;
            unit.Stats.IsHoveringAbility = false;

            abilityUI.AbilitySprite.enabled = true;
            abilityUI.AbilityCancel.enabled = false;

            foreach (GameObject preview in abilityPreviews)
            {
                if (preview.CompareTag("Player"))
                { //this is a summon preview, as its more complicated
                    fireMousePosition = preview.transform.GetChild(0).position;
                    preview.transform.GetChild(1).GetChild(0).GetComponent<Image>().enabled = false;
                    //preview.transform.GetChild(1).GetChild(0).GetComponent<Collider>().enabled = false;
                }
                else
                {
                    preview.GetComponent<Image>().enabled = false;
                    if (preview.GetComponent<Collider>())
                        preview.GetComponent<Collider>().enabled = false;
                }
            }

            GameManager.removeAbililtyIndicators();

            if (abilityUI.CardCanvasDim.rect.height < Input.mousePosition.y && unit.Stats.CanAct)
            {
                fireStartPosition = abilityPreviewCanvas.transform.position;
                if (fireMousePosition == new Vector3(-1, -1, -1))
                { //if the ability was not a summon or a movement, get the position
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

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (!isDragging && abilityUI.CanDrag && unit.Stats.IsReady)
        { //if the abililty can be dragged
            Collider[] colliders = Physics.OverlapSphere(unit.Agent.Agent.transform.position, maxRange);
            Component testComponent = abilityPrefabs[0].GetComponent(typeof(IAbility));

            Vector3 closestTargetPosition = new Vector3(-1, -1, -1);
            if (colliders.Length > 0)
            {
                float closestDistance = 9999;
                float distance;
                foreach (Collider collider in colliders)
                {
                    if (!collider.CompareTag(abilityPrefabs[0].tag) && collider.name == "Agent")
                    {
                        Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                        if (GameFunctions.WillHit((testComponent as IAbility).HeightAttackable, (testComponent as IAbility).TypeAttackable, damageable))
                        {
                            distance = Vector3.Distance(unit.Agent.Agent.transform.position, collider.transform.position);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestTargetPosition = collider.transform.position;
                            }
                        }
                    }
                }
            }

            if (closestTargetPosition != new Vector3(-1, -1, -1) && unit.Stats.CanAct)
            { //if there is a valid target within the max range
                fireStartPosition = abilityPreviewCanvas.transform.position;
                fireStartPosition.y = 0;

                if (areaMask != 1)
                {
                    UnityEngine.AI.NavMeshHit hit;
                    if (UnityEngine.AI.NavMesh.SamplePosition(closestTargetPosition, out hit, 12f, areaMask))
                        fireMousePosition = hit.position;
                    else
                        fireMousePosition = closestTargetPosition;
                }
                else if (shootRaycast)
                {
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

    private void Fire()
    {
        if(!unit.Stats.CanAct || exitOveride) {
            isFiring = false;
            exitOveride = false;
            pauseFiring = false;
            targetOverride = null;
            currentProjectileIndex = 0;
            currentDelay = 0;
            unit.Stats.IsCastingAbility = false;
        }
        else if(currentDelay < abilityDelays[currentProjectileIndex] || pauseFiring) //if we havnt reached the delay yet
            currentDelay += Time.deltaTime * unit.Stats.EffectStats.SlowedStats.CurrentSlowIntensity;
        else if(currentProjectileIndex == abilityPrefabs.Count) { //if we completed the last delay
            isFiring = false;
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
            position.y = 1;
        }
        preview.GetComponent<RectTransform>().position = position;
    }

    private void AdjustMovementPreview(GameObject preview, Movement move, Vector3 position, Vector3 direction) {
        float range = move.Range;
        float radius = move.Radius;

        RectTransform previewTransform = preview.GetComponent<RectTransform>();
        float distance = Vector3.Distance(position, abilityPreviewCanvas.transform.position);

        if (preview.GetComponent<BoxCollider>()) { //if the projectile is not a grenade
            if(distance > range - radius) {
                Vector3 distFromRadius = position - abilityPreviewCanvas.transform.position;
                distFromRadius *= (range - radius) / distance;
                position = abilityPreviewCanvas.transform.position + distFromRadius;
                position.y = 1;
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

            previewTransform.sizeDelta = new Vector2(previewTransform.sizeDelta.x, distance);
            previewTransform.localPosition = new Vector3(0, 0, -distance / 2);

            preview.GetComponent<BoxCollider>().size = new Vector3(move.Radius * 2, distance, 1);
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
                position.y = 1;
            }

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

            previewTransform.position = position;
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
            RectTransform previewRect = preview.GetComponent<RectTransform>();

            UnityEngine.AI.NavMeshHit hit;
            if (cal.TeleportStats.IsWarp && unit.Stats.MovementType == GameConstants.MOVEMENT_TYPE.GROUND)
            {
                if (UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 12f, 9))
                    position = hit.position;
            }

            previewRect.rotation = Quaternion.Euler(90f, 0f, 0f);
            if (previewRect.sizeDelta.x < previewRect.sizeDelta.y) //this is the vertical component for a linear cal
                previewRect.position = new Vector3(position.x, 1, 0);
            else if (previewRect.sizeDelta.x > previewRect.sizeDelta.y) //this is the horizontal component for a linear cal
                previewRect.position = new Vector3(0, 1, position.z);
            else
            { //these are any other components
                //if the preview in question is NOT the ally radius part of the warp, which requires a sphere collider
                if (!(cal.TeleportStats.IsWarp && cal.TeleportStats.TeleportsAllies && preview.GetComponent<SphereCollider>().radius == cal.TeleportStats.AllyRadius))
                {
                    position.y = 1;
                    previewRect.position = position;
                }
            }
        }

        /*
        else if(cal.LinearStats.IsLinear) {
            RectTransform previewRect = preview.GetComponent<RectTransform>();
            previewRect.rotation = Quaternion.Euler(90f, 0f, 0f);
            if(previewRect.sizeDelta.x < previewRect.sizeDelta.y) //this is the vertical component
                previewRect.position = new Vector3(position.x, 1, 0); 
            else if(previewRect.sizeDelta.x > previewRect.sizeDelta.y) //this is the horizontal component
                previewRect.position = new Vector3(0, 1, position.z); 
            else { //these are any other components
                position.y = 1;
                previewRect.position = position;
            }
        }
        else {
            position.y = 1;
            preview.GetComponent<RectTransform>().position = position;
        }*/
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

            GameObject go = new GameObject(); //Create the GameObject
            go.name = goProj.name;

            AbilityPreview aPrev = go.AddComponent<AbilityPreview>();
            aPrev.HeightAttackable = projectile.HeightAttackable;
            aPrev.TypeAttackable = projectile.TypeAttackable;

            Image previewImage = go.AddComponent<Image>(); //Add the Image Component script
            previewImage.color = new Color32(255, 255, 255, 100);
            previewImage.sprite = abilityPreviewLine; //Set the Sprite of the Image Component on the new GameObject
            previewImage.enabled = false;

            float range = projectile.Range;
            float width = projectile.Radius * 2;

            BoxCollider previewHitBox = go.AddComponent<BoxCollider>();
            previewHitBox.size = new Vector3(width, range, 1);
            previewHitBox.center = new Vector3(0, 0, 0);
            previewHitBox.enabled = false;

            //not all movement abilities need to highlight enemies
            if (goProj.GetComponent<Movement>())
            {
                if (goProj.GetComponent<Movement>().HighlightEnemies)
                    go.tag = "AbilityHighlight";
            }
            else
                go.tag = "AbilityHighlight";

            RectTransform previewTransform = go.GetComponent<RectTransform>();
            previewTransform.anchorMin = new Vector2(.5f, 0);
            previewTransform.anchorMax = new Vector2(.5f, 0);
            previewTransform.pivot = new Vector2(.5f, .5f);
            previewTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            previewTransform.localPosition = new Vector3(0, 0, -1 * range / 2);
            previewTransform.localRotation = Quaternion.Euler(270, 0, 0);
            previewTransform.sizeDelta = new Vector2(width, range);

            go.SetActive(true);
            abilityPreviews.Add(go);
        }

        if (projectile.SelfDestructStats.SelfDestructs || projectile.GrenadeStats.IsGrenade ||
          (projectile.LingeringStats.Lingering && projectile.LingeringStats.LingerAtEnd))
        { //we must also add the blow up range
            GameObject goBoom = new GameObject(); //Create the GameObject
            goBoom.name = goProj.name;

            AbilityPreview aPrev = goBoom.AddComponent<AbilityPreview>();
            aPrev.HeightAttackable = projectile.HeightAttackable;
            aPrev.TypeAttackable = projectile.TypeAttackable;

            Image previewImageBoom = goBoom.AddComponent<Image>(); //Add the Image Component script
            previewImageBoom.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
            previewImageBoom.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
            previewImageBoom.enabled = false;

            float radius;
            if (projectile.SelfDestructStats.SelfDestructs)
                radius = projectile.SelfDestructStats.ExplosionRadius;
            else if (projectile.GrenadeStats.IsGrenade)
                radius = projectile.GrenadeStats.ExplosionRadius;
            else
                radius = projectile.LingeringStats.LingeringRadius;

            SphereCollider previewHitBoxBoom = goBoom.AddComponent<SphereCollider>();
            previewHitBoxBoom.radius = radius;
            previewHitBoxBoom.center = new Vector3(0, 0, 0);
            previewHitBoxBoom.enabled = false;

            goBoom.tag = "AbilityHighlight";
            RectTransform previewBoomTransform = goBoom.GetComponent<RectTransform>();
            previewBoomTransform.anchorMin = new Vector2(.5f, 0);
            previewBoomTransform.anchorMax = new Vector2(.5f, 0);
            previewBoomTransform.pivot = new Vector2(.5f, .5f);
            previewBoomTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            previewBoomTransform.localPosition = new Vector3(0, 0, 0);
            previewBoomTransform.localRotation = Quaternion.Euler(270, 0, 0);
            previewBoomTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

            goBoom.SetActive(true);
            abilityPreviews.Add(goBoom);


            //now add the range circle
            GameObject goBoomRange = new GameObject();
            goBoomRange.name = goProj.name;

            Image previewImageRange = goBoomRange.AddComponent<Image>();
            previewImageRange.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
            previewImageRange.sprite = abilityPreviewRange;
            previewImageRange.enabled = false;

            RectTransform previewBoomRangeTransform = goBoomRange.GetComponent<RectTransform>();
            previewBoomRangeTransform.anchorMin = new Vector2(.5f, 0);
            previewBoomRangeTransform.anchorMax = new Vector2(.5f, 0);
            previewBoomRangeTransform.pivot = new Vector2(.5f, .5f);
            previewBoomRangeTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            previewBoomRangeTransform.localPosition = new Vector3(0, 0, 0);
            previewBoomRangeTransform.localRotation = Quaternion.Euler(270, 0, 0);
            previewBoomRangeTransform.sizeDelta = new Vector2(projectile.Range * 2, projectile.Range * 2);

            goBoomRange.SetActive(true);
            abilityPreviews.Add(goBoomRange);
        }
    }

    private void createCALPreview(GameObject goCAL)
    {
        CreateAtLocation cal = goCAL.GetComponent<CreateAtLocation>();

        if (cal.SummonStats.IsSummon)
        {
            GameObject go = Instantiate(cal.SummonStats.SummonPreview);
            go.name = goCAL.name;

            Image previewImageBoom = go.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            RectTransform previewBoomTransform = go.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
            UnityEngine.AI.NavMeshAgent previewAgent = go.transform.GetChild(0).GetComponent<UnityEngine.AI.NavMeshAgent>();

            float radius;
            if (cal.SummonStats.Size == GameConstants.SUMMON_SIZE.BIG)
            {
                radius = 6;
                previewAgent.agentTypeID = 287145453;
            }
            else if (cal.SummonStats.Size == GameConstants.SUMMON_SIZE.SMALL)
            {
                radius = 3;
                previewAgent.agentTypeID = -902729914;
            }
            else
            {
                radius = 3;
                previewAgent.agentTypeID = 0;
            }

            previewImageBoom.enabled = false;
            previewBoomTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

            go.transform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.

            go.SetActive(true);
            abilityPreviews.Add(go);
        }
        if (cal.LinearStats.IsLinear)
        {
            float width = cal.LinearStats.ExplosionWidth;

            if (cal.LinearStats.IsVertical || (!cal.LinearStats.IsVertical && !cal.LinearStats.IsHorizontal))
            {//if the linear attack has the vertical component
                GameObject goLinearVert = new GameObject();
                goLinearVert.name = goCAL.name;

                AbilityPreview aPrev = goLinearVert.AddComponent<AbilityPreview>();
                aPrev.HeightAttackable = cal.HeightAttackable;
                aPrev.TypeAttackable = cal.TypeAttackable;

                Image previewImageLinearVert = goLinearVert.AddComponent<Image>(); //Add the Image Component script
                previewImageLinearVert.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
                previewImageLinearVert.sprite = abilityPreviewLinear; //Set the Sprite of the Image Component on the new GameObject
                previewImageLinearVert.enabled = false;

                BoxCollider previewHitBoxLinearVert = goLinearVert.AddComponent<BoxCollider>();
                previewHitBoxLinearVert.size = new Vector3(width, GameManager.Instance.Ground.transform.localScale.z * 10, .5f);
                previewHitBoxLinearVert.center = new Vector3(0, 0, 0);
                previewHitBoxLinearVert.enabled = false;

                goLinearVert.tag = "AbilityHighlight";
                RectTransform previewLinearTransformVert = goLinearVert.GetComponent<RectTransform>();
                previewLinearTransformVert.anchorMin = new Vector2(.5f, 0);
                previewLinearTransformVert.anchorMax = new Vector2(.5f, 0);
                previewLinearTransformVert.pivot = new Vector2(.5f, .5f);
                previewLinearTransformVert.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
                previewLinearTransformVert.localPosition = new Vector3(0, 0, 0);
                previewLinearTransformVert.localRotation = Quaternion.Euler(270, 0, 0);
                previewLinearTransformVert.sizeDelta = new Vector2(width, GameManager.Instance.Ground.transform.localScale.z * 10);
                abilityPreviews.Add(goLinearVert);
            }
            if (cal.LinearStats.IsHorizontal)
            { //if the linear attack has the horizontal component
                GameObject goLinearHorz = new GameObject();
                goLinearHorz.name = goCAL.name;

                AbilityPreview aPrev = goLinearHorz.AddComponent<AbilityPreview>();
                aPrev.HeightAttackable = cal.HeightAttackable;
                aPrev.TypeAttackable = cal.TypeAttackable;

                Image previewImageLinearHorz = goLinearHorz.AddComponent<Image>(); //Add the Image Component script
                previewImageLinearHorz.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
                previewImageLinearHorz.sprite = abilityPreviewLinear;
                previewImageLinearHorz.enabled = false;

                BoxCollider previewHitBoxLinearHorz = goLinearHorz.AddComponent<BoxCollider>();
                previewHitBoxLinearHorz.size = new Vector3(GameManager.Instance.Ground.transform.localScale.x * 10, width, .5f);
                previewHitBoxLinearHorz.center = new Vector3(0, 0, 0);
                previewHitBoxLinearHorz.enabled = false;

                goLinearHorz.tag = "AbilityHighlight";
                RectTransform previewLinearTransformHorz = goLinearHorz.GetComponent<RectTransform>();
                previewLinearTransformHorz.anchorMin = new Vector2(.5f, 0);
                previewLinearTransformHorz.anchorMax = new Vector2(.5f, 0);
                previewLinearTransformHorz.pivot = new Vector2(.5f, .5f);
                previewLinearTransformHorz.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
                previewLinearTransformHorz.localPosition = new Vector3(0, 0, 0);
                previewLinearTransformHorz.localRotation = Quaternion.Euler(270, 0, 0);
                previewLinearTransformHorz.sizeDelta = new Vector2(GameManager.Instance.Ground.transform.localScale.x * 10, width);
                abilityPreviews.Add(goLinearHorz);
            }
        }

        if (cal.LingeringStats.Lingering || cal.SelfDestructStats.SelfDestructs)
        {
            GameObject goBoom = new GameObject(); //Create the GameObject
            goBoom.name = goCAL.name;

            AbilityPreview aPrev = goBoom.AddComponent<AbilityPreview>();
            aPrev.HeightAttackable = cal.HeightAttackable;
            aPrev.TypeAttackable = cal.TypeAttackable;

            Image previewImageBoom = goBoom.AddComponent<Image>(); //Add the Image Component script
            previewImageBoom.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
            previewImageBoom.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
            previewImageBoom.enabled = false;

            float radius = Math.Max(cal.LingeringStats.LingeringRadius, cal.SelfDestructStats.ExplosionRadius);

            SphereCollider previewHitBoxBoom = goBoom.AddComponent<SphereCollider>();
            previewHitBoxBoom.radius = radius;
            previewHitBoxBoom.center = new Vector3(0, 0, 0);
            previewHitBoxBoom.enabled = false;

            goBoom.tag = "AbilityHighlight";
            RectTransform previewBoomTransform = goBoom.GetComponent<RectTransform>();
            previewBoomTransform.anchorMin = new Vector2(.5f, 0);
            previewBoomTransform.anchorMax = new Vector2(.5f, 0);
            previewBoomTransform.pivot = new Vector2(.5f, .5f);
            previewBoomTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            previewBoomTransform.localPosition = new Vector3(0, 0, 0);
            previewBoomTransform.localRotation = Quaternion.Euler(270, 0, 0);
            previewBoomTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

            goBoom.SetActive(true);
            abilityPreviews.Add(goBoom);
        }

        if (cal.TeleportStats.IsWarp)
        {
            GameObject goWarp = new GameObject(); //Create the GameObject
            goWarp.name = goCAL.name;

            AbilityPreview aPrev = goWarp.AddComponent<AbilityPreview>();
            aPrev.HeightAttackable = cal.HeightAttackable;
            aPrev.TypeAttackable = cal.TypeAttackable;

            Image previewImageWarp = goWarp.AddComponent<Image>(); //Add the Image Component script
            previewImageWarp.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
            previewImageWarp.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
            previewImageWarp.enabled = false;

            float radius = unit.Agent.Agent.radius;

            SphereCollider previewHitBoxWarp = goWarp.AddComponent<SphereCollider>();
            previewHitBoxWarp.radius = radius;
            previewHitBoxWarp.center = new Vector3(0, 0, 0);
            previewHitBoxWarp.enabled = false;

            //goWarp.tag = "AbilityHighlight"; //we dont need it to highlight enemies
            RectTransform previewWarpTransform = goWarp.GetComponent<RectTransform>();
            previewWarpTransform.anchorMin = new Vector2(.5f, 0);
            previewWarpTransform.anchorMax = new Vector2(.5f, 0);
            previewWarpTransform.pivot = new Vector2(.5f, .5f);
            previewWarpTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            previewWarpTransform.localPosition = new Vector3(0, 0, 0);
            previewWarpTransform.localRotation = Quaternion.Euler(270, 0, 0);
            previewWarpTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

            goWarp.SetActive(true);
            abilityPreviews.Add(goWarp);

            if (cal.TeleportStats.TeleportsAllies)
            {
                GameObject goAWarp = new GameObject(); //Create the GameObject
                goAWarp.name = goCAL.name;

                AbilityPreview aPrevAlly = goAWarp.AddComponent<AbilityPreview>();
                aPrevAlly.HeightAttackable = cal.HeightAttackable;
                aPrevAlly.TypeAttackable = cal.TypeAttackable;

                Image previewImageAWarp = goAWarp.AddComponent<Image>(); //Add the Image Component script
                previewImageAWarp.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
                previewImageAWarp.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
                previewImageAWarp.enabled = false;

                radius = cal.TeleportStats.AllyRadius;

                SphereCollider previewHitBoxAWarp = goAWarp.AddComponent<SphereCollider>();
                previewHitBoxAWarp.radius = radius;
                previewHitBoxAWarp.center = new Vector3(0, 0, 0);
                previewHitBoxAWarp.enabled = false;

                goAWarp.tag = "FriendlyAbilityHighlight"; //we need this to highlight allies
                RectTransform previewAWarpTransform = goAWarp.GetComponent<RectTransform>();
                previewAWarpTransform.anchorMin = new Vector2(.5f, 0);
                previewAWarpTransform.anchorMax = new Vector2(.5f, 0);
                previewAWarpTransform.pivot = new Vector2(.5f, .5f);
                previewAWarpTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
                previewAWarpTransform.localPosition = new Vector3(0, 0, 0);
                previewAWarpTransform.localRotation = Quaternion.Euler(270, 0, 0);
                previewAWarpTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

                goAWarp.SetActive(true);
                abilityPreviews.Add(goAWarp);
            }
        }

        //now add the range circle
        GameObject goBoomRange = new GameObject();
        goBoomRange.name = goCAL.name;

        Image previewImageRange = goBoomRange.AddComponent<Image>();
        previewImageRange.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        previewImageRange.sprite = abilityPreviewRange;
        previewImageRange.enabled = false;

        RectTransform previewBoomRangeTransform = goBoomRange.GetComponent<RectTransform>();
        previewBoomRangeTransform.anchorMin = new Vector2(.5f, 0);
        previewBoomRangeTransform.anchorMax = new Vector2(.5f, 0);
        previewBoomRangeTransform.pivot = new Vector2(.5f, .5f);
        previewBoomRangeTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
        previewBoomRangeTransform.localPosition = new Vector3(0, 0, 0);
        previewBoomRangeTransform.localRotation = Quaternion.Euler(270, 0, 0);
        previewBoomRangeTransform.sizeDelta = new Vector2(cal.Range * 2, cal.Range * 2);

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
