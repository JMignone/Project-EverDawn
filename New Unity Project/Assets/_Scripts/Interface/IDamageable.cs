using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    BaseStats Stats { get; }
    List<GameObject> HitTargets { get; }
    GameObject Target { get; set; }
    int InRange { get; set; }
    Actor3D Agent { get; }

    void TakeDamage(float amount);
} 
