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

    private void Update()
    {
        if(stats.CurrHealth > 0) {
            agent.Agent.speed = stats.MoveSpeed;
            agent.Agent.stoppingDistance = stats.Range;
            if(target != null) {
                Actor3D targetAgent = (target.GetComponent(typeof(IDamageable)).gameObject.GetComponent(typeof(IDamageable)) as IDamageable).Agent; // this is to get the targets agent... there must be a better way
                agent.Agent.stoppingDistance = agent.Agent.stoppingDistance + targetAgent.Agent.radius; 
                /*
                    This makes it so the target stops when its range reaches the edge of the targets collision, not its center.
                    However, if the unit is chasing a target, no mater how slow the target is, currently the unit will never attack,
                    which is why we may need to subtract 1 from this, BUT if we do that, in the animations we must set it so while
                    a unit is in the attack animation, it does NOT move.
                    Perhaps in the animation, we can set it so the attack will not stop unless the unit as left a certain distance away from the range, then we don't have to subtract 1
                */
            }
            stats.UpdateStats(inRange);
            Attack();
            if(target != null)
                agent.Agent.SetDestination(target.transform.GetChild(0).position);
            else
                agent.Agent.SetDestination(agent.transform.position); //have the agent target itself, meaning don't move as there is no target
        }
    }

    void Attack() {
        if(target != null) {
            if(stats.CurrAttackDelay >= stats.AttackDelay) {
                Component damageable = target.GetComponent(typeof(IDamageable));

                if(damageable) { //is the target damageable
                    if(hitTargets.Contains(target)) {  //this and the above may not be needed, more of a santiy check
                        if(inRange > 0) {
                            GameFunctions.Attack(damageable, stats.BaseDamage);
                            stats.CurrAttackDelay = 0;
                        }
                    }
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) { //checks to make sure the target isnt on the same team
            Component damageable = other.transform.parent.parent.GetComponent(typeof(IDamageable));
            if(damageable) {
                Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                if(other.tag == "Range") //Are we in their range detection object?
                    (unit as IDamageable).InRange++;
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
                    (unit as IDamageable).InRange--;
                    if((unit as IDamageable).Target == gameObject)
                        (unit as IDamageable).Target = null;
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
                            GameObject go = GameFunctions.GetNearestTarget((unit as IDamageable).HitTargets, other.transform.parent.parent.tag, (unit as IDamageable).Stats); //
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
