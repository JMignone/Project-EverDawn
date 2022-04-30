using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckNameDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text deckNameText;
    [SerializeField] private TMP_InputField deckNameInputField;
    [SerializeField] private DeckManager deckManager;

    public void UpdateDisplay()
    {
        if(deckNameText != null || deckNameInputField != null)
        {
            if(deckNameText != null)
            {
                deckNameText.text = deckManager.SelectedDeck.DeckName;
            }
            if(deckNameInputField != null)
            {
                deckNameInputField.text = deckManager.SelectedDeck.DeckName;
            }
        }
    }
}