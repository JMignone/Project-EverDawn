using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    Vector3 Position();
    float Range { get; set; }
    bool AbilityControl { get; }
    float DamageMultiplier { get; }
    int AreaMask();
    PullStats PullStats { get; }
    GameConstants.HEIGHT_ATTACKABLE HeightAttackable { get; }
    GameConstants.TYPE_ATTACKABLE TypeAttackable { get; }
    void ApplyAffects(Component damageable);
}
