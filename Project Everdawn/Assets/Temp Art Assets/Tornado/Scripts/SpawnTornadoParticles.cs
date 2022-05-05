using UnityEngine;

public class SpawnTornadoParticles : MonoBehaviour
{
    [SerializeField]
    private GameObject particle;

    private void Start()
    {
        Instantiate(particle, gameObject.transform.position, new Quaternion(0, 0, 0, 1));
    }
}
