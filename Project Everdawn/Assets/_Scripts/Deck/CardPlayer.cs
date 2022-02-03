using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPlayer : MonoBehaviour
{
    private List<GameObject> prefab;
    private List<float> previewDelays;
    private Vector3 targetLocation;
    private int unitIndex;
    private string playerTag;

    private float currentDelay;
    private int currentProjectileIndex;

    public List<GameObject> Prefab
    {
        set { prefab = value; }
    }

    public List<float> PreviewDelays
    {
        set { previewDelays = value; }
    }

    public Vector3 TargetLocation
    {
        set { targetLocation = value; }
    }

    public int UnitIndex
    {
        set { unitIndex = value; }
    }

    public string PlayerTag
    {
        set { playerTag = value; }
    }

    private void FixedUpdate()
    {
        if(currentDelay < previewDelays[currentProjectileIndex]) //if we havnt reached the delay yet
            currentDelay += Time.deltaTime;
        else { //if we completed a delay
            Vector3 direction = new Vector3(0,0,1);
            if(playerTag == "Enemy")
                direction.z = -1;

            if(currentProjectileIndex == unitIndex)
                GameFunctions.SpawnUnit(prefab[currentProjectileIndex], GameManager.GetUnitsFolder(), targetLocation, playerTag);
            else if(prefab[currentProjectileIndex].GetComponent<Projectile>())
                GameFunctions.FireProjectile(prefab[currentProjectileIndex], targetLocation, targetLocation, direction, null, playerTag, 1);
            else if(prefab[currentProjectileIndex].GetComponent<CreateAtLocation>())
                GameFunctions.FireCAL(prefab[currentProjectileIndex], targetLocation, targetLocation, direction, null, playerTag, 1);
            currentDelay = 0;
            currentProjectileIndex++;
            if(currentProjectileIndex == previewDelays.Count)//if we completed the last delay
                Destroy(gameObject);
        }
    }
}
