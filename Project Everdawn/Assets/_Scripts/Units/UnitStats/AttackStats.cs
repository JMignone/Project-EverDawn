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

    [Tooltip("Determines how a projectile behaves when fired from a unit.\nTarget: Makes the projectile seek its target, and stops when it reaches it.\nAttacks Location: Makes the projectiles fire at the location the target was when first engaged\nAttacks Past: Makes the projectile fire towards its target and will fly past if set to pierce.")]
    [SerializeField]
    private GameConstants.FIRING_TYPE attackType;
    
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

    //private int areaMask;

    public IDamageable Unit
    {
        get { return unit; }
    }

    public bool FiresProjectiles
    {
        get { return firesProjectiles; }
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

    public bool SameLocation
    {
        get { return continueType == GameConstants.CONTINUE_FIRING_TYPE.SAMELOCATION; }
    }

    public bool ReTargets
    {
        get { return continueType == GameConstants.CONTINUE_FIRING_TYPE.RETARGET; }
    }

    public bool AttacksLocation
    {
        get { return attackType == GameConstants.FIRING_TYPE.ATTACKSLOCATION; }
    }

    public bool AttacksPast
    {
        get { return attackType == GameConstants.FIRING_TYPE.ATTACKSPAST; }
    }

    public void StartAttackStats(IDamageable go) {
        unit = go;

        if(firesProjectiles) {
            float totalDamage = 0;
            foreach(GameObject ability in abilityPrefabs) {
                if(ability.GetComponent<Projectile>()){
                    Projectile proj = ability.GetComponent<Projectile>();
                    totalDamage += proj.BaseDamage;
                    totalDamage += proj.SelfDestructStats.ExplosionDamage;
                }
                else if(ability.GetComponent<CreateAtLocation>()) {
                    CreateAtLocation cal = ability.GetComponent<CreateAtLocation>();
                    totalDamage += cal.SelfDestructStats.ExplosionDamage;
                    totalDamage += cal.LinearStats.ExplosionDamage;
                }
            }
            unit.Stats.BaseDamage = totalDamage;
            //this is to assist with the killFlags, but still isnt perfect. What about a unit that has a super long string of projectiles that takes long to cast?
        }
            
    }

    public void BeginFiring() {
        isFiring = true;
        unit.Stats.IsFiring = true;
        currentProjectileIndex = 0;
        currentDelay = 0;
        unit.Stats.CurrAttackDelay = 0;
        target = (unit.Target.GetComponent(typeof(IDamageable)) as IDamageable);
        targetDied = false;
    }

    public void StopFiring() {
        isFiring = false;
        unit.Stats.IsFiring = false;
        currentProjectileIndex = 0;
        currentDelay = 0;
        target = null;
        targetDied = false;
    }
    
    public void Fire() {
        if(isFiring) {
            if(SameLocation && (target as Component) != null && unit.Target == (target as Component).gameObject) //tracks the position of the target such that it fires where it died
                lastTargetLocation = target.Agent.transform.position;
            if(!unit.Stats.CanAct || unit.Stats.IsCastingAbility || 
               ( ( (target as Component) == null || unit.Target != (target as Component).gameObject ) && currentProjectileIndex == 0 && !ReTargets ) ) { //if the unit is frozen or the target has died before the first shot is fired
                StopFiring();
                return;
            }
            else if( ((target as Component) == null || unit.Target != (target as Component).gameObject) && continueType == GameConstants.CONTINUE_FIRING_TYPE.NONE ) {
                StopFiring();
                return;
            }
            else if( ((target as Component) == null || unit.Target != (target as Component).gameObject) && SameLocation && targetDied == false) {
                targetDied = true;
                if(!AttacksLocation)
                    firstTargetLocation = lastTargetLocation;
            }
            else if( ((target as Component) == null || unit.Target != (target as Component).gameObject) && ReTargets) {
                if(unit.Target == null || !unit.InRangeTargets.Contains(unit.Target)) {
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
                unit.Stats.Appear((unit as Component).gameObject, unit.ShadowStats, unit.Agent); //if the unit is invisible, make it appear

                if(currentProjectileIndex == 0 && AttacksLocation && !targetDied) 
                    firstTargetLocation = target.Agent.transform.position;
                else if(currentProjectileIndex == 0 && AttacksLocation && targetDied)
                    firstTargetLocation = lastTargetLocation;

                Vector3 fireDirection;
                if(AttacksLocation)
                    fireDirection = firstTargetLocation - unit.Agent.transform.position;
                else if(targetDied)
                    fireDirection = lastTargetLocation - unit.Agent.transform.position;
                else
                    fireDirection = target.Agent.transform.position - unit.Agent.transform.position;

                if(AttacksLocation || (targetDied && SameLocation) ) { //if the unit fires at the location rather than the target itself
                    if(abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                        GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], unit.Agent.transform.position, firstTargetLocation, fireDirection, unit, (unit as Component).gameObject.tag, unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
                    else if(abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                        GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], unit.Agent.transform.position, firstTargetLocation, fireDirection, unit, (unit as Component).gameObject.tag, unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
                }
                else if(AttacksPast) {
                    if(abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                        GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], unit.Agent.transform.position, target.Agent.transform.position, fireDirection, unit, (unit as Component).gameObject.tag, unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
                    else if(abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                        GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], unit.Agent.transform.position, target.Agent.transform.position, fireDirection, unit, (unit as Component).gameObject.tag, unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
                }
                else {
                    if(abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                        GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], unit.Agent.transform.position, target.Agent, fireDirection, unit, (unit as Component).gameObject.tag, unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
                    else if(abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                        GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], unit.Agent.transform.position, target.Agent, fireDirection, unit, (unit as Component).gameObject.tag, unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
                }
                currentDelay = 0;
                currentProjectileIndex++;
            }
        }
    }

}
