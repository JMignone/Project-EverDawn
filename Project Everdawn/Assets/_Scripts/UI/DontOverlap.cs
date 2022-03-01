using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontOverlap : MonoBehaviour
{
    private Canvas canvas;
    private Vector3 origPos;
    private CapsuleCollider col;

    private List<CapsuleCollider> colliders = new List<CapsuleCollider>();

    private void Start()
    {
        canvas = transform.GetChild(1).GetComponent<Canvas>();
        origPos = canvas.transform.localPosition;
        col = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        Vector3 changePosition = Vector3.zero;
        Vector3 colLocation = transform.position - col.center;
        foreach(CapsuleCollider collider in colliders) {
            Vector3 colliderLocation = collider.transform.position - collider.center;
            
            Vector3 direction = (colLocation - colliderLocation);
            direction = direction.normalized;
            changePosition += direction * (col.radius - Vector3.Distance(colliderLocation, colLocation)/2);
        }

        Debug.DrawRay(colLocation + new Vector3(0,10,0), changePosition, Color.red);
        canvas.transform.localPosition = origPos - changePosition;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.name == "Ability")
            colliders.Add((other as CapsuleCollider));
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.name == "Ability")
            colliders.Remove((other as CapsuleCollider));
    }
}
