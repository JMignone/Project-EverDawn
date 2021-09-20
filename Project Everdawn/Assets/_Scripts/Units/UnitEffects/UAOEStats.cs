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

    public void StartStats(GameObject go) {
        unit = (go.GetComponent(typeof(IDamageable)) as IDamageable);
    }

    public void Explode(GameObject go, GameObject target, float damage) {
        //Instantiate(explosionEffect, go.transform.position, go.transform.rotation);
        Collider[] colliders;
        if(unitCentered)
            colliders = Physics.OverlapSphere(go.transform.GetChild(0).position, explosionRadius);
        else
            colliders = Physics.OverlapSphere(target.transform.GetChild(0).position, explosionRadius);

        foreach(Collider collider in colliders) {
            if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                if(GameFunctions.CanAttack(go.tag, damageable.tag, damageable.GetComponent(typeof(IDamageable)), unit.Stats)) {
                    GameFunctions.Attack(damageable, damage);
                    unit.Stats.ApplyAffects(damageable);
                }
            }
        }
    }
}
