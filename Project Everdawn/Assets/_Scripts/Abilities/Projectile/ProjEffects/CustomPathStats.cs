using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomPathStats
{
    [SerializeField]
    private bool hasCustomPath;

    [SerializeField]
    private List<Point> lerpPoints;

    [SerializeField]
    private float speedToLastPoint;

    [SerializeField]
    private float arcMultiplierToLastPoint;

    [SerializeField]
    private Mesh customCollider;

    [SerializeField]
    private Sprite customImage;

    [SerializeField]
    private Vector2 pivot;

    [SerializeField]
    private Vector2 sizeDelta;

    private GameObject proj;
    private float angle;

    private float speed;
    private float distanceCovered;
    private Vector3 arcStart;
    private Vector3 arcEnd;
    private Vector3 arcApex;
    private int index;

    public bool HasCustomPath
    {
        get { return hasCustomPath; }
    }

    public Mesh CustomCollider
    {
        get { return customCollider; }
    }

    public Sprite CustomImage
    {
        get { return customImage; }
    }

    public Vector2 Pivot
    {
        get { return pivot; }
    }

    public Vector2 SizeDelta
    {
        get { return sizeDelta; }
    }

    /*
        To figure out what arcMultiplier to use, 
        1) Decide if the arc will go left or right from the middle. Left is positive, right is negative.
        2) Figure out how far away the two endpoints are from eachother. Set this equal to d.
        3) How far from the middle of the two endpoints will the apex be? Set this equal to y.
        Once this is figured out, arcMultiplier = (2*y)/d
    */
    public void StartStats(GameObject go, Vector3 targetLocation) {
        if(hasCustomPath) {
            proj = go;

            if((proj.GetComponent(typeof(Projectile)) as Projectile).Caster.Mirrored) {
                arcMultiplierToLastPoint *= -1;
                foreach(Point point in lerpPoints) {
                    point.P = new Vector3(point.P.x, point.P.y, -point.P.z);
                    point.arcMultiplier *= -1;
                }
            }

            angle = Vector3.SignedAngle((targetLocation - go.transform.position).normalized, Vector3.forward, Vector3.up) * Mathf.Deg2Rad;
            arcStart = go.transform.position;
            if(lerpPoints.Count != 0) {
                arcEnd = new Vector3(Mathf.Cos(angle) * lerpPoints[0].point.x - Mathf.Sin(angle) * lerpPoints[0].point.z + go.transform.position.x,
                                     0,
                                     Mathf.Sin(angle) * lerpPoints[0].point.x + Mathf.Cos(angle) * lerpPoints[0].point.z + go.transform.position.z);
                arcApex = arcStart + (arcEnd - arcStart)/2 + 
                    Vector3.Cross(arcEnd - arcStart, Vector3.up).normalized * Vector3.Distance(arcStart, arcEnd) * lerpPoints[0].arcMultiplier;
                speed = lerpPoints[0].speed;
            }
            else {
                arcEnd = targetLocation;
                arcApex = arcStart + (arcEnd - arcStart)/2 + 
                    Vector3.Cross(arcEnd - arcStart, Vector3.up).normalized * Vector3.Distance(arcStart, arcEnd) * arcMultiplierToLastPoint;
                speed = speedToLastPoint;
            }
        }
    }

    public void UpdateStats(Vector3 endLocation) {
        Vector3 destination = arcEnd;
        if(index >= lerpPoints.Count) 
            destination = endLocation;

        distanceCovered += speed/Vector3.Distance(arcStart, destination) * Time.deltaTime;
        //distanceCovered += (speed * Time.deltaTime);

        if(proj.transform.position != destination) {
            Vector3 m1 = Vector3.Lerp(arcStart, arcApex, distanceCovered);
            Vector3 m2 = Vector3.Lerp(arcApex, destination, distanceCovered);

            Vector3 direction = Vector3.Lerp(m1, m2, distanceCovered) - proj.transform.position;
            if(direction != Vector3.zero)
                proj.transform.GetChild(0).GetChild(0).rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
            proj.transform.position = Vector3.Lerp(m1, m2, distanceCovered);
        }
        else {
            index++;
            if(index < lerpPoints.Count) {
                arcStart = destination;
                arcEnd = new Vector3(Mathf.Cos(angle) * lerpPoints[index].point.x - Mathf.Sin(angle) * lerpPoints[index].point.z + proj.transform.position.x,
                                     0,
                                     Mathf.Sin(angle) * lerpPoints[index].point.x + Mathf.Cos(angle) * lerpPoints[index].point.z + proj.transform.position.z);
                arcApex = arcStart + (arcEnd - arcStart)/2 + 
                    Vector3.Cross(arcEnd - arcStart, Vector3.up).normalized * Vector3.Distance(arcStart, arcEnd) * lerpPoints[index].arcMultiplier;
                speed = lerpPoints[index].speed;
            }
            else {
                arcStart = arcEnd;
                arcEnd = endLocation;
                arcApex = arcStart + (arcEnd - arcStart)/2 + 
                    Vector3.Cross(arcEnd - arcStart, Vector3.up).normalized * Vector3.Distance(arcStart, endLocation) * arcMultiplierToLastPoint;
                speed = speedToLastPoint;
            }
            distanceCovered = 0;
        }
    }

    [System.Serializable]
    class Point {
        [SerializeField]
        public Vector3 point;
        [SerializeField]
        public float speed;
        [SerializeField] [Min(.1f)]
        public float arcMultiplier;

        public Vector3 P {
            get { return point; }
            set { point = value; }
        }

        public float ArcMultiplier {
            get { return arcMultiplier; }
            set { arcMultiplier = value; }
        }
    }
}
