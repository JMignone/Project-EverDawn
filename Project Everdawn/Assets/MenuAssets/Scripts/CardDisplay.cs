using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public SO_Card card;

    public int cardID;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text resourceCost;
    public Button cardButton;


    void Awake()
    {
        if(card != null) // Check that there is a card to bind the data to
        {
            // Bind the data from the scriptable object
            cardID = card.CardID;
            cardName.text = card.cardName;
            cardImage.sprite = card.cardImage;
            resourceCost.text = card.resourceCost.ToString();
        }
        else
        {
            Debug.Log("No card found");
        }
    }
}
