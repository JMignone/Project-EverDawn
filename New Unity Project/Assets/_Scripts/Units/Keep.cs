using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keep : Tower
{
    /*
    private bool activated;

    void Awake() {
        activated = false;
    }
    */

    //Keeps MUST have their vision detection object set to its correct value in the prefab, otherwise we will likely need the function I commented out

    protected override void Update()
    {
        if(GameManager.isTowerActive(gameObject.tag, stats.PercentHealth)) {
            /*if(!activated) {
                target = GameFunctions.GetNearestTarget(hitTargets, gameObject.tag, stats);
                activated = true;
            }*/
            if(stats.CurrHealth > 0) {
                stats.UpdateStats(inRange, agent, hitTargets, target);
                Attack();

                if((inRange > 0 || stats.CurrAttackDelay/stats.AttackDelay >= GameConstants.ATTACK_READY_PERCENTAGE) && target != null) //is in range, OR is 90% thru attack cycle -
                    lookAtTarget();
                else 
                    resetToCenter();
            }
            else {
                print(gameObject.name + "has died!");
                GameManager.RemoveObjectsFromList(gameObject);
                Destroy(gameObject);
            }
        }
    }
}
