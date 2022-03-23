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

    void Awake()
    {
        BindCardData(card);
    }

    public void BindCardData(SO_Card cd)
    {
        if(cd != null) // Check that there is a card to bind the data to
        {
            // Bind the data from the scriptable object
            cardID = cd.cardID;
            cardName.text = cd.cardName;
            cardImage.sprite = cd.cardImage;
            resourceCost.text = cd.resourceCost.ToString();
        }
        else
        {
            Debug.Log("No card found");
        }
    }
}
