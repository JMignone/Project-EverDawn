using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeleportStats
{
    private IDamageable unit;

    [SerializeField]
    private bool isWarp;

    [SerializeField]
    private bool teleportsAllies;

    [SerializeField]
    private float allyRadius;

    [Tooltip("Determines how long allys teleported are unable to act")]
    [SerializeField]
    private float allyWarpSickness;

    [SerializeField]
    private GameObject explosionEffect;

    public bool IsWarp
    {
        get { return isWarp; }
    }

    public bool TeleportsAllies
    {
        get { return teleportsAllies; }
    }

    public float AllyRadius
    {
        get { return allyRadius; }
    }

    public void StartStats(IDamageable go) {
        unit = go;
    }

    public void Warp(GameObject go) {
        //Instantiate(explosionEffect, go.transform.position, go.transform.rotation);
        if(teleportsAllies) {
            Vector3 unitPosition = new Vector3(unit.Agent.transform.position.x, 0, unit.Agent.transform.position.z);
            Collider[] colliders = Physics.OverlapSphere(unitPosition, allyRadius);

            Component ability = go.GetComponent(typeof(IAbility));
            foreach(Collider collider in colliders) {
                //MonoBehaviour.print(collider);
                if(collider.CompareTag(go.tag) && collider.name == "Agent") {
                    Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                    if((damageable as Component).gameObject != (unit as Component).gameObject && GameFunctions.WillHit((ability as IAbility).HeightAttackable, (ability as IAbility).TypeAttackable, damageable)) {
                        Vector3 position = go.transform.position - (unit.Agent.transform.position - (damageable as IDamageable).Agent.transform.position);
                        //if the friendly unit is a flying unit
                        if((damageable as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING) 
                            (damageable as IDamageable).Agent.Agent.Warp(new Vector3(position.x, (damageable as IDamageable).Agent.transform.position.y, position.z));
                        else {
                            UnityEngine.AI.NavMeshHit hit;
                            if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 12f, 9))
                                position = hit.position;
                            (damageable as IDamageable).Agent.Agent.Warp(new Vector3(position.x, (damageable as IDamageable).Agent.transform.position.y, position.z));
                        }

                        //replace freeze with stun maybe later on
                        (damageable as IDamageable).Stats.EffectStats.FrozenStats.Freeze(allyWarpSickness);
                    }
                }
            }
        }
        unit.Agent.Agent.Warp(new Vector3(go.transform.position.x, unit.Agent.transform.position.y, go.transform.position.z));
    }
}
