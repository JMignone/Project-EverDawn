using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardDisplay))]
public class ClickableCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardDisplay cardDisplay;
    [SerializeField] private CardDisplayEvent cardDisplayEvent;
    private SO_Card card;

    private void OnEnable()
    {
        card = cardDisplay.card;    
    }

    [ContextMenu("Run Click Function")]
    public void OnPointerClick(PointerEventData eventData)
    {
        cardDisplayEvent.Raise(cardDisplay);
    }
}
