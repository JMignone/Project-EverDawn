using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Actor3D agent;

    [SerializeField]
    private Actor2D unitSprite;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private bool rotates;
    
    [SerializeField]
    private GameConstants.BUILDING_TYPE buildingType; //might not need this as a building that can attack might be better off labeled as a unit

    [SerializeField]
    private GameConstants.BUILDING_SIZE buildingSize;

    [SerializeField]
    private BaseStats stats;

    [SerializeField]
    private SpawnStats spawnStats;

    [SerializeField]
    private AttackStats attackStats;

    [SerializeField]
    private BuildUpStats buildUpStats;

    //[SerializeField]
    private DashStats dashStats; //needed for interface

    [SerializeField]
    private ShadowStats shadowStats;

    [SerializeField]
    private DeathStats deathStats;

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

    public bool Rotates
    {
        get { return rotates; }
    }

    public GameConstants.BUILDING_TYPE BuildingType
    {
        get { return buildingType; } 
    }

    public GameConstants.BUILDING_SIZE BuildingSize
    {
        get { return buildingSize; } 
    }

    public BaseStats Stats
    {
        get { return stats; }
    }

    public SpawnStats SpawnStats
    {
        get { return spawnStats; }
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
    }

    public List<GameObject> InRangeTargets
    {
        get { return inRangeTargets; }
    }

    public List<GameObject> EnemyHitTargets
    {
        get { return enemyHitTargets; }
    }

    public bool IsMoving
    {
        get { return false; }
    }

    public bool ChargeAttack
    {
        get { return !attackStats.IsFiring; }
    }

    private void Start()
    {
        IDamageable unit = (gameObject.GetComponent(typeof(IDamageable)) as IDamageable);

        stats.SummoningSicknessUI.StartStats(unit);
        stats.EffectStats.StartStats(unit);
        attackStats.StartAttackStats(unit);
        buildUpStats.StartStats(unit);
        shadowStats.StartShadowStats(unit);
        
        stats.IsHoveringAbility = false;
        stats.AbilityIndicator.enabled = false;
        stats.AbilityIndicator.rectTransform.sizeDelta = new Vector2(2*agent.HitBox.radius + 1, 2*agent.HitBox.radius + 1); 
        // + 1 is better for the knob UI, if we get our own UI image, we may want to remove it
    }

    private void Update()
    {
        if(stats.CurrHealth > 0) {
            if(buildingType == GameConstants.BUILDING_TYPE.SPAWN) {
                stats.UpdateBuildingStats();
                shadowStats.UpdateShadowStats();
                Spawn();
            }
            else if(buildingType == GameConstants.BUILDING_TYPE.ATTACK) {
                if((target == null || inRangeTargets.Count == 0) && stats.CanAct && !stats.IsCastingAbility) //if the target is null, we must find the closest target in hit targets. If hit targets is empty or failed, find the closest tower
                    ReTarget();

                stats.UpdateStats(ChargeAttack, inRangeTargets.Count, agent, hitTargets, target);
                buildUpStats.UpdateStats();
                shadowStats.UpdateShadowStats();
                Attack();

                if(target != null && !attackStats.IsFiring && rotates) {
                    if(hitTargets.Contains(target)) {
                        if(inRangeTargets.Count > 0 || stats.CurrAttackDelay/stats.AttackDelay >= GameConstants.ATTACK_READY_PERCENTAGE) //is in range, OR is 90% thru attack cycle -
                            lookAtTarget();
                    }
                }
                else if(target != null && !attackStats.AttacksLocation && rotates)
                    lookAtTarget();
            }
        }
        else {
            print(gameObject.name + " has died!");
            GameManager.RemoveObjectsFromList(gameObject);
            Destroy(gameObject);
        }
    }

    void Spawn() {
        if(stats.CurrAttackDelay >= stats.AttackDelay && stats.IsReady) {
            Vector3 position = agent.transform.position;
            position += transform.forward * 7;
            Quaternion rotation = agent.transform.rotation;

            GameObject spawnedUnit = Instantiate(spawnStats.UnitToSpawn, position, rotation, transform.parent);
            GameFunctions.giveUnitTeam(spawnedUnit, gameObject.tag);

            spawnedUnit.transform.Rotate(0, 0, 0, Space.Self);
            GameManager.AddObjectToList(spawnedUnit);
        }
    }

    void Attack() {
        if(target != null) {
            if(attackStats.FiresProjectiles) { //if the unit fires projectiles rather than simply doing damage when attacking
                if(stats.CurrAttackDelay >= stats.AttackDelay && !attackStats.IsFiring) {
                    print("testsdfsdf");
                    Component damageable = target.GetComponent(typeof(IDamageable));
                    if(damageable) { //is the target damageable
                        if(hitTargets.Contains(target)) //this is needed for the rare occurance that a unit is 90% done with attack delay and the target leaves its range. It can still do its attack if its within vision given that its attack was already *90% thru
                            attackStats.BeginFiring();
                    }
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
                                GameFunctions.Attack(damageable, stats.BaseDamage * stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
                                stats.ApplyAffects(damageable);
                            }
                            buildUpStats.BuildUp();
                            stats.Appear(gameObject, shadowStats, agent);
                            stats.CurrAttackDelay = 0;
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
            if(target != null)
                (target.GetComponent(typeof(IDamageable)) as IDamageable).EnemyHitTargets.Remove(gameObject);
            if(newTarget != null)
                (newTarget.GetComponent(typeof(IDamageable)) as IDamageable).EnemyHitTargets.Add(gameObject);
            target = newTarget;
            buildUpStats.ResetStats(true);
        }
    }

    public void ReTarget() {
        if(hitTargets.Count > 0) {
            GameObject go = GameFunctions.GetNearestTarget(hitTargets, gameObject.tag, stats);
            if(go != null)
                SetTarget(go);
            else {
                List<GameObject> towers = GameManager.Instance.TowerObjects;
                towers = GameManager.GetAllEnemies(transform.GetChild(0).position, towers, gameObject.tag); //sending in only towers
                SetTarget(GameFunctions.GetNearestTarget(towers, gameObject.tag, stats));
            }
        }
        else {
            List<GameObject> towers = GameManager.Instance.TowerObjects;
            towers = GameManager.GetAllEnemies(transform.GetChild(0).position, towers, gameObject.tag); //sending in only towers
            SetTarget(GameFunctions.GetNearestTarget(towers, gameObject.tag, stats));
        }
    }

    // !! THIS WILL LIKELY NEED TO BE CHANGED SUCH THAT ONLY THE TOP OF THE BUILDING ROTATES !!
    void lookAtTarget() {
        var targetPosition = target.transform.GetChild(0).position;  //

        Vector3 direction = targetPosition - agent.Agent.transform.position;
        direction.y = 0; // Ignore Y, usful for airborne units
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        agent.Agent.transform.rotation = Quaternion.RotateTowards(agent.Agent.transform.rotation, targetRotation, stats.RotationSpeed * Time.deltaTime); //the number is degrees/second, maybe differnt per unit
    }

    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.CompareTag(gameObject.tag)) { //checks to make sure the target isnt on the same team
            if(other.CompareTag("Projectile")) { //Did we get hit by a skill shot?
                Projectile projectile = other.transform.parent.parent.GetComponent<Projectile>();
                Component unit = this.GetComponent(typeof(IDamageable));
                projectile.hit(unit);
            }
            else if(other.CompareTag("AbilityHighlight")) { //Our we getting previewed for an abililty?
                AbilityPreview ability = other.GetComponent<AbilityPreview>();
                if(stats.Damageable && GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, this.GetComponent(typeof(IDamageable)))) 
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
                        if(GameFunctions.CanAttack(unit.tag, gameObject.tag, gameObject.GetComponent(typeof(IDamageable)), (unit as IDamageable).Stats)) { //only if the unit can actually target this one should we adjust this value
                            if(!(unit as IDamageable).InRangeTargets.Contains(gameObject))
                                (unit as IDamageable).InRangeTargets.Add(gameObject);
                            if( ((unit as IDamageable).InRangeTargets.Count == 1 || (unit as IDamageable).Target == null) && (unit as IDamageable).Stats.CanAct) {
                                GameObject go = GameFunctions.GetNearestTarget((unit as IDamageable).HitTargets, other.transform.parent.parent.tag, (unit as IDamageable).Stats);
                                if(go != null)
                                    (unit as IDamageable).SetTarget(go);
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
                        }
                    }
                    else if(other.CompareTag("Vision")) { //Are we in their vision detection object?
                        if((unit as IDamageable).HitTargets.Contains(gameObject))
                            (unit as IDamageable).HitTargets.Remove(gameObject);
                        if((unit as IDamageable).Target == gameObject) //if the units target was the one who left the vision
                            (unit as IDamageable).Target = null; 
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

    void IDamageable.TakeDamage(float amount) {
        if(stats.CurrArmor > 0)
            stats.CurrArmor -= amount;
        else
            stats.CurrHealth -= amount;
        if(shadowStats.InterruptsByDamage)
            stats.Appear(gameObject, shadowStats, agent);
    }
}
