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
    private GameObject target;

    [SerializeField]
    private BaseStats stats;
    
    [SerializeField]
    private AttackStats attackStats;

    [SerializeField]
    private BuildUpStats buildUpStats;

    [SerializeField]
    private ChargeStats chargeStats;

    [SerializeField]
    private DashStats dashStats;

    [SerializeField]
    private ShadowStats shadowStats;

    [SerializeField]
    private DeathStats deathStats;

    [SerializeField]
    private NoseDiveStats noseDiveStats;

    [SerializeField]
    private List<GameObject> hitTargets;

    [SerializeField]
    private List<GameObject> inRangeTargets;

    [SerializeField]
    private List<GameObject> enemyHitTargets;

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

    public BaseStats Stats
    {
        get { return stats; }
    }

    public AttackStats AttackStats
    {
        get { return attackStats; }
    }

    public DashStats DashStats
    {
        get { return dashStats; }
    }
    
    public ShadowStats ShadowStats
    {
        get { return shadowStats; }
    }

    public DeathStats DeathStats
    {
        get { return deathStats; }
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

    public List<GameObject> EnemyHitTargets
    {
        get { return enemyHitTargets; }
    }

    public bool IsMoving
    {
        get { if(agent.Agent.enabled && !agent.Agent.isStopped && agent.Agent.speed != 0) return true; else return false; }
    }

    public bool ChargeAttack
    {
        get { return !chargeStats.IsCharging && !attackStats.IsFiring && !dashStats.IsDashing; }
    }

    private void Start()
    {
        agent.Agent.stoppingDistance = 0; //Set to be zero, incase someone forgets or accidently changes this value to be a big number
        agent.Agent.speed = stats.MoveSpeed;

        IDamageable unit = (gameObject.GetComponent(typeof(IDamageable)) as IDamageable);

        stats.SummoningSicknessUI.StartStats(unit);
        stats.EffectStats.StartStats(unit);
        attackStats.StartAttackStats(unit);
        buildUpStats.StartStats(unit);
        chargeStats.StartChargeStats(unit);
        dashStats.StartDashStats(unit);
        shadowStats.StartShadowStats(unit);
        noseDiveStats.StartStats(unit);

        stats.IsHoveringAbility = false;
        stats.AbilityIndicator.enabled = false;
        stats.AbilityIndicator.rectTransform.sizeDelta = new Vector2(2*agent.HitBox.radius + 1, 2*agent.HitBox.radius + 1); 
        // + 1 is better for the knob UI, if we get our own UI image, we may want to remove it
    }

    private void FixedUpdate()
    {
        if(noseDiveStats.IsDiving)
            noseDiveStats.UpdateStats();
        else if(stats.CurrHealth > 0) {
            if((target == null || inRangeTargets.Count == 0) && stats.CanAct && !stats.IsCastingAbility) //if the target is null, we must find the closest target in hit targets. If hit targets is empty or failed, find the closest tower
                ReTarget();

            stats.UpdateStats(ChargeAttack, inRangeTargets.Count, agent, hitTargets, target, gameObject);
            buildUpStats.UpdateStats();
            chargeStats.UpdateChargeStats();
            dashStats.UpdateDashStats();
            shadowStats.UpdateShadowStats();
            Attack();

            if(dashStats.IsDashing)
                lookAtTarget();
            else if(target != null && !attackStats.IsFiring) {
                Vector3 direction = target.transform.GetChild(0).position - agent.transform.position;
                direction.y = 0;
                agent.Agent.SetDestination(new Vector3(target.transform.GetChild(0).position.x + stats.TowerPosOffset, 0, target.transform.GetChild(0).position.z) - (direction.normalized * .25f));
                if(hitTargets.Contains(target)) {
                    if(inRangeTargets.Count > 0 || stats.CurrAttackDelay/stats.AttackDelay >= GameConstants.ATTACK_READY_PERCENTAGE) { //is in range, OR is 90% thru attack cycle -
                        lookAtTarget();
                        agent.Agent.ResetPath();
                    }
                }
            }
            else if(target != null && (!attackStats.AttacksLocation || !attackStats.FiresProjectiles)) //if the unit fires at a specific location, dont turn towards the target, keep looking at location else (which is the if) look at target
                lookAtTarget();
            else if(agent.Agent.enabled == true) //prevents errors being thrown when a units agent is temporarily disabled by being grabbed
                agent.Agent.ResetPath();
        }
        else if(deathStats.DeathSkill) {
            if(!deathStats.IsDying)
                deathStats.StartStats(gameObject);
            deathStats.FireDeathSkill();
        }
        else {
            //print(gameObject.name + " has died!");
            stats.ResetKillFlags(gameObject, target);
            GameManager.RemoveObjectsFromList(gameObject);
            if(target != null)
                (target.GetComponent(typeof(IDamageable)) as IDamageable).EnemyHitTargets.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        if(stats.EffectStats.GrabbedStats.IsGrabbed)
            stats.EffectStats.GrabbedStats.unGrab();
    }

    void Attack() {
        if(target != null) {
            if(noseDiveStats.NoseDives) {
                if(stats.CurrAttackDelay >= stats.AttackDelay)
                    noseDiveStats.StartDive((target.GetComponent(typeof(IDamageable)) as IDamageable));
            }
            else if(attackStats.FiresProjectiles) { //if the unit fires projectiles rather than simply doing damage when attacking
                if(stats.CurrAttackDelay >= stats.AttackDelay && !attackStats.IsFiring) {
                    Component damageable = target.GetComponent(typeof(IDamageable));
                    if(damageable) { //is the target damageable
                        if(hitTargets.Contains(target)) //this is needed for the rare occurance that a unit is 90% done with attack delay and the target leaves its range. It can still do its attack if its within vision given that its attack was already *90% thru
                            attackStats.BeginFiring();
                    }
                    stats.ResetKillFlags(gameObject, target);
                }
            }
            else {
                if(stats.CurrAttackDelay >= stats.AttackDelay) {
                    Component damageable = target.GetComponent(typeof(IDamageable));
                    if(damageable) { //is the target damageable
                        if(hitTargets.Contains(target)) {  //this is needed for the rare occurance that a unit is 90% done with attack delay and the target leaves its range. It can still do its attack if its within vision given that its attack was already *90% thru
                            if(stats.EffectStats.AOEStats.AreaOfEffect)
                                stats.EffectStats.AOEStats.Explode(gameObject, target, stats.BaseDamage * stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
                            else {
                                GameFunctions.Attack(damageable, stats.BaseDamage * stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity, stats.EffectStats.CritStats);
                                stats.ApplyAffects(damageable);
                            }
                            buildUpStats.BuildUp();
                            stats.Appear(gameObject, shadowStats, agent);
                            stats.CurrAttackDelay = 0;
                            stats.ResetKillFlags(gameObject, target);
                        }
                    }
                }
            }
        }
        else
            buildUpStats.ResetStats(false);
        if(attackStats.FiresProjectiles)
            attackStats.Fire();
    }

    public void SetTarget(GameObject newTarget) {
        if((newTarget != target && stats.CanAct && !stats.IsCastingAbility) || newTarget == null) {
            if(target != null) {
                (target.GetComponent(typeof(IDamageable)) as IDamageable).EnemyHitTargets.Remove(gameObject);
                stats.ResetKillFlags(gameObject, target);
            }
            if(newTarget != null)
                (newTarget.GetComponent(typeof(IDamageable)) as IDamageable).EnemyHitTargets.Add(gameObject);
            target = newTarget;
            buildUpStats.ResetStats(true);

            stats.TowerPosOffset = 0;
        }
    }

    public void ReTarget() {
        if(stats.CurrAttackDelay < stats.AttackDelay*GameConstants.ATTACK_READY_PERCENTAGE) {
            if(hitTargets.Count > 0) {
                GameObject go = GameFunctions.GetNearestTarget(hitTargets, gameObject.tag, stats);
                if(go != null) {
                    if(go != target && inRangeTargets.Count == 0) {
                        if(stats.CurrAttackDelay > stats.AttackDelay*GameConstants.ATTACK_CHARGE_LIMITER)
                            stats.CurrAttackDelay = stats.AttackDelay*GameConstants.ATTACK_CHARGE_LIMITER;
                    }
                    SetTarget(go);
                }
                else {
                    List<GameObject> towers = GameManager.Instance.TowerObjects;
                    towers = GameManager.GetAllEnemies(towers, gameObject.tag); //sending in only towers
                    SetTarget(GameFunctions.GetTowerTarget(towers, gameObject.tag, stats));

                    if(stats.CurrAttackDelay > stats.AttackDelay*GameConstants.ATTACK_CHARGE_LIMITER)
                        stats.CurrAttackDelay = stats.AttackDelay*GameConstants.ATTACK_CHARGE_LIMITER;
                }
            }
            else {
                List<GameObject> towers = GameManager.Instance.TowerObjects;
                towers = GameManager.GetAllEnemies(towers, gameObject.tag); //sending in only towers
                SetTarget(GameFunctions.GetTowerTarget(towers, gameObject.tag, stats));

                if(stats.CurrAttackDelay > stats.AttackDelay*GameConstants.ATTACK_CHARGE_LIMITER)
                    stats.CurrAttackDelay = stats.AttackDelay*GameConstants.ATTACK_CHARGE_LIMITER;
            }
            stats.IncRange = false;
        }
    }

    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag) && stats.CurrHealth != 0) { //checks to make sure the target isnt on the same team
            if(other.CompareTag("Projectile")) { //Did we get hit by a skill shot?
                Projectile projectile = other.transform.parent.parent.GetComponent<Projectile>();
                Component unit = this.GetComponent(typeof(IDamageable));
                projectile.Hit(unit);
            }
            else if(other.CompareTag("AbilityHighlight")) { //Our we getting previewed for an ability?
                AbilityPreview ability = other.GetComponent<AbilityPreview>();
                if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, this.GetComponent(typeof(IDamageable)))) 
                    stats.IncIndicatorNum();
            }
            else if(other.CompareTag("Pull")) {
                Component IAbility = other.transform.parent.parent.GetComponent(typeof(IAbility));
                stats.EffectStats.PulledStats.AddPull(IAbility);
            }
            else if(other.CompareTag("Dash")) {
                Component unit = other.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(unit) {
                    if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats))
                        (unit as IDamageable).DashStats.StartDash(gameObject);
                }
            }
            else { //is it another units vision/range?
                Component unit = other.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(unit) {
                    if(other.CompareTag("Range")) {//Are we in their range detection object?
                        if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) { //only if the unit can actually target this one should we adjust this value
                            if(!(unit as IDamageable).InRangeTargets.Contains(gameObject))
                                (unit as IDamageable).InRangeTargets.Add(gameObject);
                            if(!(unit as IDamageable).HitTargets.Contains(gameObject))
                                (unit as IDamageable).HitTargets.Add(gameObject);
                            if( ((unit as IDamageable).InRangeTargets.Count == 1 || (unit as IDamageable).Target == null) && (unit as IDamageable).Stats.CanAct) { //we need this block here as well as stay in the case that a unit is placed inside a units range
                                GameObject go = GameFunctions.GetNearestTarget((unit as IDamageable).HitTargets, other.transform.parent.parent.tag, (unit as IDamageable).Stats);
                                if(go != null) {
                                    (unit as IDamageable).SetTarget(go);
                                    (unit as IDamageable).Stats.IncRange = true;
                                }
                            }
                        }
                    }
                    else if(other.CompareTag("Vision")) { //Are we in their vision detection object?
                        if(!(unit as IDamageable).HitTargets.Contains(gameObject))
                            (unit as IDamageable).HitTargets.Add(gameObject);
                    }
                }
            }
        }
        else if(other.CompareTag("FriendlyAbilityHighlight")) { //if the hitbox is from a friendly units ability that hits friendly units
            AbilityPreview ability = other.GetComponent<AbilityPreview>();
            if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, this.GetComponent(typeof(IDamageable))))
                stats.IncIndicatorNum();
        }
    }

    public void OnTriggerExit(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) { //checks to make sure the target isnt on the same team
            if(other.CompareTag("Projectile")) { //Did we get hit by a skill shot?
                //print("Projectile");
            }
            else if(other.CompareTag("AbilityHighlight")) { //Our we getting previewed for an ability?
                AbilityPreview ability = other.GetComponent<AbilityPreview>();
                if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, this.GetComponent(typeof(IDamageable))))
                    stats.DecIndicatorNum();
            }
            else if(other.CompareTag("Pull")) {
                Component IAbility = other.transform.parent.parent.GetComponent(typeof(IAbility));
                stats.EffectStats.PulledStats.RemovePull(IAbility);
            }
            else { //is it another units vision/range?
                Component unit = other.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(unit) {
                    //Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                    if(other.CompareTag("Range")) { //Are we in their Range detection object?
                        if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) {
                            //do we need the above line? this implies that the unit might already be targeting it. Maybe its because this unit changed itself somehow
                            if((unit as IDamageable).InRangeTargets.Contains(gameObject))
                                (unit as IDamageable).InRangeTargets.Remove(gameObject);
                            if((unit as IDamageable).Target == gameObject)
                                (unit as IDamageable).ReTarget();

                            if((unit as IDamageable).InRangeTargets.Count == 0)
                                (unit as IDamageable).Stats.IncRange = false;
                        }
                    }
                    else if(other.CompareTag("Vision")) { //Are we in their vision detection object?
                        if((unit as IDamageable).HitTargets.Contains(gameObject))
                            (unit as IDamageable).HitTargets.Remove(gameObject);
                        if((unit as IDamageable).Target == gameObject) //if the units target was the one who left the vision
                            (unit as IDamageable).SetTarget(null); 
                    }
                }
            }
        }
        else if(other.CompareTag("FriendlyAbilityHighlight")) { //if the hitbox is from a friendly units ability that hits friendly units
            AbilityPreview ability = other.GetComponent<AbilityPreview>();
            if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, this.GetComponent(typeof(IDamageable))))
                stats.DecIndicatorNum();
        }
    }

    void lookAtTarget() {
        var targetPosition = target.transform.GetChild(0).position;  //

        Vector3 direction = targetPosition - agent.Agent.transform.position;
        direction.y = 0; // Ignore Y, usful for airborne units
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        agent.Agent.transform.rotation = Quaternion.RotateTowards(agent.Agent.transform.rotation, targetRotation, stats.RotationSpeed * Time.deltaTime); //the number is degrees/second, maybe differnt per unit
    }

    void IDamageable.TakeDamage(float amount) {
        if(stats.CurrArmor > 0)
            stats.CurrArmor -= amount;
        else
            stats.CurrHealth -= amount;
        if(shadowStats.InterruptsByDamage)
            stats.Appear(gameObject, shadowStats, agent);
    }
}
