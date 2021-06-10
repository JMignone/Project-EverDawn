using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameFunctions
{
    public static bool CanAttack(string playerTag, string enemyTag, Component damageable, BaseStats stats) {
        if(damageable) {
            if(playerTag != enemyTag) {
                if(stats.ObjectAttackable == GameConstants.OBJECT_ATTACKABLE.BOTH)
                    return true;
                else if(stats.ObjectAttackable == GameConstants.OBJECT_ATTACKABLE.GROUND && (damageable as IDamageable).Stats.ObjectType == GameConstants.OBJECT_TYPE.GROUND)
                    return true;
                else if(stats.ObjectAttackable == GameConstants.OBJECT_ATTACKABLE.FLYING && (damageable as IDamageable).Stats.ObjectType == GameConstants.OBJECT_TYPE.FLYING)
                    return true;
            }   
        }
        return false;
    }

    public static void Attack(Component damageable, float baseDamage) {
        if(damageable) {
            (damageable as IDamageable).TakeDamage(baseDamage);
        }
    }
    /* !! His doesnt seem to take into account whether or not the unit can attack it, so a ground unit will end up following a flying unit but not attack it !!
    public static GameObject GetNearestTarget(List<GameObject> hitTargets, SphereCollider mySc, string tag, float range) {
        if(hitTargets.Count > 0) {
            GameObject go = hitTargets[0];

            Component targetComponent = hitTargets[0].GetComponent(typeof(IDamageable));
            SphereCollider targetSc = (targetComponent as IDamageable).Stats.DetectionObject;

            float dist = Vector3.Distance(mySc.transform.position, targetSc.transform.position);

            foreach (GameObject hitTarget in hitTargets)
            {
                targetComponent = hitTarget.GetComponent(typeof(IDamageable));

                if(targetComponent) {
                    targetSc = (targetComponent as IDamageable).Stats.DetectionObject;

                    float newDist = Vector3.Distance(mySc.transform.position, targetSc.transform.position);

                    if(dist > newDist && newDist <= range) {
                        if(!hitTarget.CompareTag(tag)){
                            dist = newDist;
                            go = hitTarget;
                        }
                    }
                }
            }
            return go;
        }
        return null;
    }
    */

    public static GameObject GetNearestTarget(List<GameObject> hitTargets, SphereCollider mySc, string tag, BaseStats stats) {
        if(hitTargets.Count > 0) {
            //GameObject go = hitTargets[0];

            GameObject go = null; //

            //Component targetComponent = hitTargets[0].GetComponent(typeof(IDamageable));
            //SphereCollider targetSc = (targetComponent as IDamageable).Stats.DetectionObject;
            Component targetComponent;
            SphereCollider targetSc;

            //float dist = Vector3.Distance(mySc.transform.position, targetSc.transform.position);
            float dist = 10000;

            foreach (GameObject hitTarget in hitTargets)
            {
                targetComponent = hitTarget.GetComponent(typeof(IDamageable));

                if(targetComponent) {
                    if(GameFunctions.CanAttack(tag, hitTarget.tag, targetComponent, stats)) {//

                        targetSc = (targetComponent as IDamageable).Stats.DetectionObject;

                        float newDist = Vector3.Distance(mySc.transform.position, targetSc.transform.position);

                        //if(dist > newDist && newDist <= stats.Range) {
                        if(dist > newDist) {
                            if(!hitTarget.CompareTag(tag)){
                                dist = newDist;
                                go = hitTarget;
                            }
                        }
                    }
                }
            }
            return go;
        }
        return null;
    }


}
