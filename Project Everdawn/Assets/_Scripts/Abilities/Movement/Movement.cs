using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : Projectile
{
    [SerializeField]
    private bool passObstacles;

    [SerializeField]
    private bool highlightEnemies;

    [SerializeField]
    private RetreatStats retreatStats;

    public bool PassObstacles
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
        TargetLocation = retreatStats.GetTargetLocation(Unit.Agent.transform.position, TargetLocation);

        LastKnownLocation = new Vector3(Unit.Agent.transform.position.x, 0, Unit.Agent.transform.position.z); //remembers the start location of a unit for boomerang effects
        //we cant have a grenade and a boomerang
        if(BoomerangStats.IsBoomerang && GrenadeStats.IsGrenade)
            BoomerangStats.IsBoomerang = false;

        if(ChosenTarget == null)
            Blockable = true;

        Unit.Agent.Agent.enabled = false;
    }

    private void OnDestroy()
    {
        if(!Unit.Equals(null)) {
            Unit.Agent.Agent.enabled = true;
            if(AbilityControl)
                Unit.Stats.IsCastingAbility = false;
            StopStats();
        }
    }
    
    private void Update() {
        if(Unit.Equals(null) || !Unit.Stats.CanAct) {
            Destroy(gameObject);
            return;
        }
        Unit.Agent.Agent.transform.position = new Vector3(transform.position.x, Unit.Agent.Agent.transform.position.y, transform.position.z);
        Unit.Agent.transform.rotation = transform.rotation;
            
        if(ChosenTarget != null && !BoomerangStats.GoingBack) { //this is only used if the projectile was fired at a specified target. Must check if its a boomerang and already going back
            TargetLocation = ChosenTarget.Agent.transform.position;
            
            Vector3 direction = TargetLocation - transform.position;
            if(direction != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
            TargetLocation -= direction.normalized * (Unit.Agent.Agent.radius + ChosenTarget.Agent.radius);
        }
        if(LingeringStats.CurrentlyLingering) //if currently lingering
            LingeringStats.UpdateLingeringStats(gameObject);
        float SpeedReduction = 1; //a multiply used by boomerang projectiles to slow down near the end of flight
        if(BoomerangStats.IsBoomerang)
            SpeedReduction = BoomerangStats.SpeedReduction(gameObject, TargetLocation, LastKnownLocation);
        if(!LingeringStats.CurrentlyLingering || (LingeringStats.LingerDuringFlight && LingeringStats.IsInFlight)) { //if the projectile doesnt linger or lingers during flight
            if(Vector3.Distance(transform.position, TargetLocation) <= Radius || ( (Vector3.Distance(transform.position, LastKnownLocation) <= Radius) && BoomerangStats.IsBoomerang && BoomerangStats.GoingBack ) ){ //if the projectile is at the end of its flight
                if(GrenadeStats.IsGrenade)
                    GrenadeStats.Explode(gameObject);
                else if(SelfDestructStats.SelfDestructs)
                    SelfDestructStats.Explode(gameObject);
                bool tempGoingBack = BoomerangStats.GoingBack; //Used in case a projectile is boomerang and lingering at the end. We must save going back before changing it
                if(BoomerangStats.IsBoomerang && !BoomerangStats.GoingBack) {
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
                else {
                    //transform.position += transform.forward * Speed * SpeedReduction * Time.deltaTime;
                    //below takes more operations, but may be safer. This might be needed for projectile as well
                    transform.position += (TargetLocation - transform.position).normalized * Speed * SpeedReduction * Time.deltaTime;
                }
            }
        }
    }
}
