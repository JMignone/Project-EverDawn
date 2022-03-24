using System.Collections.Generic;
using UnityEngine;

public class CardListDisplay : MonoBehaviour
{
    [SerializeField] private SO_CardList cardList;
    [SerializeField] private GameObject cardPrefab;
    public List<GameObject> cardsBeingDisplayed = new List<GameObject>();

    /*
    private List<GameObject> CardsBeingDisplayed
    {
        get { return cardsBeingDisplayed;}
        set { }
    }
    
    // This is a weird case where getters/setters should be used to make sure this is the only class modifying the list, 
    // even if others can read it

    */

    //private CardDisplay cardInstanceDisplay;
    private GameObject cardInstance;

    private void OnEnable()
    {
        LoadCardDatabase();
    }

    private void OnDisable()
    {
        UnloadCardDatabase();
    }

    private void LoadCardDatabase()
    {
        foreach (SO_Card card in cardList.cardList)
        {
            if(card.obtained) // Check for if card owned
            {
                cardInstance = Instantiate(cardPrefab.gameObject, this.transform); // Instantiate the card from the list
                CardDisplay cardInstanceDisplay = cardInstance.GetComponent<CardDisplay>(); // Grab the instantiated card's CardDisplay
                cardInstanceDisplay.BindCardData(card); // Bind the Instance's CardDisplay to the data pulled from the list
                cardsBeingDisplayed.Add(cardInstance); // Add the new card instance to the list of cards
            }
        }
    }

    private void UnloadCardDatabase()
    {
        if(cardsBeingDisplayed != null && transform.childCount != 0) // Check that there are objects that have been instantiated and are in the list
        {
            for (int i = cardsBeingDisplayed.Count - 1; i >= 0; i--) // Reverse loop over objects in list and destroy them
            {
                GameObject.Destroy(cardsBeingDisplayed[i]);
                cardsBeingDisplayed.Remove(cardsBeingDisplayed[i]);
            }
        }
    }
}
