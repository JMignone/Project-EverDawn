using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
    private GameObject preview;
    private NavMeshAgent previewAgent;
    private int navMask; //determines what areas are sent into the function sample position
    private bool isFlying;
    private float radius;
    private RectTransform cardCanvasDim;
    private bool isBuffering;
    private Vector3 bufferingPosition;

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

    public GameObject Preview
    {
        get { return preview; }
        set { preview = value; }
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

    private void Start() 
    {
        cardCanvasDim = GameFunctions.GetCanvas().GetChild(0).GetComponent<RectTransform>();

        isBuffering = false;
        isDragging = false;
    }

    private void Update()
    {
        icon.sprite = cardInfo.Icon;
        cardName.text = cardInfo.Name;
        cost.text = cardInfo.Cost.ToString();

        transparentIcon.sprite = cardInfo.Icon;
        transparentCardName.text = cardInfo.Name;
        transparentCost.text = cardInfo.Cost.ToString();

        if(isBuffering && playerInfo.GetCurrResource >= cardInfo.Cost) { //if the player was buffering a card and now has enough resource
            SpawnUnit(bufferingPosition);
            isBuffering = false;
            playerInfo.DueResource -= cardInfo.Cost;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!playerInfo.OnDragging && !isDragging) {
            if(canDrag) {
                if(isBuffering) {
                    isBuffering = false;
                    playerInfo.DueResource -= cardInfo.Cost;
                    Destroy(preview);
                }

                GameObject go = Instantiate(cardInfo.PreviewPrefab[0]);
                preview = go;
                previewAgent = preview.transform.GetChild(0).GetComponent<NavMeshAgent>();
                radius = previewAgent.radius;

                isDragging = true;
                playerInfo.OnDragging = true;
                playerInfo.SpawnZone = true;

                preview.SetActive(false);

                Component damageable = cardInfo.Prefab[0].GetComponent(typeof(IDamageable));
                Component unit = damageable.gameObject.GetComponent(typeof(IDamageable));
                if((unit as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
                    IsFlying = true;
                else
                    IsFlying = false;

                navMask = 9;
                if((unit as IDamageable).Stats.UnitType == GameConstants.UNIT_TYPE.STRUCTURE) {
                    if(previewAgent.agentTypeID == 287145453) //the agent type id for big building
                        navMask = 32;
                    else
                        navMask = 16;
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

            if(Input.mousePosition.y > cardCanvasDim.rect.height)
                preview.SetActive(true);
            else
                preview.SetActive(false);

            NavMeshHit hit;
            Vector3 position = adjustForSpawnZones(GameFunctions.getPosition(isFlying));
            position = GameFunctions.adjustForBoundary(position);
            position = GameFunctions.adjustForTowers(position, radius);

            if(NavMesh.SamplePosition(position, out hit, 12f, navMask))
                previewAgent.Warp(hit.position);
            else
                previewAgent.Warp(position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(playerInfo.OnDragging && !isBuffering && isDragging) {
            NavMeshHit hit;
            Vector3 position = adjustForSpawnZones(GameFunctions.getPosition(isFlying));
            position = GameFunctions.adjustForBoundary(position);
            position = GameFunctions.adjustForTowers(position, radius);
            if(NavMesh.SamplePosition(position, out hit, 12f, navMask))
                position = hit.position;
            previewAgent.Warp(position);
            if(Input.mousePosition.y > cardCanvasDim.rect.height && playerInfo.GetCurrResource >= (cardInfo.Cost + playerInfo.DueResource)) //if the player isnt hovering the cancel zone and has enough resource
                SpawnUnit(position);
            else if(Input.mousePosition.y > cardCanvasDim.rect.height && playerInfo.GetCurrResource >= ( (cardInfo.Cost + playerInfo.DueResource) - 1) ) { //if the player isnt hovering the cancel zone and has 1 under enough resource
                isBuffering = true;
                bufferingPosition = position;
                playerInfo.DueResource += cardInfo.Cost;
            }
            else {
                transform.GetChild(3).localPosition = new Vector3(0,0,0);
                transform.GetChild(3).localScale = new Vector3(1,1,1);
                Destroy(preview);
            }
            isDragging = false;
            playerInfo.OnDragging = false;
            playerInfo.SpawnZone = false;
        }
    }

    private void SpawnUnit(Vector3 position)
    {
        if(playerInfo.GetCurrResource >= cardInfo.Cost) //do I need this if the call to this function requires this anyway? Just a santiy check maybe?
        {
            playerInfo.PlayersDeck.RemoveHand(cardInfo.Index);
            playerInfo.RemoveResource(cardInfo.Cost);

            GameFunctions.SpawnUnit(cardInfo.Prefab[0], GameManager.GetUnitsFolder(), position);

            Destroy(gameObject);
            Destroy(preview);
        }
    }

    //this function will fix the position of unit placment and the preview with the addition of the no place zones
    private Vector3 adjustForSpawnZones(Vector3 position) {
        float firstAreaBottom = playerInfo.LeftArea.transform.position.z - radius*2;
        float secondAreaBottom = playerInfo.TopArea.transform.position.z - radius*2;

        if(!playerInfo.LeftZone && !playerInfo.RightZone) { //If both zones are active
            if(position.z > firstAreaBottom)
                position = new Vector3(position.x, position.y, firstAreaBottom);
        }
        else if(!playerInfo.LeftZone){ //If only the left zone is active
            if(position.x < 0 && position.z > firstAreaBottom) { //AND the cursor is in the zone
                float distanceToBottom = Math.Abs(position.z - firstAreaBottom);
                float distanceToCenter = Math.Abs(position.x);
                if(distanceToBottom < distanceToCenter) //if the cursor is closer to the bottom edge of the no place zone
                    position = new Vector3(position.x, position.y, firstAreaBottom);
                else
                    position = new Vector3(0, position.y, position.z);
            }
        }
        else if(!playerInfo.RightZone) { //If only the right zone is active
            if(position.x > 0 && position.z > firstAreaBottom) { //AND the cursor is in the zone
                float distanceToBottom = Math.Abs(position.z - firstAreaBottom);
                float distanceToCenter = Math.Abs(position.x);
                if(distanceToBottom < distanceToCenter) //if the cursor is closer to the bottom edge of the no place zone
                    position = new Vector3(position.x, position.y, firstAreaBottom);
                else
                    position = new Vector3(0, position.y, position.z);
            }
        }
        if(position.z > secondAreaBottom)
            position = new Vector3(position.x, position.y, secondAreaBottom);
        return position;
    }

}
