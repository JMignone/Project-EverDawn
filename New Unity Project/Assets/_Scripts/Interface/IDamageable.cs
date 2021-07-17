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
    Actor2D UnitSprite { get; }
    Image AbilityIndicator { get; }
    int IndicatorNum { get; set; }
    bool IsHoveringAbility { get; set; }
    bool IsCastingAbility { get; set; }

    void TakeDamage(float amount);

} 
