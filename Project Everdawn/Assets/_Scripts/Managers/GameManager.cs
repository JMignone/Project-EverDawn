using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [SerializeField]
    private List<GameObject> objects;
    [SerializeField]
    private List<GameObject> towerObjects;
    [SerializeField]
    private List<PlayerStats> players;
    [SerializeField]
    private GameObject ground;
    private int playerScore;
    private int enemyScore;
    [SerializeField]
    private Text playerTextScore;
    [SerializeField]
    private Text enemyTextScore;
    [Tooltip("Time of the game is in seconds, must be set to 1 more than desired time.")]
    [SerializeField]
    private float timeLeft;
    private float timeLimit;
    [SerializeField]
    private Text textTimer;
    [SerializeField]
    private bool testSetup;

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

    public List<PlayerStats> Players
    {
        get { return players; }
        //set { players = value; }
    }

    public GameObject Ground
    {
        get { return ground; }
    }

    public float TimeLimit
    {
        get { return timeLimit; }
    }

    public int ResourceMultiplier
    {
        get { return timeLeft <= timeLimit/3.0f ? 2 : 1; }
    }

    private void Awake()
    {
        if(instance != this)
            instance = this;

        timeLimit = timeLeft - 1;

        //for testing purposes, removes all non towers from the game immediatly
        if(testSetup) {
            foreach (GameObject objectToRemove in Instance.Objects) {
                if(!Instance.TowerObjects.Contains(objectToRemove))
                    Destroy(objectToRemove);
            }

            for (var i = Instance.Objects.Count-1; i >=0; i--) {
                GameObject objectToRemove = Instance.Objects[i];
                if(objectToRemove.GetComponent<Tower>() == null)
                    Instance.Objects.RemoveAt(i);
            }
        }
    }

    //updates the timer
    private void Update() {
        timeLeft -= Time.deltaTime;
        if(timeLeft > 0) {
            string text = ((int) timeLeft/60).ToString();
            text += ":" + ((int) timeLeft%60).ToString();
            if(text.Length != 4)
                text = text.Substring(0, 2) + "0" + text.Substring(2);
            textTimer.text = text;
        }
    }

    public static void RemoveObjectsFromList(GameObject objectToRemove)
    {
        Vector3 objectToRemovePosition = objectToRemove.transform.GetChild(0).position;
        objectToRemovePosition = new Vector3(objectToRemovePosition.x, 0 ,objectToRemovePosition.z); // Setting the y to 0 to avoid increased distances with flying units
        Component objectToRemoveComponent = objectToRemove.GetComponent(typeof(IDamageable));

        Actor3D objectToRemoveAgent = (objectToRemoveComponent as IDamageable).Agent; 
        //Actor3D objectToRemoveAgent = (objectToRemove.GetComponent(typeof(IDamageable)).gameObject.GetComponent(typeof(IDamageable)) as IDamageable).Agent; //again, there must be a better way to get the agent...
        float objectToRemoveAgentRadius = objectToRemoveAgent.HitBox.radius;

        foreach (GameObject go in Instance.Objects) { //  The trigger exit doesnt get trigger if the object suddenly dies, so we need this do do it manually 
            Component component = go.GetComponent(typeof(IDamageable));
            if(component) {
                if((component as IDamageable).HitTargets.Contains(objectToRemove)) { //if an object has this now dead unit as a hit target ...
                    (component as IDamageable).HitTargets.Remove(objectToRemove);    //remove it from their possible targets
                    if((component as IDamageable).Target == objectToRemove)          //if an object has this now dead unit as a target ...
                        (component as IDamageable).Target = null;                    //make target null
                    if((component as IDamageable).InRangeTargets.Contains(objectToRemove))
                        (component as IDamageable).InRangeTargets.Remove(objectToRemove); 
                }
            }
        }
        if((objectToRemoveComponent as IDamageable).Stats.IsHoveringAbility) {
            removeAbililtyIndicators();
            Instance.Players[0].OnDragging = false;
            GameFunctions.GetCanvas().GetChild(3).GetComponent<Image>().enabled = false;
        }

        Instance.Objects.Remove(objectToRemove);
        if(Instance.TowerObjects.Contains(objectToRemove))
            Instance.TowerObjects.Remove(objectToRemove);
    }

    public static void RemoveObjectsFromList(GameObject objectToRemove, bool leftTower, bool isKeep)
    {
        Vector3 objectToRemovePosition = objectToRemove.transform.GetChild(0).position;
        objectToRemovePosition = new Vector3(objectToRemovePosition.x, 0 ,objectToRemovePosition.z); // Setting the y to 0 to avoid increased distances with flying units
        Component objectToRemoveComponent = objectToRemove.GetComponent(typeof(IDamageable));

        Actor3D objectToRemoveAgent = (objectToRemoveComponent as IDamageable).Agent; 
        //Actor3D objectToRemoveAgent = (objectToRemove.GetComponent(typeof(IDamageable)).gameObject.GetComponent(typeof(IDamageable)) as IDamageable).Agent; //again, there must be a better way to get the agent...
        float objectToRemoveAgentRadius = objectToRemoveAgent.HitBox.radius;

        foreach (GameObject go in Instance.Objects) { //  The trigger exit doesnt get trigger if the object suddenly dies, so we need this do do it manually 
            Component component = go.GetComponent(typeof(IDamageable));
            if(component) {
                if((component as IDamageable).HitTargets.Contains(objectToRemove)) { //if an object has this now dead unit as a hit target ...
                    (component as IDamageable).HitTargets.Remove(objectToRemove);    //remove it from their possible targets
                    if((component as IDamageable).Target == objectToRemove)          //if an object has this now dead unit as a target ...
                        (component as IDamageable).Target = null;                    //make target null
                    if((component as IDamageable).InRangeTargets.Contains(objectToRemove))
                        (component as IDamageable).InRangeTargets.Remove(objectToRemove); 
                }
            }
        }

        if(!objectToRemove.CompareTag(GameConstants.PLAYER_TAG)) {
            if(isKeep) {
                Instance.Players[0].Score = 3;
                //END THE GAME
            }
            else {
                if(leftTower) 
                    Instance.Players[0].LeftZone = true;
                else 
                    Instance.Players[0].RightZone = true;
                if(Instance.Players[0].Score < 3)
                    Instance.Players[0].Score++;
            }
        }
        else { //we may send the enemy score information over the internet rather than the players side instead
            if(isKeep) {
                Instance.Players[1].Score = 3;
                //END THE GAME
            }
            else {
                if(leftTower) 
                    Instance.Players[1].LeftZone = true;
                else 
                    Instance.Players[1].RightZone = true;
                if(Instance.Players[1].Score < 3)
                    Instance.Players[1].Score++;
            }
        }

        /*
            We would only need this if towers have abilities.
        if((objectToRemoveComponent as IDamageable).IsHoveringAbility)
            removeAbililtyIndicators();
        */

        Instance.Objects.Remove(objectToRemove);
        if(Instance.TowerObjects.Contains(objectToRemove))
            Instance.TowerObjects.Remove(objectToRemove);
    }


    public static void AddObjectToList(GameObject objectToAdd){
        Component component = objectToAdd.GetComponent(typeof(IDamageable));
        if((component as IDamageable).Stats.UnitGrouping == GameConstants.UNIT_GROUPING.SOLO)
            Instance.Objects.Add(objectToAdd);
        else {
            foreach(Transform go in objectToAdd.transform){
                if(go.name != "Agent")
                    Instance.Objects.Add(go.gameObject);
            }
        }
    }

    public static Transform GetUnitsFolder() {
        return Instance.transform.parent.GetChild(1);
    }

    //this is mainly used, if not only used for locating towers, maybe usful for ability targeting later on
    public static List<GameObject> GetAllEnemies(List<GameObject> objects, string tag) {
        List<GameObject> listOfEnemies = new List<GameObject>();

        foreach (GameObject go in objects) {
            if(!go.CompareTag(tag)) {
                listOfEnemies.Add(go);
            }
        }

        return listOfEnemies;
    }

    public static bool isTowerActive(string tag, float percHp) {
        List<GameObject> listOfFriendlyTowers = new List<GameObject>();

        foreach (GameObject go in Instance.TowerObjects) {
            if(go.CompareTag(tag)) {
                listOfFriendlyTowers.Add(go);
            }
        }

        if(listOfFriendlyTowers.Count == 3 && percHp == 1)
            return false;
        else
            return true;
    }

    public static void removeAbililtyIndicators() {
        foreach (GameObject go in Instance.Objects) {
            Component component = go.GetComponent(typeof(IDamageable));
            if((component as IDamageable).Stats.IndicatorNum > 0) {
                (component as IDamageable).Stats.IndicatorNum = 0;
                (component as IDamageable).Stats.UnitMaterials.RemoveAbilityHover();
            }
        }
        GameFunctions.GetCanvas().GetChild(3).GetComponent<Image>().enabled = false;
    }
}
