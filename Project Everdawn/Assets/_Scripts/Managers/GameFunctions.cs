using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public static class GameFunctions
{
    public static bool CanAttack(string playerTag, string enemyTag, Component damageable, BaseStats stats) { //returns if a unit can attack another
        if(damageable) {
            if(playerTag != enemyTag && (damageable as IDamageable).Stats.Targetable) {
                if((damageable as IDamageable).Stats.SoonToBeKilled) {
                    if(!stats.SoonToKill && !stats.SoonToKillOverride)
                        return false;
                }
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
        if(!(damageable as IDamageable).Stats.Damageable)
            return false;
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

    public static void Attack(Component damageable, float baseDamage, CritStats critStats = null) {
        if(damageable) {
            if((damageable as IDamageable).Stats.Damageable) {
                if(critStats != null) {
                    if((damageable as IDamageable).Stats.EffectStats.FrozenStats.IsFrozen)
                        baseDamage *= critStats.CritOnFrozen;
                    if((damageable as IDamageable).Stats.EffectStats.SlowedStats.IsSlowed)
                        baseDamage *= critStats.CritOnSlow;
                    if((damageable as IDamageable).Stats.EffectStats.PoisonedStats.IsPoisoned)
                        baseDamage *= critStats.CritOnPoison;
                    if((damageable as IDamageable).Stats.EffectStats.BlindedStats.IsBlinded)
                        baseDamage *= critStats.CritOnBlind;
                    if((damageable as IDamageable).Stats.EffectStats.StunnedStats.IsStunned || (damageable as IDamageable).Stats.EffectStats.GrabbedStats.Stunned)
                        baseDamage *= critStats.CritOnStun;
                }
                else
                    Debug.Log("No CritStats? This better be a poison!");

                (damageable as IDamageable).TakeDamage(baseDamage);
            }
        }
    }

    public static GameObject GetNearestTarget(List<GameObject> hitTargets, string tag, BaseStats stats) {
        if(hitTargets.Count > 0) {
            GameObject go = null; 
            Component targetComponent;
            SphereCollider targetSc;

            stats.TowerPosOffset = 0;
            float dist = 10000; //Arbitrary large number

            foreach (GameObject hitTarget in hitTargets)
            {
                targetComponent = hitTarget.GetComponent(typeof(IDamageable));
                if(targetComponent) {
                    if((targetComponent as IDamageable).Stats.Targetable && GameFunctions.CanAttack(tag, hitTarget.tag, targetComponent, stats)) {
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

    public static GameObject GetTowerTarget(List<GameObject> towers, string tag, BaseStats stats) {
        GameObject go = null; 
        Component targetComponent;
        SphereCollider targetSc;

        if( (tag == GameConstants.PLAYER_TAG && stats.DetectionObject.transform.position.z > 0) || 
        (tag != GameConstants.PLAYER_TAG && stats.DetectionObject.transform.position.z < 0)) {

            stats.TowerPosOffset = 0;
            float dist = 10000; //Arbitrary large number

            foreach (GameObject hitTarget in towers)
            {
                targetComponent = hitTarget.GetComponent(typeof(IDamageable));

                if(targetComponent) {
                    if((targetComponent as IDamageable).Stats.Targetable && GameFunctions.CanAttack(tag, hitTarget.tag, targetComponent, stats)) {
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
        else {
            foreach (GameObject hitTarget in towers)
            {
                targetComponent = hitTarget.GetComponent(typeof(IDamageable));

                if(targetComponent) {
                    if((targetComponent as IDamageable).Stats.Targetable && GameFunctions.CanAttack(tag, hitTarget.tag, targetComponent, stats)) {
                        targetSc = (targetComponent as IDamageable).Stats.DetectionObject;
                        
                        //if the x position times the other x position is positive, then we are on the correct side
                        if(towers.Count == 3 && targetSc.transform.position.x != 0 && stats.DetectionObject.transform.position.x*targetSc.transform.position.x >= 0) { //if we found a closer target
                            stats.TowerPosOffset = 0;
                            if(!hitTarget.CompareTag(tag)) //and its not on the same team (sanity check, shouldnt ever occur)
                                return hitTarget;
                        }
                        else if(towers.Count != 3 && stats.DetectionObject.transform.position.x*targetSc.transform.position.x >= 0) {
                            if(!hitTarget.CompareTag(tag)) { //and its not on the same team (sanity check, shouldnt ever occur)
                                if(towers.Count == 1)
                                    stats.TowerPosOffset = 22 * Mathf.Sign(stats.DetectionObject.transform.position.x);
                                return hitTarget;
                            }
                        }
                    }
                }
            }
        }
        return go;
    }

    public static GameObject SpawnUnit(GameObject prefab, Transform parent, Vector3 position, string tag = "Player") 
    {
        int playerOffset = 100;
        if(tag == "Enemy")
            playerOffset = -100;

        var targetPosition = position;
        targetPosition.z = playerOffset;
        Vector3 direction = targetPosition - position;
        
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        GameObject go = GameObject.Instantiate(prefab, position, targetRotation, parent);
        giveUnitTeam(go, tag);

        GameManager.AddObjectToList(go);
        return go;
    }

    //damage increase is needed for range units that fire projectiles in its auto attack
    public static void FireProjectile(GameObject prefab, Vector3 startPosition, Vector3 mousePosition, Vector3 direction, IDamageable unit, string tag, float damageMultiplier = 1.0f, float rangeIncrease = 0, ICaster skillshot = null) {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float distance = Vector3.Distance(startPosition, mousePosition);
        Vector3 endPosition = mousePosition;
        Projectile projectile = prefab.GetComponent<Projectile>();

        float range = projectile.Range + rangeIncrease;
        float radius = projectile.Radius;
        bool isGrenade = projectile.GrenadeStats.IsGrenade;
        bool selfDestructs = projectile.SelfDestructStats.SelfDestructs;
        bool isMovement = prefab.GetComponent<Movement>();
        int areaMask = 1;
        if(unit != null) {
            if(unit.Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
                areaMask = 8;
        }

        if(distance > (range - radius)) {
            endPosition = startPosition + (direction.normalized * (range - radius));
            if(isMovement)
                endPosition -= direction.normalized * radius;
        }
        else if(distance < range && !isGrenade && !selfDestructs && !isMovement)
            endPosition = startPosition + (direction.normalized * range);

        if(!isMovement)
            startPosition += direction.normalized * radius;
        else {
            endPosition += direction.normalized * radius;
            NavMeshHit hit;
            endPosition = GameFunctions.adjustForBoundary(endPosition);
            
            if(NavMesh.SamplePosition(endPosition, out hit, GameConstants.SAMPLE_POSITION_RADIUS, areaMask))
                endPosition = hit.position;
        }
        
        if(isGrenade && projectile.GrenadeStats.IsAirStrike)
            startPosition = new Vector3(0, 0, GameManager.Instance.Ground.transform.localScale.z*-5 - 10);
        GameObject go = GameObject.Instantiate(prefab, startPosition, targetRotation, GameManager.GetUnitsFolder());
        go.tag = tag;

        Projectile proj = go.GetComponent<Projectile>();
        proj.TargetLocation = endPosition;
        if(unit != null)
            proj.Unit = unit;
        proj.DamageMultiplier = damageMultiplier;
        proj.Caster = skillshot;
    }

    public static void FireProjectile(GameObject prefab, Vector3 startPosition, Actor3D chosenTarget, Vector3 direction, IDamageable unit, string tag, float damageMultiplier = 1.0f, ICaster targeter = null) {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float distance = Vector3.Distance(startPosition, chosenTarget.transform.position);
        Vector3 endPosition = chosenTarget.transform.position;
        Projectile projectile = prefab.GetComponent<Projectile>();

        float radius = projectile.Radius;
        bool isGrenade = projectile.GrenadeStats.IsGrenade;
        if(!prefab.GetComponent<Movement>())
            startPosition += direction.normalized * radius;
        else
            endPosition += direction.normalized * radius;
            
        if(isGrenade && projectile.GrenadeStats.IsAirStrike)
            startPosition = new Vector3(0, 0, GameManager.Instance.Ground.transform.localScale.z*-5 - 10);

        GameObject go = GameObject.Instantiate(prefab, startPosition, targetRotation, GameManager.GetUnitsFolder());
        go.tag = tag;

        Projectile proj = go.GetComponent<Projectile>();
        proj.TargetLocation = endPosition;
        proj.ChosenTarget = chosenTarget;
        if(unit != null) 
            proj.Unit = unit;
        proj.DamageMultiplier = damageMultiplier;
        proj.Caster = targeter;
    }


    public static void FireCAL(GameObject prefab, Vector3 startPosition, Vector3 mousePosition, Vector3 direction, IDamageable unit, string tag, float damageMultiplier = 1.0f, float rangeIncrease = 0, ICaster skillshot = null) {
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
        float distance = Vector3.Distance(startPosition, mousePosition);
        Vector3 endPosition = mousePosition;
        CreateAtLocation cal = prefab.GetComponent<CreateAtLocation>();

        float range = cal.Range + rangeIncrease;
        float radius = cal.Radius;
        if(distance > (range - radius))
            endPosition = startPosition + (direction.normalized * (range - radius));

        if(prefab.GetComponent<CreateAtLocation>().SummonStats.IsSummon) { //if its a summon, check to see if its colliding with anything
            //endPosition = GameFunctions.adjustForTowers(endPosition, prefab.GetComponent<CreateAtLocation>().Radius);

            NavMeshHit hit;
            if(NavMesh.SamplePosition(mousePosition, out hit, GameConstants.SAMPLE_POSITION_RADIUS, cal.SummonStats.AreaMask()))
                endPosition = hit.position;
        }

        if(unit != null && cal.PlayOnUnit)
            endPosition = unit.Agent.transform.position;

        GameObject go = GameObject.Instantiate(prefab, endPosition, targetRotation, GameManager.GetUnitsFolder());
        go.tag = tag;

        CreateAtLocation goCal = go.GetComponent<CreateAtLocation>();
        goCal.TargetLocation = endPosition;
        if(unit != null)
            goCal.Unit = unit;
        goCal.DamageMultiplier = damageMultiplier;
        goCal.Caster = skillshot;
    }

    public static void FireCAL(GameObject prefab, Vector3 startPosition, Actor3D chosenTarget, Vector3 direction, IDamageable unit, string tag, float damageMultiplier = 1.0f, ICaster targeter = null) {
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
        float distance = Vector3.Distance(startPosition, chosenTarget.transform.position);
        Vector3 endPosition = chosenTarget.transform.position;
        CreateAtLocation cal = prefab.GetComponent<CreateAtLocation>();

        /* The CAL should hit the target regardless of how far its target runs after initiation
        float range = cal.Range + rangeIncrease;
        float radius = cal.Radius;
        if(distance > (range - radius))
            endPosition = startPosition + (direction.normalized * (range - radius));
        */

        if(unit != null && cal.PlayOnUnit)
            endPosition = unit.Agent.transform.position;

        GameObject go = GameObject.Instantiate(prefab, endPosition, targetRotation, GameManager.GetUnitsFolder());
        go.tag = tag;
        
        CreateAtLocation goCal = go.GetComponent<CreateAtLocation>();
        goCal.TargetLocation = endPosition;
        goCal.ChosenTarget = chosenTarget;
        if(unit != null)
            goCal.Unit = unit;
        goCal.DamageMultiplier = damageMultiplier;
        goCal.Caster = targeter;
    }

    //This code was found from https://answers.unity.com/questions/566519/camerascreentoworldpoint-in-perspective.html
    //It finds the position in the game space relative to where the cursor is on the screen.
    public static Vector3 getPosition(bool isFlying) {
        Vector3 positionOffset= new Vector3(0, 0, 5); //offset the position so its not covered by users fingers
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy;
        if(isFlying)
            xy = new Plane(Vector3.up, new Vector3(0, GameConstants.FLY_ZONE_HEIGHT, 0));
        else
            xy = new Plane(Vector3.up, new Vector3(0, 0, 0));
        
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance) + positionOffset;
    }

    public static Vector3 adjustForTowers(Vector3 position, float radius) {
        return position; 

        // !!! IT SEEMS THIS FUNCTION MAY NOT BE NEEDED WITH THE ADDITION OF THE TOWER BARRIERS, ILL KEEP IT LIKE THIS FOR NOW !!!
        /*
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
        */
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

    // ! This function will fall into an endless loop or fail outright if a group unit has another group unit inside it, which shouldn't be the case !
    public static void giveUnitTeam(GameObject go, string tag) {
        IDamageable unit = (go.GetComponent(typeof(IDamageable)) as IDamageable);

        if(unit.Stats.UnitGrouping == GameConstants.UNIT_GROUPING.GROUP) {
            for(int unitIndex=1; unitIndex<go.transform.childCount; unitIndex++)
                giveUnitTeam(go.transform.GetChild(unitIndex).gameObject, tag);
        }
        else {
            go.tag = tag;
            unit.Agent.transform.tag = tag;
            unit.UnitSprite.Ability.transform.tag = tag;
            if(tag == "Enemy")
                unit.Stats.HealthBar.color = new Color32(219,37,37,255);
        }
    }

    public static void giveAbilityTeam(GameObject go, string tag) {
        go.tag = tag;
    }

    //sets the ability previews to red to display that they are disabled
    public static void DisableAbilities(GameObject go) {
        if(go.transform.GetChild(1).GetChild(5).childCount > 1) { //if the unit has an ability, set its image colors to red
            foreach(Transform child in go.transform.GetChild(1).GetChild(5).GetChild(2)) {
                if(child.childCount > 1) //this means its a complicated summon preview
                    child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,0,0,50);
                else
                    child.GetChild(0).GetComponent<Image>().color = new Color32(255,0,0,50);
            }
        }
    }

    public static void EnableAbilities(GameObject go) {
        if(go.transform.GetChild(1).GetChild(5).childCount > 1) { //if the unit has an ability, set its image colors back to green
            foreach(Transform child in go.transform.GetChild(1).GetChild(5).GetChild(2)) {
                if(child.childCount > 1) //this means its a complicated summon preview
                    child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,255,255,100);
                else
                    child.GetChild(0).GetComponent<Image>().color = new Color32(255,255,255,100);
            }
        }
    }

    public static Transform GetCanvas() {
        return GameObject.Find(GameConstants.HUD_CANVAS).transform;
    }
}
