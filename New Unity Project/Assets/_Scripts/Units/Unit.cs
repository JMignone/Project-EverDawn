using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Actor3D agent;

    [SerializeField]
    private Actor2D unitSprite;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private int inRange;

    [SerializeField]
    private BaseStats stats;

    [SerializeField]
    private List<GameObject> hitTargets;

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

    private void Start()
    {
        List<GameObject> towers = GameManager.Instance.TowerObjects;
        towers = GameManager.GetAllEnemies(transform.GetChild(0).position, towers, gameObject.tag); //sending in only towers
        target = GameFunctions.GetNearestTarget(towers, gameObject.tag, stats);
    }

    private void Update()
    {
        if(stats.CurrHealth > 0) {
            agent.Agent.speed = stats.MoveSpeed;
            agent.Agent.stoppingDistance = stats.Range; //this may no longer be needed, and should be set to a small number for all units
            if(target != null) {
                Actor3D targetAgent = (target.GetComponent(typeof(IDamageable)).gameObject.GetComponent(typeof(IDamageable)) as IDamageable).Agent; // this is to get the targets agent... there must be a better way
                //agent.Agent.stoppingDistance = agent.Agent.stoppingDistance + targetAgent.HitBox.radius;  -
                /*
                    This makes it so the target stops when its range reaches the edge of the targets collision, not its center.
                    However, if the unit is chasing a target, no mater how slow the target is, currently the unit will never attack,
                    which is why we may need to subtract 1 from this, BUT if we do that, in the animations we must set it so while
                    a unit is in the attack animation, it does NOT move.
                    Perhaps in the animation, we can set it so the attack will not stop unless the unit as left a certain distance away from the range, then we don't have to subtract 1
                */
            }
            else {
                List<GameObject> towers = GameManager.Instance.TowerObjects;
                towers = GameManager.GetAllEnemies(transform.GetChild(0).position, towers, gameObject.tag); //sending in only towers
                target = GameFunctions.GetNearestTarget(towers, gameObject.tag, stats);
            }
            stats.UpdateStats(inRange, agent, hitTargets, target);
            Attack();
            if(target != null) {
                agent.Agent.SetDestination(target.transform.GetChild(0).position);
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
                        if(inRange > 0) {
                            GameFunctions.Attack(damageable, stats.BaseDamage);
                            stats.CurrAttackDelay = 0;
                        }

                    }
                }
            }
        }
        else { //if target is null, it means there is no valid target within vision, so we will set its target to the closest tower
            List<GameObject> towers = GameManager.Instance.TowerObjects;
            towers = GameManager.GetAllEnemies(transform.GetChild(0).position, towers, gameObject.tag); //sending in only towers
            target = GameFunctions.GetNearestTarget(towers, gameObject.tag, stats);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) { //checks to make sure the target isnt on the same team
            Component damageable = other.transform.parent.parent.GetComponent(typeof(IDamageable));
            if(damageable) {
                Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                if(other.tag == "Range") {//Are we in their range detection object?
                    if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) //only if the unit can actually target this one should we adjust this value
                        (unit as IDamageable).InRange++;
                }
                else if(other.tag == "Vision") { //Are we in their vision detection object?
                    if(!(unit as IDamageable).HitTargets.Contains(gameObject))
                        (unit as IDamageable).HitTargets.Add(gameObject);
                }
            }
        }
    }

    public void OnTriggerExit(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) { //checks to make sure the target isnt on the same team
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

    void lookAtTarget() {
        var targetPosition = target.transform.GetChild(0).position;  //
        //targetPosition.y = agent.Agent.transform.position.y;       //look at your target when your attacking !! This part was removed because it was not lifelike, 
        //agent.Agent.transform.LookAt(targetPosition);              //the unit was ALWAYS in the right direction instantly. Below rotates the unit at a degrees/second speed

        Vector3 direction = targetPosition - agent.Agent.transform.position;
        direction.y = 0; // Ignore Y, usful for airborne units
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        agent.Agent.transform.rotation = Quaternion.RotateTowards(agent.Agent.transform.rotation, targetRotation, GameConstants.ROTATION_SPEED * Time.deltaTime); //the number is degrees/second, maybe differnt per unit
    }

    void IDamageable.TakeDamage(float amount) {
        stats.CurrHealth -= amount;
    }
}
