using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeamDisplay : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;

    [Space]

    [SerializeField] private bool allowDeckMultiselection;

    [Space]

    [SerializeField] private AverageResourceDisplay averageResourceDisplay;
    [SerializeField] private DeckDisplay deckDisplay;
    [SerializeField] private DeckNameDisplay deckNameDisplay;

    private CardDisplay selectedCardDisplay;

    private void OnEnable()
    {
        RefreshDisplay();
    }

    public void ChangeDeckCard()
    {

    }

    public void RefreshDisplay()
    {
        averageResourceDisplay.UpdateAverageResourceDisplay();
        deckNameDisplay.UpdateDisplay();
    }

    public void DeckChangeRequest(CardDisplay cd) // Used with event system
    {
        selectedCardDisplay = cd;
    }

    public void DeckChangeExecute(CardDisplay cd) // Used with event system
    {
        if(selectedCardDisplay != null)
        {
            cd.BindCardData(selectedCardDisplay.Card);
            if(allowDeckMultiselection == false)
            {
                selectedCardDisplay = null; // Prevents only tapping bottom card once and being able to change all top cards without re-inputting any other actions - could be useful for testing down the line? add toggle?
            }
        }
    }
}