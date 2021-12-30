using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// This and UITabGroup are largely based off of the work of Game Dev Guide on YouTube https://www.youtube.com/watch?v=211t6r12XPQ&ab_channel=GameDevGuide

[RequireComponent(typeof(Image))]
public class UITabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public UITabGroup tabGroup;

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
        tabGroup.TabButtonSubscribe(this);
        if (firstSelected)
        {
            tabGroup.OnTabSelected(this);
        }
    }

    public void OnPointerEnter(PointerEventData EventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData EventData)
    {
        tabGroup.OnTabExit(this);
    }

    public void OnPointerClick(PointerEventData EventData)
    {
        tabGroup.OnTabSelected(this);
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
