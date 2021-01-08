using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabManager : MonoBehaviour
{
    public GameObject[] Tabs; //game objects representing computer tabs
    public GameObject[] TabIndicators; //game objects representing tab ui to be turned off for active tab
    public GameObject InventoryButton; //inventory button to deactivate on ritual window
    public InventoryDisplay InventoryTabDisplay; //redraw when tabbin away from ritual
    public GameObject ExitButton; //button used to leave computer

    private int currentTab = 0; //indicated tab currently using
    private int tabCount = 3;

    // Triggers once at start
    void Start()
    {
        // remove stream tab for day 0
        if(Setting.currentDay == 0)
        {
            Tabs[2].SetActive(false);
            TabIndicators[2].SetActive(false);
            tabCount--;
        } else if(Setting.currentDay == 1)
        {
            //activate ritual tab
            ChangeTabTo(2);
            //disable non-ritual tabs on day 2
            Tabs[0].SetActive(false);
            TabIndicators[0].SetActive(false);
            Tabs[1].SetActive(false);
            TabIndicators[1].SetActive(false);
            InputManager.OnInputHit[(int)InputManager.AxisEnum.Tab] -= ChangeTab;
            //dont go outside til you finished your summoning young man
            ExitButton.SetActive(false);
        }
    }

    // Called whenever enabled
    void OnEnable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Tab] += ChangeTab;
    }

    // Called whenever disabled
    void OnDisable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Tab] -= ChangeTab;
    }

    // Switch between tabs on computer
    public void ChangeTab(float axisValue)
    {
        //deactivate current tab
        Tabs[currentTab].SetActive(false);
        TabIndicators[currentTab].SetActive(true);
        //if that tab was ritual, restore the inventory button
        if(currentTab == 2) 
        {
            OnCloseStream();
        }
        //find new tab
        if(axisValue > 0)
        {
            currentTab++;
            if(currentTab == tabCount) currentTab = 0;
        } else
        {
            currentTab --;
            if(currentTab < 0) currentTab = tabCount - 1;
        }
        //activate tab
        Tabs[currentTab].SetActive(true);
        TabIndicators[currentTab].SetActive(false);
        //if the new tab is ritual, remove the inventory button
        if(currentTab == 2) InventoryButton.SetActive(false);
    }

    // Switch between tabs on computer
    public void ChangeTabTo(int tabNumber)
    {
        if(tabNumber != currentTab)
        {
            //deactivate current tab
            Tabs[currentTab].SetActive(false);
            TabIndicators[currentTab].SetActive(true);
            //if that tab was ritual, restore the inventory button
            if(currentTab == 2)
            {
                OnCloseStream();
            }
            //change active tab
            currentTab = tabNumber;
            //activate tab
            Tabs[currentTab].SetActive(true);
            TabIndicators[currentTab].SetActive(false);
            //if the new tab is ritual, remove the inventory button
            if(currentTab == 2) InventoryButton.SetActive(false);
        }
    }

    // Clear ritual window and restore the inventory tab button when tabbing off-stream
    void OnCloseStream()
    {
        //restore the inventory tab button
        InventoryButton.SetActive(true);
        InventoryTabDisplay.RedrawUI();
    }

    // Makes all tabs accessible
    public void EnableAllTabs()
    {
        for(int i = 0; i < TabIndicators.Length; i++)
        {
            if(i != currentTab) TabIndicators[i].SetActive(true);
        }
        tabCount = TabIndicators.Length;
        ExitButton.SetActive(true);
    }
}
