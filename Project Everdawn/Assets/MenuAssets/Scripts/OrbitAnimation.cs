using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAnimation : MonoBehaviour
{
    [SerializeField] private GameObject objectToRotateAround;
    [SerializeField] private Vector3 axisOfRotation;
    [SerializeField] [Range(1,10)] private float timeToOrbit;
    private float orbitRadius;
    private float orbitLength;
    private float orbitVelocity;
    private float orbitAngle;

    // Awake is called before start
    void Awake()
    {
        #region Velocity Calculation
            Vector3 rotationCenter = objectToRotateAround.transform.position;
            orbitRadius = Vector3.Distance(transform.position, rotationCenter);
            orbitLength = orbitRadius * 2 * Mathf.PI;
            orbitVelocity = orbitLength / timeToOrbit;
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.LeanRotateAround(axisOfRotation, 360f, timeToOrbit).setLoopClamp();
    }

    // Update is called once per frame
    void Update()
    {
        orbitAngle = orbitVelocity * Time.deltaTime;
        Vector3 rotationCenter = objectToRotateAround.transform.position;
        transform.RotateAround(rotationCenter, axisOfRotation, orbitAngle);
    }
}
