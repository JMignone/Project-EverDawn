using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_SocialMenuActivity : ScriptableObject
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {testplayers.Add(new Player("testPlayer", 3, new Deck()));
    
    }

    public Clan getClanStatus(){
        return new Clan("testclan", 9001, testplayers);
        //eventually get this from API or some internal Repo
    }

    private List<Player> testplayers = new List<Player>();

}
