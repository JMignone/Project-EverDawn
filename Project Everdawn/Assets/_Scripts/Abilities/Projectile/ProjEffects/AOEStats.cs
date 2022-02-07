using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AOEStats
{
    [SerializeField]
    private bool areaOfEffect;

    [SerializeField]
    private bool unitCentered;

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
        Collider[] colliders;
        Projectile projectile = go.GetComponent<Projectile>(); 

        if(unitCentered && projectile.ChosenTarget.Agent != null) { 
            Vector3 position = new Vector3(projectile.ChosenTarget.Agent.transform.position.x, 0, projectile.ChosenTarget.Agent.transform.position.z);

            GameObject damageZone = MonoBehaviour.Instantiate(explosionEffect, position, Quaternion.identity);
            damageZone.transform.localScale = new Vector3(explosionRadius*2, .1f, explosionRadius*2);

            colliders = Physics.OverlapSphere(position, explosionRadius);
        }
        else {
            Vector3 position = new Vector3(go.transform.position.x, 0, go.transform.position.z);

            GameObject damageZone = MonoBehaviour.Instantiate(explosionEffect, position, Quaternion.identity);
            damageZone.transform.localScale = new Vector3(explosionRadius*2, .1f, explosionRadius*2);

            colliders = Physics.OverlapSphere(position, explosionRadius);
        }

        foreach(Collider collider in colliders) {
            if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));

                if(GameFunctions.WillHit(projectile.HeightAttackable, projectile.TypeAttackable, damageable)) {
                    projectile.SetHit = true;

                    float damage = projectile.BaseDamage*projectile.DamageMultiplier;
                    if(damageable.GetComponent<Tower>())
                        damage = projectile.TowerDamage*projectile.DamageMultiplier;

                    GameFunctions.Attack(damageable, damage, projectile.CritStats);
                    projectile.ApplyAffects(damageable);
                }
            }
        }
    }
}