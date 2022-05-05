using UnityEngine;

public class SpawnAirstrikePrefab : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private float zOffset;
    [SerializeField]
    private Projectile projectileScript;

    private void Start()
    {
        Vector3 spawnLocation = new Vector3(projectileScript.TargetLocation.x, 40, projectileScript.TargetLocation.z + (zOffset * gameObject.transform.forward.z));

        Instantiate(prefab, spawnLocation, gameObject.transform.rotation);
    }
}
