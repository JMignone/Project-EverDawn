using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UKnockbackStats
{
    [SerializeField]
    private bool canKnockback;

    [SerializeField]
    private float knockbackDuration;

    [SerializeField]
    private float initialSpeed;

    private Actor3D agent;

    public bool CanKnockback
    {
        get { return canKnockback; }
    }

    public float KnockbackDuration
    {
        get { return knockbackDuration; }
    }

    public float InitialSpeed
    {
        get { return initialSpeed; }
    }

    public Vector3 UnitPosition
    {
        get { return new Vector3(agent.transform.position.x, 0, agent.transform.position.z); }
    }

    public void StartKnockbackStats(IDamageable go) {
        agent = go.Agent;
    }
}
