using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Actor3D : MonoBehaviour
{
    private NavMeshAgent agent;
    private SphereCollider hitBox;

    private IDamageable unit;

    public NavMeshAgent Agent
    {
        get { return agent; }
        //set { agent = value; }
    }

    public SphereCollider HitBox
    {
        get { return hitBox; }
    }

    public IDamageable Unit
    {
        get { return unit; }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        hitBox = GetComponent<SphereCollider>();
        unit = (transform.parent.GetComponent<IDamageable>() as IDamageable);
    }
}
