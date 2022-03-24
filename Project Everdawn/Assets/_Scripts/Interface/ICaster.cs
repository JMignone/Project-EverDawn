using System.Collections.Generic;
using UnityEngine;

public interface ICaster
{
    bool PauseFiring { get; set; }
    bool ExitOverride { get; set; }
    int SkipOverride { get; set; }
    void SetNewLocation(Vector3 newLocation, Vector3 newDirection); //will be used by LocationStats
    void SetNewTarget(Actor3D newTarget);
}
