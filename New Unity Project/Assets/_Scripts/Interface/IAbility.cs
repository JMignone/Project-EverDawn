using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    GameConstants.OBJECT_ATTACKABLE ObjectAttackable { get; }
    bool WillHit(Component damageable);
    void applyAffects(Component damageable);
}
