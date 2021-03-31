using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private Actor3D agent;

    [SerializeField]
    private Actor2D unitSprite;

    [SerializeField]
    private GameObject target;

    public Actor3D Agent
    {
        get { return agent; }
        //set { agent = value; }
    }

    public Actor2D UnitSprite
    {
        get { return unitSprite; }
        //set { agent = value; }
    }

    public GameObject Target
    {
        get { return target; }
        set { target = value; }
    }

    private void Update()
    {
        if(target != null)
            agent.Agent.SetDestination(target.transform.position);
    }

}
