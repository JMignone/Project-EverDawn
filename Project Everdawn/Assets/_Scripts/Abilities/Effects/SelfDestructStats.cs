using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelfDestructStats
{
    [SerializeField]
    private bool selfDestructs;

    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField] [Min(0)]
    private float explosionDamage;

    [SerializeField] [Min(0)]
    private float towerDamage;

    [SerializeField] [Min(0)]
    private float explosionRadius;

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

    public void Explode(GameObject go) {
        //Instantiate(explosionEffect, go.transform.position, go.transform.rotation);
        if(explosionDamage > 0) { //have this here incase we want to have a selfdestruct effect but just for show, perhaps for a lingering damage projectile
            Vector3 position = new Vector3(go.transform.position.x, 0, go.transform.position.z);
            Collider[] colliders = Physics.OverlapSphere(position, explosionRadius);
            Component ability = go.GetComponent(typeof(IAbility));

            foreach(Collider collider in colliders) {
                if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                    Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));

                    if(GameFunctions.WillHit((ability as IAbility).HeightAttackable, (ability as IAbility).TypeAttackable, damageable)) {
                        (ability as IAbility).SetHit = true;

                        float damage = explosionDamage*(ability as IAbility).DamageMultiplier;
                        if(towerDamage > 0 && damageable.GetComponent<Tower>())
                            damage = towerDamage*(ability as IAbility).DamageMultiplier;

                        GameFunctions.Attack(damageable, damage);
                        (ability as IAbility).ApplyAffects(damageable);
                    }
                }
            }
        }
    }

}
