using System;
using System.Collections.Generic;

[Serializable]
public class PlayerDeck
{
    public string deckName;
    public List<int> cardsInDeck;

    public PlayerDeck Copy() // Returns clone of PlayerDeck
    {
        var result = new PlayerDeck();
        result.deckName = this.deckName; // Creates a reference to the string
        result.cardsInDeck = new List<int>(this.cardsInDeck);
        return result;
    }
}
