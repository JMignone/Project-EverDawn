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
    [SerializeField]
    private int cost;
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private GameObject previewPrefab;
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

    public GameObject Prefab
    {
        get { return prefab; }
        set { prefab = value; }
    }

    public GameObject PreviewPrefab
    {
        get { return previewPrefab; }
        set { previewPrefab = value; }
    }

    public int LayoutIndex
    {
        get { return layoutIndex; }
        set { layoutIndex = value; }
    }
}
