using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    [SerializeField]
    private string playerName;

    [SerializeField]
    private int playerLevel;

    [SerializeField]
    private Deck playerDeck;
    

    public Player(string name, int level, Deck deck)
        {
            playerName = name;
            playerLevel = level;
            playerDeck = deck;
        }

    public string PlayerName { get; }
 
    public int PlayerLevel { get {return playerLevel;} }

    public Deck PlayerDeck { get; }
}
