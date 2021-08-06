using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour, IDamageable
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
    private BaseStats stats;

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

    public BaseStats Stats
    {
        get { return stats; }
        //set { stats = value; }
    }

    public List<GameObject> HitTargets
    {
        get { return hitTargets; }
        //set { hitTargets = value; }
    }

    public List<GameObject> InRangeTargets
    {
        get { return inRangeTargets; }
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
        agent.Agent.stoppingDistance = 0; //Set to be zero, incase someone forgets or accidently changes this value to be a big number
        agent.Agent.speed = stats.MoveSpeed;

        stats.FrozenStats.StartFrozenStats(gameObject);
        stats.SlowedStats.StartSlowedStats(gameObject);
        stats.PoisonedStats.StartPoisonedStats(gameObject);

        isHoveringAbility = false;
        indicatorNum = 0;
        abilityIndicator.enabled = false;
        abilityIndicator.rectTransform.sizeDelta = new Vector2(2*agent.HitBox.radius + 1, 2*agent.HitBox.radius + 1); 
        // + 1 is better for the knob UI, if we get our own UI image, we may want to remove it
    }

    private void Update()
    {
        if(stats.CurrHealth > 0) {
            if(target == null && !stats.FrozenStats.IsFrozen && !isCastingAbility) { //if the target is null, we must find the closest target in hit targets. If hit targets is empty or failed, find the closest tower
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
            if(target != null) {
                Vector3 direction = target.transform.GetChild(0).position - agent.transform.position;
                agent.Agent.SetDestination(target.transform.GetChild(0).position - (direction.normalized * .25f));
                if(hitTargets.Contains(target)) {
                    if(inRange > 0 || stats.CurrAttackDelay/stats.AttackDelay >= GameConstants.ATTACK_READY_PERCENTAGE) { //is in range, OR is 90% thru attack cycle -
                        lookAtTarget();
                        agent.Agent.SetDestination(agent.transform.position);
                    }
                }
            }
            else
                agent.Agent.SetDestination(agent.transform.position); //have the agent target itself, meaning don't move as there is no target
        }
        else {
            print(gameObject.name + " has died!");
            GameManager.RemoveObjectsFromList(gameObject);
            Destroy(gameObject);
        }
    }

    void Attack() {
        if(target != null) {
            if(stats.CurrAttackDelay >= stats.AttackDelay) {
                Component damageable = target.GetComponent(typeof(IDamageable));

                if(damageable) { //is the target damageable
                    if(hitTargets.Contains(target)) {  //this is needed for the rare occurance that a unit is 90% done with attack delay and the target leaves its range. It can still do its attack if its within vision given that its attack was already *90% thru
                        //if(inRange > 0) { removed because this prevented the above rare occurance from happening
                            if(stats.AOEStats.AreaOfEffect)
                                stats.AOEStats.Explode(gameObject, target);
                            else
                                GameFunctions.Attack(damageable, stats.BaseDamage);
                            stats.CurrAttackDelay = 0;
                        //}

                    }
                }
            }
        } /* I dont believe this is needed, nor should it be here
        else { //if target is null, it means there is no valid target within vision, so we will set its target to the closest tower
            List<GameObject> towers = GameManager.Instance.TowerObjects;
            towers = GameManager.GetAllEnemies(transform.GetChild(0).position, towers, gameObject.tag); //sending in only towers
            target = GameFunctions.GetNearestTarget(towers, gameObject.tag, stats);
        }*/
    }

    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) { //checks to make sure the target isnt on the same team
            if(other.tag == "Projectile") { //Did we get hit by a skill shot?
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
                            if(!(unit as IDamageable).InRangeTargets.Contains(gameObject))
                                (unit as IDamageable).InRangeTargets.Add(gameObject);
                            if( ((unit as IDamageable).InRange == 1 || (unit as IDamageable).Target == null)  && !(unit as IDamageable).Stats.FrozenStats.IsFrozen) { //we need this block here as well as stay in the case that a unit is placed inside a units range
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
            if(other.tag == "Projectile") { //Did we get hit by a skill shot?
                //print("Projectile");
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
                            if((unit as IDamageable).InRangeTargets.Contains(gameObject))
                                (unit as IDamageable).InRangeTargets.Remove(gameObject);
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
                        if( ((unit as IDamageable).InRange == 0 || (unit as IDamageable).Target == null) && !(unit as IDamageable).Stats.FrozenStats.IsFrozen) {
                            GameObject go = GameFunctions.GetNearestTarget((unit as IDamageable).HitTargets, other.transform.parent.parent.tag, (unit as IDamageable).Stats);
                            if(go != null)
                                (unit as IDamageable).Target = go;
                        }
                    }
                }
            }
        }
    }

    private GameObject getAbilityObjects() {
        return transform.GetChild(1).GetChild(4).gameObject;
    }

    void lookAtTarget() {
        var targetPosition = target.transform.GetChild(0).position;  //

        Vector3 direction = targetPosition - agent.Agent.transform.position;
        direction.y = 0; // Ignore Y, usful for airborne units
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        agent.Agent.transform.rotation = Quaternion.RotateTowards(agent.Agent.transform.rotation, targetRotation, stats.RotationSpeed * Time.deltaTime); //the number is degrees/second, maybe differnt per unit
    }

    void IDamageable.TakeDamage(float amount) {
        stats.CurrHealth -= amount;
    }
}
