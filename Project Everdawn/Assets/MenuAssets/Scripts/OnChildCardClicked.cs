using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnChildCardClicked : MonoBehaviour
{
    [SerializeField] private CardDatabaseDisplay objectDisplayingCards;
    [SerializeField] private Component functionToAttach;

    private void OnEnable()
    {
        GiveCardsFunctions();
    }

    private void OnDisable()
    {
        RemoveCardFunctions();
    }

    private void GiveCardsFunctions()
    {
        foreach(GameObject cards in objectDisplayingCards.cardsBeingDisplayed)
        {
            
        }
    }

    private void RemoveCardFunctions()
    {

    }

    /*
    //public class cardClicked : UnityEvent<int>{} // Bro I don't know why I need to do this
    //public cardClicked CardClicked = new cardClicked();
    //public static event Action<SO_Card> test;


    [SerializeField] private IntEvent onCardClicked;
    [SerializeField] public UnityAction cardClick;

    private void OnEnable()
    {
        cardClick += OnCardClick; // Subscribe the method that invokes the unity event list to the action
    }

    private void OnDisable()
    {
        cardClick -= OnCardClick; // Unsubscribe the method
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void something()
    {

        cardInstanceDisplay = cardInstance.GetComponent<CardDisplay>();
        cardInstanceButton = cardInstanceDisplay.cardButton;
        cardInstanceButton.onClick.AddListener(cardClick); // Add the cardClick UnityAction to the on click listener
        // Documentation says to use Button.clicked instead

    }

    public void OnCardClick()
    {
        //beans = cardClick.
        onCardClicked.Raise(beans);
        //cardClicked.Invoke();
        //int cardInstanceID = cardClick.DynamicInvoke()

    }
    */
}
