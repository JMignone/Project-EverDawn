using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class AnimatorStateController : MonoBehaviour
{
    [SerializeField]
    private Unit unitStats;
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private float animationAttackDelayStartTiming = 1f;
    
    private Animator animator;
    private bool attackTriggered = false;
    private bool previousIsCasting = false;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
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

        // check if casting ability state as changed
        if (previousIsCasting != unitStats.Stats.IsCastingAbility)
        {
            // !previousIsCasting is a faster way of checking unitStats.Stats.IsCastingAbility in this case
            if (!previousIsCasting == true)
            {
                animator.SetTrigger("Ability");
            }

            previousIsCasting = unitStats.Stats.IsCastingAbility;
        }
    }
}
