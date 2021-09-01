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
        get { return agent.transform.position; }
    }

    public void StartKnockbackStats(GameObject go) {
        Component component = go.GetComponent(typeof(IDamageable));
        agent = (component as IDamageable).Agent;
    }
}
