using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    [SerializeField]
    private List<CardStats> cards;
    [SerializeField]
    private List<CardStats> hand;
    [SerializeField]
    private CardStats nextCard;

    public List<CardStats> Cards
    {
        get { return cards; }
        //set { cards = value; }
    }

    public List<CardStats> Hand
    {
        get { return hand; }
        //set { hand = value; }
    }

    public CardStats NextCard
    {
        get { return nextCard; }
        set { nextCard = value; }
    }

    public void Start() {
        nextCard = cards[0];
    }

    public CardStats DrawCard() {
        CardStats cs = nextCard;
        if(hand.Count < 3) {    //this is for the game setup, gives each card in hand an assosiated index for where it is in hand
            hand.Add(nextCard);
            nextCard.LayoutIndex = hand.Count;
        }
        else {
            bool isLast = true; //this will remain true if the card that got removed was the last index
            for(int index=0; index < GameConstants.MAX_HAND_SIZE-1; index++) {    //finds where to place the new card
                if(hand[index].LayoutIndex != index) {
                    hand.Insert(index, nextCard);
                    nextCard.LayoutIndex = index;
                    isLast = false;
                    break;
                }
            }
            if(isLast) {
                hand.Add(nextCard);
                nextCard.LayoutIndex = hand.Count;
            }
        }
        //The purpose of the entire above if block is to make it so the card that gets replaced is placed in the same spot, and not every card is moved
        cards.Remove(nextCard);
        nextCard = cards[0];

        return cs;
    }

    public void RemoveHand(int index) {
        foreach(CardStats cs in hand) {
            if(cs.Index == index) {
                hand.Remove(cs);
                cards.Add(cs);
                break;
            }
        }
    }
}
