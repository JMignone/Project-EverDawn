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
    [Tooltip("A number from (0-1) that will determine what percentage of health will be removed each tick. A number below .1 is highly recomended")]
    [SerializeField]
    private float healthDecay;
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
    private float rotationSpeed;
    [SerializeField]
    private SummoningSicknessUI summoningSicknessUI;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private SphereCollider detectionObject;
    [SerializeField]
    private SphereCollider visionObject;
    [Tooltip("Lets other units know whether this unit is a ground or flying unit")]
    [SerializeField]
    private GameConstants.MOVEMENT_TYPE movementType;
    [Tooltip("Determines whether this unit can attack units if they are on the ground, flying or either one.")]
    [SerializeField]
    private GameConstants.HEIGHT_ATTACKABLE heightAttackable;
    [Tooltip("Lets other units know whether this unit is a structure or not")]
    [SerializeField]
    private GameConstants.UNIT_TYPE unitType;
    [Tooltip("Lets other scripts know whether this unit came from a group spawn or not")]
    [SerializeField]
    private GameConstants.UNIT_GROUPING unitGrouping;
    [Tooltip("Determines if the unit strictly prioritizes certain enemies")]
    [SerializeField]
    private GameConstants.ATTACK_PRIORITY attackPriority;
    [SerializeField]
    private GameConstants.UNIT_RANGE unitRange;
    [SerializeField]
    private EffectStats effectStats;

    private bool isHoveringAbility;
    private bool isCastingAbility;
    private bool isAttacking;

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
        set { maxHealth = value; }
    }

    public float HealthDecay {
        get { return healthDecay; }
        //set { healthDecay = value; }
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

    public float RotationSpeed
    {
        get { return rotationSpeed; }
        set { rotationSpeed = value; }
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
    
    public GameConstants.HEIGHT_ATTACKABLE HeightAttackable
    {
        get { return heightAttackable; }
        //set { heightAttackable = value; }
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

    public EffectStats EffectStats
    {
        get { return effectStats; }
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

    public bool IsAttacking
    {
        get { return isAttacking; }
        set { isAttacking = value; }
    }

    public bool IsReady { get { return summoningSicknessUI.IsReady; } }

    public bool CanAct { get { return effectStats.CanAct() && summoningSicknessUI.IsReady; } }

    public float SpeedMultiplier() {
        if(effectStats.RootedStats.IsRooted || effectStats.FrozenStats.IsFrozen)
            return 0;
        else
            return effectStats.SlowedStats.CurrentSlowIntensity;
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

        summoningSicknessUI.UpdateStats();
        EffectStats.UpdateStats();

        unitAgent.Agent.speed = moveSpeed * SpeedMultiplier();

        detectionObject.radius = range;
        visionObject.radius = visionRange;

        if(IsReady) {
            if(healthDecay > 0)
                currHealth -= healthDecay*maxHealth * Time.deltaTime ; // lowers hp by 5% of maxHp every second ?? should this line be at the top ??

            bool inVision = false;
            if(target != null)
                inVision = hitTargets.Contains(target);       
            if( ( inRange > 0 || (currAttackDelay/attackDelay >= GameConstants.ATTACK_READY_PERCENTAGE && inVision) ) && CanAct && !IsCastingAbility && !isAttacking) { //if target is inRange, or the attack is nearly ready and their within vision AND not stunned
                
                if(target != null) {
                    Vector3 directionToTarget = unitAgent.transform.position - target.transform.GetChild(0).position;
                    directionToTarget.y = 0; // Ignore Y, usful for airborne units
                    float angle = Vector3.Angle(unitAgent.transform.forward, directionToTarget); 

                    //180 - *** might be the source of an error later on, depends on the angle of a unit agent at the start, right now they are all 180
                    if(180-Mathf.Abs(angle) < GameConstants.MAXIMUM_ATTACK_ANGLE) {
                        if(currAttackDelay < attackDelay) 
                            currAttackDelay += Time.deltaTime * effectStats.SlowedStats.CurrentSlowIntensity;
                        else
                            currAttackDelay = 0;
                    }
                    else {
                        if(currAttackDelay < attackDelay*GameConstants.ATTACK_READY_PERCENTAGE) 
                            currAttackDelay += Time.deltaTime * effectStats.SlowedStats.CurrentSlowIntensity;
                    }
                }
                else { //this may occur for a few frames when a units target dies, but there are still other units it can target, it just has not updated to the new target yet
                    if(currAttackDelay < attackDelay*GameConstants.ATTACK_READY_PERCENTAGE) 
                        currAttackDelay += Time.deltaTime * effectStats.SlowedStats.CurrentSlowIntensity;
                }
            }
            else if(inVision && !IsCastingAbility && !isAttacking) { //if the target is within vision
                if(currAttackDelay < attackDelay*GameConstants.ATTACK_CHARGE_LIMITER) 
                    currAttackDelay += Time.deltaTime * effectStats.SlowedStats.CurrentSlowIntensity;
            }
            else 
                currAttackDelay = 0;
        }
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

        summoningSicknessUI.UpdateStats();
        effectStats.FrozenStats.UpdateFrozenStats();
        effectStats.SlowedStats.UpdateSlowedStats();
        effectStats.PoisonedStats.UpdatePoisonedStats();
        effectStats.RootedStats.UpdateRootedStats();

        if(IsReady) {
            if(healthDecay > 0)
                currHealth -= healthDecay*maxHealth * Time.deltaTime ; // lowers hp by 5% of maxHp every second ?? should this line be at the top ??
            if(currAttackDelay < attackDelay ) {
                if(CanAct)
                    currAttackDelay += Time.deltaTime * effectStats.SlowedStats.CurrentSlowIntensity;
            }
            else
                currAttackDelay = 0;
        }
    }

    public void ApplyAffects(Component damageable) {
        if(effectStats.SlowStats.CanSlow)
            (damageable as IDamageable).Stats.effectStats.SlowedStats.Slow(effectStats.SlowStats.SlowDuration, effectStats.SlowStats.SlowIntensity);
        if(effectStats.KnockbackStats.CanKnockback)
            (damageable as IDamageable).Stats.effectStats.KnockbackedStats.Knockback(effectStats.KnockbackStats.KnockbackDuration, effectStats.KnockbackStats.InitialSpeed, effectStats.KnockbackStats.UnitPosition);
    }
}
