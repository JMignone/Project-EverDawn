using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keep : Tower
{
    protected override void Update()
    {
        if(GameManager.isTowerActive(gameObject.tag, stats.PercentHealth)) {
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
                GameManager.RemoveObjectsFromList(gameObject, false, true);
                Destroy(gameObject);
            }
        }
    }
}
