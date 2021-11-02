using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private ComputerStats computerStats;
    [SerializeField]
    private Deck playersDeck;
    [SerializeField]
    private List<Image> resources;
    private int score;
    [SerializeField] [Min(0)]
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
    private Canvas playerCanvas;
    [SerializeField]
    private Transform quedSpells;
    [SerializeField]
    private GameObject topArea;
    [SerializeField]
    private GameObject leftArea;
    [SerializeField]
    private GameObject rightArea;
    private bool onDragging;
    private GameConstants.SPAWN_ZONE_RESTRICTION spawnZone;
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
    }

    public GameObject CardPrefab
    {
        get { return cardPrefab; }
    }

    public Transform HandParent
    {
        get { return handParent; }
        //set { handParent = value; }
    }

    public Canvas PlayerCanvas
    {
        get { return playerCanvas; }
        set { playerCanvas = value; }
    }

    public Transform QuedSpells
    {
        get { return quedSpells; }
    }

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

    public GameConstants.SPAWN_ZONE_RESTRICTION SpawnZone
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
        //SetSpawnZone();
        spawnZone = GameConstants.SPAWN_ZONE_RESTRICTION.NONE;
        if(computerStats.IsComputer)
            computerStats.Start(this);
    }

    private void Update()
    {
        if(GetCurrResource < GameConstants.RESOURCE_MAX + 1) {
            if(!computerStats.IsComputer) resources[GetCurrResource].fillAmount = currResource - GetCurrResource;
            currResource += Time.deltaTime * GameConstants.RESOURCE_SPEED;
        }
        //Could add somthing here to make sure that we dont overflow over 10 by somthing like 0.001, but is it worth the extra computation?
        if(!computerStats.IsComputer) {
        if(spawnZone == GameConstants.SPAWN_ZONE_RESTRICTION.FULL) {
            topArea.SetActive(!topZone);
            leftArea.SetActive(!leftZone);
            rightArea.SetActive(!rightZone);
        }
        else if(spawnZone == GameConstants.SPAWN_ZONE_RESTRICTION.HALF) {
            topArea.SetActive(!topZone);
            leftArea.SetActive(false);
            rightArea.SetActive(false);
        }
        else {
            topArea.SetActive(false);
            leftArea.SetActive(false);
            rightArea.SetActive(false);
        }
        }

        computerStats.UpdateComputerStats();

        UpdateText();
        UpdateDeck();
    }

    private void UpdateText()
    {
        if(!computerStats.IsComputer) {
            textCurrResource.text = GetCurrResource.ToString();
            //textMaxResource.text = (GameConstants.RESOURCE_MAX + 1).toString(); currently not using this
        }
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

        if(!computerStats.IsComputer) {
        nextCard.CardInfo = playersDeck.NextCard;
        nextCard.PlayerInfo = this;
        }
    }

    public void RemoveResource(int cost)
    {
        currResource -= cost;
        if(!computerStats.IsComputer) {
        for(int i=0; i < resources.Count; i++) {
            resources[i].fillAmount = 0;
            if(i <= GetCurrResource) {
                resources[i].fillAmount = 1;
            }
        }
        }
    }

    /**
    public void SetSpawnZone() {
        print(GameManager.Instance.Ground.transform.localScale);
        RectTransform topTransform = topArea.GetComponent<RectTransform>();
        RectTransform leftTransform = leftArea.GetComponent<RectTransform>();
        RectTransform rightTransform = rightArea.GetComponent<RectTransform>();

        float topAmount = GameManager.Instance.Ground.transform.localScale.z * 10 / 3.45f; //3.45 is just a hueristic 
        print(topAmount);
        print(GameManager.Instance.Ground.transform.localScale.z*10/-12.5f);
        print(topAmount - .05f - GameManager.Instance.Ground.transform.localScale.z*10/-12.5f);
        topTransform.anchoredPosition = new Vector3(0, -topAmount, 0); 
        topTransform.sizeDelta = new Vector2(GameManager.Instance.Ground.transform.localScale.x * 10, topAmount - .05f);

        leftTransform.anchoredPosition = new Vector3(GameManager.Instance.Ground.transform.localScale.x * 10/-4, GameManager.Instance.Ground.transform.localScale.z*10/-12.5f, 0);
        leftTransform.sizeDelta = new Vector2(GameManager.Instance.Ground.transform.localScale.x * 10/2, topAmount - GameManager.Instance.Ground.transform.localScale.z*10/12.5f);

        rightTransform.anchoredPosition = new Vector3(GameManager.Instance.Ground.transform.localScale.x * 10/4, GameManager.Instance.Ground.transform.localScale.z*10/-12.5f, 0);
        rightTransform.sizeDelta = new Vector2(GameManager.Instance.Ground.transform.localScale.x * 10/2, GameManager.Instance.Ground.transform.localScale.z*5 - topAmount - .05f);
    }**/
}
