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

    private List<GameObject> abilityPreviews;

    [SerializeField]
    private Sprite abilityPreviewLine;

    [SerializeField]
    private Sprite abilityPreviewBomb;

    [SerializeField]
    private PlayerStats playerInfo;

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

    void Start()
    {
        abilityPreviews = new List<GameObject>();
        createAbilityPreviews();
        currentDelay = 0;
        isFiring = false;
        AbilityUI.CardCanvasDim = GameFunctions.GetCanvas().GetChild(0).GetComponent<RectTransform>();
    }


    /*
        At the moment, a boomerang that selfdestructs and lingers at the end does not have the preview that id like (the linger circle around the unit wont be there,
        as the selfdestruct location takes precidence). But this kind of projectile is probably unlikely to happen anyway
    */
    private void Update() {
        abilityUI.UpdateStats();
        if(playerInfo.OnDragging && abilityPreviews[0].GetComponent<Image>().enabled == true) {
            Vector3 position = GameFunctions.getPosition(false);
            Vector3 direction = abilityPreviewCanvas.transform.position - position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(direction);
            abilityPreviewCanvas.transform.rotation = Quaternion.Lerp(rotation, abilityPreviewCanvas.transform.rotation, 0f);
            
            position.y++;
            foreach(GameObject preview in abilityPreviews) {
                if(preview.GetComponent<SphereCollider>()) {
                    Vector3 newPosition = position;
                    GameObject projectile = abilityPrefabs.Find(go => go.name == preview.name);
                    Projectile proj = projectile.GetComponent<Projectile>();
                    float range = proj.Range;
                    float radius = proj.Radius;
                    bool previewCircleAtEnd = !proj.GrenadeStats.IsGrenade && !proj.SelfDestructStats.SelfDestructs && 
                                              (proj.LingeringStats.Lingering && proj.LingeringStats.LingerAtEnd);
                                              //if its not a grenade, doesnt selfdestruct, and lingers at the end its true

                    bool previewCircleAtBeginning = previewCircleAtEnd && proj.BoomerangStats.IsBoomerang;

                    float distance = Vector3.Distance(position, abilityPreviewCanvas.transform.position);
                    if(previewCircleAtEnd && !previewCircleAtBeginning)
                        newPosition = abilityPreviewCanvas.transform.position + (direction.normalized * (range - radius) * -1); //locks the circle at the furthest position
                    else if(previewCircleAtBeginning)
                        newPosition = abilityPreviewCanvas.transform.position;
                    else if(distance > range - radius) {
                        Vector3 distFromRadius = position - abilityPreviewCanvas.transform.position;
                        distFromRadius *= (range - radius)/distance;
                        newPosition = abilityPreviewCanvas.transform.position + distFromRadius;
                    }
                    preview.GetComponent<RectTransform>().position = newPosition;
                }       
            }
        }
        else if(isFiring) {
            Fire();
        }
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
            GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], fireStartPosition, fireMousePosition, fireDirection, unit);
            currentDelay = 0;
            currentProjectileIndex++;
        }
    }

    private void LateUpdate() {
        abilityUI.LateUpdateStats();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!playerInfo.OnDragging) {
            if(abilityUI.CanDrag) {
                playerInfo.OnDragging = true;
                unit.IsHoveringAbility = true;

                abilityUI.AbilitySprite.enabled = false;
                abilityUI.AbilityCancel.enabled = true;

                foreach(GameObject preview in abilityPreviews) {
                    preview.GetComponent<Image>().enabled = true;
                    preview.GetComponent<Collider>().enabled = true;
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
            playerInfo.OnDragging = false;
            unit.IsHoveringAbility = false;

            abilityUI.AbilitySprite.enabled = true;
            abilityUI.AbilityCancel.enabled = false;

            foreach(GameObject preview in abilityPreviews) {
                preview.GetComponent<Image>().enabled = false;
                preview.GetComponent<Collider>().enabled = false;
            }

            GameManager.removeAbililtyIndicators();

            if(abilityUI.CardCanvasDim.rect.height < Input.mousePosition.y) {
                fireStartPosition = abilityPreviewCanvas.transform.position;
                fireMousePosition = GameFunctions.getPosition(false);
                fireStartPosition.y = 0;
                fireMousePosition.y = 0;
                fireDirection = fireMousePosition - fireStartPosition;
                isFiring = true;
                unit.IsCastingAbility = true;
                abilityUI.resetAbility();
            }
        }
    }    

    public void createAbilityPreviews() {

        List<GameObject> uniqueProjectiles = new List<GameObject>();

        foreach(GameObject goProj in abilityPrefabs) {
            if(!uniqueProjectiles.Contains(goProj)) {
                Projectile projectile = goProj.GetComponent<Projectile>();
                if(!projectile.GrenadeStats.IsGrenade) {
                    uniqueProjectiles.Add(goProj);


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
                
            }
        }
    }
    
}
