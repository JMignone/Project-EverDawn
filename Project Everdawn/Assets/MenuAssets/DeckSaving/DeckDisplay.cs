using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckDisplay : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private bool saveDeckOnUnload;
    [SerializeField] private bool allowDeckMultiselection;

    [Space]
    
    [SerializeField] [Min(1)] private int selectedDeckNumber;
    [SerializeField] private PlayerDeck selectedDeck = new PlayerDeck();
    public List<GameObject> cardsBeingDisplayed = new List<GameObject>();

    private GameObject cardInstance;
    private CardDisplay selectedCardDisplay;

    private void OnEnable()
    {
        LoadDeck(selectedDeckNumber);
    }

    private void OnDisable()
    {
        UnloadDeck();
    }

    private void SaveDeck()
    {
        for(int i = 0; i < cardsBeingDisplayed.Count; i++) //Loop over the new list of cards and set the local list of IDs to reflect changes
        {
            selectedDeck.cardsInDeck[i] = cardsBeingDisplayed[i].GetComponent<CardDisplay>().cardID;
        }
        deckManager.selectedDeck = selectedDeck;
        deckManager.SaveSelectedDeck(selectedDeckNumber);
    }

    public void LoadDeck(int deckNumber)
    {
        selectedDeckNumber = deckNumber;
        if(deckNumber != 0)
            deckManager.selectedDeckNumber = deckNumber;
            
        deckManager.LoadDeck(deckNumber);
        selectedDeck = deckManager.selectedDeck;
        List<SO_Card> cardList = new List<SO_Card>();
        cardList = deckManager.ConvertIntListToCardList(selectedDeck.cardsInDeck);
        for (int i = 0; i < selectedDeck.cardsInDeck.Count; i++)
        {
            CardDisplay cardToDisplay = cardPrefab.GetComponent<CardDisplay>(); // Pull card data from card database
            cardToDisplay.card = cardList[i]; // Set new card's instance data to be the data pulled from the database
            cardInstance = Instantiate(cardToDisplay.gameObject, this.transform); // Instantiate the card from the list
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

        if(cardsBeingDisplayed != null && transform.childCount != 0) // Check that there are objects that have been instantiated and are in the list
        {
            for (int i = cardsBeingDisplayed.Count - 1; i >= 0; i--) // Reverse loop over objects in list and destroy them
            {
                GameObject.Destroy(cardsBeingDisplayed[i]);
                cardsBeingDisplayed.Remove(cardsBeingDisplayed[i]);
            }
        }
    }

    public void DeckChangeRequest(CardDisplay cd) // Used with event system
    {
        selectedCardDisplay = cd;
    }

    public void DeckChangeExecute(CardDisplay cd) // Used with event system
    {
        if(selectedCardDisplay != null)
        {
            cd.BindCardData(selectedCardDisplay.card);
            if(allowDeckMultiselection == false)
            {
                selectedCardDisplay = null; // Prevents only tapping bottom card once and being able to change all top cards without re-inputting any other actions - could be useful for testing down the line? add toggle?
            }
        }
    }
}
