using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAtLocation : MonoBehaviour, IAbility
{
    [SerializeField]
    private SphereCollider hitBox;

    private float damageMultiplier; //used for units that may have had its damage increased/decreased and fires projectiles as a ranged attack

    [SerializeField]
    private float radius;

    [SerializeField]
    private float range;

    [SerializeField]
    private GameConstants.HEIGHT_ATTACKABLE heightAttackable;

    [SerializeField]
    private GameConstants.TYPE_ATTACKABLE typeAttackable;

    //[SerializeField]
    //private bool blockable; //currently not sure if we will need this for CAL's. Description of this var is in Projectile

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
    private LingeringStats lingeringStats;

    [SerializeField]
    private SelfDestructStats selfDestructStats;

    [SerializeField]
    private LinearStats linearStats;

    [SerializeField]
    private TeleportStats teleportStats;
    private bool hasExploded;

    [SerializeField]
    private SummonStats summonStats;

    [SerializeField]
    private ApplyResistanceStats applyResistanceStats; //what resistances the projectile gives to its target or the user for a duration

    private Vector3 targetLocation;
    private IDamageable unit;

    [SerializeField]
    private Actor3D chosenTarget;

    public SphereCollider HitBox
    {
        get { return hitBox; }
    }

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    public float Range
    {
        get { return range; }
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

    public Vector3 Position()
    {
        return transform.position;
    }

    public LingeringStats LingeringStats
    {
        get { return lingeringStats; }
    }

    public SelfDestructStats SelfDestructStats
    {
        get { return selfDestructStats; }
    }

    public LinearStats LinearStats
    {
        get { return linearStats; }
    }

    public TeleportStats TeleportStats
    {
        get { return teleportStats; }
    }

    public SummonStats SummonStats
    {
        get { return summonStats; }
    }

    public Vector3 TargetLocation
    {
        get { return targetLocation; }
        set { targetLocation = value; }
    }

    public Actor3D ChosenTarget
    {
        get { return chosenTarget; }
        set { chosenTarget = value; }
    }

    public IDamageable Unit
    {
        get { return unit; }
        set { unit = value; }
    }

    public int AreaMask()
    {
        if(summonStats.IsSummon)
            return summonStats.AreaMask();
        else
            return 1;
    }

    private void Start() 
    {
        hitBox.radius = radius;
        slowStats.StartSlowStats();
        pullStats.StartPullStats(gameObject);
        strengthStats.StartStrengthStats();
        
        if(selfDestructStats.SelfDestructs)
            selfDestructStats.ExplosionRadius = radius;
        hasExploded = false;
        if(lingeringStats.Lingering) {
            lingeringStats.StartLingeringStats(gameObject);
            lingeringStats.CurrentlyLingering = true;
            lingeringStats.IsInFlight = false;
            lingeringStats.LingeringRadius = radius;
        }
        if(linearStats.IsLinear)
            linearStats.StartLinearStats(damageMultiplier);
        teleportStats.StartStats(unit);

        if(unit != null)
            applyResistanceStats.StartResistance(unit);

        //if(chosenTarget == null)
        //    blockable = true;
    }

    private void Update() {
        if(selfDestructStats.SelfDestructs && !hasExploded)
            selfDestructStats.Explode(gameObject);
        if(linearStats.IsLinear && !hasExploded)
            linearStats.Explode(gameObject);
        if(teleportStats.IsWarp && !hasExploded)
            teleportStats.Warp(gameObject);
        hasExploded = true;
        if(lingeringStats.Lingering)
            lingeringStats.UpdateLingeringStats(gameObject);
        if(SummonStats.IsSummon) {
            if(unit != null && unit.Agent != null) //if the unit is alive, then check if its stunned
                SummonStats.UpdateSummonStats(gameObject, unit.Stats.CanAct);
            else
                SummonStats.UpdateSummonStats(gameObject, false);
        }
        else if(!lingeringStats.Lingering) {
            Destroy(gameObject);
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
            (damageable as IDamageable).Stats.EffectStats.GrabbedStats.Grab(grabStats.PullDuration, grabStats.StunDuration, unit);
        if(strengthStats.CanStrength)
            (damageable as IDamageable).Stats.EffectStats.StrengthenedStats.Strengthen(strengthStats.StrengthDuration, StrengthStats.StrengthIntensity);
        applyResistanceStats.ApplyResistance((damageable as IDamageable));
    }
}
