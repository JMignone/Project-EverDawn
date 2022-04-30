using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AverageResourceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text numberDisplay;
    [SerializeField] private DeckDisplay deckDisplay;
    [SerializeField] private DeckManager deckManager;
    private float averageResource;

    private List<int> GenerateCardCostsList(List<int> selectedCardIDList) // Converts a list of card IDs to a list of their resource costs
    {
        List<int> result = new List<int>();
        List<SO_Card> selectedCardList = deckManager.ConvertIntListToCardList(selectedCardIDList);
        for(int i = 0; i < selectedCardList.Count; i++)
        {
            result.Add(selectedCardList[i].resourceCost);
        }
        return result;
    }

    private float CalculateAverageResource(List<int> cardCosts) // Finds the average value of a list of ints
    {
        float result = 0f;
        for(int i = 0; i < cardCosts.Count; i++)
        {
            result += cardCosts[i];
        }
        result /= cardCosts.Count;
        return result;
    }

    public void UpdateAverageResourceDisplay() // Calculates the average resource using the currently selected deck in the deckManager
    {
        List<int> cardList = new List<int>();
        for(int i = 0; i < deckDisplay.CardsBeingDisplayed.Count; i++)
        {
            CardDisplay cd = deckDisplay.CardsBeingDisplayed[i].GetComponent<CardDisplay>();
            cardList.Add(cd.cardID);
        }
        averageResource = CalculateAverageResource(GenerateCardCostsList(cardList));
        numberDisplay.SetText(averageResource.ToString("G2")); // I do not know if SetText() creates a reference or duplicates the given string
    }
}