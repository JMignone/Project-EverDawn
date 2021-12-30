using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAnimation : MonoBehaviour
{
    [SerializeField] private GameObject objectToOrbit;
    [SerializeField] [Tooltip("Axis to rotate around")] private Vector3 axisOfRotation;
    [SerializeField] [Tooltip("Time to orbit in seconds")] [Range(1,10)] private float timeToOrbit;
    private Transform initialTransform;

    private void Awake()
    {
        initialTransform = transform;
    }

    private void OnEnable()
    {
        transform.transform.SetPositionAndRotation(initialTransform.position, initialTransform.rotation);
        transform.LeanRotateAround(axisOfRotation, 360f, timeToOrbit).setLoopClamp();
    }

    private void OnDisable()
    {
        // TODO Set logic to pause animation
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 rotationCenter = objectToOrbit.transform.position;
        transform.RotateAround(rotationCenter, axisOfRotation, (360f * Time.deltaTime) / timeToOrbit);
    }
}
