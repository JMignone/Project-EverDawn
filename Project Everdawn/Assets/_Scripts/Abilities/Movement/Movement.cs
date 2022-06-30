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

    [Tooltip("If checked, any projectile targeting this unit will lose track at the start of the movement. This will hit a summoned unit if one exists")]
    [SerializeField]
    private bool avoidTargetedProjectiles;

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
        StartStats();
        
        TargetLocation = retreatStats.GetTargetLocation(Unit.Agent.transform.position, TargetLocation, passObstacles);

        LastKnownLocation = new Vector3(Unit.Agent.transform.position.x, 0, Unit.Agent.transform.position.z); //remembers the start location of a unit for boomerang effects

        Unit.Agent.Agent.enabled = false;

        if(hideUnit)
            Unit.Stats.UnitMaterials.MakeInvisible();
        
        if(avoidTargetedProjectiles) {
            foreach(GameObject go in Unit.Projectiles) {
                go.GetComponent<Projectile>().ChosenTarget = null;
                if(Caster.UnitSummon != null && !Caster.UnitSummon.Equals(null))
                    go.GetComponent<Projectile>().ChosenTarget = Caster.UnitSummon;
            }
            Unit.Projectiles.Clear();
        }
    }

    private void OnDestroy()
    {
        if(Unit != null && !Unit.Equals(null)) {
            Unit.Agent.Agent.enabled = true;
            if(hideUnit)
                Unit.Stats.UnitMaterials.MakeOpaque();
            StopStats();
        }
    }
    
    private bool targetReached;
    private void FixedUpdate() {
        
        if(Unit == null || Unit.Equals(null) || !Unit.Stats.CanAct) {
            Destroy(gameObject);
            return;
        }
        Unit.Agent.transform.position = new Vector3(transform.position.x, Unit.Agent.transform.position.y, transform.position.z);
        Unit.Agent.transform.rotation = transform.rotation;
        HitBox.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            
        if(ChosenTarget != null && !ChosenTarget.Equals(null) && !BoomerangStats.GoingBack) { //this is only used if the projectile was fired at a specified target. Must check if its a boomerang and already going back
            TargetLocation = ChosenTarget.Agent.transform.position;
            
            Vector3 direction = TargetLocation - transform.position;
            if(direction != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
            if(!landInsideTarget)
                TargetLocation -= direction.normalized * (Unit.Agent.Agent.radius + ChosenTarget.Agent.Agent.radius);
        }
        LocationStats.UpdateStats();
        if(LingeringStats.CurrentlyLingering) //if currently lingering
            LingeringStats.UpdateLingeringStats(gameObject);

        float SpeedReduction = 1; //a multiply used by boomerang projectiles to slow down near the end of flight
        if(BoomerangStats.IsBoomerang)
            SpeedReduction = BoomerangStats.SpeedReduction(gameObject, TargetLocation, LastKnownLocation);

        if(BoomerangStats.StartDelay && BoomerangStats.ReturnDelay > 0) { //StartDelay is only true when a boomerang reaches the first destination
            BoomerangStats.ReturnDelay -= Time.deltaTime;
            if(BoomerangStats.ReturnDelay <= 0)
                BoomerangStats.Explode(gameObject);
        }
        else if(!LingeringStats.CurrentlyLingering || (LingeringStats.LingerDuringFlight && LingeringStats.IsInFlight)) { //if the projectile isnt lingering or lingers during flight
            if(targetReached || (Vector3.Distance(transform.position, TargetLocation) <= Radius || ( (Vector3.Distance(transform.position, LastKnownLocation) <= Radius) && BoomerangStats.IsBoomerang && BoomerangStats.GoingBack ) ) ){ //if the projectile is at the end of its flight
                targetReached = true;
                NavMeshHit hit;
                if(!NavMesh.SamplePosition(Unit.Agent.transform.position, out hit, 1f, 9)) { //if the dashing unit ended up in an obstacle
                    //MonoBehaviour.print("TESTS");
                    if(retreatStats.Retreats) {
                        transform.position -= Time.deltaTime * Speed * SpeedReduction * transform.forward;
                        if(ChosenTarget != null && !ChosenTarget.Equals(null))
                            ChosenTarget.Agent.transform.position -= Time.deltaTime * Speed * SpeedReduction * transform.forward;
                    }
                    else {
                        transform.position += Time.deltaTime * Speed * SpeedReduction * transform.forward;
                        if(ChosenTarget != null && !ChosenTarget.Equals(null))
                            ChosenTarget.Agent.transform.position += Time.deltaTime * Speed * SpeedReduction * transform.forward;
                    }
                }
                else {
                    if(GrenadeStats.IsGrenade)
                        GrenadeStats.Explode(gameObject);
                    else if(SelfDestructStats.SelfDestructs)
                        SelfDestructStats.Explode(gameObject);

                    bool tempGoingBack = BoomerangStats.GoingBack; //Used in case a projectile is boomerang and lingering at the end. We must save going back before changing it
                    if(BoomerangStats.IsBoomerang && !BoomerangStats.GoingBack) //remember a proj CANNOT be both a grenade and a boomerang
                        BoomerangStats.SetBack(gameObject);

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
                        transform.position -= Time.deltaTime * Speed * SpeedReduction * transform.forward;
                        BoomerangStats.SpeedModifier += Time.deltaTime;
                    }
                    else if(CustomPathStats.HasCustomPath)
                        CustomPathStats.UpdateStats(TargetLocation);
                    else {
                        //transform.position += transform.forward * Speed * SpeedReduction * Time.deltaTime;
                        //below takes more operations, but may be safer as it accounts for the possiblity the projectile warps past its mark. This might be needed for projectile as well
                        transform.position += Time.deltaTime * Speed * SpeedReduction * (TargetLocation - transform.position).normalized;
                    }
                }
            }
        }
    }
}
