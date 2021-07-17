using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Actor3D agent;

    [SerializeField]
    private Actor2D unitSprite;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private Image abilityIndicator;
    private int indicatorNum; //We may need this for abilities that have multiple hit zones

    [SerializeField]
    private int inRange;

    [SerializeField]
    private GameObject unitToSpawn;

    [SerializeField]
    private BaseStats stats;

    [SerializeField]
    private GameConstants.BUILDING_TYPE buildingType; //might not need this as a building that can attack might be better off labeled as a unit

    [SerializeField]
    private List<GameObject> hitTargets;

    private bool isHoveringAbility;
    private bool isCastingAbility;

    public Actor3D Agent
    {
        get { return agent; }
        //set { agent = value; }
    }

    public Actor2D UnitSprite
    {
        get { return unitSprite; }
        //set { unitSprite = value; }
    }

    public Image AbilityIndicator
    {
        get { return abilityIndicator; }
    }

    public int IndicatorNum
    {
        get { return indicatorNum; }
        set { indicatorNum = value; }
    }

    public GameObject Target
    {
        get { return target; }
        set { target = value; }
    }

    public int InRange
    {
        get { return inRange; }
        set { inRange = value; }
    }

    public GameObject UnitToSpawn
    {
        get { return unitToSpawn; }
        //set { unitToSpawn = value; }
    }

    public BaseStats Stats
    {
        get { return stats; }
        //set { stats = value; }
    }

    public GameConstants.BUILDING_TYPE BuildingType
    {
        get { return buildingType; }
        //set { buildingType = value; }   
    }

    public List<GameObject> HitTargets
    {
        get { return hitTargets; }
        //set { hitTargets = value; }
    }

    public bool IsHoveringAbility
    {
        get { return isHoveringAbility; }
        set { isHoveringAbility = value; }
    }

    public bool IsCastingAbility
    {
        get { return isCastingAbility; }
        set { isCastingAbility = value; }
    }

    private void Start()
    {
        isHoveringAbility = false;
        abilityIndicator.enabled = false;
        abilityIndicator.rectTransform.sizeDelta = new Vector2(2*agent.HitBox.radius + 1, 2*agent.HitBox.radius + 1); 
        // + 1 is better for the knob UI, if we get our own UI image, we may want to remove it
    }

    private void Update()
    {
        if(stats.CurrHealth > 0) {
            agent.Agent.speed = stats.MoveSpeed;
            
            stats.UpdateBuildingStats();

            if(buildingType == GameConstants.BUILDING_TYPE.SPAWN)
                Spawn();
        }
        else {
            print(gameObject.name + " has died!");
            GameManager.RemoveObjectsFromList(gameObject);
            Destroy(gameObject);
        }
    }

    void Spawn() {
        if(stats.CurrAttackDelay >= stats.AttackDelay) {
            Vector3 position = agent.transform.position;
            position += transform.forward * -6;
            Quaternion rotation = agent.transform.rotation;

            GameObject spawnedUnit = Instantiate(unitToSpawn, position, rotation, transform.parent);
            spawnedUnit.transform.Rotate(0, 180, 0, Space.Self);
            GameManager.AddObjectToList(spawnedUnit);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) { //checks to make sure the target isnt on the same team
            if(other.tag == "SkillShot") { //Did we get hit by a skill shot?
                Projectile projectile = other.transform.parent.parent.GetComponent<Projectile>();
                Component unit = this.GetComponent(typeof(IDamageable));
                projectile.hit(unit);
            }
            else if(other.tag == "AbilityHighlight") { //Our we getting previewed for an abililty?
                indicatorNum++;
                abilityIndicator.enabled = true;
            }
            else { //is it another units vision/range?
                Component damageable = other.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(damageable) {
                    Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                    if(other.tag == "Range") {//Are we in their range detection object?
                        if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) { //only if the unit can actually target this one should we adjust this value
                            (unit as IDamageable).InRange++;
                            if((unit as IDamageable).InRange == 1 || (unit as IDamageable).Target == null) {
                                GameObject go = GameFunctions.GetNearestTarget((unit as IDamageable).HitTargets, other.transform.parent.parent.tag, (unit as IDamageable).Stats);
                                if(go != null)
                                    (unit as IDamageable).Target = go;
                            }
                        }
                    }
                    else if(other.tag == "Vision") { //Are we in their vision detection object?
                        if(!(unit as IDamageable).HitTargets.Contains(gameObject))
                            (unit as IDamageable).HitTargets.Add(gameObject);
                    }
                }
            }
        }
    }

    public void OnTriggerExit(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) { //checks to make sure the target isnt on the same team
            if(other.tag == "SkillShot") { //Did we get hit by a skill shot?
                print("SKILLSHOT");
            }
            else if(other.tag == "AbilityHighlight") { //Our we getting previewed for an abililty?
                indicatorNum--;
                if(indicatorNum == 0)
                    abilityIndicator.enabled = false;
            }
            else { //is it another units vision/range?
                Component damageable = other.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(damageable) {
                    Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                    if(other.tag == "Range") { //Are we in their Range detection object?
                        if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) {
                            (unit as IDamageable).InRange--;
                            if((unit as IDamageable).Target == gameObject)
                                (unit as IDamageable).Target = null;
                        }
                    }
                    else if(other.tag == "Vision") { //Are we in their vision detection object?
                        if((unit as IDamageable).HitTargets.Contains(gameObject))
                            (unit as IDamageable).HitTargets.Remove(gameObject);
                        if((unit as IDamageable).Target == gameObject) //if the units target was the one who left the vision
                            (unit as IDamageable).Target = null; 
                    }
                }
            }
        }
    }

    public void OnTriggerStay(Collider other) {
        if(!other.transform.parent.parent.gameObject.CompareTag(gameObject.tag)) {
            Component damageable = other.transform.parent.parent.GetComponent(typeof(IDamageable));
            if(damageable) {
                Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                if(other.tag == "Range") { // I dont think this actually needs to be here
                    // placeholder
                }
                else if(other.tag == "Vision") { //Are we in their vision detection object?
                    if((unit as IDamageable).HitTargets.Count > 0) {
                        if((unit as IDamageable).InRange == 0 || (unit as IDamageable).Target == null) {
                            GameObject go = GameFunctions.GetNearestTarget((unit as IDamageable).HitTargets, other.transform.parent.parent.tag, (unit as IDamageable).Stats);
                            if(go != null)
                                (unit as IDamageable).Target = go;
                        }
                    }
                }
            }
        }
    }

    void IDamageable.TakeDamage(float amount) {
        stats.CurrHealth -= amount;
    }
}
