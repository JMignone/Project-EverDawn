using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class GameFunctions
{
    public static bool CanAttack(string playerTag, string enemyTag, Component damageable, BaseStats stats) { //returns if a unit can attack another
        if(damageable) {
            if(playerTag != enemyTag) {
                bool heightAttackable = false;
                if(stats.HeightAttackable == GameConstants.HEIGHT_ATTACKABLE.BOTH) //If the unit can attack the flying or ground unit, continue
                    heightAttackable = true;
                else if(stats.HeightAttackable == GameConstants.HEIGHT_ATTACKABLE.GROUND && (damageable as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.GROUND)
                    heightAttackable = true;
                else if(stats.HeightAttackable == GameConstants.HEIGHT_ATTACKABLE.FLYING && (damageable as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
                    heightAttackable = true;
                if(heightAttackable) { //the inside of this if block tests if the units priority matches the unit, then return true
                    if(stats.AttackPriority == GameConstants.ATTACK_PRIORITY.EVERYTHING) //If the units priority is anything, return true
                        return true;
                    else if(stats.AttackPriority == GameConstants.ATTACK_PRIORITY.STRUCTURE && (damageable as IDamageable).Stats.UnitType == GameConstants.UNIT_TYPE.STRUCTURE)
                        return true;
                }
            }   
        }
        return false;
    }

    public static bool WillHit(GameConstants.HEIGHT_ATTACKABLE heightAttackable, GameConstants.TYPE_ATTACKABLE typeAttackable, Component damageable) { //returns if an ability will hit its target
        bool willHit = false;
        if(heightAttackable == GameConstants.HEIGHT_ATTACKABLE.BOTH) //If the unit can attack the flying or ground unit, continue
            willHit = true;
        else if(heightAttackable == GameConstants.HEIGHT_ATTACKABLE.GROUND && (damageable as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.GROUND)
            willHit = true;
        else if(heightAttackable == GameConstants.HEIGHT_ATTACKABLE.FLYING && (damageable as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
            willHit = true;
        if(willHit) {
            if(typeAttackable == GameConstants.TYPE_ATTACKABLE.BOTH)
                return true;
            else if(typeAttackable == GameConstants.TYPE_ATTACKABLE.UNIT && (damageable as IDamageable).Stats.UnitType == GameConstants.UNIT_TYPE.UNIT)
                return true;
            else if(typeAttackable == GameConstants.TYPE_ATTACKABLE.STRUCTURE && (damageable as IDamageable).Stats.UnitType == GameConstants.UNIT_TYPE.STRUCTURE)
                return true;
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

    public static GameObject SpawnUnit(GameObject prefab, Transform parent, Vector3 position) 
    {
        var targetPosition = position;
        targetPosition.z = 100; //What about the enemy player? Does this need to be -100?
        Vector3 direction = position - targetPosition;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        //This makes sure the unit is rotated the corect way

        GameObject go = GameObject.Instantiate(prefab, position, targetRotation, parent);
        GameManager.AddObjectToList(go);
        return go;
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
        if(distance > (range - radius))
            endPosition = startPosition + (direction.normalized * range);
        else if(distance < range && !isGrenade && !selfDestructs)
            endPosition = startPosition + (direction.normalized * range);
        startPosition += direction.normalized * radius;
        if(isGrenade && projectile.GrenadeStats.IsAirStrike)
            startPosition = new Vector3(0, 0, GameManager.Instance.Ground.transform.localScale.z*-5 - 10);

        GameObject go = GameObject.Instantiate(prefab, startPosition, targetRotation, GameManager.GetUnitsFolder());
        go.GetComponent<Projectile>().TargetLocation = endPosition;
        go.GetComponent<Projectile>().Unit = unit;
    }

    public static void FireProjectile(GameObject prefab, Vector3 startPosition, Actor3D chosenTarget, Vector3 direction, Unit unit) {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float distance = Vector3.Distance(startPosition, chosenTarget.transform.position);
        Vector3 endPosition = chosenTarget.transform.position;
        Projectile projectile = prefab.GetComponent<Projectile>();

        float radius = projectile.Radius;
        bool isGrenade = projectile.GrenadeStats.IsGrenade;

        startPosition += direction.normalized * radius;
        if(isGrenade && projectile.GrenadeStats.IsAirStrike)
            startPosition = new Vector3(0, 0, GameManager.Instance.Ground.transform.localScale.z*-5 - 10);

        GameObject go = GameObject.Instantiate(prefab, startPosition, targetRotation, GameManager.GetUnitsFolder());
        go.GetComponent<Projectile>().TargetLocation = endPosition;
        go.GetComponent<Projectile>().ChosenTarget = chosenTarget;
        go.GetComponent<Projectile>().Unit = unit;
    }

    public static void FireCAL(GameObject prefab, Vector3 startPosition, Vector3 mousePosition, Vector3 direction, Unit unit) {
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
        float distance = Vector3.Distance(startPosition, mousePosition);
        Vector3 endPosition = mousePosition;
        CreateAtLocation cal = prefab.GetComponent<CreateAtLocation>();

        float range = cal.Range;
        float radius = cal.Radius;
        if(distance > (range - radius))
            endPosition = startPosition + (direction.normalized * (range - radius));

        if(prefab.GetComponent<CreateAtLocation>().SummonStats.IsSummon) { //if its a summon, check to see if its colliding with anything
            //endPosition = GameFunctions.adjustForTowers(endPosition, prefab.GetComponent<CreateAtLocation>().Radius);

            NavMeshHit hit;
            if(NavMesh.SamplePosition(mousePosition, out hit, 6.1f, 9))
                endPosition = hit.position;
        }
        GameObject go = GameObject.Instantiate(prefab, endPosition, targetRotation, GameManager.GetUnitsFolder());
        go.GetComponent<CreateAtLocation>().TargetLocation = endPosition;
        go.GetComponent<CreateAtLocation>().Unit = unit;
    }

    public static void FireCAL(GameObject prefab, Vector3 startPosition, Actor3D chosenTarget, Vector3 direction, Unit unit) {
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
        float distance = Vector3.Distance(startPosition, chosenTarget.transform.position);
        Vector3 endPosition = chosenTarget.transform.position;
        CreateAtLocation cal = prefab.GetComponent<CreateAtLocation>();

        float range = cal.Range;
        float radius = cal.Radius;
        if(distance > (range - radius))
            endPosition = startPosition + (direction.normalized * (range - radius));

        GameObject go = GameObject.Instantiate(prefab, endPosition, targetRotation, GameManager.GetUnitsFolder());
        go.GetComponent<CreateAtLocation>().TargetLocation = endPosition;
        go.GetComponent<CreateAtLocation>().ChosenTarget = chosenTarget;
        go.GetComponent<CreateAtLocation>().Unit = unit;
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

    public static Vector3 adjustForTowers(Vector3 position, float radius) {
        foreach(GameObject go in GameManager.Instance.TowerObjects) {
            Component component = go.GetComponent(typeof(IDamageable));
            float towerRadius = (component as IDamageable).Agent.HitBox.radius;
            Vector3 towerPosition = go.transform.position;
            
            if(position.y < 1 &&    //If our position is currently inside a tower
               position.x - radius < towerPosition.x + towerRadius &&
               position.x + radius > towerPosition.x - towerRadius &&
               position.z - radius  < towerPosition.z + towerRadius &&
               position.z + radius  > towerPosition.z - towerRadius ) 
                {
                    float distFromLeft   = Math.Abs((position.x + radius) - towerPosition.x + towerRadius);
                    float distFromRight  = Math.Abs((position.x - radius) - towerPosition.x - towerRadius);
                    float distFromBottom = Math.Abs((position.z + radius) - towerPosition.z + towerRadius);
                    float distFromTop    = Math.Abs((position.z - radius) - towerPosition.z - towerRadius);

                    if( distFromLeft < distFromRight && distFromLeft < distFromBottom && distFromLeft < distFromTop) //If we are closest to the left side of the tower
                        position = new Vector3(towerPosition.x - towerRadius - radius, position.y, position.z);
                    else if( distFromRight < distFromLeft && distFromRight < distFromBottom && distFromRight < distFromTop)
                        position = new Vector3(towerPosition.x + towerRadius + radius , position.y, position.z);
                    else if( distFromBottom < distFromLeft && distFromBottom < distFromRight && distFromBottom < distFromTop)
                        position = new Vector3(position.x, position.y, towerPosition.z - towerRadius - radius );
                    else 
                        position = new Vector3(position.x, position.y, towerPosition.z + towerRadius + radius );
                    break;
               }        
        }
        return position;
    }

    public static Vector3 adjustForBoundary(Vector3 position) {
        Vector3 scale = GameManager.Instance.Ground.transform.localScale;

        float right = scale.x * 5.0f;
        float left = right * -1.0f;
        float top = scale.z * 5.0f;
        float bottom = top * -1.0f;

        if(position.x < left)
            position.x = left;
        else if(position.x > right)
            position.x = right;
        if(position.z < bottom)
            position.z = bottom;
        else if(position.z > top)
            position.z = top;
        
        return position;
    }

}
