using UnityEngine;

public class SpawnFreezeParticles : MonoBehaviour
{
    [SerializeField]
    private GameObject horizontalParticles;
    [SerializeField]
    private GameObject verticalParticles;

    private void Start()
    {
        Instantiate(horizontalParticles, new Vector3(0, 0, gameObject.transform.position.z), new Quaternion(0, 0, 0, 1));
        Instantiate(verticalParticles, new Vector3(gameObject.transform.position.x, 0, 0), new Quaternion(0, 0, 0, 1));
    }
}
