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
        canvas = transform.GetChild(0).GetComponent<Canvas>();
        origPos = canvas.transform.localPosition;
        col = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        Vector3 changePosition = Vector3.zero;
        Vector3 colLocation = transform.position - col.center;

        for(int i=0; i<colliders.Count;i++) {
            if(colliders[i] == null) {
                colliders.RemoveAt(i);
                continue;
            }
            Vector3 colliderLocation = colliders[i].transform.position - colliders[i].center;
            
            Vector3 direction = (colLocation - colliderLocation);
            direction = direction.normalized;
            changePosition += direction * (col.radius - Vector3.Distance(colliderLocation, colLocation)/2);
        }

        Debug.DrawRay(colLocation + new Vector3(0,10,0), changePosition, Color.red);
        canvas.transform.localPosition = origPos - changePosition;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.gameObject.name == "Ability")
            colliders.Add((other as CapsuleCollider));
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.transform.gameObject.name == "Ability")
            colliders.Remove((other as CapsuleCollider));
    }
}
