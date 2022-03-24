using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_NewArena", menuName = "ScriptableObjects/Arenas/New Arena")]
public class SO_Arena : ScriptableObject
{
    [SerializeField] private string arenaName;
    [SerializeField] private Sprite arenaSprite;
    [SerializeField] private int arenaRank;

    [SerializeField] private string arenaSceneName;
    [SerializeField] [Min(0)] private int arenaID;

    public string ArenaName
    {
        get{return arenaName;}
    }

    public Sprite ArenaImage
    {
        get{return arenaSprite;}
    }

    public int ArenaRank
    {
        get{return arenaRank;}
    }

    public string ArenaSceneName
    {
        get{return arenaSceneName;}
    }

    public int ArenaID
    {
        get{return arenaID;}
    }
}
