using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [SerializeField]
    private List<GameObject> objects;
    [SerializeField]
    private List<GameObject> towerObjects;

    public static GameManager Instance
    {
        get { return instance; }
    }

    public List<GameObject> Objects
    {
        get { return objects; }
    }

    public List<GameObject> TowerObjects
    {
        get { return towerObjects; }
    }

    private void Awake()
    {
        if(instance != this)
            instance = this;
    }

    public static void RemoveObjectsFromList(GameObject objectToRemove)
    {
        Vector3 objectToRemovePosition = objectToRemove.transform.GetChild(0).position;
        Actor3D objectToRemoveAgent = (objectToRemove.GetComponent(typeof(IDamageable)).gameObject.GetComponent(typeof(IDamageable)) as IDamageable).Agent; //again, there must be a better way to get the agent...
        float objectToRemoveAgentRadius = objectToRemoveAgent.HitBox.radius;

        foreach (GameObject go in Instance.Objects) { //  The trigger exit doesnt get trigger if the object suddenly dies, so we need this do do it manually 
            Component component = go.GetComponent(typeof(IDamageable));
            if(component) {
                if((component as IDamageable).HitTargets.Contains(objectToRemove)) { //if an object has this now dead unit as a hit target ...
                    (component as IDamageable).HitTargets.Remove(objectToRemove);    //remove it from their possible targets
                    if((component as IDamageable).Target == objectToRemove)          //if an object has this now dead unit as a target ...
                        (component as IDamageable).Target = null;                    //make target null
                    if( Vector3.Distance(objectToRemovePosition, go.transform.GetChild(0).position) <= ((component as IDamageable).Stats.Range + objectToRemoveAgentRadius) ) //If the unit that died is within range
                        (component as IDamageable).InRange--;
                }
            }
        }
        Instance.Objects.Remove(objectToRemove);
        if(Instance.TowerObjects.Contains(objectToRemove))
            Instance.TowerObjects.Remove(objectToRemove);
    }

    //this is mainly used, if not only used for locating towers, maybe usful for ability targeting later on
    public static List<GameObject> GetAllEnemies(Vector3 pos, List<GameObject> objects, string tag) {
        List<GameObject> listOfEnemies = new List<GameObject>();

        foreach (GameObject go in objects) {
            if(!go.CompareTag(tag)) {
                listOfEnemies.Add(go);
            }
        }

        return listOfEnemies;
    }

    //curently unused
    public static bool isTowerActive(string tag, float percHp) {
        List<GameObject> listOfFriendlyTowers = new List<GameObject>();

        foreach (GameObject go in Instance.TowerObjects) {
            if(go.CompareTag(tag)) {
                listOfFriendlyTowers.Add(go);
            }
        }

        if(listOfFriendlyTowers.Count == 3)
            return false;
        else
            return true;
    }
}
