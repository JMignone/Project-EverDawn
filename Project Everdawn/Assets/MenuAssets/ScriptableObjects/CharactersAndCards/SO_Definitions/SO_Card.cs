using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SO_NewCard", menuName = "ScriptableObjects/New Card")]
public class SO_Card : ScriptableObject
{
    #region Card Variables
    [Header("General Attributes")]
        public int CardID;

        [Space]

        public SO_Character character;
        
        [Space]

        public string cardName;
        public Sprite menuBackgroundImage;
        public Sprite cardImage;

    [Space]

    [Header("Creative Attributes")]
        [TextArea(3, 10)] public string cardLore;
        public SO_Faction[] affiliation;

    [Space]

    [Header("Gameplay Attributes")]
        [Range(1,10)] public int resourceCost;
        //public Spell skill;
        public CardStats cardStats;

    [Space]

    [Header("Player Attributes")]
        public bool obtained;
        //public List<UnitSkin> availableSkins;
        [Min(0)] public int selectedSkin;
    #endregion
}
