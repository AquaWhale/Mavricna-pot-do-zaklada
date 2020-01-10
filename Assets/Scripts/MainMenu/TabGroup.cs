using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    private List<TabButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabActive;
    public Color tabHoverColor;
    public Color tabIdleColor;
    private TabButton activeTab;
    public List<GameObject> contentsOfTabs;
    
    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(button);
    }

    public bool OnTabEnter (TabButton button)
    {
        ResetTabs();
        //spremenimo hover over barvo samo, če ni active tab
        if (activeTab == null || button != activeTab)
        {
            button.background.sprite = tabIdle;
            button.background.color = tabHoverColor;
            return false;
        }
        return true;

    }

    public void OnTabExit (TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected (TabButton button)
    {
        activeTab = button;
        ResetTabs();
        button.background.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < contentsOfTabs.Count; i++)
        {
            if (i == index)
            {
                contentsOfTabs[i].SetActive(true);
            }
            else
            {
                contentsOfTabs[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (TabButton tab in tabButtons)
        {
            if (activeTab != null && activeTab == tab)
            {
                continue;
            }
            tab.background.sprite = tabIdle;
            tab.background.color = tabIdleColor;
        }
    }
}
