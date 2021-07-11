using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IDamageable
{
    BaseStats Stats { get; }
    List<GameObject> HitTargets { get; }
    GameObject Target { get; set; }
    int InRange { get; set; }
    Actor3D Agent { get; }
    Image AbilityIndicator { get; }
    int IndicatorNum { get; set;}
    bool IsHoveringAbility { get; }

    void TakeDamage(float amount);
} 
