using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnStats
{
    [SerializeField]
    private GameObject unitToSpawn;

    public GameObject UnitToSpawn
    {
        get { return unitToSpawn; }
    }
}
