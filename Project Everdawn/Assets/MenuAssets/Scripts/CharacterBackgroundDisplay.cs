using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBackgroundDisplay : MonoBehaviour
{
    [SerializeField] private SO_CardDatabase cardDatabase;
    [SerializeField] private Image characterImage;
    [SerializeField] private int cardID;

    private readonly string backgroundCharacterID;

    private void OnEnable()
    {
        if(PlayerPrefs.HasKey(backgroundCharacterID))
        {
            cardID = PlayerPrefs.GetInt(backgroundCharacterID);
        }
        else
        {
            cardID = 0;
        }

        ChangeCharacterBackgroundImage(cardID);
    }

    // Used to set data via data injection from other source (like the GameEvent system)
    public void BindCardData(CardDisplay cd)
    {
        if(cd.cardID <= cardDatabase.cardList.Count)
        {
            cardID = cd.cardID;
            ChangeCharacterBackgroundImage(cardID);
        }
        else
        {
            Debug.Log("Card ID " + cd.cardID.ToString() + " was not found in the database.");
        }
    }

    private void ChangeCharacterBackgroundImage(int CardID)
    {
        SO_Card card = cardDatabase.cardList[CardID];
        characterImage.sprite = card.cardImage;

        PlayerPrefs.SetInt(backgroundCharacterID, CardID);
    }
}
