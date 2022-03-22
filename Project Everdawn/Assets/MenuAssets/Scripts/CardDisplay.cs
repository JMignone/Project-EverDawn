using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("Card")]
    public SO_Card Card;

    [Header("Bound Data")]
    public int cardID;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text resourceCost;

    [Header("Click Event Settings")]
    [SerializeField] private CardDisplayEvent cardDisplayEvent;

    void Awake()
    {
        BindCardData(Card);
    }

    public void BindCardData(SO_Card card)
    {
        if(card != null) // Check that there is an object to bind the data from
        {
            // Bind the data from the scriptable object
            Card = card;
            cardID = card.cardID;
            cardName.text = card.cardName;
            cardImage.sprite = card.cardImage;
            resourceCost.text = card.resourceCost.ToString();
        }
        else
        {
            //Debug.Log("No card found");
        }
    }

    public void RaiseCardAsEvent()
    {
        if(cardDisplayEvent != null)
        {
            cardDisplayEvent.Raise(this);
        }
    }
}
