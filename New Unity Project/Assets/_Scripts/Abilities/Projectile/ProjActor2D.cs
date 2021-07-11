using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProjActor2D : MonoBehaviour
{
    [SerializeField]
    GameObject followTarget;
    [SerializeField]
    Animator anim;
    [SerializeField]
    NavMeshAgent agent;
    [SerializeField]
    bool isFlying;

    private void Awake()
    {
        agent = followTarget.GetComponent<NavMeshAgent>();
    }

    private void LateUpdate()
    {
        if(followTarget != null)
        {
            transform.localPosition = new Vector3(
                followTarget.transform.localPosition.x, 
                followTarget.transform.localPosition.y, 
                followTarget.transform.localPosition.z
            );
            transform.rotation = followTarget.transform.rotation;
            transform.Rotate(0, 180, 0, Space.Self); // !! I feel like this could be a source of a problem in the future, if units are rotated a certain way when they are made. Note this !!
        }
    }
}
