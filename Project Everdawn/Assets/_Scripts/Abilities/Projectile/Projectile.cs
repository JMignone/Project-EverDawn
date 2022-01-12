using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IAbility
{
    [Header("Gameobjects")]
    [SerializeField]
    private SphereCollider hitBox;

    [SerializeField]
    private ProjActor2D unitSprite;

    [Header("Base Stats")]
    [SerializeField] [Min(0)]
    private float radius;

    [SerializeField] [Min(0)]
    private float speed;

    [SerializeField] [Min(0)]
    private float range;

    [SerializeField] [Min(0)]
    private float baseDamage;

    [Tooltip("If set to 0, towerDamage will be set to baseDamage")]
    [SerializeField] [Min(0)]
    private float towerDamage;
    private float damageMultiplier; //used for units that may have had its damage increased/decreased and fires projectiles as a ranged attack

    [Tooltip("Determines if the projectile can hit units on the ground, flying, or both")]
    [SerializeField]
    private GameConstants.HEIGHT_ATTACKABLE heightAttackable;

    [Tooltip("Determines if the projectile can hit units, structures, or both")]
    [SerializeField]
    private GameConstants.TYPE_ATTACKABLE typeAttackable;

    [SerializeField]
    private bool canPierce;

    [Header("Higher Level Controls")]
    [Tooltip("If checked, a targeted ability will be able to hit a unit that is not its target given the unit blocked its path")]
    [SerializeField]
    private bool blockable; //simply means that a projectile can hit somthing that it didnt nessesarly target. Automatically set to true if there is no specific target

    [Tooltip("If checked, the skillshot script will not tell BaseStats that the unit is done casting. This job will be left to the ability")]
    [SerializeField]
    private bool abilityControl;

    private bool hit; //used for the below 2 variables
    [Tooltip("If checked, the skillshot will continue once this projectile is destroyed")]
    [SerializeField]
    private bool pauseAbility;

    [Tooltip("If checked, the skillshot will have its exitOverride value flagged on a miss, meaning it will stop firing further")]
    [SerializeField]
    private bool stopOnMiss;

    [Tooltip("If checked, the skillshot will skip the last number of abilities")]
    [SerializeField]
    private bool skipLastOnMiss;

    [SerializeField] [Min(0)]
    private int skipNumber;

    [Tooltip("If checked, a target will be sent to the skillshot script for the future abilities")]
    [SerializeField]
    private bool overridesTarget;

    [Tooltip("If checked, the ability will not do damage unless its on a target")]
    [SerializeField]
    private bool onlyDamageIfTargeted;

    [Tooltip("If checked, the range preview will not display")]
    [SerializeField]
    private bool hideRange;

    [Tooltip("If checked, the previews will not display")]
    [SerializeField]
    private bool hidePreview;
    
    [Header("Higher Level Stats")]
    [SerializeField]
    private CustomPathStats customPathStats;

    [SerializeField]
    private LocationStats locationStats;

    [SerializeField]
    private AOEStats aoeStats;

    [SerializeField]
    private FreezeStats freezeStats;

    [SerializeField]
    private SlowStats slowStats;

    [SerializeField]
    private RootStats rootStats;

    [SerializeField]
    private PoisonStats poisonStats;

    [SerializeField]
    private KnockbackStats knockbackStats;

    [SerializeField]
    private PullStats pullStats;

    [SerializeField]
    private GrabStats grabStats;

    [SerializeField]
    private StrengthStats strengthStats;
    
    [SerializeField]
    private BlindStats blindStats;

    [SerializeField]
    private StunStats stunStats;

    [SerializeField]
    private BoomerangStats boomerangStats;

    [SerializeField]
    private LingeringStats lingeringStats;

    [SerializeField]
    private SelfDestructStats selfDestructStats;

    [SerializeField]
    private GrenadeStats grenadeStats;

    [SerializeField]
    private ResistEffects resistEffects; //gives the shooting unit resistance while the projectile is in play

    [SerializeField]
    private ApplyResistanceStats applyResistanceStats; //what resistances the projectile gives to its target or the user for a duration

    private Vector3 targetLocation;
    private Actor3D chosenTarget;

    private ICaster caster;
    private IDamageable unit;
    //Currently only used for boomerang, but may be needed for others. This variable gets updated wiith the location of the unit until it dies
    private Vector3 lastKnownLocation;

    public SphereCollider HitBox
    {
        get { return hitBox; }
    }

    public ProjActor2D UnitSprite
    {
        get { return unitSprite; }
    }

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    public float Speed
    {
        get { return speed; }
    }

    public float Range
    {
        get { return range; }
        set { range = value; }
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

    public float DamageMultiplier
    {
        get { return damageMultiplier; }
        set { damageMultiplier = value; }
    }

    public GameConstants.HEIGHT_ATTACKABLE HeightAttackable
    {
        get { return heightAttackable; }
    }

    public GameConstants.TYPE_ATTACKABLE TypeAttackable
    {
        get { return typeAttackable; }
    }

    public bool CanPierce
    {
        get { return canPierce; }
    }

    public bool AbilityControl
    {
        get { return abilityControl; }
    }

    public bool SetHit
    {
        get { return hit; }
        set { hit = value; }
    }

    public bool HideRange
    {
        get { return hideRange; }
    }

    public bool HidePreview
    {
        get { return hidePreview; }
    }

    public bool OnlyDamageIfTargeted
    {
        get { return onlyDamageIfTargeted; }
    }

    public CustomPathStats CustomPathStats
    {
        get { return customPathStats; }
    }

    public LocationStats LocationStats
    {
        get { return locationStats; }
    }

    public AOEStats AOEStats
    {
        get { return aoeStats; }
    }

    public FreezeStats FreezeStats
    {
        get { return freezeStats; }
    }

    public SlowStats SlowStats
    {
        get { return slowStats; }
    }

    public RootStats RootStats
    {
        get { return rootStats; }
    }

    public PoisonStats PoisonStats
    {
        get { return poisonStats; }
    }

    public KnockbackStats KnockbackStats
    {
        get { return knockbackStats; }
    }

    public PullStats PullStats
    {
        get { return pullStats; }
    }

    public GrabStats GrabStats
    {
        get { return grabStats; }
    }

    public StrengthStats StrengthStats
    {
        get { return strengthStats; }
    }

    public BlindStats BlindStats
    {
        get { return blindStats; }
    }

    public Vector3 Position()
    {
        return transform.position;
    }

    public BoomerangStats BoomerangStats
    {
        get { return boomerangStats; }
    }

    public LingeringStats LingeringStats
    {
        get { return lingeringStats; }
    }

    public SelfDestructStats SelfDestructStats
    {
        get { return selfDestructStats; }
    }

    /*
        'Base Damage', 'Can Pierce', 'Area of Effect', 'Boomerang', and 'Self Destructs'
        all have no effect if 'Is Grenade' is true
    */
    public GrenadeStats GrenadeStats
    {
        get { return grenadeStats; }
    }

    public Vector3 TargetLocation
    {
        get { return targetLocation; }
        set { targetLocation = value; }
    }

    public Vector3 LastKnownLocation
    {
        get { return lastKnownLocation; }
        set { lastKnownLocation = value; }
    }

    public Actor3D ChosenTarget
    {
        get { return chosenTarget; }
        set { chosenTarget = value; }
    }

    public bool Blockable
    {
        get { return blockable; }
        set { blockable = value; }
    }

    public ICaster Caster
    {
        get { return caster; }
        set { caster = value; }
    }

    public IDamageable Unit
    {
        get { return unit; }
        set { unit = value; }
    }

    public int AreaMask() //this function is only needed in cal, however we need it in the interface
    {
        return 1; //1 represents the 'walkable' area
    }

    private void Start() {
        hitBox.radius = radius;
        StartStats();

        //we cant have a grenade and a boomerang
        if(boomerangStats.IsBoomerang && grenadeStats.IsGrenade)
            boomerangStats.IsBoomerang = false;

        if(chosenTarget == null && !onlyDamageIfTargeted)
            blockable = true;
    }

    protected void StartStats() {
        boomerangStats.StartBoomerangStats(gameObject);
        grenadeStats.StartGrenadeStats(gameObject);
        lingeringStats.StartLingeringStats(gameObject);
        slowStats.StartSlowStats();
        pullStats.StartPullStats(gameObject);
        strengthStats.StartStrengthStats();
        locationStats.StartStats(unit, gameObject, caster);
        customPathStats.StartStats(gameObject, targetLocation);
        
        if(unit != null && !unit.Equals(null)) {
            resistEffects.StartResistance(unit);
            applyResistanceStats.StartResistance(unit);

            if(caster != null)
                caster.PauseFiring = pauseAbility;
        }

        if(towerDamage == 0)
            towerDamage = baseDamage;
    }

    protected void StopStats() {
        if(unit != null && !unit.Equals(null)) {
            resistEffects.StopResistance(unit);

            if(abilityControl)
                unit.Stats.IsCastingAbility = false;

            if(caster != null) {
                caster.PauseFiring = false;
                caster.ExitOverride = stopOnMiss && !hit;
                if(skipLastOnMiss && !hit)
                    caster.SkipOverride = skipNumber;

                if(locationStats.OverridesLocation && locationStats.OverridesAtEnd)
                    locationStats.LocationOveride();
            }
        }
    }

    private void OnDestroy()
    {
        StopStats();
    }

    private void Update() {
        hitBox.transform.position = new Vector3(hitBox.transform.position.x, 0, hitBox.transform.position.z);
        if(unit != null && !unit.Equals(null)) { //This is currently only used for boomerang
            lastKnownLocation = unit.Agent.transform.position;
            lastKnownLocation.y = 0;
        }
        if(chosenTarget != null && !boomerangStats.GoingBack) {//this is only used if the projectile was fired at a specified target. Must check if its a boomerang and already going back
            targetLocation = chosenTarget.Agent.transform.position;
            
            Vector3 direction = targetLocation - transform.position;
            if(direction != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }
        locationStats.UpdateStats();
        if(lingeringStats.CurrentlyLingering) //if currently lingering
            lingeringStats.UpdateLingeringStats(gameObject);
        float speedReduction = 1; //a multiply used by boomerang projectiles to slow down near the end of flight
        if(boomerangStats.IsBoomerang)
            speedReduction = boomerangStats.SpeedReduction(gameObject, targetLocation, lastKnownLocation);
        if(!lingeringStats.CurrentlyLingering || (lingeringStats.LingerDuringFlight && lingeringStats.IsInFlight)) { //if the projectile doesnt linger or lingers during flight
            if(Vector3.Distance(transform.position, targetLocation) <= radius || ( (Vector3.Distance(transform.position, lastKnownLocation) <= radius) && boomerangStats.IsBoomerang && boomerangStats.GoingBack ) ){ //if the projectile is at the end of its flight
                if(grenadeStats.IsGrenade)
                    grenadeStats.Explode(gameObject);
                else if(selfDestructStats.SelfDestructs)
                    selfDestructStats.Explode(gameObject);
                bool tempGoingBack = boomerangStats.GoingBack; //Used in case a projectile is boomerang and lingering at the end. We must save going back before changing it
                if(boomerangStats.IsBoomerang && !boomerangStats.GoingBack) {
                    boomerangStats.GoingBack = true;
                    targetLocation = boomerangStats.StartLocation; // !! the target location should be the unit this time !!
                }
                if( (lingeringStats.Lingering && lingeringStats.LingerAtEnd) && !(boomerangStats.IsBoomerang && !tempGoingBack) ) { //if the projectile lingers and lingers at the end
                    lingeringStats.CurrentlyLingering = true;
                    lingeringStats.IsInFlight = false;
                    lingeringStats.CurrentLingeringTime = 0;
                    hitBox.enabled = false;
                }
                else {
                    if(!boomerangStats.IsBoomerang || (boomerangStats.GoingBack && (Vector3.Distance(transform.position, lastKnownLocation) <= radius) )) //make sure its not a boomerang that just started going back
                        Destroy(gameObject);
                }
            }
            if(grenadeStats.IsGrenade)
                grenadeStats.UpdateGrenadeStats(gameObject, targetLocation, speed);
            else if(boomerangStats.IsBoomerang && boomerangStats.GoingBack) {
                Vector3 direction = transform.position - lastKnownLocation;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
                transform.position -= transform.forward * speed * speedReduction * Time.deltaTime;
                boomerangStats.UpdateBoomerangStats();
            }
            else if(customPathStats.HasCustomPath)
                customPathStats.UpdateStats(targetLocation);
            else
                transform.position += transform.forward * speed * speedReduction * Time.deltaTime;
        }
    }

    public void Hit(Component damageable) {
        if(blockable || (damageable as IDamageable).Agent == chosenTarget) { //if the projectile is blockable, or this is infact the chosen target
        
            if(GameFunctions.WillHit(heightAttackable, typeAttackable, damageable)) {
                hit = true;

                if(overridesTarget && caster != null)
                    caster.SetNewTarget((damageable as IDamageable).Agent);

                if(aoeStats.AreaOfEffect)
                    aoeStats.Explode(gameObject);
                else {
                    float damage = baseDamage*damageMultiplier;
                    if(damageable.GetComponent<Tower>())
                        damage = towerDamage*damageMultiplier;

                    GameFunctions.Attack(damageable, damage);
                    ApplyAffects(damageable);
                }
                if(!canPierce) {
                    if(selfDestructStats.SelfDestructs && !aoeStats.AreaOfEffect) //if the projectile does not do AOE, but rather self destructs
                        selfDestructStats.Explode(gameObject);
                    if(lingeringStats.Lingering && lingeringStats.LingerAtEnd) {
                        lingeringStats.CurrentlyLingering = true;
                        lingeringStats.IsInFlight = false;
                        lingeringStats.CurrentLingeringTime = 0;
                        hitBox.enabled = false;
                    }
                    else
                        Destroy(gameObject);
                }
            }
        }
    }

    public void ApplyAffects(Component damageable) {
        if(freezeStats.CanFreeze)
            (damageable as IDamageable).Stats.EffectStats.FrozenStats.Freeze(freezeStats.FreezeDuration);
        if(slowStats.CanSlow)
            (damageable as IDamageable).Stats.EffectStats.SlowedStats.Slow(slowStats.SlowDuration, slowStats.SlowIntensity);
        if(rootStats.CanRoot)
            (damageable as IDamageable).Stats.EffectStats.RootedStats.Root(RootStats.RootDuration);
        if(poisonStats.CanPoison)
            (damageable as IDamageable).Stats.EffectStats.PoisonedStats.Poison(poisonStats.PoisonDuration, poisonStats.PoisonTick, poisonStats.PoisonDamage);
        if(knockbackStats.CanKnockback)
            (damageable as IDamageable).Stats.EffectStats.KnockbackedStats.Knockback(knockbackStats.KnockbackDuration, knockbackStats.InitialSpeed, gameObject.transform.position);
        if(grabStats.CanGrab)
            (damageable as IDamageable).Stats.EffectStats.GrabbedStats.Grab(grabStats.Speed, grabStats.PullDuration, grabStats.StunDuration, grabStats.ObstaclesBlockGrab, unit);
        if(strengthStats.CanStrength)
            (damageable as IDamageable).Stats.EffectStats.StrengthenedStats.Strengthen(strengthStats.StrengthDuration, StrengthStats.StrengthIntensity);
        if(blindStats.CanBlind)
            (damageable as IDamageable).Stats.EffectStats.BlindedStats.Blind(blindStats.BlindDuration);
        if(stunStats.CanStun)
            (damageable as IDamageable).Stats.EffectStats.StunnedStats.Stun(stunStats.StunDuration);
        applyResistanceStats.ApplyResistance((damageable as IDamageable));
    }
}
