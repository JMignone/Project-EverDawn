using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private SphereCollider hitBox;

    [SerializeField]
    private ProjActor2D unitSprite;

    [SerializeField]
    private float radius;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float range;

    [SerializeField]
    private float baseDamage;

    [SerializeField]
    private GameConstants.OBJECT_ATTACKABLE objectAttackable;

    [SerializeField]
    private bool canPierce;

    [SerializeField]
    private AOEStats aoeStats;

    [SerializeField]
    private BoomerangStats boomerangStats;

    [SerializeField]
    private LingeringStats lingeringStats;

    [SerializeField]
    private SelfDestructStats selfDestructStats;

    [SerializeField]
    private GrenadeStats grenadeStats;

    private Vector3 targetLocation;
    //Below 2 are currently only used for boomerang, but may be needed for others
    private Unit unit;
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
    }

    public float BaseDamage
    {
        get { return baseDamage; }
        //set { baseDamage = value; }
    }

    public GameConstants.OBJECT_ATTACKABLE ObjectAttackable
    {
        get { return objectAttackable; }
    }

    public bool CanPierce
    {
        get { return canPierce; }
    }

    public AOEStats AOEStats
    {
        get { return aoeStats; }
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

    public Unit Unit
    {
        get { return unit; }
        set { unit = value; }
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

    private void Start() {
        hitBox.radius = radius;
        boomerangStats.StartBoomerangStats(gameObject);
        grenadeStats.StartGrenadeStats(gameObject);
        lingeringStats.StartLingeringStats();

        //we cant have a grenade and a boomerang
        if(boomerangStats.IsBoomerang && grenadeStats.IsGrenade)
            boomerangStats.IsBoomerang = false;
    }

    private void Update() {
        if(unit.Agent.transform.position != null) { //This is currently only used for boomerang
            lastKnownLocation = unit.Agent.transform.position;
            lastKnownLocation.y = 0;
        }
        if(lingeringStats.CurrentlyLingering) //if currently lingering
            lingeringStats.UpdateLingeringStats(gameObject);
        float speedReduction = 1; //a multiply used by boomerang projectiles to slow down near the end of flight
        if(boomerangStats.IsBoomerang)
            speedReduction = boomerangStats.SpeedReduction(gameObject, lastKnownLocation);
        if(!lingeringStats.CurrentlyLingering || (lingeringStats.LingerDuringFlight && lingeringStats.IsInFlight)) { //if the projectile doesnt linger or lingers during flight
            if(grenadeStats.IsGrenade)
                grenadeStats.UpdateGrenadeStats(gameObject, speed);
            else {
                if(boomerangStats.IsBoomerang && boomerangStats.GoingBack) {
                    Vector3 direction = transform.position - lastKnownLocation;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = targetRotation;
                    transform.position -= transform.forward * speed * speedReduction * Time.deltaTime;
                }
                else
                    transform.position += transform.forward * speed * speedReduction * Time.deltaTime;
            }
            if(Vector3.Distance(transform.position, targetLocation) <= radius || (Vector3.Distance(transform.position, lastKnownLocation) <= radius) && boomerangStats.IsBoomerang && boomerangStats.GoingBack){ //if the projectile is at the end of its flight
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
        }
        
    }

    public void hit(Component damageable) {
        if(WillHit(damageable)) {
            if(aoeStats.AreaOfEffect)
                aoeStats.Explode(gameObject);
            else
                GameFunctions.Attack(damageable, baseDamage);
            if(!canPierce) {
                if(selfDestructStats.SelfDestructs && !aoeStats.AreaOfEffect) //if the projectile is does not do AOE, but rather self destructs
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
