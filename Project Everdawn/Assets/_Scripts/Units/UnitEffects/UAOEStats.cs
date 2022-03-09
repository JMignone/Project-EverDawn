using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UAOEStats
{
    [SerializeField]
    private bool areaOfEffect;

    [SerializeField]
    private bool unitCentered;

    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField]
    private float explosionRadius;

    private IDamageable unit;

    public bool AreaOfEffect
    {
        get { return areaOfEffect; }
    }

    public bool UnitCentered
    {
        get { return unitCentered; }
    }

    public GameObject ExplosionEffect
    {
        get { return explosionEffect; }
    }

    public float ExplosionRadius
    {
        get { return explosionRadius; }
    }

    public void StartStats(IDamageable go) {
        unit = go;
    }

    public void Explode(GameObject go, GameObject target, float damage) {

        Collider[] colliders;
        if(unitCentered) { 
            Vector3 position = new Vector3(go.transform.GetChild(0).position.x, 0, go.transform.GetChild(0).position.z);

            GameObject damageZone = MonoBehaviour.Instantiate(explosionEffect, position, Quaternion.identity);
            damageZone.transform.localScale = new Vector3(explosionRadius*2, .1f, explosionRadius*2);

            colliders = Physics.OverlapSphere(position, explosionRadius);
        }
        else {
            Vector3 position = new Vector3(target.transform.GetChild(0).position.x, 0, target.transform.GetChild(0).position.z);

            GameObject damageZone = MonoBehaviour.Instantiate(explosionEffect, position, Quaternion.identity);
            damageZone.transform.localScale = new Vector3(explosionRadius*2, .1f, explosionRadius*2);

            colliders = Physics.OverlapSphere(position, explosionRadius);
        }
        foreach(Collider collider in colliders) {
            if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                //if(GameFunctions.CanAttack(go.tag, damageable.tag, damageable.GetComponent(typeof(IDamageable)), unit.Stats)) {
                if(GameFunctions.WillHit(unit.Stats.HeightAttackable, GameConstants.TYPE_ATTACKABLE.BOTH, damageable.GetComponent(typeof(IDamageable)) )) {
                    GameFunctions.Attack(damageable, damage, unit.Stats.EffectStats.CritStats);
                    //unit.Stats.ApplyAffects(damageable);
                    //GameManager.ApplyAffects(damageable, unit.Stats.EffectStats);
                    unit.ApplyEffectsComponents.Add(damageable);
                }
            }
        }
    }
}