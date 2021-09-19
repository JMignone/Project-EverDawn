using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// This and UI_Tab_Group are largely based off of the work of Game Dev Guide on YouTube https://www.youtube.com/watch?v=211t6r12XPQ&ab_channel=GameDevGuide

[RequireComponent(typeof(Image))]
public class UI_Tab_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public UI_Tab_Group tabGroup;

    public Image background;
    /*
    public Color idleColor = Color.white;
    public Color highlightedColor = Color.white;
    public Color pressedColor = Color.white;
    public Color selectedColor = Color.white;
    */

    public bool firstSelected = false;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    // Start is called before the first frame update
    void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Tab_Button_Subscribe(this);
        if (firstSelected)
        {
            tabGroup.On_Tab_Selected(this);
        }
    }

    public void OnPointerEnter(PointerEventData EventData)
    {
        tabGroup.On_Tab_Enter(this);
    }

    public void OnPointerExit(PointerEventData EventData)
    {
        tabGroup.On_Tab_Exit(this);
    }

    public void OnPointerClick(PointerEventData EventData)
    {
        tabGroup.On_Tab_Selected(this);
    }

    public void Select()
    {
        if(onTabSelected != null)
        {
            onTabSelected.Invoke();
        }
    }

    public void Deselect()
    {
        if (onTabDeselected != null)
        {
            onTabDeselected.Invoke();
        }
    }
}
