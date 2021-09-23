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
    private GameObject canvasAbility;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Image abilityIndicator;
    private int indicatorNum;
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
    private bool isFiring; //firing is if a unit is attacking through the attackstats, shooting projectiles
    private bool isAttacking; //attacking is if the unit attacks in the normal method
    private bool isShadow;

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

    public SummoningSicknessUI SummoningSicknessUI
    {
        get { return summoningSicknessUI; }
    }

    public GameObject CanvasAbility
    {
        get { return canvasAbility; }
    }

    public Image HealthBar
    {
        get { return healthBar; }
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

    public bool IsFiring
    {
        get { return isFiring; }
        set { isFiring = value; }
    }

    public bool IsAttacking
    {
        get { return isAttacking || isFiring; }
    }

    public bool IsShadow
    {
        get { return isShadow; }
        set { isShadow = value; }
    }

    public bool Targetable {
        get { return !isShadow; }
    }

    public bool Damageable {
        get { return !summoningSicknessUI.SummonProtection && !effectStats.ResistStats.ResistedDamage; }
    }

    public bool IsReady { get { return summoningSicknessUI.IsReady; } }

    public bool CanAct { get { return effectStats.CanAct() && summoningSicknessUI.IsReady; } }

    public float SpeedMultiplier() {
        if(effectStats.RootedStats.IsRooted || effectStats.FrozenStats.IsFrozen)
            return 0;
        else
            return effectStats.SlowedStats.CurrentSlowIntensity;
    }

    public void UpdateStats(bool chargeAttack, int inRange, Actor3D unitAgent, List<GameObject> hitTargets, GameObject target) {
        if(PercentHealth == 1) {
            HealthBar.enabled = false;
            HealthBar.transform.GetChild(0).gameObject.SetActive(false); //this is the image border
        }
        else {
            HealthBar.enabled = true;
            HealthBar.transform.GetChild(0).gameObject.SetActive(true);
        }
        HealthBar.fillAmount = PercentHealth;

        if(indicatorNum > 0 && Damageable)
            abilityIndicator.enabled = true;
        else
            abilityIndicator.enabled = false;

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
            if( ( inRange > 0 || (currAttackDelay/attackDelay >= GameConstants.ATTACK_READY_PERCENTAGE && inVision) ) && CanAct && !IsCastingAbility && chargeAttack) { //if target is inRange, or the attack is nearly ready and their within vision AND not stunned
                isAttacking = true;
                if(target != null) {
                    Vector3 directionToTarget = unitAgent.transform.position - target.transform.GetChild(0).position;
                    directionToTarget.y = 0; // Ignore Y, usful for airborne units
                    float angle = Vector3.Angle(unitAgent.transform.forward, directionToTarget); 

                    //180 - *** might be the source of an error later on, depends on the angle of a unit agent at the start, right now they are all 180
                    if(180-Mathf.Abs(angle) < GameConstants.MAXIMUM_ATTACK_ANGLE || Mathf.Abs(angle) == 0) { //it may equal 0 if the unit is right on top of the target, this may be fixed if the units are set to 0 and not 180
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
            else if(inVision && !IsCastingAbility && chargeAttack) { //if the target is within vision
                isAttacking = false;
                if(currAttackDelay < attackDelay*GameConstants.ATTACK_CHARGE_LIMITER) 
                    currAttackDelay += Time.deltaTime * effectStats.SlowedStats.CurrentSlowIntensity;
            }
            else {
                isAttacking = false;
                currAttackDelay = 0;
            }
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

        if(indicatorNum > 0 && Damageable)
            abilityIndicator.enabled = true;
        else
            abilityIndicator.enabled = false;

        summoningSicknessUI.UpdateStats();
        EffectStats.UpdateStats();

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

    public void Vanish(GameObject unit, GameObject[] enemyHitTargets) {
        if(!isShadow) {
            isShadow = true;
            indicatorNum = 0;
            foreach(GameObject go in enemyHitTargets) {
                Component targetComponent = go.GetComponent(typeof(IDamageable));
                if(targetComponent) {
                    (targetComponent as IDamageable).SetTarget(null);
                    if((targetComponent as IDamageable).InRangeTargets.Contains(unit))
                        (targetComponent as IDamageable).InRangeTargets.Remove(unit);
                    if((targetComponent as IDamageable).HitTargets.Contains(unit))
                        (targetComponent as IDamageable).HitTargets.Remove(unit);
                }
            }
            //make unit transparent
        }
    }

    public void Appear(GameObject unit, ShadowStats stats, Actor3D unitAgent) {
        if(isShadow) {
            isShadow = false;
            Collider[] colliders = Physics.OverlapSphere(unitAgent.transform.position, unitAgent.Agent.radius);
            foreach(Collider collider in colliders) {
                Component enemy = collider.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(enemy) {
                    if(!enemy.CompareTag(unit.tag) && collider.CompareTag("Vision")) { //Are we in their vision detection object?
                        if(!(enemy as IDamageable).HitTargets.Contains(unit))
                            (enemy as IDamageable).HitTargets.Add(unit);
                    }
                }
            }
            colliders = Physics.OverlapSphere(unitAgent.transform.position, unitAgent.Agent.radius);
            foreach(Collider collider in colliders) {
                Component enemy = collider.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(enemy) {
                    if(!enemy.CompareTag(unit.tag) && collider.CompareTag("Range")) {
                        if(GameFunctions.CanAttack(enemy.tag, unit.tag, unit.GetComponent(typeof(IDamageable)), (enemy as IDamageable).Stats)) { //only if the unit can actually target this one should we adjust this value
                            if(!(enemy as IDamageable).InRangeTargets.Contains(unit))
                                (enemy as IDamageable).InRangeTargets.Add(unit);
                            if( ((enemy as IDamageable).InRangeTargets.Count == 1 || (enemy as IDamageable).Target == null) && (enemy as IDamageable).Stats.CanAct) { //we need this block here as well as stay in the case that a unit is placed inside a units range
                                GameObject go = GameFunctions.GetNearestTarget((enemy as IDamageable).HitTargets, collider.transform.parent.parent.tag, (enemy as IDamageable).Stats);
                                if(go != null)
                                    (enemy as IDamageable).SetTarget(go);
                            }
                        }
                    }
                }
            }
            //make unit appear
        }
        else
            stats.CurrentDelay = 0;
    }

    public void ApplyAffects(Component damageable) {
        if(effectStats.SlowStats.CanSlow)
            (damageable as IDamageable).Stats.effectStats.SlowedStats.Slow(effectStats.SlowStats.SlowDuration, effectStats.SlowStats.SlowIntensity);
        if(effectStats.KnockbackStats.CanKnockback)
            (damageable as IDamageable).Stats.effectStats.KnockbackedStats.Knockback(effectStats.KnockbackStats.KnockbackDuration, effectStats.KnockbackStats.InitialSpeed, effectStats.KnockbackStats.UnitPosition);
    }
}
