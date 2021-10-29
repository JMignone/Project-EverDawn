using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_NewFaction", menuName = "ScriptableObjects/New Faction")]
public class SO_Faction : ScriptableObject
{
    public string factionName;
    public Sprite factionIcon;

    [TextArea(3, 10)] public string factionLore;
    
    //if we need subfactions
    //public subfaction[] subfactions;
}
