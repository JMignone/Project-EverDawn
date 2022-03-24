using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
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
    [SerializeField]
    private GameObject cardPlayer;

    private int layoutIndex;
    private bool isDragging;
    private bool specialDrag; //set to true if the card is being dragged via touching the field after the card was selected
    private bool abilityClicked; //set to true if a unit ability has been clicked, such that we cant special drag while this is true
    private Touch touch;
    private int touchId;
    private bool touched; //set to true if the user is using a touch screen and not a mouse
    private bool clickDelay; //set to true after the special drag is over for the end of an update cycle to prevent the player from deselecting the card
    private GameObject unitPreview;
    private NavMeshAgent unitPreviewAgent;
    private List<GameObject> abilityPreviews;
    private int navMask; //determines what areas are sent into the function sample position
    private bool isFlying;
    private float radius;
    private RectTransform cardCanvasDim;
    private float cardCanvasScale;
    private bool isBuffering;
    private Vector3 bufferingPosition;
    private bool played; //a flag determining if we have played our card and need a new one

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
        //set { transparentIcon = value; }
    }

    public Text TransparentCardName
    {
        get { return transparentCardName; }
        //set { transparentCardName = value; }
    }

    public Text TransparentCost
    {
        get { return transparentCost; }
        //set { transparentCost = value; }
    }

    public Image Icon
    {
        get { return icon; }
        //set { icon = value; }
    }

    public Text CardName
    {
        get { return cardName; }
        //set { cardName = value; }
    }

    public Text Cost
    {
        get { return cost; }
        //set { cost = value; }
    }

    public bool CanDrag
    {
        get { return canDrag; }
        set { canDrag = value; }
    }

    public int LayoutIndex
    {
        get { return layoutIndex; }
    }

    public GameObject UnitPreview
    {
        get { return unitPreview; }
    }

    public bool IsFlying
    {
        get { return isFlying; }
        set { isFlying = value; }
    }

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    public RectTransform CardCanvasDim
    {
        get { return cardCanvasDim; }
        set { cardCanvasDim = value; }
    }

    public float CardCanvasScale
    {
        get { return cardCanvasScale; }
        set { cardCanvasScale = value; }
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

    private void Awake()
    {
        for(int i=0; i < transform.parent.childCount; ++i) {
            if(transform.GetInstanceID() == transform.parent.GetChild(i).GetInstanceID()) {
                layoutIndex = i;
                break;
            }
        }

        abilityPreviewCanvas = playerInfo.PlayerCanvas;

        cardCanvasDim = GameFunctions.GetCanvas().GetChild(0).GetComponent<RectTransform>();
        cardCanvasScale = GameFunctions.GetCanvas().GetComponent<RectTransform>().localScale.x; //this is used to compensate for differing resolutions
    }

    public void GetNewCard() {
        cardInfo = playerInfo.PlayersDeck.DrawCard();
        if(!cardInfo.HiddenCard) {
            icon.sprite = cardInfo.Icon;
            cardName.text = cardInfo.Name;
            cost.text = cardInfo.Cost.ToString();

            transparentIcon.sprite = cardInfo.Icon;
            transparentCardName.text = cardInfo.Name;
            transparentCost.text = cardInfo.Cost.ToString();

            abilityPreviews = new List<GameObject>();
            createAbilityPreviews();
            foreach(GameObject preview in abilityPreviews)
                preview.SetActive(false);

            //work on unit prefab
            if(cardInfo.UnitIndex != -1) {
                unitPreview = Instantiate(cardInfo.PreviewPrefab);
                unitPreview.SetActive(false);
                unitPreviewAgent = unitPreview.transform.GetChild(0).GetComponent<NavMeshAgent>();
                radius = unitPreviewAgent.radius;
                                        
                Component unit = cardInfo.Prefab[cardInfo.UnitIndex].GetComponent(typeof(IDamageable));
                if((unit as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
                    IsFlying = true;
                else
                    IsFlying = false;

                if((unit as IDamageable).Stats.UnitType == GameConstants.UNIT_TYPE.STRUCTURE) {
                    if(unitPreviewAgent.agentTypeID == 287145453) //the agent type id for big building
                        navMask = 32;
                    else
                        navMask = 16;
                }
                else if((unit as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
                    navMask = 8;
                else
                    navMask = 1;
            }

            //work on spell prefabs
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

    private void FixedUpdate()
    {
        clickDelay = false;

        if(played) {
            played = false;
            GetNewCard();
        }

        if(!canDrag) {
            icon.sprite = cardInfo.Icon;
            cardName.text = cardInfo.Name;
            cost.text = cardInfo.Cost.ToString();

            transparentIcon.sprite = cardInfo.Icon;
            transparentCardName.text = cardInfo.Name;
            transparentCost.text = cardInfo.Cost.ToString();
        }

        if(isDragging && playerInfo.NumDragging == 1 && playerInfo.SelectedCardId != cardInfo.CardId)
            playerInfo.SelectNewCard(cardInfo.CardId);

        if(isBuffering && playerInfo.GetCurrResource >= cardInfo.Cost) { //if the player was buffering a card and now has enough resource
            QueCard(bufferingPosition);
            isBuffering = false;
            playerInfo.DueResource -= cardInfo.Cost;
        }
        
        //all of the below is to handle placing selected cards directly on the field, not by dragging them from the hand
        if(specialDrag) { //if we have already started the special dragging
            PointerEventData eventData = new PointerEventData (EventSystem.current);
            specialDrag = false;
            if(touched) {
                bool found = false;
                foreach(Touch t in Input.touches) {
                    if(t.fingerId == touchId) {
                        touch = t;
                        found = true;
                    }
                }
                eventData.position = touch.position;
                if(!found)
                    touch.phase = TouchPhase.Ended;
                switch(touch.phase) {
                    case TouchPhase.Moved:
                        ExecuteEvents.Execute(this.gameObject, eventData, ExecuteEvents.dragHandler);
                        specialDrag = true;
                        break;

                    case TouchPhase.Stationary:
                        specialDrag = true;
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        ExecuteEvents.Execute(this.gameObject, eventData, ExecuteEvents.endDragHandler);
                        clickDelay = true;
                        touched = false;
                        break;
                }
            }
            else {
                eventData.position = Input.mousePosition;
                if(Input.GetMouseButton(0)) { //if we are still dragging
                    ExecuteEvents.Execute(this.gameObject, eventData, ExecuteEvents.dragHandler);
                    specialDrag = true;
                }
                else {
                    ExecuteEvents.Execute(this.gameObject, eventData, ExecuteEvents.endDragHandler);
                    clickDelay = true;
                }
            }
        }
        else {
            if(!Input.GetMouseButton(0) && Input.touchCount == 0 && abilityClicked)
                abilityClicked = false;

            Vector2 pos = Vector2.zero;
            if(Input.touchCount > 0) {
                touch = Input.GetTouch(Input.touchCount-1);
                pos = touch.position;
                touchId = touch.fingerId;
                touched = true;
            }
            else if(Input.GetMouseButton(0))
                pos = Input.mousePosition;

            if((Input.GetMouseButton(0) || Input.touchCount > 0) && playerInfo.NumDragging == 0 && !isDragging && canDrag && playerInfo.SelectedCardId == cardInfo.CardId && pos.y > cardCanvasDim.rect.height*cardCanvasScale && !abilityClicked) {
                PointerEventData eventData = new PointerEventData (EventSystem.current);
                eventData.position = pos;

                abilityClicked = false;
                List<RaycastResult> raysastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, raysastResults);
                foreach(RaycastResult raysastResult in raysastResults) {
                    if(raysastResult.gameObject.name == "CooldownSprite") {
                        abilityClicked = true;
                        break;
                    }
                }

                if(!abilityClicked) {
                    ExecuteEvents.Execute(this.gameObject, eventData, ExecuteEvents.beginDragHandler);
                    ExecuteEvents.Execute(this.gameObject, eventData, ExecuteEvents.dragHandler);
                    specialDrag = true;
                }
            }
            else
                touched = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //if(!playerInfo.OnDragging && !isDragging) {
        if(!isDragging && !specialDrag) {
            if(canDrag) {
                isDragging = true;
                //SelectCard();
                if(playerInfo.NumDragging == 0)
                    playerInfo.SelectNewCard(cardInfo.CardId);

                //if the card is buffering, cancel the cast
                if(isBuffering) {
                    isBuffering = false;
                    playerInfo.DueResource -= cardInfo.Cost;
                    foreach(GameObject preview in abilityPreviews)
                        preview.SetActive(false);
                    if(cardInfo.UnitIndex != -1)
                        unitPreview.SetActive(false);

                    //re-add the colliders to the spell
                    foreach(GameObject preview in abilityPreviews) {
                        if(preview.transform.childCount > 0)
                            preview.transform.GetChild(1).GetChild(0).GetComponent<Collider>().enabled = true;
                        else {
                            if(preview.GetComponent<Collider>())
                                preview.GetComponent<Collider>().enabled = true;
                        }
                    }
                }
                
                //playerInfo.OnDragging = true;
                playerInfo.NumDragging++;
                transform.GetChild(3).position = eventData.position;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if(playerInfo.OnDragging && !isBuffering && isDragging) {
        if(!isBuffering && isDragging && !specialDrag) {
            //transform.GetChild(3).position = Input.mousePosition;
            transform.GetChild(3).position = eventData.position;

            //float scale = (cardCanvasDim.rect.height*cardCanvasScale - Input.mousePosition.y)/(cardCanvasDim.rect.height*cardCanvasScale - transform.position.y);
            float scale = (cardCanvasDim.rect.height*cardCanvasScale - eventData.position.y)/(cardCanvasDim.rect.height*cardCanvasScale - transform.position.y); 
            if(scale > 1)
                scale = 1;
            else if(scale < 0)
                scale = 0;
            transform.GetChild(3).localScale = new Vector3(scale, scale, 1);

            //if(Input.mousePosition.y > cardCanvasDim.rect.height*cardCanvasScale) {
            if(eventData.position.y > cardCanvasDim.rect.height*cardCanvasScale) {
                foreach(GameObject preview in abilityPreviews)
                    preview.SetActive(true);
                if(cardInfo.UnitIndex != -1)
                    unitPreview.SetActive(true);
            }
            else {
                foreach(GameObject preview in abilityPreviews)
                    preview.SetActive(false);
                if(cardInfo.UnitIndex != -1)
                    unitPreview.SetActive(false);
                if(cardInfo.Prefab.Count != 1 || cardInfo.UnitIndex != 0)
                    GameManager.removeAbililtyIndicators();
            }

            NavMeshHit hit;
            Vector3 position = adjustForSpawnZones(GameFunctions.getPosition(isFlying, eventData.position));
            position = GameFunctions.adjustForBoundary(position);
            if(cardInfo.UnitIndex != -1)
                position = GameFunctions.adjustForTowers(position, radius);
            
            if(cardInfo.UnitIndex != -1) {
                if(NavMesh.SamplePosition(position, out hit, GameConstants.SAMPLE_POSITION_RADIUS, navMask))
                    position = hit.position;
                unitPreviewAgent.Warp(position);
                position = unitPreviewAgent.transform.position;
            }
            position.y = .1f;

            foreach(GameObject preview in abilityPreviews) {
                GameObject go = cardInfo.Prefab.Find(go => go.name == preview.name);
                if(go.GetComponent<CreateAtLocation>() && go.GetComponent<CreateAtLocation>().LinearStats.IsLinear) { //this is a cal linear typed preview and needs special treatment
                    preview.GetComponent<RectTransform>().rotation = Quaternion.Euler(90f, 0f, 0f);
                    RectTransform previewRect = preview.GetComponent<RectTransform>();
                    if(previewRect.sizeDelta.x < previewRect.sizeDelta.y) //this is the vertical component
                        preview.GetComponent<RectTransform>().position = new Vector3(position.x, 1, 0); 
                    else //this is the horizontal component
                        preview.GetComponent<RectTransform>().position = new Vector3(0, 1, position.z); 
                }
                else
                    preview.GetComponent<RectTransform>().position = position;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if(playerInfo.OnDragging && !isBuffering && isDragging) {
        if(!isBuffering && isDragging && !specialDrag) {
            NavMeshHit hit;
            Vector3 position = adjustForSpawnZones(GameFunctions.getPosition(isFlying, eventData.position));
            position = GameFunctions.adjustForBoundary(position);
            if(cardInfo.UnitIndex != -1)
                position = GameFunctions.adjustForTowers(position, radius);

            if(cardInfo.UnitIndex != -1) {
                if(NavMesh.SamplePosition(position, out hit, GameConstants.SAMPLE_POSITION_RADIUS, navMask))
                    position = hit.position;
                unitPreviewAgent.Warp(position);
                position = unitPreviewAgent.transform.position;
            }
            position.y = .1f;

            //remove the colliders from the spell
            foreach(GameObject preview in abilityPreviews) {
                if(preview.transform.childCount > 0)
                    preview.transform.GetChild(1).GetChild(0).GetComponent<Collider>().enabled = false;
                else {
                    if(preview.GetComponent<Collider>())
                        preview.GetComponent<Collider>().enabled = false;
                }
            }

            //if(Input.mousePosition.y > cardCanvasDim.rect.height*cardCanvasScale && playerInfo.GetCurrResource >= (cardInfo.Cost + playerInfo.DueResource)) { //if the player isnt hovering the cancel zone and has enough resource
            if(eventData.position.y > cardCanvasDim.rect.height*cardCanvasScale && playerInfo.GetCurrResource >= (cardInfo.Cost + playerInfo.DueResource)) { //if the player isnt hovering the cancel zone and has enough resource
                QueCard(position);
                if(playerInfo.SelectedCardId == cardInfo.CardId)
                    playerInfo.SelectNewCard(-1);
            }
            //else if(Input.mousePosition.y > cardCanvasDim.rect.height*cardCanvasScale && playerInfo.GetCurrResource >= ( (cardInfo.Cost + playerInfo.DueResource) - 1) ) { //if the player isnt hovering the cancel zone and has 1 under enough resource
            else if(eventData.position.y > cardCanvasDim.rect.height*cardCanvasScale && playerInfo.GetCurrResource >= ( (cardInfo.Cost + playerInfo.DueResource) - 1) ) { //if the player isnt hovering the cancel zone and has 1 under enough resource
                isBuffering = true;
                bufferingPosition = position;
                playerInfo.DueResource += cardInfo.Cost;
                if(playerInfo.SelectedCardId == cardInfo.CardId)
                    playerInfo.SelectNewCard(-1);
            }
            else {
                transform.GetChild(3).localPosition = new Vector3(0,0,0);
                transform.GetChild(3).localScale = new Vector3(1,1,1);
                foreach(GameObject preview in abilityPreviews) {
                    preview.SetActive(false);
                    if(preview.transform.childCount > 0)
                        preview.transform.GetChild(1).GetChild(0).GetComponent<Collider>().enabled = true;
                    else {
                        if(preview.GetComponent<Collider>())
                            preview.GetComponent<Collider>().enabled = true;
                    }
                }

                if(cardInfo.UnitIndex != -1)
                    unitPreview.SetActive(false);
            }
            isDragging = false;

            //if(playerInfo.SelectedCardId == -1)
            //    playerInfo.OnDragging = false;
            playerInfo.NumDragging--;
            

            if(cardInfo.Prefab.Count != 1 || cardInfo.UnitIndex != 0)
                GameManager.removeAbililtyIndicators();
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        if(playerInfo.NumDragging == 0 && !isDragging && canDrag && !specialDrag && !clickDelay) {
            transform.GetChild(3).localPosition = new Vector3(0,0,0);
            transform.GetChild(3).localScale = new Vector3(1,1,1);

            if(playerInfo.SelectedCardId != cardInfo.CardId) //if we selected a new card
                playerInfo.SelectNewCard(cardInfo.CardId);
            else //else we are deselecting a card and need to check if its buffering
                playerInfo.SelectNewCard(-1);     
            
            if(isBuffering) {
                isBuffering = false;
                playerInfo.DueResource -= cardInfo.Cost;
                foreach(GameObject preview in abilityPreviews)
                    preview.SetActive(false);
                if(cardInfo.UnitIndex != -1)
                    unitPreview.SetActive(false);
                //re-add the colliders to the spell
                foreach(GameObject preview in abilityPreviews) {
                    if(preview.transform.childCount > 0)
                        preview.transform.GetChild(1).GetChild(0).GetComponent<Collider>().enabled = true;
                    else {
                        if(preview.GetComponent<Collider>())
                            preview.GetComponent<Collider>().enabled = true;
                    }
                }
            }
        }
    }

    public void QueCard(Vector3 position) {
        //playerInfo.SpawnZone = GameConstants.SPAWN_ZONE_RESTRICTION.NONE;
        //playerInfo.SelectNewCard(-1);

        playerInfo.PlayersDeck.RemoveHand(cardInfo.CardId);
        playerInfo.RemoveResource(cardInfo.Cost);

        GameObject go = Instantiate(cardPlayer);
        CardPlayer cardPlayerStats = go.GetComponent<CardPlayer>();
        cardPlayerStats.Prefab = cardInfo.Prefab;
        cardPlayerStats.PreviewDelays = cardInfo.PreviewDelays;
        cardPlayerStats.TargetLocation = position;
        cardPlayerStats.UnitIndex = cardInfo.UnitIndex;
        cardPlayerStats.PlayerTag = playerInfo.gameObject.tag;

        played = true;

        if(!cardInfo.HiddenCard) {
            transform.GetChild(3).localPosition = new Vector3(0,0,0);
            transform.GetChild(3).localScale = new Vector3(1,1,1);

            foreach(GameObject preview in abilityPreviews)
                Destroy(preview);
            if(cardInfo.UnitIndex != -1)
                Destroy(unitPreview);

            if(cardInfo.UnitIndex != -1)
                unitPreview.SetActive(false);
        }
    }

    //this function will fix the position of unit placment and the preview with the addition of the no place zones
    private Vector3 adjustForSpawnZones(Vector3 position) {
        float firstAreaBottom = playerInfo.LeftArea.transform.position.z - radius;
        float secondAreaBottom = playerInfo.TopArea.transform.position.z - radius;
        if(cardInfo.SpawnZoneRestrictions == GameConstants.SPAWN_ZONE_RESTRICTION.FULL && !playerInfo.LeftZone && !playerInfo.RightZone) { //If both zones are active
            if(position.z > firstAreaBottom)
                position = new Vector3(position.x, position.y, firstAreaBottom);
        }
        else if(cardInfo.SpawnZoneRestrictions == GameConstants.SPAWN_ZONE_RESTRICTION.FULL && !playerInfo.LeftZone){ //If only the left zone is active
            if(position.x < radius && position.z > firstAreaBottom) { //AND the cursor is in the zone
                float distanceToBottom = Math.Abs(position.z - firstAreaBottom);
                float distanceToCenter = Math.Abs(position.x)+radius;
                if(distanceToBottom < distanceToCenter) //if the cursor is closer to the bottom edge than the middler edge of the no place zone
                    position = new Vector3(position.x, position.y, firstAreaBottom);
                else 
                    position = new Vector3(radius, position.y, position.z);
            }
            if(position.z > secondAreaBottom)
                position = new Vector3(position.x, position.y, secondAreaBottom);
        }
        else if(cardInfo.SpawnZoneRestrictions == GameConstants.SPAWN_ZONE_RESTRICTION.FULL && !playerInfo.RightZone) { //If only the right zone is active
            if(position.x > -radius && position.z > firstAreaBottom) { //AND the cursor is in the zone
                float distanceToBottom = Math.Abs(position.z - firstAreaBottom);
                float distanceToCenter = Math.Abs(position.x)+radius;
                if(distanceToBottom < distanceToCenter) //if the cursor is closer to the bottom edge of the no place zone
                    position = new Vector3(position.x, position.y, firstAreaBottom);
                else
                    position = new Vector3(-radius, position.y, position.z);
            }
            if(position.z > secondAreaBottom)
                position = new Vector3(position.x, position.y, secondAreaBottom);
        }
        else if(cardInfo.SpawnZoneRestrictions != GameConstants.SPAWN_ZONE_RESTRICTION.NONE && position.z > secondAreaBottom)
            position = new Vector3(position.x, position.y, secondAreaBottom);
        return position;
    }

    public void createAbilityPreviews() {
        List<GameObject> uniqueProjectiles = new List<GameObject>();
        for(int i=0; i<cardInfo.Prefab.Count; i++) {
            if(i != cardInfo.UnitIndex) {
                if(!uniqueProjectiles.Contains(cardInfo.Prefab[i])) {
                    uniqueProjectiles.Add(cardInfo.Prefab[i]);
                    if(cardInfo.Prefab[i].GetComponent<Projectile>())
                        createProjectilePreview(cardInfo.Prefab[i]);
                    else if(cardInfo.Prefab[i].GetComponent<CreateAtLocation>())
                        createCALPreview(cardInfo.Prefab[i]);
                }
            }
        }
    }

    private void createProjectilePreview(GameObject goProj) {
        Projectile projectile = goProj.GetComponent<Projectile>();
        if(!projectile.GrenadeStats.IsGrenade) {

            GameObject go = new GameObject(); //Create the GameObject
            go.name = goProj.name;

            AbilityPreview aPrev = go.AddComponent<AbilityPreview>();
            aPrev.HeightAttackable = projectile.HeightAttackable;
            aPrev.TypeAttackable = projectile.TypeAttackable;

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

            AbilityPreview aPrev = goBoom.AddComponent<AbilityPreview>();
            aPrev.HeightAttackable = projectile.HeightAttackable;
            aPrev.TypeAttackable = projectile.TypeAttackable;

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

            float radius;
            if(cal.SummonStats.Size == GameConstants.SUMMON_SIZE.BIG)
                radius = 6;
            else 
                radius = 3;

            previewImageBoom.enabled = false;
            previewBoomTransform.sizeDelta = new Vector2(radius*2, radius*2);

            go.transform.SetParent(abilityPreviewCanvas.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.

            go.SetActive(true);
            abilityPreviews.Add(go);
        }
        if(cal.LinearStats.IsLinear) {          
            float width = cal.LinearStats.ExplosionWidth;

            if(cal.LinearStats.IsVertical || (!cal.LinearStats.IsVertical && !cal.LinearStats.IsHorizontal) ) {//if the linear attack has the vertical component
                GameObject goLinearVert = new GameObject();
                goLinearVert.name = goCAL.name;

                AbilityPreview aPrev = goLinearVert.AddComponent<AbilityPreview>();
                aPrev.HeightAttackable = cal.HeightAttackable;
                aPrev.TypeAttackable = cal.TypeAttackable;

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

                AbilityPreview aPrev = goLinearHorz.AddComponent<AbilityPreview>();
                aPrev.HeightAttackable = cal.HeightAttackable;
                aPrev.TypeAttackable = cal.TypeAttackable;

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
        else if(cal.LingeringStats.Lingering || cal.SelfDestructStats.SelfDestructs) {
            GameObject goBoom = new GameObject(); //Create the GameObject
            goBoom.name = goCAL.name;

            AbilityPreview aPrev = goBoom.AddComponent<AbilityPreview>();
            aPrev.HeightAttackable = cal.HeightAttackable;
            aPrev.TypeAttackable = cal.TypeAttackable;

            Image previewImageBoom = goBoom.AddComponent<Image>(); //Add the Image Component script
            previewImageBoom.GetComponent<Image>().color = new Color32(255,255,255,100);
            previewImageBoom.sprite = abilityPreviewBomb; //Set the Sprite of the Image Component on the new GameObject
            previewImageBoom.enabled = false;

            float radius = Math.Max(cal.LingeringStats.LingeringRadius, cal.SelfDestructStats.ExplosionRadius);

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
