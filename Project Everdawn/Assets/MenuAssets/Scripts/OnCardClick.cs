using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardDisplay))]
public class ClickableCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardDisplay cardDisplay;
    [SerializeField] private CardEvent cardEvent;
    private SO_Card card;

    /*
    private void Awake()
    {
        cardEvent = ScriptableObject.CreateInstance<CardEvent>();
    }
    */

    private void OnEnable()
    {
        card = cardDisplay.card;    
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cardEvent.Raise(card);
    }
}