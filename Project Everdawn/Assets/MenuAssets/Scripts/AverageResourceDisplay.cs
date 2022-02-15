using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AverageResourceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text numberDisplay;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private float averageResource;

    private List<int> GenerateCardCostsList(List<int> selectedCardIDList) // Converts a list of card IDs to a list of their resource costs
    {
        List<int> result = new List<int>();
        List<SO_Card> selectedCardList = deckManager.ConvertIntListToCardList(selectedCardIDList);
        for(int i = 0; i <= selectedCardList.Count; i++)
        {
            result.Add(selectedCardList[i].resourceCost);
        }
        return result;
    }

    private float CalculateAverageResource(List<int> cardCosts) // Finds the average value of a list of ints
    {
        float result = 0f;
        for(int i = 0; i <= cardCosts.Count; i++)
        {
            result += cardCosts[i];
        }
        result /= cardCosts.Count;
        return result;
    }

    private void DisplayAverageResource() // Calculates the average resource using the currently selected deck in the deckManager
    {
        averageResource = CalculateAverageResource(GenerateCardCostsList(deckManager.selectedDeck.cardsInDeck));
        numberDisplay.SetText(averageResource.ToString()); // I do not know if SetText() creates a reference or duplicates the given string
    }
}
