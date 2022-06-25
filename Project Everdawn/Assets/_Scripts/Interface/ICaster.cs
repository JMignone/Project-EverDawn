using System.Collections.Generic;
using UnityEngine;

public interface ICaster
{
    bool Mirrored { get; }
    bool PauseFiring { get; set; }
    bool ExitOverride { get; set; }
    int SkipOverride { get; set; }
    IDamageable UnitSummon { get; set; }
    AbilityUI AbilityUI { get; }
    Canvas AbilityPreviewCanvas { get; }
    void SetNewLocation(Vector3 newLocation, Vector3 newDirection); //will be used by LocationStats
    void SetNewTarget(IDamageable newTarget);
}
