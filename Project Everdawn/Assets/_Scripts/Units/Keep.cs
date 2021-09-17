using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keep : Tower
{
    protected override void Update()
    {
        if(GameManager.isTowerActive(gameObject.tag, stats.PercentHealth)) {
            if(stats.CurrHealth > 0) {
                if((target == null || InRangeTargets.Count == 0) && stats.CanAct) { //if the target is null, we must find the closest target in hit targets. If hit targets is empty or failed, find the closest tower
                    if(hitTargets.Count > 0) {
                        GameObject go = GameFunctions.GetNearestTarget(hitTargets, gameObject.tag, stats);
                        if(go != null)
                            SetTarget(go);
                    }
                }

                stats.UpdateStats(true, InRangeTargets.Count, agent, hitTargets, target);
                Attack();

                if(stats.CanAct) { //if its stunend, we want to keep the tower looking in the same direction
                    if((InRangeTargets.Count > 0 || stats.CurrAttackDelay/stats.AttackDelay >= GameConstants.ATTACK_READY_PERCENTAGE) && target != null) //is in range, OR is 90% thru attack cycle -
                        lookAtTarget();
                    else 
                        resetToCenter();
                }
            }
            else {
                print(gameObject.name + "has died!");
                GameManager.RemoveObjectsFromList(gameObject, false, true);
                Destroy(gameObject);
            }
        }
    }
}
