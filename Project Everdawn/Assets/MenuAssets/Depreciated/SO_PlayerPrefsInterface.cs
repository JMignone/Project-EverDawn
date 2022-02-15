using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SO_PlayerPrefsInterface", menuName = "ScriptableObjects/PlayerPrefsInterface")]
public class SO_PlayerPrefsInterface : ScriptableObject
{
    public void SetPlayerPrefFloat(string keyName, float value)
    {
        PlayerPrefs.SetFloat(keyName, value);
        PlayerPrefs.Save();
    }

    public float GetPlayerPrefFloat(string keyName)
    {
        return PlayerPrefs.GetFloat(keyName);
    }

    public void SetPlayerPrefInt(string keyName, int value)
    {
        PlayerPrefs.SetInt(keyName, value);
        PlayerPrefs.Save();
    }

    public int GetPlayerPrefInt(string keyName)
    {
        return PlayerPrefs.GetInt(keyName);
    }
    
    public void SetPlayerPrefString(string keyName, string value)
    {
        PlayerPrefs.SetString(keyName, value);
        PlayerPrefs.Save();
    }

    public string GetPlayerPrefString(string keyName)
    {
        return PlayerPrefs.GetString(keyName);
    }
}
