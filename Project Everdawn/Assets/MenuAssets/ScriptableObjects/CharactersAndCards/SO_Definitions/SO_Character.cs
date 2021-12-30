using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SO_NewCharacter", menuName = "ScriptableObjects/New Character")]
public class SO_Character : ScriptableObject
{
    [SerializeField] public string characterName;
    [SerializeField] public List<SO_Card> cards;
    [SerializeField] [TextArea(3, 10)] public string characterLore;
}
