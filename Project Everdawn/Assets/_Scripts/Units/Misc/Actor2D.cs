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
    bool isPreview;

    public Animator Animator
    {
        get { return anim; }
    }

    private void Awake()
    {
        if(anim != null) {
            anim = GetComponent<Animator>();
            anim.SetBool("IsPreview", isPreview);
        }
        if(agent != null) //temporary so tower doesnt scream errors for lack of animation
            agent = followTarget.GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if(!isPreview && agent != null) { //temporary so tower doesnt scream errors for lack of animation
            anim.SetBool("IsWalking", agent.velocity == Vector3.zero ? false : true);

            Component damageable = followTarget.transform.parent.GetComponent(typeof(IDamageable));
            Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update

            anim.SetBool("IsCasting", (unit as IDamageable).Stats.IsCastingAbility);
            anim.SetBool("IsAttacking", ( ((unit as IDamageable).InRangeTargets.Count > 0
                                        || (unit as IDamageable).Stats.IsAttacking
                                        || ( (unit as IDamageable).Stats.CurrAttackDelay/(unit as IDamageable).Stats.AttackDelay >= GameConstants.ATTACK_READY_PERCENTAGE && (unit as IDamageable).HitTargets.Contains((unit as IDamageable).Target) ) ) 
                && (unit as IDamageable).Stats.CanAct ) ? true : false); //is in range, OR (is nearly done with attack and within vision)?
                
            //anim.SetBool("IsReady", ((unit as IDamageable).Stats.CurrAttackDelay/(unit as IDamageable).Stats.AttackDelay >= GameConstants.ATTACK_READY_PERCENTAGE && (unit as IDamageable).HitTargets.Contains((unit as IDamageable).Target)) ? true : false); //is nearly done with attack and within vision?
            /*
                We may need a seperate value for all units, as their animations for attacking might take longer, even tho we may want them to attack at similar speeds.
                This way they can properly do their entire attack animation
            */
        }
    }

    private void LateUpdate() {
        if(followTarget != null) {
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
