using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IDamageable
{
    BaseStats Stats { get; }
    List<GameObject> HitTargets { get; }
    List<GameObject> InRangeTargets { get; }
    List<GameObject> EnemyHitTargets { get; }
    GameObject Target { get; set; }
    Actor3D Agent { get; }
    Actor2D UnitSprite { get; }
    ShadowStats ShadowStats { get; }
    bool IsMoving { get; }

    void TakeDamage(float amount);
    void SetTarget(GameObject newTarget);
} 
