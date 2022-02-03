using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckDisplay : MonoBehaviour
{
    [SerializeField] private SO_CardDatabase cardDatabase;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private DeckSaver deckSaver;
    [SerializeField] private bool saveDeckOnUnload;

    [Space]
    
    [SerializeField] private List<int> cardIDList = new List<int>();
    public List<GameObject> cardsBeingDisplayed = new List<GameObject>();

    private GameObject cardInstance;
    private CardDisplay selectedCardDisplay;

    private void OnEnable()
    {
        LoadDeck();
    }

    private void OnDisable()
    {
        UnloadDeck();
    }

    private void LoadDeckData()
    {
        deckSaver.Load();
        cardIDList = deckSaver.selectedCards;
    }

    private void SaveDeckData()
    {
        for(int i = 0; i < cardsBeingDisplayed.Count; i++) //Loop over the new list of cards and set the local list of IDs to reflect changes
        {
            cardIDList[i] = cardsBeingDisplayed[i].GetComponent<CardDisplay>().cardID;
        }
        deckSaver.selectedCards = cardIDList;
        deckSaver.Save();
    }

    private void LoadDeck()
    {
        LoadDeckData();
        for (int i = 0; i < cardIDList.Count; i++)
        {
            CardDisplay cardToDisplay = cardPrefab.GetComponent<CardDisplay>(); // Pull card data from card database
            cardToDisplay.card = cardDatabase.cardList[cardIDList[i]]; // Set new card's instance data to be the data pulled from the database
            cardInstance = Instantiate(cardToDisplay.gameObject, this.transform); // Instantiate the card from the list
            cardsBeingDisplayed.Add(cardInstance); // Add the new card instance to the list of cards
        }
    }

    private void UnloadDeck()
    {
        if(saveDeckOnUnload == true)
        {
        SaveDeckData();
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
            selectedCardDisplay = null; // Prevents only tapping bottom card once and being able to change all top cards without re-inputting any other actions - could be useful for testing down the line? add toggle?
        }
    }
}
