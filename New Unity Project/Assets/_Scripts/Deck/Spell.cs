using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Spell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private PlayerStats playerInfo;
    [SerializeField]
    private CardStats cardInfo;
    [SerializeField]
    private Image transparentIcon;
    [SerializeField]
    private Text transparentCardName;
    [SerializeField]
    private Text transparentCost;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Text cardName;
    [SerializeField]
    private Text cost;
    [SerializeField]
    private bool canDrag;

    private bool isDragging;
    private List<GameObject> abilityPreviews;
    private RectTransform cardCanvasDim;
    private bool isBuffering;
    private Vector3 bufferingPosition;

    //ability stats below
    private Canvas abilityPreviewCanvas;

    [SerializeField]
    private Sprite abilityPreviewLine;

    [SerializeField]
    private Sprite abilityPreviewLinear;

    [SerializeField]
    private Sprite abilityPreviewBomb;

    [SerializeField]
    private Sprite abilityPreviewRange;

    private float currentDelay;
    private bool isFiring;
    private int currentProjectileIndex;
    private Vector3 targetLocation;

    public PlayerStats PlayerInfo
    {
        get { return playerInfo; }
        set { playerInfo = value; }
    }

    public CardStats CardInfo
    {
        get { return cardInfo; }
        set { cardInfo = value; }
    }

    public Image TransparentIcon
    {
        get { return transparentIcon; }
    }

    public Text TransparentCardName
    {
        get { return transparentCardName; }
    }

    public Text TransparentCost
    {
        get { return transparentCost; }
    }

    public Image Icon
    {
        get { return icon; }
    }

    public Text CardName
    {
        get { return cardName; }
    }

    public Text Cost
    {
        get { return cost; }
    }

    public bool CanDrag
    {
        get { return canDrag; }
        set { canDrag = value; }
    }

    public List<GameObject> AbilityPreviews
    {
        get { return abilityPreviews; }
        set { abilityPreviews = value; }
    }

    public RectTransform CardCanvasDim
    {
        get { return cardCanvasDim; }
        set { cardCanvasDim = value; }
    }

    public bool IsBuffering
    {
        get { return isBuffering; }
        set { isBuffering = value; }
    }

    public Vector3 BufferingPosition
    {
        get { return bufferingPosition; }
        set { bufferingPosition = value; }
    }

    public Canvas AbilityPreviewCanvas
    {
        get { return abilityPreviewCanvas; }
        //set { abilityPreviewCanvas = value; }
    }

    public Sprite AbilityPreviewLine
    {
        get { return abilityPreviewLine; }
    }

    public Sprite AbilityPreviewLinear
    {
        get { return abilityPreviewLinear; }
    }

    public Sprite AbilityPreviewBomb
    {
        get { return abilityPreviewBomb; }
    }

    public Sprite AbilityPreviewRange
    {
        get { return abilityPreviewRange; }
    }

    public bool IsFiring
    {
        get { return isFiring; }
    }

    public float CurrentDelay
    {
        get { return currentDelay; }
        set { currentDelay = value; }
    }

    public int CurrentProjectileIndex
    {
        get { return currentProjectileIndex; }
        set { currentProjectileIndex = value; }
    }

    private void Start() 
    {
        abilityPreviewCanvas = playerInfo.PlayerCanvas;
        abilityPreviews = new List<GameObject>();
        createAbilityPreviews();
        foreach(GameObject preview in abilityPreviews)
            preview.SetActive(false);

        cardCanvasDim = GameFunctions.GetCanvas().GetChild(0).GetComponent<RectTransform>();

        isBuffering = false;
        isDragging = false;
        isFiring = false;
        currentDelay = 0;
    }

    private void Update()
    {
        if(!isFiring) {
            icon.sprite = cardInfo.Icon;
            cardName.text = cardInfo.Name;
            cost.text = cardInfo.Cost.ToString();

            transparentIcon.sprite = cardInfo.Icon;
            transparentCardName.text = cardInfo.Name;
            transparentCost.text = cardInfo.Cost.ToString();

            if(isBuffering && playerInfo.GetCurrResource >= cardInfo.Cost) { //if the player was buffering a card and now has enough resource
                QueSpells(bufferingPosition);
                isBuffering = false;
                playerInfo.DueResource -= cardInfo.Cost;
            }
        }
        else
            Fire();
    }

    private void Fire() {
        if(currentDelay < cardInfo.PreviewDelays[currentProjectileIndex]) //if we havnt reached the delay yet
            currentDelay += Time.deltaTime;
        else { //if we completed a delay
            if(cardInfo.Prefab[currentProjectileIndex].GetComponent<Projectile>())
                GameFunctions.FireProjectile(cardInfo.Prefab[currentProjectileIndex], targetLocation, targetLocation, new Vector3(0,0,1), null);
            else if(cardInfo.Prefab[currentProjectileIndex].GetComponent<CreateAtLocation>())
                GameFunctions.FireCAL(cardInfo.Prefab[currentProjectileIndex], targetLocation, targetLocation, new Vector3(0,0,1), null);
            currentDelay = 0;
            currentProjectileIndex++;
            if(currentProjectileIndex == cardInfo.PreviewDelays.Count)//if we completed the last delay
                Destroy(gameObject);
        }
    }

    private void QueSpells(Vector3 position) {
        if(playerInfo.GetCurrResource >= cardInfo.Cost) //do I need this if the call to this function requires this anyway? Just a santiy check maybe?
        {
            playerInfo.PlayersDeck.RemoveHand(cardInfo.Index);
            playerInfo.RemoveResource(cardInfo.Cost);

            //gameObject.SetActive(false);
            gameObject.transform.SetParent(playerInfo.QuedSpells);

            foreach(GameObject preview in abilityPreviews)
                Destroy(preview);

            targetLocation = position;
            isFiring = true;
        }
    }

    public void createAbilityPreviews() {
        List<GameObject> uniqueProjectiles = new List<GameObject>();

        foreach(GameObject goAbility in cardInfo.Prefab) {
            if(!uniqueProjectiles.Contains(goAbility)) {
                uniqueProjectiles.Add(goAbility);
                if(goAbility.GetComponent<Projectile>())
                    createProjectilePreview(goAbility);
                else if(goAbility.GetComponent<CreateAtLocation>())
                    createCALPreview(goAbility);
        
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!playerInfo.OnDragging && !isDragging) {
            if(canDrag) {
                if(isBuffering) {
                    isBuffering = false;
                    playerInfo.DueResource -= cardInfo.Cost;
                }

                isDragging = true;
                playerInfo.OnDragging = true;
                //playerInfo.SpawnZone = true;

                foreach(GameObject preview in abilityPreviews) {
                    preview.SetActive(false);
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
        if(playerInfo.OnDragging && !isBuffering && isDragging) {
            transform.GetChild(3).position = Input.mousePosition;

            float scale = (cardCanvasDim.rect.height - Input.mousePosition.y)/(cardCanvasDim.rect.height - transform.position.y); 
            if(scale > 1)
                scale = 1;
            else if(scale < 0)
                scale = 0;
            transform.GetChild(3).localScale = new Vector3(scale, scale, 1);

            if(Input.mousePosition.y > cardCanvasDim.rect.height) {
                foreach(GameObject preview in abilityPreviews)
                    preview.SetActive(true);
            }
            else {
                foreach(GameObject preview in abilityPreviews)
                    preview.SetActive(false);
                GameManager.removeAbililtyIndicators();
            }

            Vector3 position = GameFunctions.adjustForBoundary(GameFunctions.getPosition(false));
            position.y = 1;
            foreach(GameObject preview in abilityPreviews) {
                GameObject go = cardInfo.Prefab.Find(go => go.name == preview.name);
                if(go.GetComponent<CreateAtLocation>()) { 
                    if(go.GetComponent<CreateAtLocation>().LinearStats.IsLinear) { //this is a cal linear typed preview and needs special treatment
                        preview.GetComponent<RectTransform>().rotation = Quaternion.Euler(90f, 0f, 0f);
                        RectTransform previewRect = preview.GetComponent<RectTransform>();
                        if(previewRect.sizeDelta.x < previewRect.sizeDelta.y) //this is the vertical component
                            preview.GetComponent<RectTransform>().position = new Vector3(position.x, 1, 0); 
                        else //this is the horizontal component
                            preview.GetComponent<RectTransform>().position = new Vector3(0, 1, position.z); 
                    }
                }
                else
                    preview.GetComponent<RectTransform>().position = position;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        if(playerInfo.OnDragging && !isBuffering && isDragging) {
            Vector3 position = GameFunctions.adjustForBoundary(GameFunctions.getPosition(false));
            foreach(GameObject preview in abilityPreviews)
                preview.GetComponent<RectTransform>().position = position;
            if(Input.mousePosition.y > cardCanvasDim.rect.height && playerInfo.GetCurrResource >= (cardInfo.Cost + playerInfo.DueResource)) //if the player isnt hovering the cancel zone and has enough resource
                QueSpells(position);
            else if(Input.mousePosition.y > cardCanvasDim.rect.height && playerInfo.GetCurrResource >= ( (cardInfo.Cost + playerInfo.DueResource) - 1) ) { //if the player isnt hovering the cancel zone and has 1 under enough resource
                isBuffering = true;
                bufferingPosition = position;
                playerInfo.DueResource += cardInfo.Cost;
            }
            else {
                transform.GetChild(3).localPosition = new Vector3(0,0,0);
                transform.GetChild(3).localScale = new Vector3(1,1,1);
                foreach(GameObject preview in abilityPreviews)
                    preview.SetActive(false);
            }
            isDragging = false;
            playerInfo.OnDragging = false;
            //playerInfo.SpawnZone = false;

            GameManager.removeAbililtyIndicators();
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
            previewHitBox.center = new Vector3(0, range/2, -.5f);
            previewHitBox.enabled = false;

            go.tag = "AbilityHighlight";
            RectTransform previewTransform = go.GetComponent<RectTransform>();
            previewTransform.anchorMin = new Vector2(.5f, 0);
            previewTransform.anchorMax = new Vector2(.5f, 0);
            previewTransform.pivot = new Vector2(.5f, 0);
            previewTransform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
            //previewTransform.localPosition = new Vector3(0, -4, -1 * range/2);
            previewTransform.localRotation = Quaternion.Euler(0, 0, 0);
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
            previewBoomTransform.localRotation = Quaternion.Euler(0, 0, 0);
            previewBoomTransform.sizeDelta = new Vector2(radius*2, radius*2);
            
            goBoom.SetActive(true);
            abilityPreviews.Add(goBoom);
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
        if(cal.LinearStats.IsLinear) {          
            float width = cal.LinearStats.ExplosionWidth;

            if(cal.LinearStats.IsVertical || (!cal.LinearStats.IsVertical && !cal.LinearStats.IsHorizontal) ) {//if the linear attack has the vertical component
                GameObject goLinearVert = new GameObject();
                goLinearVert.name = goCAL.name;

                Image previewImageLinearVert = goLinearVert.AddComponent<Image>(); //Add the Image Component script
                previewImageLinearVert.GetComponent<Image>().color = new Color32(255,255,255,100);
                previewImageLinearVert.sprite = abilityPreviewLinear; //Set the Sprite of the Image Component on the new GameObject
                previewImageLinearVert.enabled = false;

                BoxCollider previewHitBoxLinearVert = goLinearVert.AddComponent<BoxCollider>();
                previewHitBoxLinearVert.size = new Vector3(width, GameManager.Instance.Ground.transform.localScale.z * 10, .5f);
                previewHitBoxLinearVert.center = new Vector3(0, 0, 0);
                previewHitBoxLinearVert.enabled = false;

                goLinearVert.tag = "AbilityHighlight";
                RectTransform previewLinearTransformVert =  goLinearVert.GetComponent<RectTransform>();
                previewLinearTransformVert.anchorMin = new Vector2(.5f, 0);
                previewLinearTransformVert.anchorMax = new Vector2(.5f, 0);
                previewLinearTransformVert.pivot = new Vector2(.5f, .5f);
                previewLinearTransformVert.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
                previewLinearTransformVert.localPosition = new Vector3(0, -5, 0);
                previewLinearTransformVert.localRotation = Quaternion.Euler(0, 0, 0);
                previewLinearTransformVert.sizeDelta = new Vector2(width, GameManager.Instance.Ground.transform.localScale.z * 10);
                abilityPreviews.Add(goLinearVert);
            }
            if(cal.LinearStats.IsHorizontal) { //if the linear attack has the horizontal component
                GameObject goLinearHorz = new GameObject();
                goLinearHorz.name = goCAL.name;

                Image previewImageLinearHorz = goLinearHorz.AddComponent<Image>(); //Add the Image Component script
                previewImageLinearHorz.GetComponent<Image>().color = new Color32(255,255,255,100);
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
                previewLinearTransformHorz.localPosition = new Vector3(0, -5, 0);
                previewLinearTransformHorz.localRotation = Quaternion.Euler(0, 0, 0);
                previewLinearTransformHorz.sizeDelta = new Vector2(GameManager.Instance.Ground.transform.localScale.x * 10, width);
                abilityPreviews.Add(goLinearHorz);
            }      
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
            previewBoomTransform.localRotation = Quaternion.Euler(0, 0, 0);
            previewBoomTransform.sizeDelta = new Vector2(radius*2, radius*2);

            goBoom.SetActive(true);
            abilityPreviews.Add(goBoom);
        }
    }
}
