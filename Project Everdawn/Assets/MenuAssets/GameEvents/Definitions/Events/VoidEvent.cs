using UnityEngine;

[CreateAssetMenu(fileName = "SO_NewVoidEvent", menuName = "ScriptableObjects/GameEvents/New Void Event")]
public class VoidEvent : GameEvent<Void>
{
    public void Raise() => Raise(new Void());
}