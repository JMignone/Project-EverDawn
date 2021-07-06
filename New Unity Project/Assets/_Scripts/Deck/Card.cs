using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private GameObject preview;
    private bool isFlying;
    private RectTransform cardCanvasDim;

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

    public RectTransform CardCanvasDim
    {
        get { return cardCanvasDim; }
        set { cardCanvasDim = value; }
    }

    private void Start() 
    {
        cardCanvasDim = GameFunctions.GetCanvas().GetChild(0).GetComponent<RectTransform>();;
    }

    private void Update()
    {
        icon.sprite = cardInfo.Icon;
        cardName.text = cardInfo.Name;
        cost.text = cardInfo.Cost.ToString();

        transparentIcon.sprite = cardInfo.Icon;
        transparentCardName.text = cardInfo.Name;
        transparentCost.text = cardInfo.Cost.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!playerInfo.OnDragging) {
            if(canDrag) {
                playerInfo.OnDragging = true;
                playerInfo.SpawnZone = true;

                GameObject go = Instantiate(cardInfo.PreviewPrefab);
                preview = go;

                preview.SetActive(false);

                Component damageable = cardInfo.Prefab.GetComponent(typeof(IDamageable));
                Component unit = damageable.gameObject.GetComponent(typeof(IDamageable));
                if((unit as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
                    IsFlying = true;
                else
                    IsFlying = false;

            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(playerInfo.OnDragging) {
            transform.GetChild(3).position = Input.mousePosition;

            float scale = (cardCanvasDim.rect.height - Input.mousePosition.y)/(cardCanvasDim.rect.height - transform.position.y); 
            if(scale > 1)
                scale = 1;
            else if(scale < 0)
                scale = 0;
            transform.GetChild(3).localScale = new Vector3(scale, scale, 1);

            /* 
                The function SamplePosition takes in a point, and it will swarch for the closest point on the navmesh to it.
                For a preview unit, this is very useful. If the preview is aleady on the navmesh, then it takes that point,
                but if the user has placed the unit over the river, it will place the preview to the closest position
                it should be. This will also be used for placing the actual unit as well. It works exactly how we want it to.

                The '9' in the function is the area mask, as a binary number. This mask includes 'walkable=1' and 'flyable=8'
            */
            if(Input.mousePosition.y > cardCanvasDim.rect.height)
                preview.SetActive(true);
            else
                preview.SetActive(false);

            UnityEngine.AI.NavMeshHit hit;
            Vector3 position = adjustForSpawnZones(getPosition());
            position = adjustForTowers(position);
            if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 6.1f, 9))
                preview.transform.GetChild(0).GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(hit.position);
            else
                preview.transform.GetChild(0).GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(playerInfo.OnDragging) {
            UnityEngine.AI.NavMeshHit hit;
            Vector3 position = adjustForSpawnZones(getPosition());
            position = adjustForTowers(position);
            if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 6.1f, 9))
                preview.transform.GetChild(0).GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(hit.position);
            else
                preview.transform.GetChild(0).GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(position);
            if(Input.mousePosition.y > cardCanvasDim.rect.height && playerInfo.GetCurrResource >= cardInfo.Cost) //only works at a specific resolution, but its a proof of concept
                SpawnUnit();
            else {
                transform.GetChild(3).localPosition = new Vector3(0,0,0);
                transform.GetChild(3).localScale = new Vector3(1,1,1);
                Destroy(preview);
            }
            playerInfo.OnDragging = false;
            playerInfo.SpawnZone = false;
        }
    }

    private void SpawnUnit()
    {
        if(playerInfo.GetCurrResource >= cardInfo.Cost) //do I need this if the call to this function requires this anyway? Just a santiy check maybe?
        {
            playerInfo.PlayersDeck.RemoveHand(cardInfo.Index);
            playerInfo.RemoveResource(cardInfo.Cost);

            UnityEngine.AI.NavMeshHit hit;
            Vector3 position = adjustForSpawnZones(getPosition());
            position = adjustForTowers(position);
            if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 6.1f, 9))
                GameFunctions.SpawnUnit(cardInfo.Prefab, GameManager.GetUnitsFolder(), hit.position);
            else
                GameFunctions.SpawnUnit(cardInfo.Prefab, GameManager.GetUnitsFolder(), position);

            Destroy(gameObject);
            Destroy(preview);
        }
    }

    //This code was found from https://answers.unity.com/questions/566519/camerascreentoworldpoint-in-perspective.html
    //It finds the position in the game space relative to where the cursor is on the screen.
    private Vector3 getPosition() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy;
        if(isFlying)
            xy = new Plane(Vector3.up, new Vector3(0, 20, 0));
        else
            xy = new Plane(Vector3.up, new Vector3(0, 0, 0));
        
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    //this function will fix the position of unit placment and the preview with the addition of the no place zones
    //The values will likely need to be adjusted for diffent arenas and things like that, have to figure that out later
    private Vector3 adjustForSpawnZones(Vector3 position) {
        float unitOffset = 4; //This is just a number that moves the units away from the red area slighlty
        float firstAreaBottom = playerInfo.LeftArea.transform.position.z - unitOffset;
        float secondAreaBottom = playerInfo.TopArea.transform.position.z - unitOffset;

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

    private Vector3 adjustForTowers(Vector3 position) {
        foreach(GameObject go in GameManager.Instance.TowerObjects) {
            Component component = go.GetComponent(typeof(IDamageable));
            float towerRadius = (component as IDamageable).Agent.HitBox.radius;
            Vector3 towerPosition = go.transform.position;
            
            if(position.y < 1 &&    //If our position is currently inside a tower
               position.x < towerPosition.x + towerRadius &&
               position.x > towerPosition.x - towerRadius &&
               position.z < towerPosition.z + towerRadius &&
               position.z > towerPosition.z - towerRadius ) 
                {
                    float distFromLeft   = Math.Abs(position.x - towerPosition.x + towerRadius);
                    float distFromRight  = Math.Abs(position.x - towerPosition.x - towerRadius);
                    float distFromBottom = Math.Abs(position.z - towerPosition.z + towerRadius);
                    float distFromTop    = Math.Abs(position.z - towerPosition.z - towerRadius);

                    if( distFromLeft < distFromRight && distFromLeft < distFromBottom && distFromLeft < distFromTop) //If we are closest to the left side of the tower
                        position = new Vector3(towerPosition.x - towerRadius, position.y, position.z);
                    else if( distFromRight < distFromLeft && distFromRight < distFromBottom && distFromRight < distFromTop)
                        position = new Vector3(towerPosition.x + towerRadius, position.y, position.z);
                    else if( distFromBottom < distFromLeft && distFromBottom < distFromRight && distFromBottom < distFromTop)
                        position = new Vector3(position.x, position.y, towerPosition.z - towerRadius);
                    else 
                        position = new Vector3(position.x, position.y, towerPosition.z + towerRadius);
                    break;
               }        
        }
        return position;
    }

}
