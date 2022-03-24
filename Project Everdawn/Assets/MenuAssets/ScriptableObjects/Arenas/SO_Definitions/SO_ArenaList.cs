using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_ArenaList", menuName = "ScriptableObjects/Arenas/ArenaList")]
public class SO_ArenaList : ScriptableObject
{
    public List<SO_Arena> Arenas;
}
