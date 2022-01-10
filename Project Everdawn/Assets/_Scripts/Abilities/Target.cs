using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* ! NOTES ! 
    Cannot use a summon ability with target.

    When creating a target ability, all projectiles should have the same range. It will still work, but the previews are not desirable

    When using a movement ability, 'passObstacles' should be set to true
*/
public class Target : MonoBehaviour, ICaster, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Gameobject")]
    [SerializeField]
    private Unit unit;

    [SerializeField]
    private Building building;

    [Header("Ability Order and Timings")]
    [SerializeField]
    private List<GameObject> abilityPrefabs;

    [SerializeField]
    private List<float> abilityDelays;

    [SerializeField]
    private float currentDelay;

    [SerializeField]
    private ApplyResistanceStats applyResistanceStats; //A list of effects than can be set to be resisted while casting

    private bool isFiring;
    private int currentProjectileIndex;
    private Vector3 fireStartPosition;
    private Actor3D target;
    private Vector3 fireDirection;

    [SerializeField]
    private AbilityUI abilityUI;

    [SerializeField]
    private Canvas abilityPreviewCanvas;

    [SerializeField]
    private List<GameObject> abilityPreviews;

    [Header("Ability default previews and settings")]
    [SerializeField]
    private bool hidePreview;

    [SerializeField]
    private Sprite abilityPreviewLine;

    [SerializeField]
    private Sprite abilityPreviewLinear;

    [SerializeField]
    private Sprite abilityPreviewBomb;

    [SerializeField]
    private Sprite abilityPreviewRange;

    private bool isDragging;

    private bool abilityControl; //sets it so the skillshot doesnt unset unit casting, but rather the projectile. Usful for movements like dashes or maybe pulls
    private float maxRange;

    //a flag set if an ability controls when the skillshot will continue. Usage: a unit fires a grappler and will not be pulled forward until the grappler has hit somthing
    private bool pauseFiring;
    //a flag set by an ability to stop continuing the skillshot regardless if there are more abilities to follow
    private bool exitOverride;
    //an amount of abilities to skip starting from the last ability
    private int skipOverride;

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
        //Below sets up what will be needed for clicking the ability, such that we only need to calculate maxRange and areaMask once.
        maxRange = 0;
        foreach(GameObject ability in abilityPrefabs) { //this finds the largest range of all the abilitys shot by this skillshot
            Component component = ability.GetComponent(typeof(IAbility));
            if((component as IAbility).AbilityControl)
                abilityControl = true;
            if((component as IAbility).Range > maxRange)
                maxRange = (component as IAbility).Range;
        }

        abilityPreviews = new List<GameObject>();
        createAbilityPreviews();
        currentDelay = 0;

        isFiring = false;
        isDragging = false;

        AbilityUI.StartStats();

        target = null;
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
                preview.transform.GetChild(0).GetComponent<Image>().enabled = true;
                if(preview.GetComponent<Collider>())
                    preview.GetComponent<Collider>().enabled = true;
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
        if(isDragging) {
            Vector3 position = GameFunctions.getPosition(false);

            fireStartPosition = abilityPreviewCanvas.transform.position;
            fireStartPosition.y = 0;

            Vector3 direction = position - fireStartPosition;
            direction.y = 0;

            float distance = Vector3.Distance(fireStartPosition, position);
            if(distance > (maxRange - 3))
                position = fireStartPosition + (direction.normalized * (maxRange - 3));
            Actor3D potentialTarget = FindTarget(position);
            target = potentialTarget;

            if(potentialTarget != null)
                position = potentialTarget.transform.position;
            position.y = .1f;

            Quaternion rotation = Quaternion.LookRotation(direction);
            abilityPreviewCanvas.transform.rotation = Quaternion.Lerp(rotation, abilityPreviewCanvas.transform.rotation, 0f);

            foreach(GameObject preview in abilityPreviews) {
                if(preview.GetComponent<SphereCollider>() || preview.GetComponent<BoxCollider>()) {
                    GameObject go = abilityPrefabs.Find(go => go.name == preview.name);
                    if(go.GetComponent<Projectile>() && !preview.GetComponent<BoxCollider>()) //if the preview corresponds to a projectile and doesnt have a box collider. Reason being, all projectiles with a box collider doesnt move away from the unit
                        AdjustProjectilePreview(preview, go.GetComponent<Projectile>(), position, direction);
                    else if(go.GetComponent<CreateAtLocation>())
                        AdjustCALPreview(preview, go.GetComponent<CreateAtLocation>(), position, direction);
                }
                else if(preview.name == "Targeter")
                    preview.transform.GetChild(0).GetComponent<RectTransform>().position = position;
            }
        }
        else if(isFiring)
            Fire();
        else
            target = null;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if(isDragging) {
            isDragging = false;
            unit.Stats.IsHoveringAbility = false;

            abilityUI.AbilitySprite.enabled = true;
            abilityUI.AbilityCancel.enabled = false;

            foreach(GameObject preview in abilityPreviews) {
                preview.transform.GetChild(0).GetComponent<Image>().enabled = false;
                if(preview.GetComponent<Collider>())
                    preview.GetComponent<Collider>().enabled = false;
            }

            GameManager.removeAbililtyIndicators();

            if(abilityUI.CardCanvasDim.rect.height < Input.mousePosition.y && target != null && unit.Stats.CanAct) {
                fireStartPosition = abilityPreviewCanvas.transform.position;
                fireStartPosition.y = 0;

                fireDirection = target.transform.position - fireStartPosition;

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

            Actor3D closestTarget = null;
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
                                closestTarget = collider.gameObject.GetComponent<Actor3D>();
                            }
                        }
                    }
                }
            }
            if(closestTarget != null && unit.Stats.CanAct) {
                fireStartPosition = abilityPreviewCanvas.transform.position;
                fireStartPosition.y = 0;

                target = closestTarget;
                fireDirection = closestTarget.transform.position - fireStartPosition;

                isFiring = true;
                unit.Stats.IsCastingAbility = true;
                unit.SetTarget(null);
                abilityUI.resetAbility();
                applyResistanceStats.ApplyResistance(unit);
            }
        }
    }

    private Actor3D FindTarget(Vector3 position) {
        Collider[] colliders = Physics.OverlapSphere(position, 3);
        Component testComponent = abilityPrefabs[0].GetComponent(typeof(IAbility));

        Actor3D chosenTarget = null;
        if(colliders.Length > 0) {
            float closestDistance = 9999;
            float distance;
            foreach(Collider collider in colliders) {
                if(!collider.CompareTag(abilityPrefabs[0].tag) && collider.name == "Agent") {
                    Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                    if((damageable as IDamageable).Stats.Targetable && GameFunctions.WillHit((testComponent as IAbility).HeightAttackable, (testComponent as IAbility).TypeAttackable, damageable)) {
                        distance = Vector3.Distance(unit.Agent.Agent.transform.position, collider.transform.position);
                        if(distance < closestDistance) {
                            closestDistance = distance;
                            chosenTarget = collider.gameObject.GetComponent<Actor3D>();
                        }
                    }
                }
            }
        }
        return chosenTarget;
    }

    private void Fire() {
        if(!unit.Stats.CanAct || target == null || exitOverride) {
            if(currentProjectileIndex == 0) //if we started firing a target projectile, but before one goes off the target dies, reset the cooldown
                abilityUI.CurrCooldownDelay = abilityUI.CooldownDelay;

            isFiring = false;
            exitOverride = false;
            pauseFiring = false;
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
            skipOverride = 0;
            currentProjectileIndex = 0;
            currentDelay = 0;
            if(!abilityControl)
                unit.Stats.IsCastingAbility = false;
            target = null;
        }
        else { //if we completed a delay
            fireStartPosition = abilityPreviewCanvas.transform.position;
            if(abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], fireStartPosition, target, fireDirection, unit, transform.parent.tag, 1, this);
            else if(abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], fireStartPosition, target, fireDirection, unit, transform.parent.tag, 1, this);
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

        //RectTransform previewRect = preview.transform.GetChild(0).GetComponent<RectTransform>();
        /*
                UnityEngine.AI.NavMeshHit hit;
                if(cal.TeleportStats.IsWarp && unit.Stats.MovementType == GameConstants.MOVEMENT_TYPE.GROUND) {
                    if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 12f, 9))
                        position = hit.position;
                }*/

        
        /*
        previewRect.rotation = Quaternion.Euler(90f, 0f, 0f);
        if(previewRect.sizeDelta.x < previewRect.sizeDelta.y) //this is the vertical component for a linear cal
            previewRect.position = new Vector3(position.x, 1, 0);
        else if(previewRect.sizeDelta.x > previewRect.sizeDelta.y) //this is the horizontal component for a linear cal
            previewRect.position = new Vector3(0, 1, position.z);
*/
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

    public void createAbilityPreviews() {
        //first create the standard target circle
        List<GameObject> uniqueProjectiles = new List<GameObject>();
        float radius = 3;

        /* ----- Add the Targeter circle ----- */
        GameObject goTarget = new GameObject(); //Create the GameObject
        goTarget.name = "Targeter";

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
        imageTransform.SetParent(goTarget.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
        imageTransform.localPosition = Vector3.zero;
        imageTransform.localRotation = Quaternion.Euler(270, 0, 0);
        imageTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

        /* -- Adjust the GameObjects transform, directly affecting the collider -- */
        goTarget.transform.SetParent(abilityPreviewCanvas.transform); 
        goTarget.transform.localPosition = Vector3.zero;
        goTarget.transform.localRotation = Quaternion.Euler(Vector3.zero);
        goTarget.SetActive(true);
        abilityPreviews.Add(goTarget);


        /* ----- Add the range circle ----- */
        if(!hidePreview) {
            GameObject goRange = new GameObject();
            goRange.name = "TargetRange";

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
            imageRangeTransform.SetParent(goRange.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            imageRangeTransform.localPosition = Vector3.zero;
            imageRangeTransform.localRotation = Quaternion.Euler(270, 0, 0);
            imageRangeTransform.sizeDelta = new Vector2(maxRange * 2, maxRange * 2);

            goRange.transform.SetParent(abilityPreviewCanvas.transform);
            goRange.transform.localPosition = Vector3.zero;
            goRange.transform.localRotation = Quaternion.Euler(Vector3.zero);
            goRange.SetActive(true);
            abilityPreviews.Add(goRange);
        }

        foreach(GameObject goAbility in abilityPrefabs) {
            if(!uniqueProjectiles.Contains(goAbility)) {
                uniqueProjectiles.Add(goAbility);
                if(goAbility.GetComponent<Projectile>())
                    createProjectilePreview(goAbility);
                else if(goAbility.GetComponent<CreateAtLocation>())
                    createCALPreview(goAbility);
            }
        }
    }

    private void createProjectilePreview(GameObject goProj) {
        Projectile projectile = goProj.GetComponent<Projectile>();

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
        }
    }

    private void createCALPreview(GameObject goCAL) {
        CreateAtLocation cal = goCAL.GetComponent<CreateAtLocation>();

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

        if(cal.TeleportStats.IsWarp && cal.TeleportStats.TeleportsAllies) {
            float radius = cal.TeleportStats.AllyRadius;

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
    
    //LocationStats should only be used on skillshots
    public void SetNewLocation(Vector3 newLocation, Vector3 newDirection) {
        //pass
    }
    //this should only be used by skillshot
    public void SetNewTarget(Actor3D newTarget) {
        //pass
    }
}
