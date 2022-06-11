using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KnockbackStats
{
    [SerializeField]
    private bool canKnockback;

    [SerializeField] [Min(0)]
    private float knockbackDuration;

    [SerializeField] [Min(0)]
    private float initialSpeed;

    [Tooltip("Direction will be in relation of the direction the projectile is moving")]
    [SerializeField]
    private bool angleOverride;

    [SerializeField] [Range(0,90)]
    private float angle;

    [Tooltip("Direction will reverse if a boomerang projectile is coming back. (ONLY WORKS IF ANGLEOVERRIDE IS SET)")]
    [SerializeField]
    private bool boomerangOverride;
    [Tooltip("Magnitude that the knockback speed changes when boomerang starts coming back (ONLY WORKS IF ANGLEOVERRIDE IS SET)")]
    [SerializeField]
    private bool speedChange;

    public bool CanKnockback
    {
        get { return canKnockback; }
    }

    public float KnockbackDuration
    {
        get { return knockbackDuration; }
    }

    public float InitialSpeed
    {
        get { return initialSpeed; }
    }

    public Vector3? Direction(Vector3 Pos, Vector3 unitPos, Vector3 direction, bool goingBack) {
        if(!angleOverride)
            return null;
        
        float newAngle = angle;
        if(Vector3.Cross((Pos - unitPos).normalized, direction).y < 0){
            newAngle = -angle;
            Debug.Log("ASDASD");
        }
        
        if(boomerangOverride && goingBack) {
            newAngle += 180.0f;
        }

         Debug.Log(newAngle);

        Vector3 newDir = Quaternion.AngleAxis(newAngle, Vector3.up) * direction;
        newDir.y = 0;
        return newDir.normalized;
    }
}
