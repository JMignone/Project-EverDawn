using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CoPilotStats
{
    private IDamageable unit;

    [Tooltip("Makes the unit have a 'coPilot' firing independently from the unit")]
    [SerializeField]
    private bool hasCoPilot;

    [SerializeField] [Min(0)]
    private float range;

    [SerializeField] [Min(0)]
    private float delay;
    private float currDelay;

    //[SerializeField] [Min(1)]
    //private float numberOfTargets;

    private bool isFiring;
    private IDamageable chosenTarget;

    [SerializeField]
    private List<GameObject> abilityPrefabs;
    private IAbility testComponent; //needs to check what an ability can hit, uses the first to represent all of them

    [Tooltip("Determines the amount of time waited before firing each shot. Number of delays must be 1 more than the number of projectiles")]
    [SerializeField]
    private List<float> abilityDelays;
    private float abilityDelay;
    private int currentProjectileIndex;
    private string playerTag;

    public void StartStats(IDamageable go) {
        if(hasCoPilot) {
            unit = go;
            testComponent = abilityPrefabs[0].GetComponent(typeof(IAbility)) as IAbility;
            playerTag = (unit as Component).gameObject.tag;
        }
    }

    public void UpdateStats() {
        if(hasCoPilot) {
            if(unit.Stats.CanAct && !unit.Stats.IsCastingAbility) {
                if(isFiring)
                    Fire();
                else if(currDelay < delay)
                    currDelay += Time.deltaTime;
                else {
                    Collider[] colliders = Physics.OverlapSphere(unit.Agent.transform.position, range);

                    IDamageable closestTarget = null;
                    if(colliders.Length > 0) {
                        float closestDistance = 9999;
                        float distance;
                        foreach(Collider collider in colliders) {
                            if(!collider.CompareTag((testComponent as Component).gameObject.tag) && collider.name == "Agent") {
                                Component enemyUnit = collider.transform.parent.GetComponent(typeof(IDamageable));

                                if(GameFunctions.WillHit(testComponent.HeightAttackable, testComponent.TypeAttackable, enemyUnit)) {
                                    distance = Vector3.Distance(unit.Agent.transform.position, collider.transform.position);
                                    if(distance < closestDistance) {
                                        closestDistance = distance;
                                        closestTarget = (enemyUnit as IDamageable);
                                    }
                                }
                            }
                        }
                    }
                    if(closestTarget != null) {
                        chosenTarget = closestTarget;
                        isFiring = true;

                        Debug.Log(closestTarget);
                    }
                    currDelay = 0;
                }
            }
            else {
                currentProjectileIndex = 0;
                isFiring = false;
            }
        }
    }

    private void Fire() {
        if(abilityDelay < abilityDelays[currentProjectileIndex]) //if we havnt reached the delay yet
            abilityDelay += Time.deltaTime;
        else { //if we completed a delay
            abilityDelay = 0;
            if(currentProjectileIndex < abilityDelays.Count - 1) {
                if(chosenTarget.Equals(null)) {
                    currentProjectileIndex = 0;
                    isFiring = false;
                }
                else {
                    Vector3 direction = (chosenTarget.Agent.transform.position - unit.Agent.transform.position).normalized;
                    if(abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                        GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], unit.Agent.transform.position, chosenTarget, direction, unit, playerTag, 1);
                    else if(abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                        GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], unit.Agent.transform.position, chosenTarget, direction, unit, playerTag, 1);
                    currentProjectileIndex++;
                }
            }
            else { //if we completed the last delay
                currentProjectileIndex = 0;
                isFiring = false;
            }
        }
    }
}
