using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour, IDamageable
{
    [SerializeField]
    private BaseStats stats;
    [SerializeField]
    private List<GameObject> hitTargets;
    [SerializeField]
    GameObject target;
    [SerializeField]
    private int inRange;

    public BaseStats Stats {
        get { return stats; }
    }

    public List<GameObject> HitTargets {
        get { return hitTargets; }
    }

    public GameObject Target {
        get { return target; }
        set { target = value; }
    }

    public int InRange
    {
        get { return inRange; }
        set { inRange = value; }
    }

    private void Update()
    {
        if(stats.CurrHealth > 0) {
            stats.UpdateStats();
            Attack();
        }
    }

    void Attack() {
        if(target != null) {
            if(stats.CurrAttackDelay >= stats.AttackDelay) {
                Component damageable = target.GetComponent(typeof(IDamageable));

                if(damageable) { //is the target damageable
                    if(hitTargets.Contains(target)) {
                        if(GameFunctions.CanAttack(gameObject.tag, target.tag, damageable, stats)) {
                            GameFunctions.Attack(damageable, stats.BaseDamage);
                            stats.CurrAttackDelay = 0;
                        }
                    }
                }
            }
        }
    }
    
    public void OnTriggerEnter(Collider other) {
        if(!other.transform.parent.parent.gameObject.CompareTag(gameObject.tag)) {//checks to make sure the target isnt on the same team
            Component damageable = other.transform.parent.GetComponent(typeof(IDamageable));
            //Component damageable = other.transform.parent.parent.gameObject.GetComponent(typeof(IDamageable)); which one
            if(damageable) {
                if(!hitTargets.Contains(damageable.gameObject)) {
                    hitTargets.Add(damageable.gameObject);
                }
            }
        }
    }

    public void OnTriggerStay(Collider other) {
        if(!other.transform.parent.parent.gameObject.CompareTag(gameObject.tag)) {
            if(hitTargets.Count > 0) {
                //GameObject go = GameFunctions.GetNearestTarget(hitTargets, stats.DetectionObject, gameObject.tag, stats.Range);
                GameObject go = GameFunctions.GetNearestTarget(hitTargets, stats.DetectionObject, gameObject.tag, stats);
                if(go != null)
                    target = go;
            }
        }
    }

    void IDamageable.TakeDamage(float amount) {
        stats.CurrHealth -= amount;
    }

}
