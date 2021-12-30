using System;
using UnityEngine;

[System.Serializable]
public abstract class GameEvent<T> : ScriptableObject
{
	// Only this class can trigger the event
	public event Action<T> eventListeners = delegate {};

	public void Raise(T item)
	{
		eventListeners(item);
	}
}