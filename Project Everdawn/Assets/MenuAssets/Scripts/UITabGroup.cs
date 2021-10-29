using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This and UITabButton are largely based off of the work of Game Dev Guide on YouTube https://www.youtube.com/watch?v=211t6r12XPQ&ab_channel=GameDevGuide

public class UITabGroup : MonoBehaviour
{
    public List<UITabButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public UITabButton selectedTab;
    public List<GameObject> objectsToSwap;

    public void TabButtonSubscribe(UITabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<UITabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(UITabButton button)
    {
        ResetTabs();
        if(selectedTab == null || button != selectedTab)
        {
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(UITabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(UITabButton button)
    {
        if(selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = button;

        selectedTab.Select();

        ResetTabs();
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

    public void ResetTabs()
    {
        foreach(UITabButton button in tabButtons)
        {
            if(selectedTab != null && button == selectedTab)
            {
                continue;
            }
            button.background.sprite = tabIdle;
        }
    }
}
