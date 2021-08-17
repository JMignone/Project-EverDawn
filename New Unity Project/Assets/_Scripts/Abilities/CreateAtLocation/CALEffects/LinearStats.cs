using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LinearStats
{
    [SerializeField]
    private bool isLinear;

    [SerializeField]
    private bool isVertical;

    [SerializeField]
    private bool isHorizontal;

    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField]
    private float explosionDamage;

    [SerializeField]
    private float explosionWidth;

    public bool IsLinear
    {
        get { return isLinear; }
    }

    public bool IsVertical
    {
        get { return isVertical; }
    }

    public bool IsHorizontal
    {
        get { return isHorizontal; }
    }

    public GameObject ExplosionEffect
    {
        get { return explosionEffect; }
    }

    public float ExplosionDamage
    {
        get { return explosionDamage; }
    }

    public float ExplosionWidth
    {
        get { return explosionWidth; }
        set { explosionWidth = value; }
    }

    public void StartLinearStats() {
        if(!isVertical && !isHorizontal) //if neither option was selected, just default vertical
            isVertical = true;
    }

    public void Explode(GameObject go) {
        //Instantiate(explosionEffect, go.transform.position, go.transform.rotation);
        if(explosionDamage > 0) { //have this here incase we want to have a selfdestruct effect but just for show, perhaps for a lingering damage projectile
            Component ability = go.GetComponent(typeof(IAbility));
            if(isVertical) {
                Collider[] collidersVert = Physics.OverlapBox(go.transform.position, new Vector3(explosionWidth/2, .5f, GameManager.Instance.Ground.transform.localScale.z * 10));
                foreach(Collider collider in collidersVert) {
                    if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                        Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                        if(GameFunctions.WillHit((ability as IAbility).ObjectAttackable, damageable)) {
                            GameFunctions.Attack(damageable, explosionDamage);
                            (ability as IAbility).ApplyAffects(damageable);
                        }
                    }
                }
            }
            if(isHorizontal) {
                Collider[] collidersHorz = Physics.OverlapBox(go.transform.position, new Vector3(GameManager.Instance.Ground.transform.localScale.x * 10, .5f, explosionWidth/2));
                foreach(Collider collider in collidersHorz) {
                    if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                        Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                        if(GameFunctions.WillHit((ability as IAbility).ObjectAttackable, damageable)) {
                            GameFunctions.Attack(damageable, explosionDamage);
                            (ability as IAbility).ApplyAffects(damageable);
                        }
                    }
                }
            }
        }
    }

}
