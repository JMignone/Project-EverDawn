using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArenaDisplay : MonoBehaviour
{
    [Header("Arena")]
    public SO_Arena Arena;

    [Header("Bound Data")]
    public int ArenaID;
    [SerializeField] private TMP_Text arenaName;
    [SerializeField] private Image arenaImage;
    [SerializeField] private TMP_Text arenaRank;

    [Header("Click Event Settings")]
    [SerializeField] private ArenaDisplayEvent arenaDisplayEvent;

    void Awake()
    {
        BindArenaData(Arena);
    }

    public void BindArenaData(SO_Arena arena)
    {
        if(arena != null) // Check that there is an object to bind the data from
        {
            // Bind the data from the scriptable object
            Arena = arena;
            ArenaID = arena.ArenaID;
            arenaName.text = arena.ArenaName;
            arenaImage.sprite = arena.ArenaImage;
            arenaRank.text = arena.ArenaRank.ToString();
        }
        else
        {
            //Debug.Log("No arena found");
        }
    }

    public void RaiseArenaAsEvent()
    {
        if(arenaDisplayEvent != null)
        {
            arenaDisplayEvent.Raise(this);
        }
    }

    private SO_Arena RetrieveArenaFromDisplay(ArenaDisplay arenaDisplay)
    {
        SO_Arena result;

        if(arenaDisplay != null)
        {
            result = arenaDisplay.Arena;
        }
        else
        {
            result = null;
        }

        return result;
    }

    public void BindArenaFromDisplayEvent(ArenaDisplay arenaDisplay)
    {
        BindArenaData(RetrieveArenaFromDisplay(arenaDisplay));
    }
}