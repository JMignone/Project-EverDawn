using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BaseStats
{
    [Header("Health and Armor")]
    [SerializeField] [Min(0)]
    private float currHealth;
    [SerializeField] [Min(0)]
    private float maxHealth;
    [SerializeField] [Min(0)]
    private float currArmor;
    [SerializeField] [Min(0)]
    private float maxArmor;
    [Tooltip("A number from (0-1) that will determine what percentage of health will be removed each tick. A number below .1 is highly recomended")]
    [SerializeField] [Range(0,1)]
    private float healthDecay;
    [SerializeField]
    private bool leavesArena;
    [Tooltip("A timer for when the unit leaves regardless of hp. Setting this to 0 will disable it")]
    [SerializeField] [Min(0)]
    private float leaveTimer;

    [Header("Range and Vision")]
    [SerializeField] [Min(0)]
    private float range;
    private bool incRange; //Used to increment the units range by 1 when it has units within its range to prevent chase, stop, chase
    [SerializeField] [Min(0)]
    private float visionRange;

    [Header("Damage and Attack Speed")]
    [SerializeField] [Min(0)]
    private float baseDamage;
    [SerializeField] [Min(0)]
    private float towerDamage;
    [SerializeField] [Min(0)]
    private float attackDelay;
    [SerializeField] [Min(0)]
    private float currAttackDelay;
    [Tooltip("An attack will charge to this point even if the target is not within range but within vision. MUST BE LESS THAN attackChargeLimiter")]
    [SerializeField] [Range(0,1)]
    private float attackChargeLimiter;
    [Tooltip("Once the attack reaches this point, it will happen even if the target is not within range, but within vision and in front of unit")]
    [SerializeField] [Range(0,1)]
    private float attackReadyPercentage;
    [Tooltip("The maximum angle a unit can have while still charging its attack")]
    [SerializeField] [Range(0,360)]
    private float maximumAttackAngle;

    [Header("Movement")]
    [SerializeField] [Min(0)]
    private float moveSpeed;
    [Tooltip("Determines if a unit must rotate towards it target to fire")]
    [SerializeField]
    private bool noRotation;
    [SerializeField] [Min(0)]
    private float rotationSpeed;

    [SerializeField]
    private SummoningSicknessUI summoningSicknessUI;

    [Header("Unit GameObjects")]
    [SerializeField]
    private GameObject canvasAbility;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Image armorBar;
    [SerializeField]
    private Image abilityIndicator;
    //[SerializeField]
    private int indicatorNum;
    [SerializeField]
    private SphereCollider detectionObject;
    [SerializeField]
    private SphereCollider visionObject;
    [SerializeField]
    private UnitMaterials unitMaterials;

    [Header("Unit Enums")]
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
    //[SerializeField]
    //private GameConstants.UNIT_RANGE unitRange;
    [SerializeField]
    private EffectStats effectStats;

    private bool isHoveringAbility;
    private bool isCastingAbility;
    private bool isFiring; //firing is if a unit is attacking through the attackstats, shooting projectiles
    private bool isAttacking; //attacking is if the unit attacks in the normal method
    private bool isShadow;

    private bool soonToBeKilled;     //a flag set if the unit is about to be killed
    private bool soonToKill;         //a flag set if the unit is about to kill
    private bool soonToKillOverride; //a flag set if the target is about to be killed and is granted access to finish an attack on a soonToBeKilled target
    private bool wasSoonToBeKilled;  //a flag set if the unit was ever soon to be killed. This is set such that this unit will never be able to flag another unit

    private float towerPosOffset;     //Used to direct untis moore concretly to the correct side of the arena given there is only 1 tower left

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

    public bool LeavesArena {
        get { return leavesArena; }
    }

    public float LeaveTimer {
        get { return leaveTimer; }
    }

    public float PercentArmor {
        get { return currArmor/maxArmor; }
    }

    public float CurrArmor {
        get { return currArmor; }
        set { 
            if(value <= 0)
                currArmor = 0;
            else if(value >= maxArmor)
                currArmor = maxArmor;
            else
                currArmor = value;
        }
    }

    public float Range
    {
        get { return range; }
        set { range = value; }
    }

    public bool IncRange
    {
        get { return incRange; }
        set { incRange = value; }
    }

    public float VisionRange
    {
        get { return visionRange; }
        set { visionRange = value; }
    }

    public float BaseDamage
    {
        get { return baseDamage; }
        set { baseDamage = value; }
    }

    public float TowerDamage
    {
        get { return towerDamage; }
        set { towerDamage = value; }
    }

    public float AttackDelay
    {
        get { return attackDelay; }
        set { attackDelay = value; }
    }

    public float CurrAttackDelay
    {
        get { return currAttackDelay; }
        set { currAttackDelay = value; }
    }

    public float AttackChargeLimiter
    {
        get { return attackChargeLimiter; }
        set { attackChargeLimiter = value; }
    }

    public float MaximumAttackAngle
    {
        get { return maximumAttackAngle; }
        set { maximumAttackAngle = value; }
    }

    public float AttackReadyPercentage
    {
        get { return attackReadyPercentage; }
        set { attackReadyPercentage = value; }
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

    public Image ArmorBar
    {
        get { return armorBar; }
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

    public UnitMaterials UnitMaterials
    {
        get { return unitMaterials; }
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

    public bool SoonToBeKilled
    {
        get { return soonToBeKilled; }
        set { soonToBeKilled = value; }
    }

    public bool SoonToKill
    {
        get { return soonToKill; }
        set { soonToKill = value; }
    }

    public bool SoonToKillOverride
    {
        get { return soonToKillOverride; }
        set { soonToKillOverride = value; }
    }

    public bool WasSoonToBeKilled
    {
        get { return wasSoonToBeKilled; }
        set { wasSoonToBeKilled = value; }
    }

    public float TowerPosOffset
    {
        get { return towerPosOffset; }
        set { towerPosOffset = value; }
    }

    public bool Targetable {
        get { return !isShadow; }
    }

    public bool Damageable {
        get { return !summoningSicknessUI.SummonProtection && !effectStats.ResistStats.ResistedDamage && !effectStats.ResistStats.CantDamage; }
    }

    public bool IsReady { get { return summoningSicknessUI.IsReady; } }

    public bool CanAct { get { return effectStats.CanAct() && summoningSicknessUI.IsReady; } }

    public float SpeedMultiplier() {
        if(effectStats.RootedStats.IsRooted || effectStats.FrozenStats.IsFrozen)
            return 0;
        else
            return effectStats.SlowedStats.CurrentSlowIntensity;
    }

    public void UpdateStats(bool chargeAttack, int inRange, Actor3D unitAgent, List<GameObject> hitTargets, GameObject target, GameObject unit) {
        UpdateHealth();

        if(indicatorNum > 0 && Damageable)
            abilityIndicator.enabled = true;
        else
            abilityIndicator.enabled = false;

        summoningSicknessUI.UpdateStats();
        EffectStats.UpdateStats();

        unitAgent.Agent.speed = moveSpeed * SpeedMultiplier();

        detectionObject.radius = range + Convert.ToInt32(incRange); //in order to keep this, may have to union hittargets and inrangetargets when using the triggers
        //below is nessesary because we need the vision collider to go off first, as bigger colliders will go off first
        if(detectionObject.radius >= visionRange)
            visionObject.radius = detectionObject.radius + .01f;
        else
            visionObject.radius = visionRange;
    
        if(IsReady) {
            if(healthDecay > 0)
                currHealth -= healthDecay*maxHealth * Time.deltaTime ; // lowers hp by 5% of maxHp every second ?? should this line be at the top ??
            
            if(leavesArena) {
                if(leaveTimer > 0)
                    leaveTimer -= Time.deltaTime;
                /*else {
                    ResetKillFlags(unit, target);
                    GameManager.RemoveObjectsFromList(unit);
                    if(target != null)
                        (target.GetComponent(typeof(IDamageable)) as IDamageable).EnemyHitTargets.Remove(unit);
                    (unit.GetComponent(typeof(IDamageable)) as IDamageable).SetTarget(null);
                    MonoBehaviour.Destroy(unit);
                }*/
            }
  
            if( ( inRange > 0 || (currAttackDelay/attackDelay >= attackReadyPercentage && hitTargets.Contains(target)) ) && CanAct && !IsCastingAbility && chargeAttack) { //if target is inRange, or the attack is nearly ready and their within vision AND not stunned
                isAttacking = true;
                if(target != null) {
                    Vector3 directionToTarget = target.transform.GetChild(0).position - unitAgent.transform.position;
                    directionToTarget.y = 0; // Ignore Y, usful for airborne units
                    float angle = Vector3.Angle(unitAgent.transform.forward, directionToTarget); 

                    if(noRotation || Mathf.Abs(angle) < maximumAttackAngle) {
                        if(!soonToKill && !soonToKillOverride && currAttackDelay/attackDelay >= attackReadyPercentage)
                            SetKillFlags(unit, target);
                        else if(wasSoonToBeKilled && soonToKill)
                            ResetKillFlags(unit, target);    
                        //will the above work if 2 units attacking eachother are both about to be killed by eachother?

                        if(currAttackDelay < attackDelay) 
                            currAttackDelay += Time.deltaTime * effectStats.SlowedStats.CurrentSlowIntensity;
                        else
                            currAttackDelay = 0;
                    }
                    else {
                        if(currAttackDelay < attackDelay*attackReadyPercentage) 
                            currAttackDelay += Time.deltaTime * effectStats.SlowedStats.CurrentSlowIntensity;
                        ResetKillFlags(unit, target);
                    }
                }
                else { //this may occur for a few frames when a units target dies, but there are still other units it can target, it just has not updated to the new target yet
                    if(currAttackDelay < attackDelay*attackChargeLimiter) 
                        currAttackDelay += Time.deltaTime * effectStats.SlowedStats.CurrentSlowIntensity;
                    ResetKillFlags(unit, target);
                }
            }
            else if(!IsCastingAbility && chargeAttack) {
                isAttacking = false;
                if(CanAct) {
                    if(currAttackDelay < attackDelay*attackChargeLimiter)
                        currAttackDelay += Time.deltaTime * effectStats.SlowedStats.CurrentSlowIntensity;
                    else
                        currAttackDelay = attackDelay*attackChargeLimiter;
                }
                else {
                    if(currAttackDelay > attackDelay*attackReadyPercentage)
                        currAttackDelay = attackDelay*attackReadyPercentage -.001f; //-.001 nessesary for errors in float arithmatic
                }
                ResetKillFlags(unit, target);
            }
            /*
            else if(inVision && !IsCastingAbility && chargeAttack) { //if the target is within vision
                isAttacking = false;
                if(currAttackDelay < attackDelay*attackChargeLimiter) 
                    currAttackDelay += Time.deltaTime * effectStats.SlowedStats.CurrentSlowIntensity;
                ResetKillFlags(unit, target);
            }
            else {
                isAttacking = false;
                currAttackDelay = 0;
                ResetKillFlags(unit, target);
            }*/
        }
    }

    public void UpdateBuildingStats() {
        UpdateHealth();

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

    public void SetKillFlags(GameObject unit, GameObject target) {
        IDamageable enemyUnit = (target.GetComponent(typeof(IDamageable)) as IDamageable);
        if(enemyUnit.Stats.CurrArmor == 0 && enemyUnit.Stats.CurrHealth <= baseDamage*effectStats.StrengthenedStats.CurrentStrengthIntensity) {

            if(!wasSoonToBeKilled) {
                foreach(GameObject go in enemyUnit.EnemyHitTargets) { //go through every unit targeting our target and see if any of them will kill this unit first
                    if(go != unit && go != null) {
                        IDamageable friendlyUnit = (go.GetComponent(typeof(IDamageable)) as IDamageable);
                        float timeToKill = ( (Mathf.Ceil(enemyUnit.Stats.CurrHealth/(friendlyUnit.Stats.BaseDamage*friendlyUnit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity))) * friendlyUnit.Stats.AttackDelay) - friendlyUnit.Stats.CurrAttackDelay;
                        //MonoBehaviour.print(timeToKill);
                        if(attackDelay - currAttackDelay > timeToKill) {
                            soonToKillOverride = true;
                            return;
                        }
                    }
                }

                soonToKill = true;
                enemyUnit.Stats.SoonToBeKilled = true;
                enemyUnit.Stats.WasSoonToBeKilled = true;
                foreach(GameObject go in enemyUnit.EnemyHitTargets.ToArray()) { //go through every unit targeting our target and see if any of them will kill this unit first
                    if(go != unit && go != null) {
                        IDamageable friendlyUnit = (go.GetComponent(typeof(IDamageable)) as IDamageable);
                        if(friendlyUnit.Stats.CurrAttackDelay > friendlyUnit.Stats.AttackDelay*attackChargeLimiter)
                            friendlyUnit.Stats.SoonToKillOverride = true;
                        else {
                            friendlyUnit.SetTarget(null);
                            if(friendlyUnit.InRangeTargets.Contains(target))
                                friendlyUnit.InRangeTargets.Remove(target);
                            if(friendlyUnit.InRangeTargets.Count == 0)
                                friendlyUnit.Stats.IncRange = false;
                        }
                    }
                }
            }
            else
                enemyUnit.Stats.WasSoonToBeKilled = true; 
            //we set the enemy unit to only have the was bool set as our unit also had it set. We shouldnt have 2 units about to kill eachother set their flags
        }
    }

    public void ResetKillFlags(GameObject unit, GameObject target) {
        if(soonToKillOverride) {
            soonToKillOverride = false;
            if(target != null) {
                IDamageable enemyUnit = (target.GetComponent(typeof(IDamageable)) as IDamageable);
                if(enemyUnit.Stats.SoonToBeKilled)
                    (unit.GetComponent(typeof(IDamageable)) as IDamageable).SetTarget(null);
            }
        }

        if(soonToKill) {
            soonToKill = false;
            if(target != null) {
                IDamageable enemyUnit = (target.GetComponent(typeof(IDamageable)) as IDamageable);
                enemyUnit.Stats.SoonToBeKilled = false;

                foreach(GameObject go in enemyUnit.EnemyHitTargets) { //go through every unit targeting our target
                    if(go != unit && go != null) {
                        IDamageable friendlyUnit = (go.GetComponent(typeof(IDamageable)) as IDamageable);
                        friendlyUnit.Stats.SoonToKillOverride = false;
                    }
                }

                Collider[] colliders = Physics.OverlapSphere(target.transform.GetChild(0).position, target.transform.GetChild(0).GetComponent<SphereCollider>().radius);
                foreach(Collider collider in colliders) {
                    Component enemy = collider.transform.parent.parent.GetComponent(typeof(IDamageable));
                    if(enemy) {
                        if(!enemy.CompareTag(target.tag) && collider.CompareTag("Range")) {
                            if(GameFunctions.CanAttack(enemy.tag, target.tag, target.GetComponent(typeof(IDamageable)), (enemy as IDamageable).Stats)) { //only if the unit can actually target this one should we adjust this value
                                if(!(enemy as IDamageable).InRangeTargets.Contains(target))
                                    (enemy as IDamageable).InRangeTargets.Add(target);
                                if( ((enemy as IDamageable).InRangeTargets.Count == 1 || (enemy as IDamageable).Target == null) && (enemy as IDamageable).Stats.CanAct) { //we need this block here as well as stay in the case that a unit is placed inside a units range
                                    GameObject go = GameFunctions.GetNearestTarget((enemy as IDamageable).HitTargets, collider.transform.parent.parent.tag, (enemy as IDamageable).Stats);
                                    if(go != null) {
                                        (enemy as IDamageable).SetTarget(go);
                                        (enemy as IDamageable).Stats.IncRange = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void UpdateHealth() {
        if(currArmor > 0) {
            if(PercentArmor == 1) {
                armorBar.enabled = false;
                armorBar.transform.GetChild(0).gameObject.SetActive(false); //this is the image border
            }
            else {
                armorBar.enabled = true;
                armorBar.transform.GetChild(0).gameObject.SetActive(true);
            }
            armorBar.fillAmount = PercentArmor;
        }
        else {
            armorBar.enabled = false;
            armorBar.transform.GetChild(0).gameObject.SetActive(false);

            if(PercentHealth == 1 && maxArmor == 0) {
                healthBar.enabled = false;
                healthBar.transform.GetChild(0).gameObject.SetActive(false); //this is the image border
            }
            else {
                healthBar.enabled = true;
                healthBar.transform.GetChild(0).gameObject.SetActive(true);
            }
            healthBar.fillAmount = PercentHealth;
        }
    }

    public void Vanish(GameObject unit, Actor3D unitAgent) {//GameObject[] enemyHitTargets) {
        if(!isShadow) {
            isShadow = true;
            Vector3 position = unitAgent.transform.position;
            position.y = 0;
            Collider[] colliders = Physics.OverlapSphere(unitAgent.transform.position, unitAgent.HitBox.radius);
            foreach(Collider collider in colliders) {
                Component enemy = collider.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(enemy) {
                    if(!enemy.CompareTag(unit.tag) && collider.CompareTag("Vision")) { //Are we in their vision detection object?
                        if((enemy as IDamageable).Target == unit) 
                            (enemy as IDamageable).SetTarget(null);
                        if((enemy as IDamageable).HitTargets.Contains(unit))
                            (enemy as IDamageable).HitTargets.Remove(unit);
                        if((enemy as IDamageable).InRangeTargets.Contains(unit))
                            (enemy as IDamageable).InRangeTargets.Remove(unit);
                    }
                }
            }
        }
    }

    public void Appear(GameObject unit, ShadowStats shadowStats, Actor3D unitAgent) {
        if(isShadow) {
            isShadow = false;
            Vector3 position = unitAgent.transform.position;
            position.y = 0;
            Collider[] colliders = Physics.OverlapSphere(position, unitAgent.HitBox.radius);
            foreach(Collider collider in colliders) {
                Component enemy = collider.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(enemy) {
                    if(!enemy.CompareTag(unit.tag)) {
                        if(collider.CompareTag("Vision")) {
                            if(!(enemy as IDamageable).HitTargets.Contains(unit))
                                (enemy as IDamageable).HitTargets.Add(unit);
                        }
                        else if(collider.CompareTag("Range")) {
                            if(!(enemy as IDamageable).HitTargets.Contains(unit))
                                (enemy as IDamageable).HitTargets.Add(unit);
                            if(GameFunctions.CanAttack(enemy.tag, unit.tag, unit.GetComponent(typeof(IDamageable)), (enemy as IDamageable).Stats)) { //only if the unit can actually target this one should we adjust this value
                                if(!(enemy as IDamageable).InRangeTargets.Contains(unit))
                                    (enemy as IDamageable).InRangeTargets.Add(unit);
                                if( ((enemy as IDamageable).InRangeTargets.Count == 1 || (enemy as IDamageable).Target == null) && (enemy as IDamageable).Stats.CanAct) { //we need this block here as well as stay in the case that a unit is placed inside a units range
                                    GameObject go = GameFunctions.GetNearestTarget((enemy as IDamageable).HitTargets, collider.transform.parent.parent.tag, (enemy as IDamageable).Stats);
                                    if(go != null) {
                                        (enemy as IDamageable).SetTarget(go);
                                        (enemy as IDamageable).Stats.IncRange = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //make unit appear
            unitMaterials.MakeOpaque();
        }
        else if(shadowStats != null)
            shadowStats.CurrentDelay = 0;
    }

    public void IncIndicatorNum() {
        if(indicatorNum == 0)
            unitMaterials.AbilityHover();
        indicatorNum++;
    }

    public void DecIndicatorNum() {
        if(indicatorNum > 0) {
            indicatorNum--;
            if(indicatorNum == 0)
                unitMaterials.RemoveAbilityHover();
        }
        else
            Debug.Log("WARNING: indicatorNum ATTEMPTED TO BE DECREMENTED PAST 0. HAS THIS UNIT BECOME UNDAMAGEABLE?");
    }

    public void ApplyAffects(Component damageable) {
        if(effectStats.SlowStats.CanSlow)
            (damageable as IDamageable).Stats.effectStats.SlowedStats.Slow(effectStats.SlowStats.SlowDuration, effectStats.SlowStats.SlowIntensity);
        if(effectStats.KnockbackStats.CanKnockback)
            (damageable as IDamageable).Stats.effectStats.KnockbackedStats.Knockback(effectStats.KnockbackStats.KnockbackDuration, effectStats.KnockbackStats.InitialSpeed, effectStats.KnockbackStats.UnitPosition);
    }
}
