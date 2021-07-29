using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private Deck playersDeck;
    [SerializeField]
    private List<Image> resources;
    [SerializeField]
    private int score;
    [SerializeField]
    private float currResource;
    private float dueResource;
    [SerializeField]
    private Text textCurrResource;
    //[SerializeField]
    //private Text textMaxResource;
    [SerializeField]
    private Text textScore;
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    private Transform handParent;
    [SerializeField]
    private Card nextCard;
    [SerializeField]
    private GameObject topArea;
    [SerializeField]
    private GameObject leftArea;
    [SerializeField]
    private GameObject rightArea;
    private bool onDragging;
    private bool spawnZone;
    private bool topZone;
    private bool leftZone;
    private bool rightZone;

    public Deck PlayersDeck
    {
        get { return playersDeck; }
        //set { playersDeck = value; }
    }

    public List<Image> Resources
    {
        get { return resources; }
        //set { resources = value; }
    }

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    public float CurrResource
    {
        get { return currResource; }
        set { currResource = value; }
    }

    public float DueResource
    {
        get { return dueResource; }
        set { dueResource = value; }
    }

    public int GetCurrResource 
    {
        get { return (int)currResource; }
    }

    public Text TextCurrResource
    {
        get { return textCurrResource; }
        //set { textCurrResource = value; }
    }
    /*
    public Text TextMaxResource
    {
        get { return textMaxResource; }
        //set { textMaxResource = value; }
    }
    */
    public Text TextScore
    {
        get { return textScore; }
        //set { textScore = value; }
    }

    //I added these 2 so far
    public GameObject CardPrefab
    {
        get { return cardPrefab; }
        //set { cardPrefab = value; }
    }

    public Transform HandParent
    {
        get { return handParent; }
        //set { handParent = value; }
    }
    //

    public GameObject TopArea
    {
        get { return topArea; }
        set { topArea = value; }
    }

    public GameObject LeftArea
    {
        get { return leftArea; }
        set { leftArea = value; }
    }

    public GameObject RightArea
    {
        get { return rightArea; }
        set { rightArea = value; }
    }

    public bool OnDragging
    {
        get { return onDragging; }
        set { onDragging = value; }
    }

    public bool SpawnZone
    {
        get { return spawnZone; }
        set { spawnZone = value; }
    }

    public bool TopZone
    {
        get { return topZone; }
        set { topZone = value; }
    }

    public bool LeftZone
    {
        get { return leftZone; }
        set { leftZone = value; }
    }

    public bool RightZone
    {
        get { return rightZone; }
        set { rightZone = value; }
    }

    private void Start()
    {
        playersDeck.Start();
        dueResource = 0;
        //spawnZone = false;
    }

    private void Update()
    {
        if(GetCurrResource < GameConstants.RESOURCE_MAX + 1) {
            resources[GetCurrResource].fillAmount = currResource - GetCurrResource;
            currResource += Time.deltaTime * GameConstants.RESOURCE_SPEED;
        }
        //Could add somthing here to make sure that we dont overflow over 10 by somthing like 0.001, but is it worth the extra computation?

        if(spawnZone) {
            topArea.SetActive(!topZone);
            leftArea.SetActive(!leftZone);
            rightArea.SetActive(!rightZone);
        }
        else {
            topArea.SetActive(false);
            leftArea.SetActive(false);
            rightArea.SetActive(false);
        }

        UpdateText();
        UpdateDeck();
    }

    private void UpdateText()
    {
        textCurrResource.text = GetCurrResource.ToString();
        //textMaxResource.text = (GameConstants.RESOURCE_MAX + 1).toString(); currently not using this
        textScore.text = score.ToString();
    }

    private void UpdateDeck()
    {
        if(playersDeck.Hand.Count < GameConstants.MAX_HAND_SIZE) {
            CardStats cs = playersDeck.DrawCard();
            GameObject go = Instantiate(cardPrefab, handParent);
            go.transform.SetSiblingIndex(cs.LayoutIndex);
            Card c = go.GetComponent<Card>();
            c.PlayerInfo = this;
            c.CardInfo = cs;
        }

        nextCard.CardInfo = playersDeck.NextCard;
        nextCard.PlayerInfo = this;
    }

    public void RemoveResource(int cost)
    {
        currResource -= cost;
        for(int i=0; i < resources.Count; i++) {
            resources[i].fillAmount = 0;
            if(i <= GetCurrResource) {
                resources[i].fillAmount = 1;
            }
        }
    }

}
