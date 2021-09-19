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
    [SerializeField]
    private GameObject gameplay_HUD;
    [SerializeField]
    private GameObject end_game_screen;

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

    public GameObject Gameplay_HUD
    {
        get { return gameplay_HUD; }
    }

    public GameObject End_Game_Screen
    {
        get { return end_game_screen; }
    }

    private void Awake()
    {
        if(instance != this)
            instance = this;
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
                    if((component as IDamageable).InRangeTargets.Contains(objectToRemove)) {
                        (component as IDamageable).InRange--;
                        (component as IDamageable).InRangeTargets.Remove(objectToRemove); 
                    }
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
                    if( Vector3.Distance(objectToRemovePosition, new Vector3(go.transform.GetChild(0).position.x, 0, go.transform.GetChild(0).position.z)) <= ((component as IDamageable).Stats.Range + objectToRemoveAgent.HitBox.radius) ) //If the unit that died is within range
                        (component as IDamageable).InRange--;
                }
            }
        }

        if(!objectToRemove.CompareTag(GameConstants.PLAYER_TAG)) {
            if(isKeep) {
                Instance.Players[0].Score = 3;
                GameManager.EndGame();
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
    /*  else { //we may send the enemy score information over the internet rather than the players side instead
            if(leftTower) 
                Instance.Players[1].LeftZone = true;
            else 
                Instance.Players[1].RightZone = true;
            Instance.Players[1].Score++;
        }
    */

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
    public static List<GameObject> GetAllEnemies(Vector3 pos, List<GameObject> objects, string tag) {
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
            if((component as IDamageable).IndicatorNum > 0) {
                (component as IDamageable).AbilityIndicator.enabled = false;
                (component as IDamageable).IndicatorNum = 0;
            }
        }
        GameObject hud = GameObject.Find(GameConstants.HUD_CANVAS);
        hud.transform.GetChild(2).GetComponent<Image>().enabled = false;
    }

    //Ends the game
    public static void EndGame()
    {
        //Destroy all non-tower type game objects that are in play and remove them from the game 
        foreach (GameObject go in Instance.Objects)
        {
            GameObject.Destroy(go);
            //RemoveObjectsFromList(go);
        }

        //Destroy all tower type game objects that are in play and remove them from the game 
        foreach (GameObject go in Instance.TowerObjects)
        {
            GameObject.Destroy(go);
            //RemoveObjectsFromList(go);
        }

        /*TODO
         * While not exactly necessary, since it'll get reset when the scene unloads, it'd be good to clear the Objects and TowerObjects lists at the end of the game.
         * The lower loop currently does not work. If included in the above foreach loops it fails to destroy any game objects lower in the list than the enemy keep.
        
        foreach (GameObject go in Instance.Objects)
        {
            RemoveObjectsFromList(go);
        }
        */

        //Hide Gameplay HUD
        Instance.Gameplay_HUD.SetActive(false);

        //Show End Game Screen
        Instance.End_Game_Screen.SetActive(true);
    }
}
