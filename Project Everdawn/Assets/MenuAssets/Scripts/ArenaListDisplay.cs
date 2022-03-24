using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaListDisplay : MonoBehaviour
{
    [SerializeField] private SO_ArenaList arenaList;
    [SerializeField] private GameObject arenaDisplayPrefab;
    public List<GameObject> ArenasBeingDisplayed = new List<GameObject>();

    private ArenaDisplay arenaInstanceDisplay;
    private GameObject arenaInstance;

    private void OnEnable()
    {
        LoadArenaList();
    }

    private void OnDisable()
    {
        UnloadArenaList();
    }

    private void LoadArenaList()
    {
        foreach (SO_Arena arena in arenaList.Arenas)
        {
            if(true) // Need to check that player is of correct rank eventually
            {
                arenaInstance = Instantiate(arenaDisplayPrefab.gameObject, this.transform); // Instantiate the arena display from the list
                ArenaDisplay arenaInstanceDisplay = arenaInstance.GetComponent<ArenaDisplay>(); // Grab the instantiated arena's ArenaDisplay
                arenaInstanceDisplay.BindArenaData(arena); // Bind the Instance's ArenaDisplay to the data pulled from the list
                ArenasBeingDisplayed.Add(arenaInstance); // Add the new instance to the list of objects
            }
        }
    }

    private void UnloadArenaList()
    {
        if(ArenasBeingDisplayed != null && transform.childCount != 0) // Check that there are objects that have been instantiated and are in the list
        {
            for (int i = ArenasBeingDisplayed.Count - 1; i >= 0; i--) // Reverse loop over objects in list and destroy them
            {
                GameObject.Destroy(ArenasBeingDisplayed[i]);
                ArenasBeingDisplayed.Remove(ArenasBeingDisplayed[i]);
            }
        }
    }
}
