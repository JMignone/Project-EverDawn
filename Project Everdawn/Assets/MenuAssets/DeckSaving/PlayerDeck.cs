using System;
using System.Collections.Generic;

[Serializable]
public class PlayerDeck
{
    public string DeckName;
    public List<int> CardsInDeck = new List<int>();

    public PlayerDeck Copy() // Returns clone of PlayerDeck
    {
        var result = new PlayerDeck();
        result.DeckName = this.DeckName; // Creates a reference to the string
        result.CardsInDeck = new List<int>(this.CardsInDeck);
        return result;
    }
}
