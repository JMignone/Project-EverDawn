using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This and UI_Tab_Button are largely based off of the work of Game Dev Guide on YouTube https://www.youtube.com/watch?v=211t6r12XPQ&ab_channel=GameDevGuide

public class UI_Tab_Group : MonoBehaviour
{
    public List<UI_Tab_Button> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public UI_Tab_Button selectedTab;
    public List<GameObject> objectsToSwap;

    public void Tab_Button_Subscribe(UI_Tab_Button button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<UI_Tab_Button>();
        }

        tabButtons.Add(button);
    }

    public void On_Tab_Enter(UI_Tab_Button button)
    {
        Reset_Tabs();
        if(selectedTab == null || button != selectedTab)
        {
            button.background.sprite = tabHover;
        }
    }

    public void On_Tab_Exit(UI_Tab_Button button)
    {
        Reset_Tabs();
    }

    public void On_Tab_Selected(UI_Tab_Button button)
    {
        if(selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = button;

        selectedTab.Select();

        Reset_Tabs();
        button.background.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();
        for(int i=0; i<objectsToSwap.Count; i++)
        {
            if(i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void Reset_Tabs()
    {
        foreach(UI_Tab_Button button in tabButtons)
        {
            if(selectedTab != null && button == selectedTab)
            {
                continue;
            }
            button.background.sprite = tabIdle;
        }
    }
}
