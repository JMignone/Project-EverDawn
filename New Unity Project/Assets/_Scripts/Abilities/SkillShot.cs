using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillShot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Unit unit;

    [SerializeField]
    private List<GameObject> abilityPrefabs;

    [SerializeField]
    private List<float> abilityDelays;

    [SerializeField]
    private float currentDelay;

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
    private Sprite abilityPreviewBomb;

    [SerializeField]
    private Sprite abilityPreviewRange;

    [SerializeField]
    private PlayerStats playerInfo;
    private bool isDragging;

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

    public float currentDelays
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

    public Vector3 FireMousePosition
    {
        get { return fireMousePosition; }
        set { fireMousePosition = value; }
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

    public Canvas AbilityPreviewCanvas
    {
        get { return abilityPreviewCanvas; }
        //set { abilityPreviewCanvas = value; }
    }

    public List<GameObject> AbilityPreviews
    {
        get { return abilityPreviews; }
    }

    public Sprite AbilityPreviewLine
    {
        get { return abilityPreviewLine; }
    }

    public Sprite AbilityPreviewBomb
    {
        get { return abilityPreviewBomb; }
    }

    public Sprite AbilityPreviewRange
    {
        get { return abilityPreviewRange; }
    }

    void Start()
    {
        abilityPreviews = new List<GameObject>();
        createAbilityPreviews();
        currentDelay = 0;

        isFiring = false;
        isDragging = false;
        fireMousePosition = new Vector3(0, 1, 0);

        AbilityUI.CardCanvasDim = GameFunctions.GetCanvas().GetChild(0).GetComponent<RectTransform>();
    }


    /*
        At the moment, a boomerang that selfdestructs and lingers at the end does not have the preview that id like (the linger circle around the unit wont be there,
        as the selfdestruct location takes precidence). But this kind of projectile is probably unlikely to happen anyway
    */
    private void Update() {
        abilityUI.UpdateStats();
        if(playerInfo.OnDragging && isDragging) {
            Vector3 position = GameFunctions.getPosition(false);
            Vector3 direction = abilityPreviewCanvas.transform.position - position;
            direction.y = 0;
            position.y++;

            Quaternion rotation = Quaternion.LookRotation(direction);
            abilityPreviewCanvas.transform.rotation = Quaternion.Lerp(rotation, abilityPreviewCanvas.transform.rotation, 0f);
            
            foreach(GameObject preview in abilityPreviews) {
                if(preview.GetComponent<SphereCollider>()) {
                    Vector3 newPosition = position;
                    GameObject go = abilityPrefabs.Find(go => go.name == preview.name);
                    if(go.GetComponent<Projectile>())
                        AdjustProjectilePreview(preview, go.GetComponent<Projectile>(), position, direction);
                    else if(go.GetComponent<CreateAtLocation>())
                        AdjustCALPreview(preview, go.GetComponent<CreateAtLocation>(), position, direction);
                }       
            }
        }
        else if(isFiring)
            Fire();
        else
            fireMousePosition = new Vector3(0, 1, 0); //this is to reset the mouse position, needed because of special summon location conditions
    }

    private void Fire() {
        if(currentDelay < abilityDelays[currentProjectileIndex]) //if we havnt reached the delay yet
            currentDelay += Time.deltaTime;
        else if(currentProjectileIndex == abilityPrefabs.Count) { //if we completed the last delay
            isFiring = false;
            currentProjectileIndex = 0;
            currentDelay = 0;
            unit.IsCastingAbility = false;
        }
        else { //if we completed a delay
            if(abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], fireStartPosition, fireMousePosition, fireDirection, unit);
            else if(abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], fireStartPosition, fireMousePosition, fireDirection, unit);
            currentDelay = 0;
            currentProjectileIndex++;
        }
    }

    private void AdjustProjectilePreview(GameObject preview, Projectile proj, Vector3 position, Vector3 direction) {
        float range = proj.Range;
        float radius = proj.Radius;
        bool previewCircleAtEnd = !proj.GrenadeStats.IsGrenade && !proj.SelfDestructStats.SelfDestructs && 
                                  (proj.LingeringStats.Lingering && proj.LingeringStats.LingerAtEnd);
                                  //if its not a grenade, doesnt selfdestruct, and lingers at the end its true

        bool previewCircleAtBeginning = previewCircleAtEnd && proj.BoomerangStats.IsBoomerang;

        float distance = Vector3.Distance(position, abilityPreviewCanvas.transform.position);
        if(previewCircleAtEnd && !previewCircleAtBeginning)
            position = abilityPreviewCanvas.transform.position + (direction.normalized * (range - radius) * -1); //locks the circle at the furthest position
        else if(previewCircleAtBeginning)
            position = abilityPreviewCanvas.transform.position;
        else if(distance > range - radius) {
            Vector3 distFromRadius = position - abilityPreviewCanvas.transform.position;
            distFromRadius *= (range - radius)/distance;
            position = abilityPreviewCanvas.transform.position + distFromRadius;
            position.y=1;
        }
        preview.GetComponent<RectTransform>().position = position;
    }

    private void AdjustCALPreview(GameObject preview, CreateAtLocation cal, Vector3 position, Vector3 direction) {
        float range = cal.Range;
        float radius = cal.Radius;
        float distance = Vector3.Distance(position, abilityPreviewCanvas.transform.position);
        if(distance > range - radius) {
            Vector3 distFromRadius = position - abilityPreviewCanvas.transform.position;
            distFromRadius *= (range - radius)/distance;
            position = abilityPreviewCanvas.transform.position + distFromRadius;
        }

        position = GameFunctions.adjustForBoundary(position);

        if(cal.SummonStats.IsSummon) { //if its a summon, we dont want the preview to appear in places the summon cant spawn
            position.y = 0;
            //position = GameFunctions.adjustForTowers(position, cal.Radius);
            UnityEngine.AI.NavMeshHit hit;
            if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 10f, 16)) //16 = just preview check area
                position = hit.position;
            preview.transform.GetChild(0).GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(position);
        }
        else {
            position.y = 1;
            preview.GetComponent<RectTransform>().position = position;
        }
    }

    private void LateUpdate() {
        abilityUI.LateUpdateStats();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!playerInfo.OnDragging && !isDragging) {
            if(abilityUI.CanDrag) {
                isDragging = true;
                playerInfo.OnDragging = true;
                unit.IsHoveringAbility = true;

                abilityUI.AbilitySprite.enabled = false;
                abilityUI.AbilityCancel.enabled = true;

                foreach(GameObject preview in abilityPreviews) {
                    if(preview.transform.childCount > 0 ) { //this is a summon preview, as its more complicated
                        preview.transform.GetChild(1).GetChild(0).GetComponent<Image>().enabled = true;
                        preview.transform.GetChild(1).GetChild(0).GetComponent<Collider>().enabled = true;
                    }
                    else {
                    preview.GetComponent<Image>().enabled = true;
                    if(preview.GetComponent<Collider>())
                        preview.GetComponent<Collider>().enabled = true;
                    }
                }
                
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //pass
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(playerInfo.OnDragging) {
            isDragging = false;
            playerInfo.OnDragging = false;
            unit.IsHoveringAbility = false;

            abilityUI.AbilitySprite.enabled = true;
            abilityUI.AbilityCancel.enabled = false;

            foreach(GameObject preview in abilityPreviews) {
                if(preview.transform.childCount > 0 ) { //this is a summon preview, as its more complicated
                    fireMousePosition = preview.transform.GetChild(0).position;
                    preview.transform.GetChild(1).GetChild(0).GetComponent<Image>().enabled = false;
                    preview.transform.GetChild(1).GetChild(0).GetComponent<Collider>().enabled = false;
                }
                else {
                preview.GetComponent<Image>().enabled = false;
                if(preview.GetComponent<Collider>())
                    preview.GetComponent<Collider>().enabled = false;
                }
            }

            GameManager.removeAbililtyIndicators();

            if(abilityUI.CardCanvasDim.rect.height < Input.mousePosition.y) {
                fireStartPosition = abilityPreviewCanvas.transform.position;
                if(fireMousePosition == new Vector3(0, 1, 0)) {
                    fireMousePosition = GameFunctions.getPosition(false);
                    fireMousePosition.y = 0;
                }
                fireStartPosition.y = 0;
                fireDirection = fireMousePosition - fireStartPosition;
                isFiring = true;
                unit.IsCastingAbility = true;
                abilityUI.resetAbility();
            }
        }
    }    

    public void createAbilityPreviews() {

        List<GameObject> uniqueProjectiles = new List<GameObject>();

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
        if(!projectile.GrenadeStats.IsGrenade) {

            GameObject go = new GameObject(); //Create the GameObject
            go.name = goProj.name;

            Image previewImage = go.AddComponent<Image>(); //Add the Image Component script
            previewImage.color = new Color32(255,255,255,100);
            previewImage.sprite = abilityPreviewLine; //Set the Sprite of the Image Component on the new GameObject
            previewImage.enabled = false;

            float range = projectile.Range;
            float width = projectile.Radius * 2;

            BoxCollider previewHitBox = go.AddComponent<BoxCollider>();
            previewHitBox.size = new Vector3(width, range, 1);
            previewHitBox.center = new Vector3(0, 0, -.5f);
            previewHitBox.enabled = false;

            go.tag = "AbilityHighlight";
            RectTransform previewTransform = go.GetComponent<RectTransform>();
            previewTransform.anchorMin = new Vector2(.5f, 0);
            previewTransform.anchorMax = new Vector2(.5f, 0);
            previewTransform.pivot = new Vector2(.5f, .5f);
            previewTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            previewTransform.localPosition = new Vector3(0, -4, -1 * range/2);
            previewTransform.localRotation = Quaternion.Euler(270, 0, 0);
            previewTransform.sizeDelta = new Vector2(width, range);

            go.SetActive(true);
            abilityPreviews.Add(go);
        }

        if(projectile.SelfDestructStats.SelfDestructs || projectile.GrenadeStats.IsGrenade || 
          (projectile.LingeringStats.Lingering && projectile.LingeringStats.LingerAtEnd)) { //we must also add the blow up range
            GameObject goBoom = new GameObject(); //Create the GameObject
            goBoom.name = goProj.name;

            Image previewImageBoom = goBoom.AddComponent<Image>(); //Add the Image Component script
            previewImageBoom.GetComponent<Image>().color = new Color32(255,255,255,100);
            previewImageBoom.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
            previewImageBoom.enabled = false;

            float radius;
            if(projectile.SelfDestructStats.SelfDestructs)
                radius = projectile.SelfDestructStats.ExplosionRadius;
            else if(projectile.GrenadeStats.IsGrenade)
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
            previewBoomTransform.localPosition = new Vector3(0, -5, 0);
            previewBoomTransform.localRotation = Quaternion.Euler(270, 0, 0);
            previewBoomTransform.sizeDelta = new Vector2(radius*2, radius*2);
            
            goBoom.SetActive(true);
            abilityPreviews.Add(goBoom);


            //now add the range circle
            GameObject goBoomRange = new GameObject();
            goBoomRange.name = goProj.name;
            
            Image previewImageRange = goBoomRange.AddComponent<Image>();
            //previewImageRange.GetComponent<Image>().color = new Color32(255,255,255,100);
            previewImageRange.sprite = abilityPreviewRange;
            previewImageRange.enabled = false;

            RectTransform previewBoomRangeTransform = goBoomRange.GetComponent<RectTransform>();
            previewBoomRangeTransform.anchorMin = new Vector2(.5f, 0);
            previewBoomRangeTransform.anchorMax = new Vector2(.5f, 0);
            previewBoomRangeTransform.pivot = new Vector2(.5f, .5f);
            previewBoomRangeTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            previewBoomRangeTransform.localPosition = new Vector3(0, -5, 0);
            previewBoomRangeTransform.localRotation = Quaternion.Euler(270, 0, 0);
            previewBoomRangeTransform.sizeDelta = new Vector2(projectile.Range*2, projectile.Range*2);

            goBoomRange.SetActive(true);
            abilityPreviews.Add(goBoomRange);
        }
    }

    private void createCALPreview(GameObject goCAL) {
        CreateAtLocation cal = goCAL.GetComponent<CreateAtLocation>();
        
        if(cal.SummonStats.IsSummon) {
            GameObject go = Instantiate(cal.SummonStats.SummonPreview);
            go.name = goCAL.name;

            Image previewImageBoom = go.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            RectTransform previewBoomTransform = go.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
            SphereCollider previewHitBoxBoom = go.transform.GetChild(1).GetChild(0).GetComponent<SphereCollider>();

            float radius = cal.Radius;

            previewImageBoom.enabled = false;
            previewBoomTransform.sizeDelta = new Vector2(radius*2, radius*2);
            previewHitBoxBoom.radius = radius;
            previewHitBoxBoom.enabled = false;

            go.transform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.

            go.SetActive(true);
            abilityPreviews.Add(go);
        }
        else {
            GameObject goBoom = new GameObject(); //Create the GameObject
            goBoom.name = goCAL.name;

            Image previewImageBoom = goBoom.AddComponent<Image>(); //Add the Image Component script
            previewImageBoom.GetComponent<Image>().color = new Color32(255,255,255,100);
            previewImageBoom.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
            previewImageBoom.enabled = false;

            float radius = cal.Radius;

            SphereCollider previewHitBoxBoom = goBoom.AddComponent<SphereCollider>();
            previewHitBoxBoom.radius = radius;
            previewHitBoxBoom.center = new Vector3(0, 0, 0);
            previewHitBoxBoom.enabled = false;

            goBoom.tag = "AbilityHighlight";
            RectTransform previewBoomTransform =  goBoom.GetComponent<RectTransform>();
            previewBoomTransform.anchorMin = new Vector2(.5f, 0);
            previewBoomTransform.anchorMax = new Vector2(.5f, 0);
            previewBoomTransform.pivot = new Vector2(.5f, .5f);
            previewBoomTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            previewBoomTransform.localPosition = new Vector3(0, -5, 0);
            previewBoomTransform.localRotation = Quaternion.Euler(270, 0, 0);
            previewBoomTransform.sizeDelta = new Vector2(radius*2, radius*2);

            goBoom.SetActive(true);
            abilityPreviews.Add(goBoom);
        }


        //now add the range circle
        GameObject goBoomRange = new GameObject();
        goBoomRange.name = goCAL.name;
            
        Image previewImageRange = goBoomRange.AddComponent<Image>();
        previewImageRange.sprite = abilityPreviewRange;
        previewImageRange.enabled = false;

        RectTransform previewBoomRangeTransform = goBoomRange.GetComponent<RectTransform>();
        previewBoomRangeTransform.anchorMin = new Vector2(.5f, 0);
        previewBoomRangeTransform.anchorMax = new Vector2(.5f, 0);
        previewBoomRangeTransform.pivot = new Vector2(.5f, .5f);
        previewBoomRangeTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
        previewBoomRangeTransform.localPosition = new Vector3(0, -5, 0);
        previewBoomRangeTransform.localRotation = Quaternion.Euler(270, 0, 0);
        previewBoomRangeTransform.sizeDelta = new Vector2(cal.Range*2, cal.Range*2);

        goBoomRange.SetActive(true);
        abilityPreviews.Add(goBoomRange);
    }
    
}
