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

    public BaseStats Stats
    {
        get { return stats; }
        //set { stats = value; }
    }

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

    public List<GameObject> HitTargets
    {
        get { return hitTargets; }
        //set { hitTargets = value; }
    }

    private void Update()
    {
        if(stats.CurrHealth > 0) {
            agent.Agent.speed = stats.MoveSpeed;
            agent.Agent.stoppingDistance = stats.Range; //
            if(inRange > 0)
                agent.Agent.stoppingDistance = agent.Agent.stoppingDistance + 4; // !! the 4 must be the target unit radius !!
            stats.UpdateStats();
            Attack();
            if(target != null) {
                //agent.Agent.SetDestination(target.transform.position);
                agent.Agent.SetDestination(target.transform.GetChild(0).position);
            }
            else {
                agent.Agent.SetDestination(gameObject.transform.GetChild(0).position);
            }

        }

    }

    void Attack() {
        if(target != null) {
            if(stats.CurrAttackDelay >= stats.AttackDelay) {
                Component damageable = target.GetComponent(typeof(IDamageable));

                if(damageable) { //is the target damageable
                    if(hitTargets.Contains(target)) {  //this and the above may not be needed, more of a santiy check
                        //if(GameFunctions.CanAttack(gameObject.tag, target.tag, damageable, stats)) {
                        if(inRange > 0) {
                            GameFunctions.Attack(damageable, stats.BaseDamage);
                            stats.CurrAttackDelay = 0;
                        }
                        //}
                    }
                }
            }
        }
    }
    /*
    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) {//checks to make sure the target isnt on the same team
            if(other.tag == "Range") { //Are we the Range detection object
                print("RANGE");
                Component damageable = other.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(damageable) {
                    if(!hitTargets.Contains(damageable.gameObject)) {
                        hitTargets.Add(damageable.gameObject);
                    }
                }
            }
            else if(other.tag == "Vision") { //Are we the vision detection object
                print("VISION");
            }
            print("My tag: " + gameObject.tag);
            print("The Units tag: " + other.transform.parent.tag);
            print("The Units detection tag: " + other.tag);
        }
    }*/

    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) {//checks to make sure the target isnt on the same team
            Component damageable = other.transform.parent.parent.GetComponent(typeof(IDamageable));
            if(damageable) {
                Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                if(other.tag == "Range") { //Are we in their range detection object?
                    GameObject targetsTarget = (unit as IDamageable).Target;
                    //if(targetsTarget == gameObject)
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
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) {//checks to make sure the target isnt on the same team
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
                    if((unit as IDamageable).Target == gameObject)
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
                if(other.tag == "Range") { // I dont think this actaully needs to be here
                    /*if(inRange == 0) {
                        GameObject go = GameFunctions.GetNearestTarget(hitTargets, stats.DetectionObject, gameObject.tag, stats); //
                        if(go != null)
                            target = go;
                    }*/
                }
                else if(other.tag == "Vision") { //Are we in their vision detection object?
                    if((unit as IDamageable).HitTargets.Count > 0) {
                        if((unit as IDamageable).InRange == 0 || (unit as IDamageable).Target == null) {
                            GameObject go = GameFunctions.GetNearestTarget((unit as IDamageable).HitTargets, (unit as IDamageable).Stats.DetectionObject, other.transform.parent.parent.tag, (unit as IDamageable).Stats); //
                            if(go != null)
                                (unit as IDamageable).Target = go;
                        }
                    }
                }
            }
        }
    }

    /*
    public void OnTriggerStay(Collider other) {
        if(!other.transform.parent.parent.gameObject.CompareTag(gameObject.tag)) {
            if(other.tag == "Range") {
                if(hitTargets.Count > 0) {
                    //GameObject go = GameFunctions.GetNearestTarget(hitTargets, stats.DetectionObject, gameObject.tag, stats.Range);
                    GameObject go = GameFunctions.GetNearestTarget(hitTargets, stats.DetectionObject, gameObject.tag, stats); //

                    if(go != null)
                        target = go;
                }
            }
            else if(other.tag == "Vision") { //Are we the vision detection object
                print("VISION");
            }
        }
    }*/

    void IDamageable.TakeDamage(float amount) {
        stats.CurrHealth -= amount;
    }
}
