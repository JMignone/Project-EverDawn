using UnityEngine;
using UnityEngine.AI;

public class FaceTarget : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Needs either a Unit or Building component")]
    private Unit unitStats;
    [SerializeField]
    [Tooltip("Needs either a Unit or Building component")]
    private Building buildingStats;
    [SerializeField]
    private float turnSpeed = 1f;

    void Update()
    {
        if (unitStats != null)
        {
            if (unitStats.Target == null)
            {
                return;
            }

            UnitRotateTowardTarget();
        }

        if (buildingStats != null)
        {
            if (buildingStats.Target == null)
            {
                return;
            }

            BuildingRotateTowardTarget();
        }
    }

    private void UnitRotateTowardTarget()
    {
        Vector3 lookDirection = gameObject.transform.position - unitStats.Target.GetComponentInChildren<NavMeshAgent>().transform.position;
        lookDirection.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    private void BuildingRotateTowardTarget()
    {
        Vector3 lookDirection = gameObject.transform.position - buildingStats.Target.GetComponentInChildren<NavMeshAgent>().transform.position;
        lookDirection.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }
}
