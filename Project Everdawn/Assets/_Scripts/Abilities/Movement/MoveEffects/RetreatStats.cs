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

    public Vector3 GetTargetLocation(Vector3 startLocation, Vector3 targetLocation) {
        //if the movement isnt a retreat, just return that original target location
        if(!retreats)
            return targetLocation;

        Vector3 direction = (startLocation - targetLocation).normalized;

        targetLocation = startLocation + (direction*distance);

        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.Raycast(startLocation, targetLocation, out hit, 1);
        return hit.position;
    }
}
