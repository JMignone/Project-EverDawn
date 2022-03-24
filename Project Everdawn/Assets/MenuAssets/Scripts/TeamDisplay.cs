using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeamDisplay : MonoBehaviour
{
    [SerializeField] private AverageResourceDisplay averageResourceDisplay;
    [SerializeField] private DeckDisplay deckDisplay;
    [SerializeField] private PlayerDeck selectedDeck;
    [SerializeField] private TMP_Text teamName;
}
