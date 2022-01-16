using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RetreatStats
{
    [SerializeField]
    private bool retreats;

    [SerializeField]
    private float distance;

    public bool Retreats
    {
        get { return retreats; }
    }

    public Vector3 GetTargetLocation(Vector3 startLocation, Vector3 targetLocation, GameConstants.PASS_OBSTACLES passObstacles) {
        //if the movement isnt a retreat, just return that original target location
        if(!retreats)
            return targetLocation;

        UnityEngine.AI.NavMeshHit hit;

        Vector3 direction = (startLocation - targetLocation).normalized;
        targetLocation = startLocation + (direction*distance);

        if(passObstacles == GameConstants.PASS_OBSTACLES.PASS) {
            targetLocation = GameFunctions.adjustForBoundary(targetLocation);
            if(UnityEngine.AI.NavMesh.SamplePosition(targetLocation, out hit, GameConstants.SAMPLE_POSITION_RADIUS, 9))
                targetLocation = hit.position;
        }
        else if(passObstacles == GameConstants.PASS_OBSTACLES.HALF) {
            if(!UnityEngine.AI.NavMesh.SamplePosition(targetLocation, out hit, 1f, 9)) {
                UnityEngine.AI.NavMesh.Raycast(startLocation, targetLocation, out hit, 1);
                targetLocation = hit.position;
            }
        }
        else {
            UnityEngine.AI.NavMesh.Raycast(startLocation, targetLocation, out hit, 1);
            targetLocation = hit.position;
        }

        targetLocation.y = 0;
        return targetLocation;
    }
}
