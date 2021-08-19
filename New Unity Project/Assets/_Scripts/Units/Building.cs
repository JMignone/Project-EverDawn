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
    private Image abilityIndicator;
    private int indicatorNum; //We may need this for abilities that have multiple hit zones

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private int inRange;

    [SerializeField]
    private bool rotates;
    
    [SerializeField]
    private GameConstants.BUILDING_TYPE buildingType; //might not need this as a building that can attack might be better off labeled as a unit

    [SerializeField]
    private GameConstants.BUILDING_SIZE buildingSize;

    [SerializeField]
    private BaseStats stats;

    [SerializeField]
    private SpawnStats spawnStats;

    [SerializeField]
    private List<GameObject> hitTargets;

    [SerializeField]
    private List<GameObject> inRangeTargets;

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

    public bool Rotates
    {
        get { return rotates; }
    }

    public GameConstants.BUILDING_TYPE BuildingType
    {
        get { return buildingType; } 
    }

    public GameConstants.BUILDING_SIZE BuildingSize
    {
        get { return buildingSize; } 
    }

    public BaseStats Stats
    {
        get { return stats; }
        //set { stats = value; }
    }

    public SpawnStats SpawnStats
    {
        get { return spawnStats; }
    }

    public List<GameObject> HitTargets
    {
        get { return hitTargets; }
    }

    public List<GameObject> InRangeTargets
    {
        get { return inRangeTargets; }
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
        stats.StartStats(gameObject);
        
        isHoveringAbility = false;
        indicatorNum = 0;
        abilityIndicator.enabled = false;
        abilityIndicator.rectTransform.sizeDelta = new Vector2(2*agent.HitBox.radius + 1, 2*agent.HitBox.radius + 1); 
        // + 1 is better for the knob UI, if we get our own UI image, we may want to remove it
    }

    private void Update()
    {
        if(stats.CurrHealth > 0) {
            agent.Agent.speed = stats.MoveSpeed;

            if(buildingType == GameConstants.BUILDING_TYPE.SPAWN) {
                stats.UpdateBuildingStats();
                Spawn();
            }
            else if(buildingType == GameConstants.BUILDING_TYPE.ATTACK) {
                if((target == null || inRange == 0) && stats.CanAct()) { //if the target is null, we must find the closest target in hit targets. If hit targets is empty or failed, find the closest tower
                    if(hitTargets.Count > 0) {
                        GameObject go = GameFunctions.GetNearestTarget(hitTargets, gameObject.tag, stats);
                        if(go != null)
                            target = go;
                        else {
                            List<GameObject> towers = GameManager.Instance.TowerObjects;
                            towers = GameManager.GetAllEnemies(transform.GetChild(0).position, towers, gameObject.tag); //sending in only towers
                            target = GameFunctions.GetNearestTarget(towers, gameObject.tag, stats);
                        }
                    }
                    else {
                        List<GameObject> towers = GameManager.Instance.TowerObjects;
                        towers = GameManager.GetAllEnemies(transform.GetChild(0).position, towers, gameObject.tag); //sending in only towers
                        target = GameFunctions.GetNearestTarget(towers, gameObject.tag, stats);
                    }
                }

                stats.UpdateStats(inRange, agent, hitTargets, target);
                Attack();

                if(target != null && rotates) {
                    if(hitTargets.Contains(target)) {
                        if(inRange > 0 || stats.CurrAttackDelay/stats.AttackDelay >= GameConstants.ATTACK_READY_PERCENTAGE) { //is in range, OR is 90% thru attack cycle -
                            lookAtTarget();
                        }
                    }
                }
            }
        }
        else {
            print(gameObject.name + " has died!");
            GameManager.RemoveObjectsFromList(gameObject);
            Destroy(gameObject);
        }
    }

    void Spawn() {
        if(stats.CurrAttackDelay >= stats.AttackDelay && stats.IsReady()) {
            Vector3 position = agent.transform.position;
            position += transform.forward * -7;
            Quaternion rotation = agent.transform.rotation;

            GameObject spawnedUnit = Instantiate(spawnStats.UnitToSpawn, position, rotation, transform.parent);
            spawnedUnit.transform.Rotate(0, 180, 0, Space.Self);
            GameManager.AddObjectToList(spawnedUnit);
        }
    }

    void Attack() {
        if(target != null) {
            if(stats.CurrAttackDelay >= stats.AttackDelay) {
                Component damageable = target.GetComponent(typeof(IDamageable));

                if(damageable) { //is the target damageable
                    if(hitTargets.Contains(target)) {  //this is needed for the rare occurance that a unit is 90% done with attack delay and the target leaves its range. It can still do its attack if its within vision given that its attack was already *90% thru
                        if(stats.AOEStats.AreaOfEffect)
                            stats.AOEStats.Explode(gameObject, target);
                        else {
                            GameFunctions.Attack(damageable, stats.BaseDamage);
                            stats.ApplyAffects(damageable);
                        }
                        stats.CurrAttackDelay = 0;
                    }
                }
            }
        } 
    }

    // !! THIS WILL LIKELY NEED TO BE CHANGED SUCH THAT ONLY THE TOP OF THE BUILDING ROTATES !!
    void lookAtTarget() {
        var targetPosition = target.transform.GetChild(0).position;  //

        Vector3 direction = targetPosition - agent.Agent.transform.position;
        direction.y = 0; // Ignore Y, usful for airborne units
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        agent.Agent.transform.rotation = Quaternion.RotateTowards(agent.Agent.transform.rotation, targetRotation, stats.RotationSpeed * Time.deltaTime); //the number is degrees/second, maybe differnt per unit
    }

    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) { //checks to make sure the target isnt on the same team
            if(other.CompareTag("Projectile")) { //Did we get hit by a skill shot?
                Projectile projectile = other.transform.parent.parent.GetComponent<Projectile>();
                Component unit = this.GetComponent(typeof(IDamageable));
                projectile.hit(unit);
            }
            else if(other.CompareTag("AbilityHighlight")) { //Our we getting previewed for an abililty?
                AbilityPreview ability = other.GetComponent<AbilityPreview>();
                if(GameFunctions.WillHit(ability.ObjectAttackable, this.GetComponent(typeof(IDamageable)))) {
                    indicatorNum++;
                    abilityIndicator.enabled = true;
                }
            }
            else { //is it another units vision/range?
                Component damageable = other.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(damageable) {
                    Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                    if(other.CompareTag("Range")) {//Are we in their range detection object?
                        if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) { //only if the unit can actually target this one should we adjust this value
                            (unit as IDamageable).InRange++;
                            if(!(unit as IDamageable).InRangeTargets.Contains(gameObject))
                                (unit as IDamageable).InRangeTargets.Add(gameObject);
                            if(((unit as IDamageable).InRange == 1 || (unit as IDamageable).Target == null) && (unit as IDamageable).Stats.CanAct()) {
                                GameObject go = GameFunctions.GetNearestTarget((unit as IDamageable).HitTargets, other.transform.parent.parent.tag, (unit as IDamageable).Stats);
                                if(go != null)
                                    (unit as IDamageable).Target = go;
                            }
                        }
                    }
                    else if(other.CompareTag("Vision")) { //Are we in their vision detection object?
                        if(!(unit as IDamageable).HitTargets.Contains(gameObject))
                            (unit as IDamageable).HitTargets.Add(gameObject);
                    }
                }
            }
        }
    }

    public void OnTriggerExit(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) { //checks to make sure the target isnt on the same team
            if(other.CompareTag("Projectile")) { //Did we get hit by a skill shot?
                //print("Projectile");
            }
            else if(other.CompareTag("AbilityHighlight")) { //Our we getting previewed for an abililty?
                AbilityPreview ability = other.GetComponent<AbilityPreview>();
                if(GameFunctions.WillHit(ability.ObjectAttackable, this.GetComponent(typeof(IDamageable)))) {
                    indicatorNum--;
                    if(indicatorNum == 0)
                        abilityIndicator.enabled = false;
                }
            }
            else { //is it another units vision/range?
                Component damageable = other.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(damageable) {
                    Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                    if(other.CompareTag("Range")) { //Are we in their Range detection object?
                        if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) {
                            (unit as IDamageable).InRange--;
                            if((unit as IDamageable).InRangeTargets.Contains(gameObject))
                                (unit as IDamageable).InRangeTargets.Remove(gameObject);
                            if((unit as IDamageable).Target == gameObject)
                                (unit as IDamageable).Target = null;
                        }
                    }
                    else if(other.CompareTag("Vision")) { //Are we in their vision detection object?
                        if((unit as IDamageable).HitTargets.Contains(gameObject))
                            (unit as IDamageable).HitTargets.Remove(gameObject);
                        if((unit as IDamageable).Target == gameObject) //if the units target was the one who left the vision
                            (unit as IDamageable).Target = null; 
                    }
                }
            }
        }
    }
    /*
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
                        if(((unit as IDamageable).InRange == 0 || (unit as IDamageable).Target == null) && (unit as IDamageable).Stats.CanAct()) {
                            GameObject go = GameFunctions.GetNearestTarget((unit as IDamageable).HitTargets, other.transform.parent.parent.tag, (unit as IDamageable).Stats);
                            if(go != null)
                                (unit as IDamageable).Target = go;
                        }
                    }
                }
            }
        }
    }*/

    void IDamageable.TakeDamage(float amount) {
        stats.CurrHealth -= amount;
    }
}
