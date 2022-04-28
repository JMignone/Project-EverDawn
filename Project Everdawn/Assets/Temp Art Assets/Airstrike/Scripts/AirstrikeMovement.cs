using UnityEngine;

public class AirstrikeMovement : MonoBehaviour
{
    [SerializeField]
    private Vector3 movementVector;

    private void Update()
    {
        gameObject.transform.position += Vector3.Scale(movementVector, gameObject.transform.forward) * Time.deltaTime;
    }
}
