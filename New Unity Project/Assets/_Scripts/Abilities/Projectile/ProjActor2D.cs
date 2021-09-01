using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProjActor2D : MonoBehaviour
{
    //[SerializeField]
    //GameObject followTarget;
    [SerializeField]
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    /*
    private void LateUpdate()
    {
        if(followTarget != null)
        {
            transform.position = new Vector3(
                followTarget.transform.position.x, 
                followTarget.transform.position.y, 
                followTarget.transform.position.z
            );
            transform.rotation = followTarget.transform.rotation;
        }
    }*/
}
