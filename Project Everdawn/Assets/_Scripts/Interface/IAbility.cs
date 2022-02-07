using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    Vector3 Position();
    float Range { get; set; }
    Vector3 TargetLocation { get; set; }
    bool SetHit { get; set; }
    bool AbilityControl { get; }
    bool HideRange { get; }
    bool HidePreview { get; }
    float DamageMultiplier { get; }
    int AreaMask();
    PullStats PullStats { get; }
    CritStats CritStats { get; }
    GameConstants.HEIGHT_ATTACKABLE HeightAttackable { get; }
    GameConstants.TYPE_ATTACKABLE TypeAttackable { get; }
    void ApplyAffects(Component damageable);
}
