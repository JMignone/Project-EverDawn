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
    private Actor3D target;

    [SerializeField]
    private List<GameObject> abilityPrefabs;
    private IAbility testComponent; //needs to check what an ability can hit, uses the first to represent all of them

    [Tooltip("Determines the amount of time waited before firing each shot. Number of delays must be 1 more than the number of projectiles")]
    [SerializeField]
    private List<float> abilityDelays;

    public void StartStats(IDamageable go) {
        if(hasCoPilot) {
            unit = go;
            testComponent = abilityPrefabs[0].GetComponent(typeof(IAbility)) as IAbility;
        }
    }

    public void UpdateStats() {
        if(hasCoPilot) {
            if(currDelay < delay)
                currDelay += Time.deltaTime;
            else {
                Collider[] colliders = Physics.OverlapSphere(unit.Agent.transform.position, range);

                Actor3D closestTarget = null;
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
                                    closestTarget = collider.gameObject.GetComponent<Actor3D>();
                                }
                            }
                        }
                    }
                }
                if(closestTarget != null) {
                    target = closestTarget;
                    isFiring = true;

                    Debug.Log(closestTarget);
                }
            }
        }
    }
}
