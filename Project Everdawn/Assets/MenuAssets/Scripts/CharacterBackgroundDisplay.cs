using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBackgroundDisplay : MonoBehaviour
{
    [SerializeField] private int cardID;
    [SerializeField] private SO_CardDatabase cardDatabase;
    [SerializeField] private Image characterImage;

    public void ChangeCharacterBackgroundImage(int cardIndex)
    {
        if(cardIndex <= cardDatabase.cardList.Count)
        {
            cardID = cardIndex;
            SO_Card card = cardDatabase.cardList[cardID];
            characterImage.sprite = card.cardImage;
        }
        else
        {
            Debug.Log("Card ID " + cardIndex.ToString() + " was not found in the database.");
        }
    }
}
