using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected Actor3D agent;

    [SerializeField]
    private Actor2D unitSprite;

    [SerializeField]
    protected GameObject target;

    [SerializeField]
    protected BaseStats stats;

    [SerializeField]
    private AttackStats attackStats;

    //[SerializeField]
    private DashStats dashStats;

    //[SerializeField]
    private ShadowStats shadowStats;
    
    //[SerializeField]
    private DeathStats deathStats;

    [SerializeField]
    protected List<GameObject> hitTargets;

    [SerializeField]
    private List<GameObject> inRangeTargets;

    [SerializeField]
    private List<GameObject> enemyHitTargets;

    [SerializeField]
    private List<GameObject> projectiles;

    [SerializeField]
    protected bool leftTower;

    private List<Component> applyEffectsComponents = new List<Component>();

    public Actor3D Agent
    {
        get { return agent; }
    }

    public Actor2D UnitSprite
    {
        get { return unitSprite; }
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
    }

    public List<GameObject> InRangeTargets
    {
        get { return inRangeTargets; }
    }

    public List<GameObject> EnemyHitTargets
    {
        get { return enemyHitTargets; }
    }

    public List<GameObject> Projectiles
    {
        get { return projectiles; }
    }

    public List<Component> ApplyEffectsComponents
    {
        get { return applyEffectsComponents; }
    }

    public bool IsMoving
    {
        get { return false; }
    }

    public bool LeftTower
    {
        get { return leftTower; }
        set { leftTower = value; }
    }

    protected void Start()
    {        
        stats.HealthBar.enabled = false;
        stats.HealthBar.transform.GetChild(0).gameObject.SetActive(false);

        agent.Agent.angularSpeed = stats.RotationSpeed;

        IDamageable unit = (gameObject.GetComponent(typeof(IDamageable)) as IDamageable);
        stats.SummoningSicknessUI.StartStats(unit);
        stats.EffectStats.StartStats(unit);
        attackStats.StartAttackStats(unit);

        stats.IsHoveringAbility = false;
        stats.AbilityIndicator.enabled = false;
        stats.AbilityIndicator.rectTransform.sizeDelta = new Vector2(2*agent.HitBox.radius + 1, 2*agent.HitBox.radius + 1); 
        // + 1 is better for the knob UI, if we get our own UI image, we may want to remove it

        if(stats.AttackChargeLimiter == 0)
            stats.AttackChargeLimiter = GameConstants.ATTACK_CHARGE_LIMITER;
        if(stats.AttackReadyPercentage == 0)
            stats.AttackReadyPercentage = GameConstants.ATTACK_READY_PERCENTAGE;
        if(stats.MaximumAttackAngle == 0)
            stats.MaximumAttackAngle = GameConstants.MAXIMUM_ATTACK_ANGLE;
        if(stats.TowerDamage == 0)
            stats.TowerDamage = stats.BaseDamage;
    }

    protected virtual void FixedUpdate()
    {
        if(applyEffectsComponents.Count > 0) {
            foreach(Component component in applyEffectsComponents) {
                if(component != null)
                    stats.ApplyAffects(component);
            }
            applyEffectsComponents = new List<Component>();
        }

        if(stats.CurrHealth > 0) {
            stats.IncRange = false; //towers should never have its range incremented
            if((target == null || inRangeTargets.Count == 0) && stats.CanAct) //if the target is null, we must find the closest target in hit targets. If hit targets is empty or failed, find the closest tower
                ReTarget();

            stats.UpdateStats(true, inRangeTargets.Count, agent, hitTargets, target, gameObject);
            Attack();

            if(stats.CanAct) { //if its stunned, we want to keep the tower looking in the same direction
                if((inRangeTargets.Count > 0 || stats.CurrAttackDelay/stats.AttackDelay > stats.AttackReadyPercentage) && target != null) //is in range, OR is 90% thru attack cycle -
                    lookAtTarget();
                else 
                    resetToCenter();
            }
        }
        else {
            print(gameObject.name + " has died!");
            stats.ResetKillFlags(gameObject, target);
            GameManager.RemoveObjectsFromList(gameObject, leftTower, false);
            if(target != null)
               (target.GetComponent(typeof(IDamageable)) as IDamageable).EnemyHitTargets.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    protected void Attack() {
        if(target != null) {
            if(attackStats.FiresProjectiles) { //if the unit fires projectiles rather than simply doing damage when attacking
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
                        float damage = stats.BaseDamage;
                        if(damageable.GetComponent<Tower>())
                            damage = stats.TowerDamage;
                        if(hitTargets.Contains(target)) {  //this and the above may not be needed, more of a santiy check
                            if(stats.EffectStats.AOEStats.AreaOfEffect)
                                stats.EffectStats.AOEStats.Explode(gameObject, target, damage * stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
                            else {
                                GameFunctions.Attack(damageable, damage * stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity, stats.EffectStats.CritStats);
                                //stats.ApplyAffects(damageable);
                                applyEffectsComponents.Add(damageable);
                            }
                            stats.CurrAttackDelay = 0;
                            stats.ResetKillFlags(gameObject, target);
                        }
                    }
                }
            }
        }
        if(attackStats.FiresProjectiles)
            attackStats.Fire();
    }

    public void SetTarget(GameObject newTarget) {
        if(newTarget != target) {
            if(target != null) {
                (target.GetComponent(typeof(IDamageable)) as IDamageable).EnemyHitTargets.Remove(gameObject);
                stats.ResetKillFlags(gameObject, target);
            }
            if(newTarget != null)
                (newTarget.GetComponent(typeof(IDamageable)) as IDamageable).EnemyHitTargets.Add(gameObject);
            target = newTarget;
        }
    }

    public void ReTarget() {
        if(hitTargets.Count > 0) {
            GameObject go = GameFunctions.GetNearestTarget(hitTargets, gameObject.tag, stats);
            if(go != null)
                SetTarget(go);
        }
    }

    /* I dont think structres need a vision radius, keep it for now */
    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag) && stats.CurrHealth != 0) { //checks to make sure the target isnt on the same team
            if(other.CompareTag("Projectile")) { //Did we get hit by a skill shot?
                Projectile projectile = other.transform.parent.parent.GetComponent<Projectile>();
                Component unit = this.GetComponent(typeof(IDamageable));
                projectile.Hit(unit);
            }
            else if(other.CompareTag("AbilityHighlight")) { //Our we getting previewed for an abililty?
                AbilityPreview ability = other.GetComponent<AbilityPreview>();
                if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, this.GetComponent(typeof(IDamageable)))) 
                stats.IncIndicatorNum();
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
                    //Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                    if(other.CompareTag("Range")) {//Are we in their range detection object?
                        if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) {//anything can attack a tower, ill leave it hear incase somthing with an ability gives a need for this
                            if(!(unit as IDamageable).InRangeTargets.Contains(gameObject))
                                (unit as IDamageable).InRangeTargets.Add(gameObject);
                            if(!(unit as IDamageable).HitTargets.Contains(gameObject))
                                (unit as IDamageable).HitTargets.Add(gameObject);
                                
                            if((unit as IDamageable).InRangeTargets.Count == 1)
                                (unit as IDamageable).Stats.IncRange = true;
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
            else if(other.CompareTag("AbilityHighlight")) { //Our we getting previewed for an abililty?
                AbilityPreview ability = other.GetComponent<AbilityPreview>();
                if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, this.GetComponent(typeof(IDamageable)))) 
                    stats.DecIndicatorNum();
            }
            else { //is it another units vision/range?
                Component unit = other.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(unit) {
                    //Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                    if(other.CompareTag("Range")) { //Are we in their Range detection object?
                        if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) {
                            if((unit as IDamageable).InRangeTargets.Contains(gameObject))
                                (unit as IDamageable).InRangeTargets.Remove(gameObject);
                            if((unit as IDamageable).Target == gameObject)
                                (unit as IDamageable).SetTarget(null);

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

    protected void lookAtTarget() {
        var targetPosition = target.transform.GetChild(0).position;
        Vector3 direction = targetPosition - agent.Agent.transform.position; //flip this as needed if the tower is in the oppisite direction
        direction.y = 0; // Ignore Y
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // encase somthing here such that the base of the turret does not rotate
        agent.Agent.transform.rotation = Quaternion.RotateTowards(agent.Agent.transform.rotation, targetRotation, stats.RotationSpeed * Time.deltaTime); //the number is degrees/second, maybe differnt per unit
         //
    }

    protected void resetToCenter() { //resets a towers direction
        var targetPosition = agent.Agent.transform.position;
        targetPosition.z = 0;
        Vector3 direction = targetPosition - agent.Agent.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        agent.Agent.transform.rotation = Quaternion.RotateTowards(agent.Agent.transform.rotation, targetRotation, stats.RotationSpeed * Time.deltaTime);
    }

    void IDamageable.TakeDamage(float amount) {
        if(stats.CurrArmor > 0)
            stats.CurrArmor -= amount;
        else
            stats.CurrHealth -= amount;
    }

}
