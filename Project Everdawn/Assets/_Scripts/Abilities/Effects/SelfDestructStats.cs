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
            Collider[] colliders = Physics.OverlapSphere(go.transform.position, explosionRadius);
            Component ability = go.GetComponent(typeof(IAbility));

            foreach(Collider collider in colliders) {
                if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                    Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                    if(GameFunctions.WillHit((ability as IAbility).HeightAttackable, (ability as IAbility).TypeAttackable, damageable)) {
                        GameFunctions.Attack(damageable, explosionDamage);
                        (ability as IAbility).ApplyAffects(damageable);
                    }
                }
            }
        }
    }

}
