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
                //transform.SetParent(GameFunctions.GetCanvas());
                
                //GameObject go = Instantiate(playerInfo.CardPrefab, playerInfo.HandParent);
                //Card c = go.GetComponent<Card>();
                //c.PlayerInfo = this.playerInfo;
                //c.CardInfo = this.cardInfo;

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

            //The below only works at a specific resolution, but its a proof of concept
            float scale = (85 - Input.mousePosition.y)/30;
            if(scale > 1)
                scale = 1;
            else if(scale < 0)
                scale = 0;
            transform.GetChild(3).localScale = new Vector3(scale, scale, 1);

            /* 
                The function SamplePosition takes in a point, and it will swarch for the closest point on the navmesh to it.
                For a preview unit, this is very useful. If the preview is aleady on the navmesh, then it takes that point,
                but if the user has placed the unit over the river, it will place the preview to the closest position
                it should be. Tihs will also be used for placing the actual unit as well. It works exactly how we want it to.
            */
            if(Input.mousePosition.y > 100)
                preview.SetActive(true);
            else
                preview.SetActive(false);
            UnityEngine.AI.NavMeshHit hit;
            if(UnityEngine.AI.NavMesh.SamplePosition(getPosition(), out hit, 6.0f, UnityEngine.AI.NavMesh.AllAreas))
				preview.transform.GetChild(0).GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(hit.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(playerInfo.OnDragging) {
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(getPosition(), out hit, 6.0f, UnityEngine.AI.NavMesh.AllAreas))
				preview.transform.GetChild(0).GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(hit.position);
            if(Input.mousePosition.y > 100 && playerInfo.GetCurrResource >= cardInfo.Cost) //only works at a specific resolution, but its a proof of concept
                SpawnUnit();
            else {
                transform.GetChild(3).localPosition = new Vector3(0,0,0);
                transform.GetChild(3).localScale = new Vector3(1,1,1);
                Destroy(preview);
            }
            playerInfo.OnDragging = false;
        }
    }

    private void SpawnUnit()
    {
        if(playerInfo.GetCurrResource >= cardInfo.Cost) //do I need this if the call to this function requires this anyway? Just a santiy check maybe?
        {
            playerInfo.PlayersDeck.RemoveHand(cardInfo.Index);
            playerInfo.RemoveResource(cardInfo.Cost);

            UnityEngine.AI.NavMeshHit hit;
            if(UnityEngine.AI.NavMesh.SamplePosition(getPosition(), out hit, 6.0f, UnityEngine.AI.NavMesh.AllAreas))//This should be true, but the else part is a backup
                GameFunctions.SpawnUnit(cardInfo.Prefab, GameManager.GetUnitsFolder(), hit.position);
            else
                GameFunctions.SpawnUnit(cardInfo.Prefab, GameManager.GetUnitsFolder(), getPosition());
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

}
