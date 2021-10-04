using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardStats
{
    [SerializeField]
    private int index;
    [SerializeField]
    private string name;
    [SerializeField]
    private Sprite icon;
    [SerializeField] [Range(0,10)]
    private int cost;
    [SerializeField]
    private GameConstants.SPAWN_ZONE_RESTRICTION spawnZoneRestrictions;
    [SerializeField]
    private List<GameObject> prefab;
    [SerializeField]
    private GameObject previewPrefab;
    [SerializeField]
    private List<float> previewDelays;
    [Tooltip("The index of the unit in the lists, must match or be -1, meaning there is no unit")]
    [SerializeField]
    private int unitIndex;
    //[SerializeField] uncomment for debug purposes
    private int layoutIndex;

    public int Index
    {
        get { return index; }
        set { index = value; }
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public Sprite Icon
    {
        get { return icon; }
        set { icon = value; }
    }

    public int Cost
    {
        get { return cost; }
        set { cost = value; }
    }

    public GameConstants.SPAWN_ZONE_RESTRICTION SpawnZoneRestrictions
    {
        get { return spawnZoneRestrictions; }
        set { spawnZoneRestrictions = value; }
    }

    public List<GameObject> Prefab
    {
        get { return prefab; }
        set { prefab = value; }
    }

    public GameObject PreviewPrefab
    {
        get { return previewPrefab; }
        set { previewPrefab = value; }
    }

    public List<float> PreviewDelays
    {
        get { return previewDelays; }
        set { previewDelays = value; }
    }

    public int UnitIndex
    {
        get { return unitIndex; }
        set { unitIndex = value; }
    }

    public int LayoutIndex
    {
        get { return layoutIndex; }
        set { layoutIndex = value; }
    }
}
