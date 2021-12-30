using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardDisplay))]
public class OnCardClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardDisplay cardDisplay;
    [SerializeField] private CardEvent cardEvent;
    private SO_Card card;

    private void OnEnable()
    {
        card = cardDisplay.card;    
    }

    [ContextMenu("Run Click Function")]
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(card.cardName.ToString());
        cardEvent.Raise(card);
    }
}
