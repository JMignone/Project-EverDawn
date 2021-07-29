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

    [SerializeField]
    private FreezeStats freezeStats;

    [SerializeField]
    private LingeringStats lingeringStats;

    [SerializeField]
    private SelfDestructStats selfDestructStats;
    private bool hasExploded;

    [SerializeField]
    private SummonStats summonStats;

    private Vector3 targetLocation;
    private Unit unit;

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

    public LingeringStats LingeringStats
    {
        get { return lingeringStats; }
    }

    public SelfDestructStats SelfDestructStats
    {
        get { return selfDestructStats; }
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

    public Unit Unit
    {
        get { return unit; }
        set { unit = value; }
    }

    private void Start() 
    {
        hitBox.radius = radius;
        if(selfDestructStats.SelfDestructs) {
            selfDestructStats.ExplosionRadius = radius;
            hasExploded = false;
        }
        if(lingeringStats.Lingering) {
            lingeringStats.StartLingeringStats();
            lingeringStats.CurrentlyLingering = true;
            lingeringStats.IsInFlight = false;
            lingeringStats.LingeringRadius = radius;
        }
    }

    private void Update() {
        if(selfDestructStats.SelfDestructs && !hasExploded){
            selfDestructStats.Explode(gameObject);
            hasExploded = true;
        }
        if(lingeringStats.Lingering)
            lingeringStats.UpdateLingeringStats(gameObject);
        if(SummonStats.IsSummon) {
            if(unit != null) //if the unit is alive, then check if its frozen
                SummonStats.UpdateSummonStats(gameObject, unit.Stats.FrozenStats.IsFrozen);
            else
                SummonStats.UpdateSummonStats(gameObject, true);
        }
        else if(!lingeringStats.Lingering) {
            Destroy(gameObject);
        }
    }

    public bool WillHit(Component damageable) {
        bool willHit = false;
        if(objectAttackable == GameConstants.OBJECT_ATTACKABLE.BOTH) //If the unit can attack the flying or ground unit, continue
            willHit = true;
        else if(objectAttackable == GameConstants.OBJECT_ATTACKABLE.GROUND && (damageable as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.GROUND)
            willHit = true;
        else if(objectAttackable == GameConstants.OBJECT_ATTACKABLE.FLYING && (damageable as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
            willHit = true;
        return willHit;
    }

    public void applyAffects(Component damageable) {
        if(freezeStats.CanFreeze)
            (damageable as IDamageable).Stats.FrozenStats.Freeze(freezeStats.FreezeDuration);
    }
}
