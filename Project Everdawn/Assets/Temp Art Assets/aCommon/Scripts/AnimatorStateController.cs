using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class AnimatorStateController : MonoBehaviour
{
    [SerializeField]
    protected Unit unitStats;
    [SerializeField]
    protected NavMeshAgent agent;
    [SerializeField]
    protected float animationAttackDelayStartTiming = 1f;
    
    protected Animator animator;
    protected bool attackTriggered = false;

    protected void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (agent != null)
        {
            if (agent.velocity != new Vector3(0, 0, 0))
            {
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
        }

        // keep track of whether the attack animation has been triggered yet
        if (attackTriggered == false)
        {
            // set attack animation trigger when it reaches the correct time in the attack timer
            if (unitStats.Stats.CurrAttackDelay >= animationAttackDelayStartTiming)
            {
                animator.SetTrigger("Attack");

                attackTriggered = true;
            }
        }
        else
        {
            // reset attack triggered boolean when attack has finished
            if (unitStats.Stats.CurrAttackDelay <= unitStats.Stats.AttackChargeLimiter)
            {
                attackTriggered = false;
            }
        }
    }
}
