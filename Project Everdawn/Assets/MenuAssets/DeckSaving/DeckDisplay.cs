using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckDisplay : MonoBehaviour
{
    [SerializeField] private SO_CardDatabase cardDatabase;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private DeckSaver deckSaver;
    
    [SerializeField] private List<int> cardIDList = new List<int>();
    public List<GameObject> cardsBeingDisplayed = new List<GameObject>();
    [SerializeField] private List<int> editedCardList = new List<int>();

    private GameObject cardInstance;
    private SO_Card selectedCard;

    private int cardIDIndex;

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
        SaveDeckData();
        if(cardsBeingDisplayed != null && transform.childCount != 0) // Check that there are objects that have been instantiated and are in the list
        {
            for (int i = cardsBeingDisplayed.Count - 1; i >= 0; i--) // Reverse loop over objects in list and destroy them
            {
                GameObject.Destroy(cardsBeingDisplayed[i]);
                cardsBeingDisplayed.Remove(cardsBeingDisplayed[i]);
            }
        }
    }

    private void ReloadDeck()
    {
        UnloadDeck();
        LoadDeck();
    }

    public void DeckChangeRequest(SO_Card card)
    {
        selectedCard = card;
    }

    public void DeckChangeExecute(SO_Card card)
    {
        if (cardIDList.Exists(cardIDList => cardIDList == card.cardID))
        {
            cardIDIndex = cardIDList.FindIndex(0, cardIDList.Count, card.cardID.Equals);
        }
        cardIDList[cardIDIndex] = selectedCard.cardID;
        SaveDeckData();
        ReloadDeck();
    }
}
