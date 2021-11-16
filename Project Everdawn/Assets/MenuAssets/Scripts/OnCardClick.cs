using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardDisplay))]
public class OnCardClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardEvent cardEvent;
    [SerializeField] private SO_Card card;

    public void OnPointerClick(PointerEventData eventData)
    {
        card = gameObject.GetComponent<CardDisplay>().card;
        Debug.Log(card.cardName.ToString());
        cardEvent.Raise(card);
    }
}
