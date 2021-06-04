using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Actor3D agent;

    [SerializeField]
    private Actor2D unitSprite;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private BaseStats stats;

    [SerializeField]
    private List<GameObject> hitTargets;

    public BaseStats Stats
    {
        get { return stats; }
        //set { stats = value; }
    }

    public Actor3D Agent
    {
        get { return agent; }
        //set { agent = value; }
    }

    public Actor2D UnitSprite
    {
        get { return unitSprite; }
        //set { unitSprite = value; }
    }

    public GameObject Target
    {
        get { return target; }
        set { target = value; }
    }

    public List<GameObject> HitTargets
    {
        get { return hitTargets; }
        //set { hitTargets = value; }
    }

    private void Update()
    {
        if(stats.CurrHealth > 0) {
            agent.Agent.speed = stats.MoveSpeed;
            stats.UpdateStats();

            if(target != null)
            agent.Agent.SetDestination(target.transform.position);
        }

    }

    void IDamageable.TakeDamage(float amount) {
        stats.CurrHealth -= amount;
    }
}
