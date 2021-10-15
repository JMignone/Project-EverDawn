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

    [SerializeField] [Min(0)]
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

        foreach(Collider collider in colliders) {
            if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));

                if(GameFunctions.WillHit(projectile.HeightAttackable, projectile.TypeAttackable, damageable)) {

                    float damage = projectile.BaseDamage*projectile.DamageMultiplier;
                    if(damageable.GetComponent<Tower>())
                        damage = projectile.TowerDamage*projectile.DamageMultiplier;

                    GameFunctions.Attack(damageable, damage);
                    projectile.ApplyAffects(damageable);
                }
            }
        }
    }
}
