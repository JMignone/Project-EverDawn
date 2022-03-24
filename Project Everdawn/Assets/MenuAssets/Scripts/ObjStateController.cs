using System.Collections.Generic;
using UnityEngine;

public class ObjStateController : MonoBehaviour
{
    [System.Serializable] private class objectStatePair // Internal class used to organize states and their associated object
    {
        [SerializeField] public GameObject gameObject;
        [SerializeField] public BaseState state;
    }

    [SerializeField] private List<objectStatePair> pairsToToggle;
    [SerializeField] private objectStatePair currentActivePair;

    public void ToggleObjects(BaseState state) // Used with events to dynamically check for state changes
    {
        if(state != null)
        {
            foreach(objectStatePair pair in pairsToToggle) // Checks if input state has a match in the list
            {
                if(!pair.state == state)
                {
                    continue;
                }
                else if(pair.state == state)
                {
                    TogglePairs(pair);
                    break;
                }
            }
        }
    }

    private void TogglePairs(objectStatePair pair) // Handles actually setting the objects on or off
    {
        currentActivePair.gameObject.SetActive(false);
        pair.gameObject.SetActive(true);
        currentActivePair = pair;
    }
}