using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CardDatabaseDisplay : MonoBehaviour
{
    [SerializeField] private SO_CardDatabase cardDatabase;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private bool attachButtonsToCards;
    private List<GameObject> cardList = new List<GameObject>();
    private GameObject cardInstance;

    public UnityEvent cardClicked;
    public static event Action<SO_Card> test;

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
        if(true) // Check for if card owned
        {
            foreach (SO_Card card in cardDatabase.cardList)
            {
                CardDisplay cardsToDisplay = cardPrefab.GetComponent<CardDisplay>(); //Pull card data from card database
                cardsToDisplay.card = card; //Set new card's instance data to be the data pulled from the database
                cardInstance = Instantiate(cardsToDisplay.gameObject, this.transform); //Instantiate the card from the list
                cardList.Add(cardInstance); //Add the new card instance to the list of cards
                if(attachButtonsToCards == true)
                {
                    cardClicked = cardsToDisplay.cardButton.onClick;
                }
            }
        }
    }

    private void UnloadCardDatabase()
    {
        if(cardList != null && transform.childCount != 0) // Check that there are objects that have been instantiated and are in the list
        {
            foreach(GameObject go in cardList) // Loop over objects in list and destroy them
            {
                GameObject.Destroy(go);
            }

            // This probably needs a better solution
            cardList.Clear();

            /* This theoretically should be a better and more stable way to destroy the objects and then removing them from the list but I have no idea why it isn't working I've also tried doing cardInstance instead of cardList[i]
            for(int i = 0; i < cardList.Count; i++)
            {
                cardList.Remove(cardList[i]);
                GameObject.Destroy(cardList[i]);
            }
            */
        }
    }
}
