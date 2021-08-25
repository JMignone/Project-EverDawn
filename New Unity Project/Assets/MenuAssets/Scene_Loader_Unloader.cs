using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Loader_Unloader : MonoBehaviour
{
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
