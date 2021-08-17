using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAtLocation : MonoBehaviour, IAbility
{
    [SerializeField]
    private SphereCollider hitBox;

    [SerializeField]
    private float radius;

    [SerializeField]
    private float range;

    [SerializeField]
    private GameConstants.OBJECT_ATTACKABLE objectAttackable;

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
    private LingeringStats lingeringStats;

    [SerializeField]
    private SelfDestructStats selfDestructStats;

    [SerializeField]
    private LinearStats linearStats;
    private bool hasExploded;

    [SerializeField]
    private SummonStats summonStats;

    private Vector3 targetLocation;
    private Unit unit;

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

    public GameConstants.OBJECT_ATTACKABLE ObjectAttackable
    {
        get { return objectAttackable; }
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

    public Unit Unit
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
        
        if(selfDestructStats.SelfDestructs)
            selfDestructStats.ExplosionRadius = radius;
        hasExploded = false;
        if(lingeringStats.Lingering) {
            lingeringStats.StartLingeringStats();
            lingeringStats.CurrentlyLingering = true;
            lingeringStats.IsInFlight = false;
            lingeringStats.LingeringRadius = radius;
        }

        //if(chosenTarget == null)
        //    blockable = true;
    }

    private void Update() {
        if(selfDestructStats.SelfDestructs && !hasExploded)
            selfDestructStats.Explode(gameObject);
        if(linearStats.IsLinear && !hasExploded)
            linearStats.Explode(gameObject);
        hasExploded = true;
        if(lingeringStats.Lingering)
            lingeringStats.UpdateLingeringStats(gameObject);
        if(SummonStats.IsSummon) {
            if(unit != null) //if the unit is alive, then check if its stunned
                SummonStats.UpdateSummonStats(gameObject, !unit.Stats.CanAct());
            else
                SummonStats.UpdateSummonStats(gameObject, true);
        }
        else if(!lingeringStats.Lingering) {
            Destroy(gameObject);
        }
    }

    public void ApplyAffects(Component damageable) {
        if(freezeStats.CanFreeze)
            (damageable as IDamageable).Stats.FrozenStats.Freeze(freezeStats.FreezeDuration);
        if(slowStats.CanSlow)
            (damageable as IDamageable).Stats.SlowedStats.Slow(slowStats.SlowDuration, slowStats.SlowIntensity);
        if(rootStats.CanRoot)
            (damageable as IDamageable).Stats.RootedStats.Root(RootStats.RootDuration);
        if(poisonStats.CanPoison)
            (damageable as IDamageable).Stats.PoisonedStats.Poison(poisonStats.PoisonDuration, poisonStats.PoisonTick, poisonStats.PoisonDamage);
        if(knockbackStats.CanKnockback)
            (damageable as IDamageable).Stats.KnockbackedStats.Knockback(knockbackStats.KnockbackDuration, knockbackStats.InitialSpeed, gameObject.transform.position);
    }
}
