using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Clan
{
    [SerializeField]
    private List<Player> clanMembers;
    [SerializeField]
    private int clanLevel;
    [SerializeField]
    private string clanName;
    public Clan(string name, int level, List<Player> members)
    {
        clanName = name;
        clanLevel = level;
        clanMembers = members;
    }
 
    public string ClanName { get; }
 
    public int ClanLevel { get {return clanLevel;} }

    public Player ClanMembers { get; }
}

