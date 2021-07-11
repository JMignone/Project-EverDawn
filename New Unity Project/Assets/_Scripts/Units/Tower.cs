using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System;
public class Tower : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected Actor3D agent;

    


    [SerializeField]
    protected Image abilityIndicator;
    private int indicatorNum; //We may need this for abilities that have multiple hit zones

    [SerializeField]
    protected GameObject target;

    [SerializeField]
    protected int inRange;

    [SerializeField]
    protected BaseStats stats;

    [SerializeField]
    protected List<GameObject> hitTargets;

    [SerializeField]
    protected bool leftTower;

    protected bool isHoveringAbility;

    public Actor3D Agent
    {
        get { return agent; }
        //set { agent = value; }
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
    }

    public List<GameObject> HitTargets 
    {
        get { return hitTargets; }
    }

    public bool LeftTower
    {
        get { return leftTower; }
        set { leftTower = value; }
    }

    public bool IsHoveringAbility
    {
        get { return isHoveringAbility; }
        set { isHoveringAbility = value; }
    }

    protected void Start()
    {
        isHoveringAbility = false;
        abilityIndicator.enabled = false;
        abilityIndicator.rectTransform.sizeDelta = new Vector2(2*agent.HitBox.radius + 1, 2*agent.HitBox.radius + 1); 
        // + 1 is better for the knob UI, if we get our own UI image, we may want to remove it
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
            GameManager.RemoveObjectsFromList(gameObject, leftTower);
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