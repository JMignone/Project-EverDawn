using System.Collections;
using UnityEngine;

public class DestroySelfOnDelay : MonoBehaviour
{
    [SerializeField]
    private float delayInSeconds;

    private void Start()
    {
        StartCoroutine(WaitToDestroy());
    }

    private IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(delayInSeconds);

        Destroy(gameObject);
    }
}
