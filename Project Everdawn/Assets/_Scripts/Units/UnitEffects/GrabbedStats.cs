using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[System.Serializable]
public class GrabbedStats
{
    [SerializeField]
    private bool cantBeGrabbed;
    private bool outSideResistance;

    [SerializeField]
    private bool isGrabbed;

    private Vector3 direction;

    private float grabDelay;
    private bool stunned;
    private float currentStunDelay;

    private float totalDistance;
    private IDamageable unit;
    private IDamageable enemyUnit;
    private bool enemyController;

    private Vector3 destination;
    private bool obstaclesBlockGrab;
    private bool obstacleDetected;

    public bool CantBeGrabbed
    {
        get { return cantBeGrabbed; }
        set { cantBeGrabbed = value; }
    }

    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
    }

    public bool IsGrabbed
    {
        get { return isGrabbed; }
        set { isGrabbed = value; }
    }

    public bool Stunned
    {
        get { return stunned; }
    }

    public void StartGrabbedStats(IDamageable go) {
        unit = go;
    }

    public void UpdateGrabbedStats() {
        if(isGrabbed) {

            Vector3 unitAgentPos = new Vector3(unit.Agent.transform.position.x, 0, unit.Agent.transform.position.z);
            Vector3 enemyAgentPos = Vector3.zero;

            NavMeshHit hit;

            bool enemyCanAct = false;
            if(enemyUnit.Agent != null && enemyUnit.Stats.CanAct) {
                enemyCanAct = true;
                enemyAgentPos = new Vector3(enemyUnit.Agent.transform.position.x, 0, enemyUnit.Agent.transform.position.z);
                obstacleDetected = false;

                if(obstaclesBlockGrab && unit.Stats.MovementType != GameConstants.MOVEMENT_TYPE.FLYING) {
                    if(NavMesh.Raycast(unit.Agent.transform.position, enemyAgentPos, out hit, 1)) { //if an obstacle is detected
                        obstacleDetected = true;
                        enemyAgentPos = hit.position;
                    }
                }
            }

            /*

                Need to add a check to see if the grabbed unit can still be grabbed. Maybe it got knocked up, canceling the grab

            */

            //Debug.DrawLine(unitAgentPos, enemyAgentPos, Color.green);
            //Debug.DrawRay(enemyAgentPos, Vector3.up*10, Color.red);

            float obstacleAdjustment = 0;
            if(obstacleDetected)
                obstacleAdjustment = unit.Agent.HitBox.radius/2 + enemyUnit.Agent.HitBox.radius;
            if(enemyCanAct && !stunned && Vector3.Distance(unitAgentPos, enemyAgentPos) > unit.Agent.HitBox.radius + enemyUnit.Agent.HitBox.radius - obstacleAdjustment) {
                direction = enemyAgentPos - unitAgentPos;
                direction.y = 0;
                unit.Agent.transform.position += Time.deltaTime * totalDistance/grabDelay * direction.normalized;
            }
            else if(!NavMesh.SamplePosition(unitAgentPos, out hit, 1f, 9)) { //if the grabbed unit ended up in an obstacle
                //NavMesh.Raycast(enemyAgentPos, unitAgentPos, out hit, 1); //find where the navmesh starts
                //Debug.DrawLine(unitAgentPos, hit.position, Color.green);
                //Debug.DrawRay(hit.position, Vector3.up*20, Color.yellow);
                if(enemyUnit.Agent != null) {
                    direction = enemyAgentPos - unitAgentPos;
                    direction.y = 0;
                    enemyUnit.Agent.transform.position += Time.deltaTime * totalDistance/grabDelay * direction.normalized;
                }
                unit.Agent.transform.position += Time.deltaTime * totalDistance/grabDelay * direction.normalized;
                /*
                    If somehow a weird interaction causes the unit to move in the direction outside the boundary, the unit will float away from the arena
                    forever, however I don't think there is any possible way for this to happen. Hopefully I am not wrong.
                */
            }
            else if(currentStunDelay > 0) {
                if(!stunned && enemyCanAct) {
                    if(enemyController)
                        enemyUnit.Stats.IsCastingAbility = false;
                    enemyUnit.SetTarget((unit as Component).gameObject);
                    enemyUnit.Stats.CurrAttackDelay = enemyUnit.Stats.AttackDelay * enemyUnit.Stats.AttackChargeLimiter;
                }
                stunned = true;
                unit.Agent.Agent.enabled = true;
                currentStunDelay -= Time.deltaTime;
            }
            else
                unGrab();
        }
    }

    public void Grab(float grabSpeed, float grabDuration, float stunDuration, bool obstaclesBlockGrab, bool enemyController, IDamageable enemy) {
        if(!cantBeGrabbed && !outSideResistance && enemy.Agent != null && enemy.Stats.CanAct) {
            unGrab(); //set to false as we need to be able to grab an already grabbed unit
            isGrabbed = true;
            grabDelay = grabDuration;
            //if we are grabbed by a friendly unit, don't get stunned
            if((enemy as Component).gameObject.tag != (unit as Component).gameObject.tag)
                currentStunDelay = stunDuration;
            else
                currentStunDelay = 0;
            enemyUnit = enemy;
            this.enemyController = enemyController;
            this.obstaclesBlockGrab = obstaclesBlockGrab;

            Vector3 unitAgentPos = new Vector3(unit.Agent.transform.position.x, 0, unit.Agent.transform.position.z);
            Vector3 enemyAgentPos = new Vector3(enemyUnit.Agent.transform.position.x, 0, enemyUnit.Agent.transform.position.z);

            totalDistance = Vector3.Distance(unitAgentPos, enemyAgentPos);

            //if the speed option was used, set for correct speed
            if(grabSpeed != 0) 
                grabDelay = totalDistance/grabSpeed;

            unit.JumpStats.CancelJump();
            unit.Agent.Agent.enabled = false;
            unit.SetTarget(null);
            unit.Stats.IsCastingAbility = false; //normally this is done automatically, but some abilitys use the 'abilityOverride', so we will need to set it
            GameFunctions.DisableAbilities(unit);
        }
    }

    public void unGrab() {
        isGrabbed = false;
        stunned = false;
        obstacleDetected = false;
        unit.Agent.Agent.enabled = true;
        GameFunctions.EnableAbilities(unit);

        if(enemyController && enemyUnit != null)
            enemyUnit.Stats.IsCastingAbility = false;
        enemyController = false;
    }
}
