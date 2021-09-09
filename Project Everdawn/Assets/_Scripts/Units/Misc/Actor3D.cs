using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Actor3D : MonoBehaviour
{
    private NavMeshAgent agent;
    private SphereCollider hitBox;

    public NavMeshAgent Agent
    {
        get { return agent; }
        //set { agent = value; }
    }

    public SphereCollider HitBox
    {
        get { return hitBox; }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        hitBox = GetComponent<SphereCollider>();
    }
}
