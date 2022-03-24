using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArenaStateController", menuName = "ScriptableObjects/StateMachines/Controllers/ArenaStateController")]
public class ArenaStateController : BaseStateController
{
    [SerializeField] private ArenaState defaultArenaState;
    [SerializeField] [Tooltip("Used to check through possible states to load after application quit")]
        private List<ArenaState> states;
    [SerializeField] private SO_SceneLoading sceneLoading;

    private readonly string selectedArena = "selectedArena";

    private void Awake()
    {
        int prevArenaID = LoadArenaNumberFromPlayerPrefs();
        foreach(ArenaState arenaState in states)
        {
            if(prevArenaID != arenaState.Arena.ArenaID)
            {
                continue;
            }
            else if(prevArenaID == arenaState.Arena.ArenaID)
            {
                ChangeState(arenaState);
            }
        }
    }

    private void OnDisable()
    {
        ArenaState arenaState = (ArenaState)this.currentState;
        if(arenaState is ArenaState)
        {
            //Debug.Log(arenaState.Arena.ArenaID.ToString() + " is currentState on shutdown");
            SaveArenaNumberToPlayerPrefs(arenaState.Arena.ArenaID);
        }
    }

    private void SaveArenaNumberToPlayerPrefs(int arenaNumber)
    {
        //Debug.Log("Saving " + arenaState.Arena.ArenaID.ToString() + " as selected arena before shutdown");
        PlayerPrefs.SetInt(selectedArena, arenaNumber);
    }

    private int LoadArenaNumberFromPlayerPrefs()
    {
        int result;

        if(PlayerPrefs.HasKey(selectedArena))
        {
            result = PlayerPrefs.GetInt(selectedArena);
        }
        else
        {
            result = defaultArenaState.Arena.ArenaID;
        }
        //Debug.Log("Loading " + result.ToString() + " as selected arena from before shutdown");
        return result;
    }

    public void LoadCurrentArenaStateAsScene()
    {
        ArenaState state = (ArenaState)currentState;
        sceneLoading.LoadScene(state.Arena.ArenaSceneName);
    }

    public void ChangeStateFromArenaDisplay(ArenaDisplay arenaDisplay)
    {
        int arenaID = arenaDisplay.ArenaID;
        foreach(ArenaState arenaState in states)
        {
            if(arenaID != arenaState.Arena.ArenaID)
            {
                continue;
            }
            else if(arenaID == arenaState.Arena.ArenaID)
            {
                ChangeState(arenaState);
            }
        }
    }
}
