using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SO_NewDeck", menuName = "ScriptableObjects/New Deck")]
public class SO_Deck : ScriptableObject
{
    public List<SO_Card> deck = new List<SO_Card>(new SO_Card[8]);
}
