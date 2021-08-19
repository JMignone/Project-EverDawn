using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PullStats
{
    [SerializeField]
    private bool canPull;

    [SerializeField]
    private float radius;

    [SerializeField]
    private float speed;

    public bool CanPull
    {
        get { return canPull; }
    }

    public float Radius
    {
        get { return radius; }
    }

    public float Speed
    {
        get { return speed; }
    }

    public void StartPullStats(GameObject go) {
        GameObject pullGo = new GameObject();
        pullGo.name = "PullBox";
        pullGo.tag = "Pull";

        SphereCollider pullBox = pullGo.AddComponent<SphereCollider>();
        pullBox.radius = radius;
        pullBox.center = new Vector3(0, 0, 0);
        pullBox.enabled = true;

        pullGo.SetActive(true);
        pullGo.transform.SetParent(go.transform.GetChild(0));
        pullGo.transform.localPosition = Vector3.zero;
    }
}
