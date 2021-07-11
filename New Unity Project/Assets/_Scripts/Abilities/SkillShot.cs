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
        createAbilityPreviews();
        currentDelay = 0;
        isFiring = false;
        AbilityUI.CardCanvasDim = GameFunctions.GetCanvas().GetChild(0).GetComponent<RectTransform>();
    }

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
                    GameObject projectile = abilityPrefabs.Find(go => go.name == preview.name);
                    float range = projectile.GetComponent<Projectile>().Range;
                    float distance = Vector3.Distance(position, abilityPreviewCanvas.transform.position);
                    if(distance > range) {
                        Vector3 distFromRadius = position - abilityPreviewCanvas.transform.position;
                        distFromRadius *= range/distance;
                        position = abilityPreviewCanvas.transform.position + distFromRadius;
                    }
                    preview.GetComponent<RectTransform>().position = position;
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
        }
        else { //if we completed a delay
            abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>().Fire(fireStartPosition, fireMousePosition, fireDirection);
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
                fireMousePosition.y = 5.1f; //
                fireDirection = fireStartPosition - fireMousePosition;
                isFiring = true;
                abilityUI.resetAbility();
            }
        }
    }    

    public void createAbilityPreviews() {

        List<GameObject> uniqueProjectiles = new List<GameObject>();

        foreach(GameObject projectile in abilityPrefabs) {
            if(!uniqueProjectiles.Contains(projectile)) {
                if(!projectile.GetComponent<Projectile>().IsGrenade) {
                    uniqueProjectiles.Add(projectile);

                    GameObject go = new GameObject(); //Create the GameObject
                    go.name = projectile.name;

                    Image previewImage = go.AddComponent<Image>(); //Add the Image Component script
                    previewImage.color = new Color32(255,255,255,100);
                    previewImage.sprite = abilityPreviewLine; //Set the Sprite of the Image Component on the new GameObject
                    previewImage.enabled = false;

                    float range = projectile.GetComponent<Projectile>().Range;
                    float width = projectile.GetComponent<Projectile>().Radius * 2;

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

                if(projectile.GetComponent<Projectile>().SelfDestructs || projectile.GetComponent<Projectile>().IsGrenade) { //we must also add the blow up range
                    GameObject goBoom = new GameObject(); //Create the GameObject
                    goBoom.name = projectile.name;

                    Image previewImageBoom = goBoom.AddComponent<Image>(); //Add the Image Component script
                    previewImageBoom.GetComponent<Image>().color = new Color32(255,255,255,100);
                    previewImageBoom.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
                    previewImageBoom.enabled = false;

                    float radius = projectile.GetComponent<Projectile>().ExplosionRadius;

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
