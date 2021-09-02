using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AOEStats
{
    [SerializeField]
    private bool areaOfEffect;

    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField]
    private float explosionRadius;

    public bool AreaOfEffect
    {
        get { return areaOfEffect; }
    }

    public GameObject ExplosionEffect
    {
        get { return explosionEffect; }
    }

    public float ExplosionRadius
    {
        get { return explosionRadius; }
    }

    public void Explode(GameObject go) {
        //Instantiate(explosionEffect, go.transform.position, go.transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(go.transform.position, explosionRadius);
        Projectile projectile = go.GetComponent<Projectile>();
        float damage = projectile.BaseDamage;

        foreach(Collider collider in colliders) {
            if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                if(GameFunctions.WillHit(projectile.HeightAttackable, projectile.TypeAttackable, damageable)) {
                    GameFunctions.Attack(damageable, damage);
                    projectile.ApplyAffects(damageable);
                }
            }
        }
    }
}
