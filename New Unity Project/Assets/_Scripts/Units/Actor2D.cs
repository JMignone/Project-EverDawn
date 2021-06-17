using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Actor2D : MonoBehaviour
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
        anim = GetComponent<Animator>();
        if(agent != null) //temporary so tower doesnt scream errors for lack of animation
            agent = followTarget.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {   if(agent != null) { //temporary so tower doesnt scream errors for lack of animation
            if(!isFlying) {
                anim.SetBool("IsWalking", agent.velocity == Vector3.zero ? false : true);

                Component damageable = followTarget.transform.parent.GetComponent(typeof(IDamageable));
                Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update
                //print((unit as IDamageable).InRange);
                anim.SetBool("IsAttacking", (unit as IDamageable).InRange > 0 || ((unit as IDamageable).Stats.CurrAttackDelay/(unit as IDamageable).Stats.AttackDelay >= GameConstants.ATTACK_READY_PERCENTAGE && (unit as IDamageable).HitTargets.Contains((unit as IDamageable).Target)) ? true : false); //is in range, OR (is nearly done with attack and within vision)?
            }
        }
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
