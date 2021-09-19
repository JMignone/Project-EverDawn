using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Loader_Unloader : MonoBehaviour
{
    /* This class/attached object will likely be turned into a singleton later, so this is just some framework for later.
    public static Scene_Loader_Unloader scene_control;

    //Awake is called before start(), this is here so the script makes the object act as a singleton which is not destroyed when a new scene is loaded.
    private void Awake()
    {
        if (scene_control == null)
        {
            DontDestroyOnLoad(gameObject);
            scene_control = this;
        }

        else if (scene_control != this)
        {
            Destroy(gameObject);
        }
    }
    */

    //Recieve a scene's name as a string to load
    public void LoadScene(string SceneName)
    {
        //Load the given scene and unload all other scenes
        SceneManager.LoadSceneAsync(SceneName);
    }

    //Recieve a scene's name as a string to load additively
    public void LoadSceneAdditive(string SceneName)
    {
        //Load the given scene without unloading any other scenes
        SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
    }

    //Recieve a scene's name as a string to unload
    public void UnloadScene(string SceneName)
    {
        //Unload the given scene
        SceneManager.UnloadSceneAsync(SceneName);
    }
   
}
