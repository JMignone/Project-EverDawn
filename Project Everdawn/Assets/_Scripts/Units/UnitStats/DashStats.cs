using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DashStats
{
    private IDamageable unit;
    private IDamageable chosenTarget;

    [Tooltip("Makes the unit dash towards its target")]
    [SerializeField]
    private bool dashes;
    private bool isDashing;
    private bool isMoving;

    [SerializeField] [Min(0)]
    private float dashDamage;

    [SerializeField] [Min(0)]
    private float towerDamage;

    [SerializeField] [Min(0)]
    private float dashSpeed;

    [SerializeField] [Min(0)]
    private float dashRange;

    [Tooltip("If true, the dasher will check after casting an ability and on spawning if a potential target is within a certain threshold where it can still dash despite not being close enough")]
    [SerializeField]
    private bool continousRange;
    [SerializeField] [Min(0)]
    private float innerRange;

    [Tooltip("Determines the amount of time before starting to dash")]
    [SerializeField] [Min(0)]
    private float dashDelay;
    private float currentDelay;

    [Tooltip("Makes the dash continue even if the target dies")]
    [SerializeField]
    private bool continueIfTargetDead;
    private Vector3 lastKnownLocation;
    private float targetRadius;

    [Tooltip("Makes the end of a dash an ability cast instead of normal damage")]
    [SerializeField]
    private bool StartWithAbility;

    [SerializeField]
    private List<GameObject> startPrefabs;

    [SerializeField]
    private List<float> startDelays;

    [Tooltip("Makes the end of a dash an ability cast instead of normal damage")]
    [SerializeField]
    private bool EndWithAbility;

    [SerializeField]
    private List<GameObject> endPrefabs;

    [SerializeField]
    private List<float> endDelays;
    private float abilityDelay;
    private int currentProjectileIndex;
    private string playerTag;
    private bool isStartFiring;
    private bool isEndFiring;
    private Vector3 fireDirection;

    public bool Dashes
    {
        get { return dashes; }
    }

    public bool IsDashing
    {
        get { return isDashing; }
    }

    public void StartDashStats(IDamageable go) {
        if(dashes) {
            unit = go;
            playerTag = (unit as Component).gameObject.tag;
        
            GameObject dashGo = new GameObject();
            dashGo.name = "DashDetectionObject";
            dashGo.tag = "Dash";

            SphereCollider dashBox = dashGo.AddComponent<SphereCollider>();
            dashBox.radius = dashRange;
            dashBox.center = new Vector3(0, 0, 0);
            dashBox.enabled = true;

            if(towerDamage == 0)
                towerDamage = dashDamage;

            dashGo.SetActive(true);
            dashGo.transform.SetParent(unit.UnitSprite.transform);
            dashGo.transform.localPosition = Vector3.zero;

            if(StartWithAbility) {
                startDelays.Add(dashDelay);
                dashDelay = 0;
            }
        }
    }

    public void UpdateDashStats() {
        if(dashes && isDashing) {
            if(!unit.Stats.CanAct || unit.Target == null || unit.Stats.IsCastingAbility)
                StopDash();
            else {
                if(!isMoving && !isStartFiring) {
                    if(Vector3.Distance(unit.Agent.transform.position, (unit.Target.GetComponent(typeof(IDamageable)) as IDamageable).Agent.transform.position) 
                    > dashRange + (unit.Target.GetComponent(typeof(IDamageable)) as IDamageable).Agent.HitBox.radius + 2.5f){
                        StopDash();
                        return;
                    }
                    if(currentDelay < dashDelay) 
                        currentDelay += Time.deltaTime * unit.Stats.EffectStats.SlowedStats.CurrentSlowIntensity;
                    else {
                        currentDelay = 0;
                        chosenTarget = (unit.Target.GetComponent(typeof(IDamageable)) as IDamageable);
                        lastKnownLocation = chosenTarget.Agent.transform.position;
                        targetRadius = chosenTarget.Agent.Agent.radius;
                        if(StartWithAbility)
                            isStartFiring = true;
                        else {
                            isMoving = true;
                            unit.Agent.Agent.enabled = false;
                        }
                    }
                }
                else if(isStartFiring)
                    FireStart();
                else if(isMoving && (!chosenTarget.Equals(null) || continueIfTargetDead) && !isEndFiring) {
                    if(!chosenTarget.Equals(null))
                        lastKnownLocation = chosenTarget.Agent.transform.position;
                    Vector3 direction = (lastKnownLocation - unit.Agent.transform.position).normalized;
                    unit.Agent.transform.position += Time.deltaTime * dashSpeed * direction;
                    if(Vector3.Distance(unit.Agent.transform.position, lastKnownLocation) < unit.Stats.Range + targetRadius) {
                        if(!chosenTarget.Equals(null))
                            DashAttack();
                        else
                            unit.Stats.Appear((unit as Component).gameObject, unit.ShadowStats, unit.Agent);
                        if(EndWithAbility) {
                            isEndFiring = true;
                            fireDirection = direction;
                        }
                        else {
                            if(!chosenTarget.Equals(null))
                                unit.SetTarget((chosenTarget as Component).gameObject);
                            StopDash();
                        }
                    }
                }
                else if(isEndFiring)
                    FireEnd();
                else
                    StopDash();
            }
        }
    }

    public void StartDash(GameObject go) {
        if(go == unit.Target && unit.Stats.CanAct && !unit.Stats.IsAttacking && !isDashing && !unit.Stats.IsCastingAbility && !unit.JumpStats.Jumping) {
            isDashing = true;
            unit.Agent.Agent.ResetPath();
        }
    }

    public void StopDash() {
        currentDelay = 0;
        isDashing = false;
        isMoving = false;
        isStartFiring = false;
        isEndFiring = false;
        unit.Agent.Agent.enabled = true;
        currentProjectileIndex = 0;
    }

    private void DashAttack() {
        float damage = dashDamage;
        if((chosenTarget as Component).gameObject.GetComponent<Tower>())
            damage = towerDamage;
        if(unit.Stats.EffectStats.AOEStats.AreaOfEffect)
            unit.Stats.EffectStats.AOEStats.Explode((unit as Component).gameObject, (chosenTarget as Component).gameObject, damage * unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
        else {
            GameFunctions.Attack((chosenTarget as Component), damage * unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity, unit.Stats.EffectStats.CritStats);
            unit.Stats.ApplyAffects((chosenTarget as Component));
        }
        unit.Stats.Appear((unit as Component).gameObject, unit.ShadowStats, unit.Agent);
    }

    public void checkDash() {
        if(continousRange) {
            GameObject closestTarget = null;
            float closestDist = 9999;
            float dist;

            Collider[] colliders = Physics.OverlapSphere(unit.Agent.transform.position, dashRange);
            foreach(Collider collider in colliders) {
                if(!collider.CompareTag((unit as Component).gameObject.tag) && collider.name == "Agent") {
                    Component damageableGo = collider.transform.parent.GetComponent(typeof(IDamageable));
                    IDamageable damageable = collider.transform.parent.GetComponent(typeof(IDamageable)) as IDamageable;
                    if(GameFunctions.CanAttack((unit as Component).gameObject.tag, damageableGo.tag, damageableGo, unit.Stats)) {
                        dist = Vector3.Distance(unit.Agent.transform.position, damageable.Agent.transform.position) - damageable.Agent.HitBox.radius;
                        if(dist < innerRange)
                            return;
                        if(dist >= innerRange && dist < dashRange) {
                            if(dist < closestDist) {
                                closestDist = dist;
                                closestTarget = damageableGo.gameObject;
                            }
                        }
                    }
                }
            }
            if(closestTarget != null) {
                unit.SetTarget(closestTarget);
                StartDash(closestTarget);
            }
        }
    }

    private void FireStart() {
        if(abilityDelay < startDelays[currentProjectileIndex]) //if we havnt reached the delay yet
            abilityDelay += Time.deltaTime;
        else { //if we completed a delay
            if(currentProjectileIndex < startDelays.Count - 1) {
                if(chosenTarget.Equals(null))
                    StopDash();
                else {
                    Vector3 direction = (lastKnownLocation - unit.Agent.transform.position).normalized;
                    if(startPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                        GameFunctions.FireProjectile(startPrefabs[currentProjectileIndex], unit.Agent.transform.position, chosenTarget, direction, null, playerTag, 1);
                    else if(startPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                        GameFunctions.FireCAL(startPrefabs[currentProjectileIndex], unit.Agent.transform.position, chosenTarget, direction, null, playerTag, 1);
                    abilityDelay = 0;
                    currentProjectileIndex++;
                }
            }
            else { //if we completed the last delay
                if(!chosenTarget.Equals(null) && Vector3.Distance(unit.Agent.transform.position, chosenTarget.Agent.transform.position) 
                > dashRange + chosenTarget.Agent.HitBox.radius + 2.5f){
                    StopDash();
                    return;
                }

                currentProjectileIndex = 0;
                abilityDelay = 0;
                isStartFiring = false;
                unit.Agent.Agent.enabled = false;
                isMoving = true;
            }
        }
    }

    private void FireEnd() {
        if(abilityDelay < endDelays[currentProjectileIndex]) //if we havnt reached the delay yet
            abilityDelay += Time.deltaTime;
        else { //if we completed a delay
            if(endPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                GameFunctions.FireProjectile(endPrefabs[currentProjectileIndex], unit.Agent.transform.position, unit.Agent.transform.position, fireDirection, null, playerTag, 1);
            else if(endPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                GameFunctions.FireCAL(endPrefabs[currentProjectileIndex], unit.Agent.transform.position, unit.Agent.transform.position, fireDirection, null, playerTag, 1);
            abilityDelay = 0;
            currentProjectileIndex++;
            if(currentProjectileIndex == endDelays.Count) { //if we completed the last delay
                if(!chosenTarget.Equals(null))
                    unit.SetTarget((chosenTarget as Component).gameObject);
                StopDash();
            }
        }
    }
}
