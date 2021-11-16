using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_CardClickFunction", menuName = "ScriptableObjects/New Card Click Function")]
public class SO_CardClickFunction : ScriptableObject
{
    [SerializeField] private GameObject objectToDoStuffTo;
}
