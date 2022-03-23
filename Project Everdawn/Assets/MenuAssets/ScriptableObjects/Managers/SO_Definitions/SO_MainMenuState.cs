using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[CreateAssetMenu(fileName = "SO_NewMainMenuState", menuName = "ScriptableObjects/New Main Menu State")]
public class SO_MainMenuState : ScriptableObject
{
    [SerializeField] private bool devComToggle;
    [SerializeField] private bool quickAccessToggle;
    [SerializeField] private int backgroundCharacter;

    /*
    public void SetDevComToggle(bool DevComToggle)
    {
        PlayerPrefs.SetInt("DevComToggle", (DevComToggle ? 1 : 0));
        DevComToggle = (PlayerPrefs.GetInt("DevComToggle") != 0);
        Debug.Log(DevComToggle.ToString());
        devComToggle = DevComToggle;
    }
    */

    /*
    public void SetDevComToggle(bool beans)
    {
        if(beans == true)
        {
            devComToggle = true;
            Debug.Log("On");
        }
        else if(beans == false)
        {
            devComToggle = false;
            Debug.Log("Off");
        }
    }
    */
}
