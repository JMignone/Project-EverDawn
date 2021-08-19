using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    Vector3 Position();
    float Range { get; }
    int AreaMask();
    PullStats PullStats { get; }
    GameConstants.OBJECT_ATTACKABLE ObjectAttackable { get; }
    void ApplyAffects(Component damageable);
}
