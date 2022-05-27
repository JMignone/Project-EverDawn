using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelfDestructStats
{
    [Tooltip("If checked, the ability explode when getting destroyed")]
    [SerializeField]
    private bool selfDestructs;

    [Tooltip("If checked, the damage will occur at the target, not where the ability is")]
    [SerializeField]
    private bool onTarget;

    [Tooltip("If checked, effects will not be applied")]
    [SerializeField]
    private bool noAffects;

    [Tooltip("If checked, the ability will not explode if no units were hit")]
    [SerializeField]
    private bool onlyExplodeOnHit;

    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField] [Min(0)]
    private float explosionDamage;

    [SerializeField] [Min(0)]
    private float towerDamage;

    [SerializeField] [Min(0)]
    private float explosionRadius;

    [SerializeField] [Min(0)]
    private float explosionTimer;
    private bool startExplosion;

    public bool SelfDestructs
    {
        get { return selfDestructs; }
    }

    public GameObject ExplosionEffect
    {
        get { return explosionEffect; }
    }

    public float ExplosionDamage
    {
        get { return explosionDamage; }
    }

    public float ExplosionRadius
    {
        get { return explosionRadius; }
        set { explosionRadius = value; }
    }

    public bool StartExplosion
    {
        get { return startExplosion; }
    }

    public float ExplosionTimer
    {
        set { explosionTimer = value; }
    }

    public void Explode(GameObject go, List<Collider> customColliders = null) {
        if(onlyExplodeOnHit && !(go.GetComponent(typeof(IAbility)) as IAbility).SetHit)
            return;

        startExplosion = true;
        if(explosionTimer > 0)
            explosionTimer -= Time.deltaTime;
        else {
            Vector3 position = Vector3.zero;

            IAbility ability = (go.GetComponent(typeof(IAbility)) as IAbility);
            if(onTarget)
                position = ability.TargetLocation;
            else
                position = new Vector3(go.transform.position.x, 0, go.transform.position.z);
            
            GameObject damageZone = MonoBehaviour.Instantiate(explosionEffect, position, go.transform.rotation);
            //if(damageZone.GetComponent<SphereCollider>())
                damageZone.transform.localScale = new Vector3(explosionRadius*2, .1f, explosionRadius*2);

            if(explosionDamage > 0) { //have this here incase we want to have a selfdestruct effect but just for show, perhaps for a lingering damage projectile
                if(customColliders == null) {
                    Collider[] colliders = Physics.OverlapSphere(position, explosionRadius);

                    foreach(Collider collider in colliders) {
                        if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                            Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));

                            if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, damageable)) {
                                ability.SetHit = true;

                                float damage = explosionDamage*ability.DamageMultiplier;
                                if(towerDamage > 0 && damageable.GetComponent<Tower>())
                                    damage = towerDamage*ability.DamageMultiplier;

                                GameFunctions.Attack(damageable, damage, ability.CritStats);
                                if(!noAffects)
                                    ability.ApplyAffects(damageable);
                            }
                        }
                    }
                }
                else {
                    foreach(Collider collider in customColliders) {
                        if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                            Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));

                            if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, damageable)) {
                                ability.SetHit = true;

                                float damage = explosionDamage*ability.DamageMultiplier;
                                if(towerDamage > 0 && damageable.GetComponent<Tower>())
                                    damage = towerDamage*ability.DamageMultiplier;

                                GameFunctions.Attack(damageable, damage, ability.CritStats);
                                if(!noAffects)
                                    ability.ApplyAffects(damageable);
                            }
                        }
                    }
                }
            }
            startExplosion = false;
        }
    }

}
