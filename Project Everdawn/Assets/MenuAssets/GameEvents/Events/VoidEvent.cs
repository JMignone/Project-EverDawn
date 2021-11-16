using UnityEngine;

[CreateAssetMenu(fileName = "SO_NewVoidEvent", menuName = "ScriptableObjects/GameEvents/NewVoidEvent")]
public class VoidEvent : GameEvent<Void>
{
    public void Raise() => Raise(new Void());
}