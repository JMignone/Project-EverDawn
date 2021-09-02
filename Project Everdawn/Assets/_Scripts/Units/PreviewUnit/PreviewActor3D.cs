using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PreviewActor3D : MonoBehaviour
{
    private NavMeshAgent agent;

    public NavMeshAgent Agent
    {
        get { return agent; }
        //set { agent = value; }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
}
