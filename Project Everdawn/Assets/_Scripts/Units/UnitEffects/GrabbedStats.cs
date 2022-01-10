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

    [SerializeField]
    private Vector3 direction;

    [SerializeField] [Min(0)]
    private float grabDelay;
    private bool stunned;
    [SerializeField] [Min(0)]
    private float currentStunDelay;

    private float totalDistance;
    private IDamageable unit;
    private IDamageable enemyUnit;

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

            Debug.DrawLine(unitAgentPos, enemyAgentPos, Color.green);
            Debug.DrawRay(enemyAgentPos, Vector3.up*10, Color.red);

            float obstacleAdjustment = 0;
            if(obstacleDetected)
                obstacleAdjustment = unit.Agent.HitBox.radius/2 + enemyUnit.Agent.HitBox.radius;
            if(enemyCanAct && !stunned && Vector3.Distance(unitAgentPos, enemyAgentPos) > unit.Agent.HitBox.radius + enemyUnit.Agent.HitBox.radius - obstacleAdjustment) {
                direction = enemyAgentPos - unitAgentPos;
                direction.y = 0;
                unit.Agent.transform.position += direction.normalized * totalDistance/grabDelay * Time.deltaTime;
            }
            else if(!NavMesh.SamplePosition(unitAgentPos, out hit, 1f, 9)) { //if the grabbed unit ended up in an obstacle
                //NavMesh.Raycast(enemyAgentPos, unitAgentPos, out hit, 1); //find where the navmesh starts
                //Debug.DrawLine(unitAgentPos, hit.position, Color.green);
                //Debug.DrawRay(hit.position, Vector3.up*20, Color.yellow);
                if(enemyUnit.Agent != null) {
                    direction = enemyAgentPos - unitAgentPos;
                    direction.y = 0;
                    enemyUnit.Agent.transform.position += direction.normalized * totalDistance/grabDelay * Time.deltaTime;
                }
                unit.Agent.transform.position += direction.normalized * totalDistance/grabDelay * Time.deltaTime;
                /*
                    If somehow a weird interaction causes the unit to move in the direction outside the boundary, the unit will float away from the arena
                    forever, however I don't think there is any possible way for this to happen. Hopefully I am not wrong.
                */
            }
            else if(currentStunDelay > 0) {
                stunned = true;
                unit.Agent.Agent.enabled = true;
                currentStunDelay -= Time.deltaTime;
            }
            else
                unGrab();
        }
    }

    public void Grab(float grabSpeed, float grabDuration, float stunDuration, bool obstaclesBlockGrab, IDamageable enemy) {
        if(!cantBeGrabbed && !outSideResistance && enemy.Agent != null && enemy.Stats.CanAct) {
            isGrabbed = true;
            grabDelay = grabDuration;
            currentStunDelay = stunDuration;
            enemyUnit = enemy;
            this.obstaclesBlockGrab = obstaclesBlockGrab;

            Vector3 unitAgentPos = new Vector3(unit.Agent.transform.position.x, 0, unit.Agent.transform.position.z);
            Vector3 enemyAgentPos = new Vector3(enemyUnit.Agent.transform.position.x, 0, enemyUnit.Agent.transform.position.z);

            totalDistance = Vector3.Distance(unitAgentPos, enemyAgentPos);

            //if the speed option was used, set for correct speed
            if(grabSpeed != 0) 
                grabDelay = totalDistance/grabSpeed;

            unit.Agent.Agent.enabled = false;
            unit.SetTarget(null);
            unit.Stats.CurrAttackDelay = 0;
            GameFunctions.DisableAbilities((unit as Component).gameObject);
        }
    }

    public void unGrab() {
        isGrabbed = false;
        stunned = false;
        obstacleDetected = false;
        GameFunctions.EnableAbilities((unit as Component).gameObject);
    }
}
