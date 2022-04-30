using UnityEngine;
using UnityEngine.AI;

public class Actor2D : MonoBehaviour
{
    [SerializeField]
    GameObject followTarget;
    [SerializeField]
    Animator anim;
    //[SerializeField]
    NavMeshAgent agent;
    [SerializeField]
    GameObject ability;
    [SerializeField]
    bool isPreview;
    /*
    // added to support having an animation trigger (temporary, recommended future refactor) -Eagle
    private bool previousIsCastingAbility = false;
    private bool previousIsAttacking = false;
    */
    float offset;

    public Animator Animator
    {
        get { return anim; }
    }

    public GameObject Ability
    {
        get { return ability; }
    }

    private void Awake()
    {
        // following code commented out to allow animator to be a serialized field -Eagle
        /*
        if(GetComponent<Animator>()) {
            anim = GetComponent<Animator>();
            anim.SetBool("IsPreview", isPreview);
        }
        */
        if(followTarget.GetComponent<NavMeshAgent>()) {
            agent = followTarget.GetComponent<NavMeshAgent>();
            offset = -agent.baseOffset;
        }
    }
    /* removed for new animation code in AnimatorStateController -Eagle
    private void Update()
    {
        // new animation code -Eagle
        if (!isPreview && anim != null)
        {
            anim.SetBool("IsWalking", agent.velocity == Vector3.zero ? false : true);

            Component damageable = followTarget.transform.parent.GetComponent(typeof(IDamageable));
            Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update

            // restore this block when code is refactored to support individual attacks -Eagle
            //if (previousIsCastingAbility == false && (unit as IDamageable).Stats.IsCastingAbility == true)
            //{
            //    anim.SetTrigger("Ability");
            //}

            //if (previousIsAttacking == false && (unit as IDamageable).Stats.IsAttacking == true)
            //{
            //    anim.SetTrigger("Attack");
            //}
            

            // remove this block when code is refactored to support individual attacks -Eagle
            if ((unit as IDamageable).Stats.IsCastingAbility == true)
            {
                anim.SetTrigger("Ability");
            }
            if ((unit as IDamageable).Stats.IsAttacking == true)
            {
                anim.SetTrigger("Attack");
            }

            previousIsCastingAbility = (unit as IDamageable).Stats.IsCastingAbility;
            previousIsAttacking = (unit as IDamageable).Stats.IsAttacking;
        }
    }
    
    private void FixedUpdate()
    {
        if (!isPreview && anim != null)
        { //temporary so tower doesnt scream errors for lack of animation
            anim.SetBool("IsWalking", agent.velocity == Vector3.zero ? false : true);

            Component damageable = followTarget.transform.parent.GetComponent(typeof(IDamageable));
            Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update

            anim.SetBool("IsCasting", (unit as IDamageable).Stats.IsCastingAbility);
            anim.SetBool("IsAttacking", (((unit as IDamageable).InRangeTargets.Count > 0
                                        || (unit as IDamageable).Stats.IsAttacking
                                        || ((unit as IDamageable).Stats.CurrAttackDelay / (unit as IDamageable).Stats.AttackDelay >= (unit as IDamageable).Stats.AttackReadyPercentage && (unit as IDamageable).HitTargets.Contains((unit as IDamageable).Target)))
                && (unit as IDamageable).Stats.CanAct) ? true : false); //is in range, OR (is nearly done with attack and within vision)?

            //anim.SetBool("IsReady", ((unit as IDamageable).Stats.CurrAttackDelay/(unit as IDamageable).Stats.AttackDelay >= GameConstants.ATTACK_READY_PERCENTAGE && (unit as IDamageable).HitTargets.Contains((unit as IDamageable).Target)) ? true : false); //is nearly done with attack and within vision?
            
                //We may need a seperate value for all units, as their animations for attacking might take longer, even tho we may want them to attack at similar speeds.
                //This way they can properly do their entire attack animation
        }
    }
    */
    private void LateUpdate()
    {
        if (followTarget != null)
        {
            transform.localPosition = new Vector3(
                followTarget.transform.localPosition.x,
                offset,
                followTarget.transform.localPosition.z
            );
            transform.rotation = followTarget.transform.rotation;
            //transform.Rotate(0, 0, 0, Space.Self); // !! I feel like this could be a source of a problem in the future, if units are rotated a certain way when they are made. Note this !!
        }
    }

}
