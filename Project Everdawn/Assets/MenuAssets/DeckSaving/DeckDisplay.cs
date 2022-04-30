using System.Collections.Generic;
using UnityEngine;

public class DeckDisplay : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private bool saveDeckOnUnload;

    [Space]
    
    [SerializeField] private int selectedDeckNumber;
    [SerializeField] private PlayerDeck selectedDeck = new PlayerDeck();
    private List<GameObject> cardsBeingDisplayed = new List<GameObject>();

    private GameObject cardInstance;

    public List<GameObject> CardsBeingDisplayed
    {
        get{return cardsBeingDisplayed;}
    }

    private void OnEnable()
    {
        LoadDeck(deckManager.SelectedDeckNumber);
    }

    private void OnDisable()
    {
        UnloadDeck();
    }

    private void SaveDeck()
    {
        for(int i = 0; i < CardsBeingDisplayed.Count; i++) //Loop over the new list of cards and set the local list of IDs to reflect changes
        {
            selectedDeck.CardsInDeck[i] = CardsBeingDisplayed[i].GetComponent<CardDisplay>().cardID;
        }
        deckManager.SaveDeck(selectedDeckNumber, selectedDeck);
    }

    private void LoadDeck(int deckNumber)
    {
        selectedDeckNumber = deckNumber;
        deckManager.ChangeSelectedDeck(deckNumber);
        selectedDeck = deckManager.SelectedDeck;
        List<SO_Card> cardList = deckManager.ConvertIntListToCardList(deckManager.SelectedDeck.CardsInDeck);
        for (int i = 0; i < selectedDeck.CardsInDeck.Count; i++)
        {
            cardInstance = Instantiate(cardPrefab.gameObject, this.transform); // Instantiate the card from the list
            CardDisplay cardInstanceDisplay = cardInstance.GetComponent<CardDisplay>(); // Grab the instantiated card's CardDisplay
            cardInstanceDisplay.BindCardData(cardList[i]); // Bind the Instance's CardDisplay to the data pulled from the list
            cardsBeingDisplayed.Add(cardInstance); // Add the new card instance to the list of cards
        }
    }

    public void ChangeDeck(int deckNumber)
    {
        UnloadDeck();
        LoadDeck(deckNumber);
    }

    private void UnloadDeck()
    {
        if(saveDeckOnUnload == true)
        {
            SaveDeck();
        }

        // still need to pool objects
        if(CardsBeingDisplayed != null && transform.childCount != 0) // Check that there are objects that have been instantiated and are in the list
        {
            for (int i = CardsBeingDisplayed.Count - 1; i >= 0; i--) // Reverse loop over objects in list and destroy them
            {
                GameObject.Destroy(CardsBeingDisplayed[i]);
                CardsBeingDisplayed.Remove(CardsBeingDisplayed[i]);
            }
        }
    }
}