using System.Collections;
using UnityEngine;

public class RepeatedlyInstantiate : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectsToInstantiate;
    [SerializeField] [Tooltip("How long to wait after the previous object is instantiated")]
    private float[] instantiationDelays;

    [SerializeField]
    private bool destroyWhenFinished = true;
    [SerializeField]
    private float destroyDelay = 1f;

    private void Start()
    {
        // check that all arrays are the same length
        if (objectsToInstantiate.Length != instantiationDelays.Length)
        {
            // log error and remove script to avoid bugs
            Debug.LogError("Instantiation data arrays are not equal in length. Removing functionality. Gameobject ID: " + gameObject.GetInstanceID().ToString());
            Destroy(this);
        }

        StartCoroutine(InstantiateRepeatedly());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator InstantiateRepeatedly()
    {
        // instantiate each prefab in order, with each delay starting after the previous object is instantiated
        for (int i = 0, l = objectsToInstantiate.Length; i < l; i++)
        {
            if (instantiationDelays[i] == 0)
            {
                Instantiate(objectsToInstantiate[i], gameObject.transform);
            }
            else
            {
                yield return new WaitForSeconds(instantiationDelays[i]);
                Instantiate(objectsToInstantiate[i], gameObject.transform);
            }
        }

        if (destroyWhenFinished)
        {
            yield return new WaitForSeconds(destroyDelay);
            Destroy(gameObject);
        }
    }
}
