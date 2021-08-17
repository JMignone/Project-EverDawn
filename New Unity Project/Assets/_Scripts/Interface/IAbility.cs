using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    float Range { get; }
    int AreaMask();
    GameConstants.OBJECT_ATTACKABLE ObjectAttackable { get; }
    void ApplyAffects(Component damageable);
}
