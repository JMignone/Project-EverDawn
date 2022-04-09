using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChainStats
{
    [Tooltip("If checked, the ability will try to find another target to fire at")]
    [SerializeField]
    private bool chains;

    [SerializeField]
    private float chainRadius;

    [SerializeField]
    private int chainCount;

    public bool Chains
    {
        get { return chains; }
    }

    public float ChainRadius
    {
        get { return chainRadius; }
    }

    public int ChainCount
    {
        get { return chainCount; }
    }

    public Actor3D FindTarget(GameObject go, GameObject enemyGo, IAbility ability, IDamageable unit) {
        if(!chains || chainCount == 0)
            return null;
        chainCount--;

        Vector3 position = (enemyGo.GetComponent(typeof(IDamageable)) as IDamageable).Agent.transform.position;
        Collider[] colliders = Physics.OverlapSphere(position, chainRadius);

        Actor3D enemyTarget = null;
        if(colliders.Length > 0) {
            float closestDistance = 9999;
            float distance;
            
            foreach(Collider collider in colliders) {
                if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                    Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                    //make sure we are not targeting ourselves
                    if(unit != null && !unit.Equals(null)) {
                        if((unit as Component).gameObject == damageable.gameObject)
                            continue;
                    }

                    //make sure we are not rebounding to the same object
                    if(enemyGo == damageable.gameObject)
                        continue;

                    if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, damageable)) {
                        distance = Vector3.Distance(damageable.transform.position, collider.transform.position);
                        if(distance < closestDistance) {
                            closestDistance = distance;
                            enemyTarget = (damageable as IDamageable).Agent;
                        }
                    }
                }
            }
        }

        return enemyTarget;
    }
}
