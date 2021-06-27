using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameFunctions
{
    public static bool CanAttack(string playerTag, string enemyTag, Component damageable, BaseStats stats) {
        if(damageable) {
            if(playerTag != enemyTag) {
                bool objectAttackable = false;
                if(stats.ObjectAttackable == GameConstants.OBJECT_ATTACKABLE.BOTH) //If the unit can attack the flying or ground unit, continue
                    objectAttackable = true;
                else if(stats.ObjectAttackable == GameConstants.OBJECT_ATTACKABLE.GROUND && (damageable as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.GROUND)
                    objectAttackable = true;
                else if(stats.ObjectAttackable == GameConstants.OBJECT_ATTACKABLE.FLYING && (damageable as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
                    objectAttackable = true;
                if(objectAttackable) { //the inside of this if block tests if the units priority matches the unit, then return true
                    if(stats.AttackPriority == GameConstants.ATTACK_PRIORITY.EVERYTHING) //If the units priority is anything, return true
                        return true;
                    else if(stats.AttackPriority == GameConstants.ATTACK_PRIORITY.STRUCTURE && (damageable as IDamageable).Stats.UnitType == GameConstants.UNIT_TYPE.STRUCTURE)
                        return true;
                }
            }   
        }
        return false;
    }

    public static void Attack(Component damageable, float baseDamage) {
        if(damageable) {
            (damageable as IDamageable).TakeDamage(baseDamage);
        }
    }

    public static GameObject GetNearestTarget(List<GameObject> hitTargets, string tag, BaseStats stats) {
        if(hitTargets.Count > 0) {
            GameObject go = null; 
            Component targetComponent;
            SphereCollider targetSc;

            float dist = 10000; //Arbitrary large number

            foreach (GameObject hitTarget in hitTargets)
            {
                targetComponent = hitTarget.GetComponent(typeof(IDamageable));

                if(targetComponent) {
                    if(GameFunctions.CanAttack(tag, hitTarget.tag, targetComponent, stats)) {
                        targetSc = (targetComponent as IDamageable).Stats.DetectionObject;
                        float newDist = Vector3.Distance(stats.DetectionObject.transform.position, targetSc.transform.position);

                        if(dist > newDist) { //if we found a closer target
                            if(!hitTarget.CompareTag(tag)){ //and its not on the same team (sanity check, shouldnt ever occur)
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

    public static Transform GetCanvas() {
        return GameObject.Find(GameConstants.HUD_CANVAS).transform;
    }

    public static void SpawnUnit(GameObject prefab, Transform parent, Vector3 position) 
    {
        var targetPosition = position;
        targetPosition.z = 100; //What about the enemy player? Does this need to be -100?
        Vector3 direction = position - targetPosition;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        //This makes sure the unit is rotated the corect way

        GameObject go = GameObject.Instantiate(prefab, position, targetRotation, parent);
        GameManager.AddObjectToList(go);
    }


}
