using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class JumpStats
{
    private IDamageable unit;
    private NavMeshLink link;
    private bool jumping;
    private Vector3 jumpEndPoint;

    [Tooltip("Allows the unit to jump over the river")]
    [SerializeField]
    private bool jumps;

    [Tooltip("The max angle from the vertical the unit is allowed to jump.")]
    [SerializeField]
    private float maxJumpAngle;

    public bool Jumps
    {
        get { return jumps; }
        set { jumps = value; }
    }

    public bool Jumping
    {
        get { return jumping; }
    }

    public Vector3 JumpEndPoint
    {
        get { return jumpEndPoint; }
    }

    public void StartStats(IDamageable go) {
        unit = go;
        unit.Agent.Agent.autoTraverseOffMeshLink = false;
        if(jumps)
            unit.Agent.Agent.areaMask += 4; //adding 4 gives the areaMask jump
    }

    public void StartJump() {
        if(!jumping) {
            jumping = true;
            Vector3 jumpStartPoint = unit.Agent.Agent.currentOffMeshLinkData.startPos;
            jumpEndPoint = unit.Agent.Agent.currentOffMeshLinkData.endPos;
            if(jumpEndPoint.z > jumpStartPoint.z)
                jumpEndPoint.z += 1;
            else
                jumpEndPoint.z -= 1;

            link = (NavMeshLink) unit.Agent.Agent.navMeshOwner;
            link.costModifier = -1; //resets the link so other units can use

            float angle = Vector3.Angle(jumpEndPoint - jumpStartPoint, Vector3.forward);
            if(angle > 90)
               angle = 180 - angle;

            if(angle > maxJumpAngle) {
                float offset = Mathf.Abs(jumpStartPoint.z - jumpEndPoint.z) * Mathf.Tan(maxJumpAngle * Mathf.Deg2Rad);
                if(jumpEndPoint.x > jumpStartPoint.x)
                    jumpEndPoint.x = jumpStartPoint.x + offset;
                else
                    jumpEndPoint.x = jumpStartPoint.x - offset;
            }

            GameFunctions.DisableAbilities((unit as Component).gameObject);
        }
    }

    public void Jump() {
        if(Vector3.Distance(jumpEndPoint, unit.Agent.transform.position) > unit.Agent.Agent.radius/3) {
            Vector3 direction = jumpEndPoint - unit.Agent.transform.position;
            direction.y = 0;
            direction = direction.normalized;

            unit.Agent.transform.position += direction * unit.Stats.MoveSpeed * AdjustedSpeedMultiplier() * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            unit.Agent.transform.rotation = Quaternion.RotateTowards(unit.Agent.transform.rotation, targetRotation, unit.Stats.RotationSpeed*2 * Time.deltaTime); //the number is degrees/second, maybe differnt per unit
        }
        else {
            unit.Agent.Agent.CompleteOffMeshLink();
            unit.Agent.Agent.Warp(unit.Agent.transform.position);
            GameFunctions.EnableAbilities((unit as Component).gameObject);
            jumping = false;
        }
    }

    public void DirectionalInfluence(Vector3 direction) {
        Vector3 origDirection = jumpEndPoint - unit.Agent.transform.position;
        origDirection.y = 0;
        origDirection = origDirection.normalized * unit.Stats.MoveSpeed * AdjustedSpeedMultiplier() * Time.deltaTime;

        //Vector3 newDirection = direction;   //this way sets the new jump to the direction of the knockback
        Vector3 newDirection = (direction * Time.deltaTime) + origDirection;   //this way sets the new jump as a combination vector of the 2 directions
        
        Vector3 newEndPoint;
        if(newDirection == Vector3.zero)
            return;

        if(newDirection.z == 0)
            newDirection.z = origDirection.z/100;
        
        if(newDirection.z * origDirection.z >= 0)
            newEndPoint = GameFunctions.FindIntersection(unit.Agent.transform.position, unit.Agent.transform.position + (1000 * newDirection),
                                               new Vector3(-100, 0, jumpEndPoint.z), new Vector3(100, 0, jumpEndPoint.z) );
        else
            newEndPoint = GameFunctions.FindIntersection(unit.Agent.transform.position, unit.Agent.transform.position + (1000 * newDirection),
                                               new Vector3(-100, 0, -jumpEndPoint.z), new Vector3(100, 0, -jumpEndPoint.z) );

        if(newEndPoint == Vector3.positiveInfinity) {
            Debug.Log("THIS SHOULD NOT HAVE HAPPENED");
            return;
        }

        float angle = Vector3.Angle(newEndPoint - unit.Agent.transform.position, Vector3.forward);
        if(angle > 90)
            angle = 180 - angle;

        if(angle > maxJumpAngle) {
            float offset = Mathf.Abs(unit.Agent.transform.position.z - newEndPoint.z) * Mathf.Tan(maxJumpAngle * Mathf.Deg2Rad);
            if(newEndPoint.x > unit.Agent.transform.position.x)
                newEndPoint.x = unit.Agent.transform.position.x + offset;
            else
                newEndPoint.x = unit.Agent.transform.position.x - offset;
        }
        jumpEndPoint = newEndPoint;
    }

    public void CancelJump() {
        if(jumping) {
            unit.Agent.Agent.CompleteOffMeshLink();
            unit.Agent.Agent.Warp(unit.Agent.transform.position);
            GameFunctions.EnableAbilities((unit as Component).gameObject);
            jumping = false;
        }
    }

    public float AdjustedSpeedMultiplier() {
        if(unit.Stats.EffectStats.FrozenStats.IsFrozen || unit.Stats.EffectStats.StunnedStats.IsStunned)
            return 0;
        else
            return unit.Stats.EffectStats.SlowedStats.CurrentSlowIntensity;
    }
}
