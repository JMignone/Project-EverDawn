using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackStats
{
    private IDamageable unit;

    [Tooltip("Makes the unit fire projectiles rather than simply dealing damage")]
    [SerializeField]
    private bool firesProjectiles;

    [Tooltip("Makes the unit fire projectiles to the last location the target was before firing the first projectile. Has no effect if 'firesProjectiles' is unchecked.")]
    [SerializeField]
    private bool attacksLocation;
    private Vector3 firstTargetLocation;
    private Vector3 lastTargetLocation;

    [Tooltip("What a unit should do in the event its target dies while mid firing a volley of projectiles")]
    [SerializeField]
    private GameConstants.CONTINUE_FIRING_TYPE continueType;

    [SerializeField]
    private List<GameObject> abilityPrefabs;

    [Tooltip("Determines the amount of time waited before firing each shot. Number of delays must be 1 more than the number of projectiles")]
    [SerializeField]
    private List<float> abilityDelays;

    [SerializeField]
    private float currentDelay;

    private bool isFiring;
    private int currentProjectileIndex;
    private IDamageable target;
    private bool targetDied;

    private int areaMask;

    public IDamageable Unit
    {
        get { return unit; }
    }

    public bool FiresProjectiles
    {
        get { return firesProjectiles; }
    }

    public bool AttacksLocation
    {
        get { return attacksLocation; }
    }

    public bool IsFiring
    {
        get { return isFiring; }
        set { isFiring = value; }
    }

    public int CurrentProjectileIndex
    {
        get { return currentProjectileIndex; }
        set { currentProjectileIndex = value; }
    }

    public IDamageable Target
    {
        get { return target; }
    }

    public void StartAttackStats(GameObject go) {
        unit = (go.GetComponent(typeof(IDamageable)) as IDamageable);
    }
    /*
    public bool CanRetarget() {
        if(isFiring && continueType != GameConstants.CONTINUE_FIRING_TYPE.RETARGET)
            return false;
        return true;
    }
    */
    public void BeginFiring() {
        isFiring = true;
        unit.Stats.IsAttacking = true;
        currentProjectileIndex = 0;
        currentDelay = 0;
        target = (unit.Target.GetComponent(typeof(IDamageable)) as IDamageable);
        targetDied = false;
    }

    public void StopFiring() {
        isFiring = false;
        unit.Stats.IsAttacking = false;
        currentProjectileIndex = 0;
        currentDelay = 0;
        target = null;
        targetDied = false;
    }
    
    public void Fire() {
        if(isFiring) {
            if(continueType == GameConstants.CONTINUE_FIRING_TYPE.SAMELOCATION && (target as Component) != null && unit.Target == (target as Component).gameObject)
                lastTargetLocation = target.Agent.Agent.transform.position;
            if(!unit.Stats.CanAct) {
                MonoBehaviour.print("1");
                StopFiring();
                return;
            }
            else if( ((target as Component) == null || unit.Target != (target as Component).gameObject) && continueType == GameConstants.CONTINUE_FIRING_TYPE.NONE ) {
                MonoBehaviour.print("2");
                StopFiring();
                return;
            }
            else if( ((target as Component) == null || unit.Target != (target as Component).gameObject) && continueType == GameConstants.CONTINUE_FIRING_TYPE.SAMELOCATION && targetDied == false) {
                MonoBehaviour.print("3");
                targetDied = true;
                if(!attacksLocation)
                    firstTargetLocation = lastTargetLocation;
            }
            else if( ((target as Component) == null || unit.Target != (target as Component).gameObject) && continueType == GameConstants.CONTINUE_FIRING_TYPE.RETARGET) {
                MonoBehaviour.print("4");
                if(unit.Target == null || !unit.InRangeTargets.Contains(unit.Target)) {
                    MonoBehaviour.print("4.5");
                    StopFiring();
                    return;
                }
                else
                    target = (unit.Target.GetComponent(typeof(IDamageable)) as IDamageable);
            }
            if(currentDelay < abilityDelays[currentProjectileIndex]) //if we havnt reached the delay yet
                currentDelay += Time.deltaTime * unit.Stats.EffectStats.SlowedStats.CurrentSlowIntensity;
            else if(currentProjectileIndex == abilityPrefabs.Count) //if we completed the last delay
                StopFiring();
            else { //if we completed a delay
                if(currentProjectileIndex == 0 && attacksLocation && !targetDied) 
                    firstTargetLocation = target.Agent.Agent.transform.position;
                else if(currentProjectileIndex == 0 && attacksLocation && targetDied)
                    firstTargetLocation = lastTargetLocation;

                Vector3 fireDirection;
                if(attacksLocation)
                    fireDirection = unit.Agent.Agent.transform.position - firstTargetLocation;
                else if(targetDied)
                    fireDirection = unit.Agent.Agent.transform.position - lastTargetLocation;
                else
                    fireDirection = unit.Agent.Agent.transform.position - target.Agent.Agent.transform.position;

                if(attacksLocation || (targetDied && continueType == GameConstants.CONTINUE_FIRING_TYPE.SAMELOCATION) ) { //if the unit fires at the location rather than the target itself
                    if(abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                        GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], unit.Agent.Agent.transform.position, firstTargetLocation, fireDirection, unit);
                    else if(abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                        GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], unit.Agent.Agent.transform.position, firstTargetLocation, fireDirection, unit);
                }
                else {
                    if(abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                        GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], unit.Agent.Agent.transform.position, target.Agent, fireDirection, unit);
                    else if(abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                        GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], unit.Agent.Agent.transform.position, target.Agent, fireDirection, unit);
                }
                currentDelay = 0;
                currentProjectileIndex++;
            }
        }
    }

}
