using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BaseStats
{
    [SerializeField]
    private float currHealth;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float range;
    [SerializeField]
    private float visionRange;
    [SerializeField]
    private float baseDamage;
    [SerializeField]
    private float attackDelay;
    [SerializeField]
    private float currAttackDelay;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private SphereCollider detectionObject;
    [SerializeField]
    private SphereCollider visionObject;
    [SerializeField]
    private GameConstants.MOVEMENT_TYPE movementType;
    [SerializeField]
    private GameConstants.OBJECT_ATTACKABLE objectAttackable;
    [SerializeField]
    private GameConstants.UNIT_TYPE unitType;
    [SerializeField]
    private GameConstants.UNIT_GROUPING unitGrouping;
    [SerializeField]
    private GameConstants.ATTACK_PRIORITY attackPriority;
    [SerializeField]
    private GameConstants.UNIT_RANGE unitRange;
    
    public float PercentHealth {
        get { return currHealth/maxHealth; }
    }

    public float CurrHealth {
        get { return currHealth; }
        set { 
            if(value <= 0)
                currHealth = 0;
            else if(value >= maxHealth)
                currHealth = maxHealth;
            else
                currHealth = value;
        }
    }

    public float MaxHealth {
        get { return maxHealth; }
        //set { maxHealth = value; }
    }

    public float Range
    {
        get { return range; }
        //set { range = value; }
    }

    public float VisionRange
    {
        get { return visionRange; }
        //set { visionRange = value; }
    }

    public float BaseDamage
    {
        get { return baseDamage; }
        //set { baseDamage = value; }
    }

    public float AttackDelay
    {
        get { return attackDelay; }
        //set { attackDelay = value; }
    }

    public float CurrAttackDelay
    {
        get { return currAttackDelay; }
        set { currAttackDelay = value; }
    }

    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public Image HealthBar
    {
        get { return healthBar; }
        //set { healthBar = value; }
    }

    public SphereCollider DetectionObject
    {
        get { return detectionObject; }
        //set { detectionObject = value; }
    }

    public SphereCollider VisionObject
    {
        get { return visionObject; }
        //set { visionObject = value; }
    }

    public GameConstants.MOVEMENT_TYPE MovementType
    {
        get { return movementType; }
        //set { movementType = value; }
    }
    
    public GameConstants.OBJECT_ATTACKABLE ObjectAttackable
    {
        get { return objectAttackable; }
        //set { objectAttackable = value; }
    }

    public GameConstants.UNIT_TYPE UnitType
    {
        get { return unitType; }
        //set { unitType = value; }
    }

    public GameConstants.UNIT_GROUPING UnitGrouping
    {
        get { return unitGrouping; }
        //set { unitGrouping = value; }
    }

    public GameConstants.ATTACK_PRIORITY AttackPriority
    {
        get { return attackPriority; }
        //set { attackPriority = value; }
    }

    public GameConstants.UNIT_RANGE UnitRange
    {
        get { return unitRange; }
        //set { unitRange = value; }
    }

    public void UpdateStats(int inRange, Actor3D unitAgent, List<GameObject> hitTargets, GameObject target) {
        if(PercentHealth == 1) {
            HealthBar.enabled = false;
            HealthBar.transform.GetChild(0).gameObject.SetActive(false); //this is the image border
        }
        else {
            HealthBar.enabled = true;
            HealthBar.transform.GetChild(0).gameObject.SetActive(true);
        }
        HealthBar.fillAmount = PercentHealth;

        detectionObject.radius = range;
        visionObject.radius = visionRange;

        bool inVision = false;
        if(target != null)
            inVision = hitTargets.Contains(target);       
        
        if(inRange > 0 || (currAttackDelay/attackDelay >= GameConstants.ATTACK_READY_PERCENTAGE && inVision)) { //if target is inRange, or the attack is nearly ready and their within vision
            
            if(target != null) {
                Vector3 directionToTarget = unitAgent.transform.position - target.transform.GetChild(0).position;
                directionToTarget.y = 0; // Ignore Y, usful for airborne units
                float angle = Vector3.Angle(unitAgent.transform.forward, directionToTarget); 

                //180 - *** might be the source of an error later on, depends on the angle of a unit agent at the start, right now they are all 180
                if(180-Mathf.Abs(angle) < GameConstants.MAXIMUM_ATTACK_ANGLE) {
                    if(currAttackDelay < attackDelay) 
                        currAttackDelay += Time.deltaTime;
                    else
                        currAttackDelay = 0;
                }
                else {
                    if(currAttackDelay < attackDelay*GameConstants.ATTACK_READY_PERCENTAGE) 
                        currAttackDelay += Time.deltaTime;
                }
            }
            else { //this may occur for a few frames when a units target dies, but there are still other units it can target, it just has not updated to the new target yet
                if(currAttackDelay < attackDelay*GameConstants.ATTACK_READY_PERCENTAGE) 
                    currAttackDelay += Time.deltaTime;
            }
        }
        else if(inVision) { //if the target is within vision
            if(currAttackDelay < attackDelay*GameConstants.ATTACK_CHARGE_LIMITER) 
                currAttackDelay += Time.deltaTime;
        }
        else 
            currAttackDelay = 0;
    }

    public void UpdateBuildingStats() {
        if(PercentHealth == 1) {
            HealthBar.enabled = false;
            HealthBar.transform.GetChild(0).gameObject.SetActive(false); //this is the image border
        }
        else {
            HealthBar.enabled = true;
            HealthBar.transform.GetChild(0).gameObject.SetActive(true);
        }
        HealthBar.fillAmount = PercentHealth;
        currHealth -= .05f*maxHealth * Time.deltaTime ; // lowers hp by 5% of maxHp every second ?? should this line be at the top ??

        if(currAttackDelay < attackDelay) 
            currAttackDelay += Time.deltaTime;
        else
            currAttackDelay = 0;
    }
    
}
