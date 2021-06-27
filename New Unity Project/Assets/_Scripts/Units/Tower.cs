using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
public class Tower : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected Actor3D agent;

    


    [SerializeField]
    protected GameObject target;

    [SerializeField]
    protected int inRange;

    [SerializeField]
    protected BaseStats stats;

    [SerializeField]
    protected List<GameObject> hitTargets;

    public Actor3D Agent
    {
        get { return agent; }
        //set { agent = value; }
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
    }

    public List<GameObject> HitTargets 
    {
        get { return hitTargets; }
    }

    protected virtual void Update()
    {
        if(stats.CurrHealth > 0) {
            stats.UpdateStats(inRange, agent, hitTargets, target);
            Attack();

            if((inRange > 0 || stats.CurrAttackDelay/stats.AttackDelay >= GameConstants.ATTACK_READY_PERCENTAGE) && target != null) //is in range, OR is 90% thru attack cycle -
                lookAtTarget();
            else 
                resetToCenter();
        }
        else {
            print(gameObject.name + "has died!");
            GameManager.RemoveObjectsFromList(gameObject);
            Destroy(gameObject);
        }
    }

    protected void Attack() {
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

    /* I dont think structres need a vision radius */

    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) { //checks to make sure the target isnt on the same team
            Component damageable = other.transform.parent.parent.GetComponent(typeof(IDamageable));
            if(damageable) {
                Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                if(other.tag == "Range") {//Are we in their range detection object?
                    //if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) anything can attack a tower, ill leave it hear incase somthing with an ability gives a need for this
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
                    //if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) {
                        (unit as IDamageable).InRange--;
                        if((unit as IDamageable).Target == gameObject)
                            (unit as IDamageable).Target = null;
                    //}
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

    protected void lookAtTarget() {
        var targetPosition = target.transform.GetChild(0).position;
        Vector3 direction = targetPosition - agent.Agent.transform.position; //flip this as needed if the tower is in the oppisite direction
        direction.y = 0; // Ignore Y
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // encase somthing here such that the base of the turret does not rotate
        agent.Agent.transform.rotation = Quaternion.RotateTowards(agent.Agent.transform.rotation, targetRotation, GameConstants.ROTATION_SPEED * Time.deltaTime); //the number is degrees/second, maybe differnt per unit
         //
    }

    protected void resetToCenter() { //resets a towers direction
        var targetPosition = agent.Agent.transform.position;
        targetPosition.z = 0;
        Vector3 direction = targetPosition - agent.Agent.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        agent.Agent.transform.rotation = Quaternion.RotateTowards(agent.Agent.transform.rotation, targetRotation, GameConstants.ROTATION_SPEED * Time.deltaTime);
    }

    void IDamageable.TakeDamage(float amount) {
        stats.CurrHealth -= amount;
    }

}
