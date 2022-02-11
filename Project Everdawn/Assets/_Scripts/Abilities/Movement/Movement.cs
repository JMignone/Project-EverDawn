using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : Projectile
{
    [SerializeField]
    private GameConstants.PASS_OBSTACLES passObstacles;

    [Tooltip("If checked and the ability has a target, the movement will land inside the target, rather than giving some wiggle room")]
    [SerializeField]
    private bool landInsideTarget;

    [SerializeField]
    private bool highlightEnemies;

    [Tooltip("If checked the unit will go invisible while dashing")]
    [SerializeField]
    private bool hideUnit;

    [SerializeField]
    private RetreatStats retreatStats;

    public GameConstants.PASS_OBSTACLES PassObstacles
    {
        get { return passObstacles; }
    }

    public bool HighlightEnemies
    {
        get { return highlightEnemies; }
    }

    private void Start() {
        HitBox.radius = Radius;
        StartStats();
        TargetLocation = retreatStats.GetTargetLocation(Unit.Agent.transform.position, TargetLocation, passObstacles);

        LastKnownLocation = new Vector3(Unit.Agent.transform.position.x, 0, Unit.Agent.transform.position.z); //remembers the start location of a unit for boomerang effects
        //we cant have a grenade and a boomerang
        if(BoomerangStats.IsBoomerang && GrenadeStats.IsGrenade)
            BoomerangStats.IsBoomerang = false;

        if(ChosenTarget == null && !OnlyDamageIfTargeted)
            Blockable = true;

        Unit.Agent.Agent.enabled = false;

        if(hideUnit)
            Unit.Stats.UnitMaterials.MakeInvisible();
    }

    private void OnDestroy()
    {
        if(!Unit.Equals(null)) {
            Unit.Agent.Agent.enabled = true;
            if(hideUnit)
                Unit.Stats.UnitMaterials.MakeOpaque();
            StopStats();
        }
    }
    
    private bool targetReached;
    private void FixedUpdate() {
        if(Unit.Equals(null) || !Unit.Stats.CanAct) {
            Destroy(gameObject);
            return;
        }
        Unit.Agent.Agent.transform.position = new Vector3(transform.position.x, Unit.Agent.transform.position.y, transform.position.z);
        Unit.Agent.transform.rotation = transform.rotation;
            
        if(ChosenTarget != null && !BoomerangStats.GoingBack) { //this is only used if the projectile was fired at a specified target. Must check if its a boomerang and already going back
            TargetLocation = ChosenTarget.Agent.transform.position;
            
            Vector3 direction = TargetLocation - transform.position;
            if(direction != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
            if(!landInsideTarget)
                TargetLocation -= direction.normalized * (Unit.Agent.Agent.radius + ChosenTarget.Agent.radius);
        }
        LocationStats.UpdateStats();
        if(LingeringStats.CurrentlyLingering) //if currently lingering
            LingeringStats.UpdateLingeringStats(gameObject);
        float SpeedReduction = 1; //a multiply used by boomerang projectiles to slow down near the end of flight
        if(BoomerangStats.IsBoomerang)
            SpeedReduction = BoomerangStats.SpeedReduction(gameObject, TargetLocation, LastKnownLocation);
        if(!LingeringStats.CurrentlyLingering || (LingeringStats.LingerDuringFlight && LingeringStats.IsInFlight)) { //if the projectile isnt lingering or lingers during flight
            if(targetReached || (Vector3.Distance(transform.position, TargetLocation) <= Radius || ( (Vector3.Distance(transform.position, LastKnownLocation) <= Radius) && BoomerangStats.IsBoomerang && BoomerangStats.GoingBack ) ) ){ //if the projectile is at the end of its flight
                targetReached = true;
                NavMeshHit hit;
                if(!NavMesh.SamplePosition(Unit.Agent.Agent.transform.position, out hit, 1f, 9)) { //if the dashing unit ended up in an obstacle
                    //MonoBehaviour.print("TESTS");
                    if(retreatStats.Retreats) {
                        transform.position -= transform.forward * Speed * SpeedReduction * Time.deltaTime;
                        if(ChosenTarget != null)
                            ChosenTarget.Agent.transform.position -= transform.forward * Speed * SpeedReduction * Time.deltaTime;
                    }
                    else {
                        transform.position += transform.forward * Speed * SpeedReduction * Time.deltaTime;
                        if(ChosenTarget != null)
                            ChosenTarget.Agent.transform.position += transform.forward * Speed * SpeedReduction * Time.deltaTime;
                    }
                }
                else {
                    if(GrenadeStats.IsGrenade)
                        GrenadeStats.Explode(gameObject);
                    else if(SelfDestructStats.SelfDestructs)
                        SelfDestructStats.Explode(gameObject);
                    bool tempGoingBack = BoomerangStats.GoingBack; //Used in case a projectile is boomerang and lingering at the end. We must save going back before changing it
                    if(BoomerangStats.IsBoomerang && !BoomerangStats.GoingBack) { //remember a proj CANNOT be both a grenade and a boomerang
                        BoomerangStats.GoingBack = true;
                        TargetLocation = BoomerangStats.StartLocation; // !! the target location should be the Unit this time !!
                    }
                    if( (LingeringStats.Lingering && LingeringStats.LingerAtEnd) && !(BoomerangStats.IsBoomerang && !tempGoingBack) ) { //if the projectile lingers and lingers at the end
                        LingeringStats.CurrentlyLingering = true;
                        LingeringStats.IsInFlight = false;
                        LingeringStats.CurrentLingeringTime = 0;
                        HitBox.enabled = false;
                    }
                    else {
                        if(!BoomerangStats.IsBoomerang || (BoomerangStats.GoingBack && (Vector3.Distance(transform.position, LastKnownLocation) <= Radius) )) //make sure its not a boomerang that just started going back
                            Destroy(gameObject);
                    }
                }
            }
            else {
                if(GrenadeStats.IsGrenade)
                    GrenadeStats.UpdateGrenadeStats(gameObject, TargetLocation, Speed);
                else {
                    if(BoomerangStats.IsBoomerang && BoomerangStats.GoingBack) {
                        Vector3 direction = transform.position - LastKnownLocation;
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = targetRotation;
                        transform.position -= transform.forward * Speed * SpeedReduction * Time.deltaTime;
                        BoomerangStats.UpdateBoomerangStats();
                    }
                    else if(CustomPathStats.HasCustomPath)
                        CustomPathStats.UpdateStats(TargetLocation);
                    else {
                        //transform.position += transform.forward * Speed * SpeedReduction * Time.deltaTime;
                        //below takes more operations, but may be safer. This might be needed for projectile as well
                        transform.position += (TargetLocation - transform.position).normalized * Speed * SpeedReduction * Time.deltaTime;
                    }
                }
            }
        }
    }
}
