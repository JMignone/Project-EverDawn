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

    public static void FireProjectile(GameObject prefab, Vector3 startPosition, Vector3 mousePosition, Vector3 direction, Unit unit) {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float distance = Vector3.Distance(startPosition, mousePosition);
        Vector3 endPosition = mousePosition;
        Projectile projectile = prefab.GetComponent<Projectile>();

        float range = projectile.Range;
        float radius = projectile.Radius;
        bool isGrenade = projectile.GrenadeStats.IsGrenade;
        bool selfDestructs = projectile.SelfDestructStats.SelfDestructs;
        if(distance > range)
            endPosition = startPosition + (direction.normalized * range);
        else if(distance < range && !isGrenade && !selfDestructs)
            endPosition = startPosition + (direction.normalized * range);
        startPosition += direction.normalized * radius;
        GameObject go = GameObject.Instantiate(prefab, startPosition, targetRotation, GameManager.GetUnitsFolder());
        //go.GetComponent<Projectile>().Agent.Agent.SetDestination(endPosition);
        go.GetComponent<Projectile>().TargetLocation = endPosition;
        go.GetComponent<Projectile>().Unit = unit;
    }

    //This code was found from https://answers.unity.com/questions/566519/camerascreentoworldpoint-in-perspective.html
    //It finds the position in the game space relative to where the cursor is on the screen.
    public static Vector3 getPosition(bool isFlying) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy;
        if(isFlying)
            xy = new Plane(Vector3.up, new Vector3(0, 20, 0));
        else
            xy = new Plane(Vector3.up, new Vector3(0, 0, 0));
        
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

}
