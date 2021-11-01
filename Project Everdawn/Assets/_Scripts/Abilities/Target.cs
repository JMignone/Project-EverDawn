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
public class Target : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField]
    private Unit unit;

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

    public Vector3 FireStartPosition
    {
        get { return fireStartPosition; }
        set { fireStartPosition = value; }
    }

    public Vector3 FireDirection
    {
        get { return fireDirection; }
        set { fireDirection = value; }
    }

    public AbilityUI AbilityUI
    {
        get { return abilityUI; }
        //set { abilityUI = value; }
    }

    void Start()
    {
        //Below sets up what will be needed for clicking the ability, such that we only need to calculate maxRange and areaMask once.
        maxRange = 0;
        foreach (GameObject ability in abilityPrefabs)
        { //this finds the largest range of all the abilitys shot by this skillshot
            Component component = ability.GetComponent(typeof(IAbility));
            if ((component as IAbility).AbilityControl)
                abilityControl = true;
            if ((component as IAbility).Range > maxRange)
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
                preview.GetComponent<Image>().enabled = true;
                if (preview.GetComponent<Collider>())
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
    private void Update()
    {
        abilityUI.UpdateStats();
        if (isDragging)
        {
            Vector3 position = GameFunctions.getPosition(false);

            fireStartPosition = abilityPreviewCanvas.transform.position;
            fireStartPosition.y = 0;

            Vector3 direction = position - fireStartPosition;
            direction.y = 0;

            float distance = Vector3.Distance(fireStartPosition, position);
            if (distance > (maxRange - 3))
                position = fireStartPosition + (direction.normalized * (maxRange - 3));
            Actor3D potentialTarget = FindTarget(position);
            target = potentialTarget;

            if (potentialTarget != null)
                position = potentialTarget.transform.position;
            position.y = 1;

            Quaternion rotation = Quaternion.LookRotation(direction);
            abilityPreviewCanvas.transform.rotation = Quaternion.Lerp(rotation, abilityPreviewCanvas.transform.rotation, 0f);

            foreach (GameObject preview in abilityPreviews)
            {
                if (preview.GetComponent<SphereCollider>() || preview.GetComponent<BoxCollider>())
                {
                    GameObject go = abilityPrefabs.Find(go => go.name == preview.name);
                    if (go.GetComponent<Projectile>() && !preview.GetComponent<BoxCollider>()) //if the preview corresponds to a projectile and doesnt have a box collider. Reason being, all projectiles with a box collider doesnt move away from the unit
                        AdjustProjectilePreview(preview, go.GetComponent<Projectile>(), position, direction);
                    else if (go.GetComponent<CreateAtLocation>())
                        AdjustCALPreview(preview, go.GetComponent<CreateAtLocation>(), position, direction);
                }
                else if (preview.name == "Targeter")
                    preview.GetComponent<RectTransform>().position = position;
            }
        }
        else if (isFiring)
            Fire();
        else
            target = null;
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
                preview.GetComponent<Image>().enabled = false;
                if (preview.GetComponent<Collider>())
                    preview.GetComponent<Collider>().enabled = false;
            }

            GameManager.removeAbililtyIndicators();

            if (abilityUI.CardCanvasDim.rect.height < Input.mousePosition.y && target != null && unit.Stats.CanAct)
            {
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

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (!isDragging && abilityUI.CanDrag && unit.Stats.IsReady)
        { //if the abililty can be dragged
            Collider[] colliders = Physics.OverlapSphere(unit.Agent.Agent.transform.position, maxRange);
            Component testComponent = abilityPrefabs[0].GetComponent(typeof(IAbility));

            Actor3D closestTarget = null;
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
                                closestTarget = collider.gameObject.GetComponent<Actor3D>();
                            }
                        }
                    }
                }
            }
            if (closestTarget != null && unit.Stats.CanAct)
            {
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

    private Actor3D FindTarget(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 3);
        Component testComponent = abilityPrefabs[0].GetComponent(typeof(IAbility));

        Actor3D chosenTarget = null;
        if (colliders.Length > 0)
        {
            float closestDistance = 9999;
            float distance;
            foreach (Collider collider in colliders)
            {
                if (!collider.CompareTag(abilityPrefabs[0].tag) && collider.name == "Agent")
                {
                    Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                    if ((damageable as IDamageable).Stats.Targetable && GameFunctions.WillHit((testComponent as IAbility).HeightAttackable, (testComponent as IAbility).TypeAttackable, damageable))
                    {
                        distance = Vector3.Distance(unit.Agent.Agent.transform.position, collider.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            chosenTarget = collider.gameObject.GetComponent<Actor3D>();
                        }
                    }
                }
            }
        }
        return chosenTarget;
    }

    private void Fire()
    {
        if (!unit.Stats.CanAct || target == null)
        {
            if (currentProjectileIndex == 0) //if we started firing a target projectile, but before one goes off the target dies, reset the cooldown
                abilityUI.CurrCooldownDelay = abilityUI.CooldownDelay;

            isFiring = false;
            currentProjectileIndex = 0;
            currentDelay = 0;
            if (!abilityControl)
                unit.Stats.IsCastingAbility = false;
        }
        else if (currentDelay < abilityDelays[currentProjectileIndex]) //if we havnt reached the delay yet
            currentDelay += Time.deltaTime * unit.Stats.EffectStats.SlowedStats.CurrentSlowIntensity;
        else if (currentProjectileIndex == abilityPrefabs.Count)
        { //if we completed the last delay
            isFiring = false;
            currentProjectileIndex = 0;
            currentDelay = 0;
            if (!abilityControl)
                unit.Stats.IsCastingAbility = false;
            target = null;
        }
        else
        { //if we completed a delay
            fireStartPosition = abilityPreviewCanvas.transform.position;
            if (abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], fireStartPosition, target, fireDirection, unit, transform.parent.tag, 1);
            else if (abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], fireStartPosition, target, fireDirection, unit, transform.parent.tag, 1);
            currentDelay = 0;
            currentProjectileIndex++;
        }
    }

    private void AdjustProjectilePreview(GameObject preview, Projectile proj, Vector3 position, Vector3 direction)
    {
        float range = proj.Range;
        float radius = proj.Radius;

        float distance = Vector3.Distance(position, abilityPreviewCanvas.transform.position);

        bool previewCircleAtEnd = !proj.GrenadeStats.IsGrenade && !proj.SelfDestructStats.SelfDestructs &&
                                  (proj.LingeringStats.Lingering && proj.LingeringStats.LingerAtEnd);
        //if its not a grenade, doesnt selfdestruct, and lingers at the end its true        
        bool previewCircleAtBeginning = previewCircleAtEnd && proj.BoomerangStats.IsBoomerang;

        if (previewCircleAtEnd && !previewCircleAtBeginning)
            position = abilityPreviewCanvas.transform.position + (direction.normalized * (range - radius) * -1); //locks the circle at the furthest position
        else if (previewCircleAtBeginning)
            position = abilityPreviewCanvas.transform.position;
        else if (distance > range - radius)
        {
            Vector3 distFromRadius = position - abilityPreviewCanvas.transform.position;
            distFromRadius *= (range - radius) / distance;
            position = abilityPreviewCanvas.transform.position + distFromRadius;
            position.y = 1;
        }
        preview.GetComponent<RectTransform>().position = position;
    }

    private void AdjustCALPreview(GameObject preview, CreateAtLocation cal, Vector3 position, Vector3 direction)
    {
        float range = cal.Range;
        float radius = cal.Radius;
        float distance = Vector3.Distance(position, abilityPreviewCanvas.transform.position);

        if (distance > range - radius)
        {
            Vector3 distFromRadius = position - abilityPreviewCanvas.transform.position;
            distFromRadius *= (range - radius) / distance;
            position = abilityPreviewCanvas.transform.position + distFromRadius;
        }

        position = GameFunctions.adjustForBoundary(position);

        RectTransform previewRect = preview.GetComponent<RectTransform>();
        /*
                UnityEngine.AI.NavMeshHit hit;
                if(cal.TeleportStats.IsWarp && unit.Stats.MovementType == GameConstants.MOVEMENT_TYPE.GROUND) {
                    if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 12f, 9))
                        position = hit.position;
                }*/

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
        /*
        if(cal.LinearStats.IsLinear) {
            preview.GetComponent<RectTransform>().rotation = Quaternion.Euler(90f, 0f, 0f);
            RectTransform previewRect = preview.GetComponent<RectTransform>();
            if(previewRect.sizeDelta.x < previewRect.sizeDelta.y) //this is the vertical component
                preview.GetComponent<RectTransform>().position = new Vector3(position.x, 1, 0); 
            else //this is the horizontal component
                preview.GetComponent<RectTransform>().position = new Vector3(0, 1, position.z); 
        }
        else {
            position.y = 1;
            preview.GetComponent<RectTransform>().position = position;
        }*/
    }

    public void createAbilityPreviews()
    {
        //first create the standard target circle
        List<GameObject> uniqueProjectiles = new List<GameObject>();
        float radius = 3;

        GameObject goTarget = new GameObject(); //Create the GameObject
        goTarget.name = "Targeter";

        Image previewImageBoom = goTarget.AddComponent<Image>(); //Add the Image Component script
        previewImageBoom.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        previewImageBoom.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
        previewImageBoom.enabled = false;

        RectTransform previewBoomTransform = goTarget.GetComponent<RectTransform>();
        previewBoomTransform.anchorMin = new Vector2(.5f, 0);
        previewBoomTransform.anchorMax = new Vector2(.5f, 0);
        previewBoomTransform.pivot = new Vector2(.5f, .5f);
        previewBoomTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
        previewBoomTransform.localPosition = new Vector3(0, -5, 0);
        previewBoomTransform.localRotation = Quaternion.Euler(270, 0, 0);
        previewBoomTransform.sizeDelta = new Vector2(radius * 2, radius * 2);

        goTarget.SetActive(true);
        abilityPreviews.Add(goTarget);

        //adding the range circle
        GameObject goRange = new GameObject();
        goRange.name = "TargetRange";

        Image previewImageRange = goRange.AddComponent<Image>();
        previewImageRange.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        previewImageRange.sprite = abilityPreviewRange;
        previewImageRange.enabled = false;

        RectTransform previewBoomRangeTransform = goRange.GetComponent<RectTransform>();
        previewBoomRangeTransform.anchorMin = new Vector2(.5f, 0);
        previewBoomRangeTransform.anchorMax = new Vector2(.5f, 0);
        previewBoomRangeTransform.pivot = new Vector2(.5f, .5f);
        previewBoomRangeTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
        previewBoomRangeTransform.localPosition = new Vector3(0, 0, 0);
        previewBoomRangeTransform.localRotation = Quaternion.Euler(270, 0, 0);
        previewBoomRangeTransform.sizeDelta = new Vector2(maxRange * 2, maxRange * 2);

        goRange.SetActive(true);
        abilityPreviews.Add(goRange);

        foreach (GameObject goAbility in abilityPrefabs)
        {
            if (!uniqueProjectiles.Contains(goAbility))
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
        }
    }

    private void createCALPreview(GameObject goCAL)
    {
        CreateAtLocation cal = goCAL.GetComponent<CreateAtLocation>();

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

        if (cal.TeleportStats.IsWarp && cal.TeleportStats.TeleportsAllies)
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

            float radius = cal.TeleportStats.AllyRadius;

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
}
