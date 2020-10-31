using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class RitualNavigator : MonoBehaviour
{
    public static List<RitualNavigator> RitualNavigators = new List<RitualNavigator>(); //all instances of this script

    public InventoryDisplay InventoryBox; //the navigable inventory display on the side
    public GameObject Selector; //Object used to highlight selected panel
    public GameObject[] Panels; //Selectable panels for items etc.
    public int inventoryCutoff; //last index of inventory panels
    public int scrollTime; //time between scrolls
    public bool hasRitual = true; //is there literally even a ritual being done
    public Slider TrashMeter; //shows how long trash has been filled, inventorytab version only
    public float trashRate = .05f; //trash width per frame held
    public InventoryTab InventoryTabButton; //reference to the inventory tab button, used outside ritual window only


    private int selectedPanel = 0; //panel the selector is currently on
    private int currentScroll = 0; //number of rows scrolled down we are
    private int maxScroll; //max number of rows we can scroll down
    private int[] upRitualPanels = {0, 0, 1, 2, 8, 6, 7, 0, 0, 4}; //which panel is up from which within the ritual section
    private int[] downRitualPanels = {8, 2, 3, 4, 9, 4, 5, 6, 4, 9}; //which panel is down from which within the ritual section
    private int[] rightRitualPanels = {1, 2, 2, 2, 3, 4, 8, 0, 2, 9}; //which panel is right from which in the ritual view
    private int[] leftRitualPanels = {7, 0, 8, 4, 5, 6, -1, 6, 6, -1}; //which panel is left from each panel in the ritual view
    
    private int[] upPanelsLimited = {0, -1, -1, 8, -1, 8, -1, -1, 0, 5};
    private int[] downPanelsLimited = {8, -1, -1, 9, -1, 9, -1, -1, 5, 9};
    private int[] rightPanelsLimited = {3, -1, -1, 3, -1, 3, -1, -1, 3, 3};
    private int[] leftPanelsLimited = {5, -1, -1, 5, -1, -1, -1, -1, 5, 5};
    
    private int ritualEntryPanel; //panel to enter the ritual window into
    private int lastInventoryPanel = 2; //panel you left the inventory from
    private GameObject HeldItem; //Item we are holding
    private bool itemHeld = false; //are we holding an item
    private Vector3 heldItemOldPosition; //location to return the item to if we drop it
    private int heldItemIndex = -1; //index in the inventory of our held item
    private GameObject[] RitualItemIcons = new GameObject[9]; //icons for RitualItems
    private int[] RitualItemIndices = {-1, -1, -1, -1, -1, -1, -1, -1, -1}; //actual items used in the ritual
    private int[] RitualItemIds = {0, 0, 0, 0, 0, 0, 0, 0, 0}; //id of the item in each ritual panel
    private int scrollTimer; //frames until next scroll
    private bool catalystIsAlive = false; //is our catalyst a card

    // Start is called before the first frame update
    void Start()
    {
        maxScroll = (PlayerCharacter.Instance.Inventory.Count / InventoryBox.columnCount) - InventoryBox.rowCount + 1;
        RitualNavigators.Add(this);
        if(hasRitual == true && Calendar.currentDay < 10) //ARBITRARY DAY MUST BE CHANGED
        {
            //remove panels we've yet to unlock
            int[] lockedPanels = {1, 2, 4, 6, 7};
            foreach(int i in lockedPanels)
            {
                Panels[i + inventoryCutoff + 1].SetActive(false);
            }
            //change controls to account for less panels
            ritualEntryPanel = inventoryCutoff + 6;
            upRitualPanels = upPanelsLimited;
            downRitualPanels = downPanelsLimited;
            rightRitualPanels = rightPanelsLimited;
            leftRitualPanels = leftPanelsLimited;
        } else
        {
            ritualEntryPanel = inventoryCutoff + 7;
        }
    }

    // Activate input handlers when enabled
    void OnEnable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Horizontal] += ChangeSelectionHorizontal;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Vertical] += ChangeSelectionVertical;
        if(hasRitual == true)
        {
            InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] += OnConfirm;
            InputManager.OnInputHit[(int)InputManager.AxisEnum.Cancel] += OnCancel;
        } else
        {
            InputManager.OnInputHit[(int)InputManager.AxisEnum.Cancel] += InventoryTabButton.CloseInventory;
            InputManager.OnInput[(int)InputManager.AxisEnum.Trash] += OnTrash;
            InputManager.OnInputRelease[(int)InputManager.AxisEnum.Trash] += OnTrashRelease;
        }
    }

    // Deactivate input handlers when disabled
    void OnDisable()
    {
        //drop our item to avoid confusion
        if(itemHeld == true) DropItem();
        //then remove input events for this
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Horizontal] -= ChangeSelectionHorizontal;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Vertical] -= ChangeSelectionVertical;
        if(hasRitual == true)
        {
            InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] -= OnConfirm;
            InputManager.OnInputHit[(int)InputManager.AxisEnum.Cancel] -= OnCancel;
        } else
        {
            InputManager.OnInputHit[(int)InputManager.AxisEnum.Cancel] -= InventoryTabButton.CloseInventory;
            InputManager.OnInput[(int)InputManager.AxisEnum.Trash] -= OnTrash;
            InputManager.OnInputRelease[(int)InputManager.AxisEnum.Trash] -= OnTrashRelease;
        }
    }

    // Move selector right/left one
    void ChangeSelectionHorizontal(float axisValue)
    {
        //if(inMouseMode == true) return;
        float sign = (axisValue > 0)? 1 : -1;
        if(selectedPanel <= inventoryCutoff)
        {
            //move right/left normally if in the inventory
            if(sign > 0)
            {
                //move right
                if(selectedPanel % InventoryBox.columnCount == InventoryBox.columnCount - 1)
                {

                    //move to the ritual window if on the edge
                    EnterRitualWindow();
                } else
                {
                    //otherwise just move right one space within the inventory
                    selectedPanel++;
                    Selector.transform.position = Panels[selectedPanel].transform.position;
                }
            } else
            {
                //move left if possible
                if(selectedPanel % InventoryBox.columnCount != 0)
                {
                    selectedPanel--;
                    Selector.transform.position = Panels[selectedPanel].transform.position;
                }
            }
        } else
        {
            //if not in the inventory, move via predetermined ritual panel transitions
            int ritualIndex = GetRitualIndex(selectedPanel);
            ritualIndex = (sign > 0)? rightRitualPanels[ritualIndex] : leftRitualPanels[ritualIndex];
            //shift to last inventory panel if ritual panel was negative, otherwise modify it to fit the whole panel array
            selectedPanel = (ritualIndex >= 0)? ritualIndex + inventoryCutoff + 1 : lastInventoryPanel;
            Selector.transform.position = Panels[selectedPanel].transform.position;
        }
        
    }

    // Move selector up/down one
    void ChangeSelectionVertical(float axisValue)
    {
        float sign = (axisValue > 0)? 1 : -1;
        if(selectedPanel <= inventoryCutoff)
        {
        //move up/down normally if in the inventory
        if(sign > 0)
        {
            //move up
            if(selectedPanel < InventoryBox.columnCount)
            {
                //scroll items if at first row already
                if(currentScroll > 0)
                {
                    CloseItemHelp(selectedPanel);
                    currentScroll--;
                    InventoryBox.ItemsRoot.GetComponent<RectTransform>().localPosition += new Vector3(0, InventoryBox.ySpace, 0);
                }
            } else
            {
                //otherwise just move the selector up one row normally
                selectedPanel -= InventoryBox.columnCount;
                Selector.transform.position = Panels[selectedPanel].transform.position;
            }
        } else
        {
            //move down
            if(selectedPanel > inventoryCutoff - InventoryBox.columnCount)
            {
                //scroll items if in last row already
                if(currentScroll < maxScroll)
                {
                    CloseItemHelp(selectedPanel);
                    currentScroll++;
                    InventoryBox.ItemsRoot.GetComponent<RectTransform>().localPosition -= new Vector3(0, InventoryBox.ySpace, 0);
                }
            } else
            {
                //otherwise just move the selector down one row normally
                selectedPanel += InventoryBox.columnCount;
                Selector.transform.position = Panels[selectedPanel].transform.position;
            }
        }
        } else
        {
            //if not in the inventory, move to the specified ritual panel
            int ritualIndex = GetRitualIndex(selectedPanel);
            selectedPanel = (sign > 0)? upRitualPanels[ritualIndex] : downRitualPanels[ritualIndex];
            selectedPanel += inventoryCutoff + 1;
            Selector.transform.position = Panels[selectedPanel].transform.position;
        }
    }

    // Grab or place item
    public void OnConfirm(float axisValue)
    {
        //set flag to alter behaviour based on mouse usage
        bool viaClick = false;
        if(axisValue < 0) viaClick = true;
        //then determine what the confirm does based on context
        if(selectedPanel <= inventoryCutoff)
        {
            //if in the inventory, grab the item at the selected panel
            if(itemHeld == true)
            {
                //if we are holding an item, put it down first
                DropItem();
                if(GetItemIndexFromPanel(selectedPanel) == heldItemIndex)
                {
                    //return if we are on the tile we got it from
                    return;
                }
            }
            GrabItemFromInventory(viaClick);
        } else
        {
            //first get ritual index if in ritual area
            int ritualIndex = GetRitualIndex(selectedPanel);
            if(ritualIndex == RitualItemIds.Length)
            {
                //execute the ritual if we are on the execute button
                Summon();
            } else
            {
                //otherwise interact with the item slots in the ritual zone
                if(RitualItemIds[ritualIndex] > 0)
                {
                    //if an item already exists in that location, take it
                    if(itemHeld == true)
                    {
                        //if we are holding an item, put it down first
                        DropItem();
                    }
                    GrabItemFromRitual(ritualIndex, viaClick);
                } else
                {
                    //if no item exists we're good to put one there
                    if(itemHeld == true)
                    {
                        PlaceItem(ritualIndex);
                    }
                }
            }
        }
    }

    // Basically just drop whatever we got
    void OnCancel(float axisValue)
    {
        //cancel
        if(itemHeld == true)
        {
            //drop an item if we got it
            DropItem();
        }
        if(selectedPanel > inventoryCutoff)
        {
            //return to the inventory if in the ritual window
            selectedPanel = lastInventoryPanel;
            Selector.transform.position = Panels[selectedPanel].transform.position;
        }
    }

    // Increase trash meter until it fills then throw out our item
    void OnTrash(float axisValue)
    {
        //fill the bar
        TrashMeter.value += trashRate;
        if(TrashMeter.value >= 1)
        {
            //delete item once filled
            DeleteCurrentItem();
            //stop tracking this input until release
            InputManager.OnInput[(int)InputManager.AxisEnum.Trash] -= OnTrash;
            //return the meter to its old state
            TrashMeter.value = 0;
        }
    }

    // Reset trash meter when released
    void OnTrashRelease(float axisValue)
    {
        //reset meter
        TrashMeter.value = 0;
        //restore input
        InputManager.OnInput[(int)InputManager.AxisEnum.Trash] += OnTrash;
    }

    // Update is called once per frame
    void Update()
    {
        if(scrollTimer == 0)
        {
            //check mouse scroll 
            float mouseY = Input.mouseScrollDelta.y;
            if(Mathf.Abs(mouseY) > 0.05)
            {
                InventoryScroll(mouseY);
                scrollTimer = scrollTime;
            }
        } else
        {
            //count down to next scroll chance
            scrollTimer--;
        }
    }

    // Move the inventory up/ down based on sign
    public void InventoryScroll(float sign)
    {
        if(sign < 0)
        {
            if(currentScroll > 0)
            {
                CloseItemHelp(selectedPanel);
                currentScroll--;
                InventoryBox.ItemsRoot.GetComponent<RectTransform>().localPosition += new Vector3(0, InventoryBox.ySpace, 0);
            }
        } else if(currentScroll < maxScroll)
        {
            CloseItemHelp(selectedPanel);
            currentScroll++;
            InventoryBox.ItemsRoot.GetComponent<RectTransform>().localPosition -= new Vector3(0, InventoryBox.ySpace, 0);
        }
    }

    // Move selector into the ritual area, remembering which panel we came from
    void EnterRitualWindow()
    {
        //return if no ritual is present
        if(hasRitual == false) return; 
        //log the panel we left from to return later
        lastInventoryPanel = selectedPanel;
        //move into the ritual window via the entry panel
        selectedPanel = ritualEntryPanel;
        Selector.transform.position = Panels[selectedPanel].transform.position;
    }

    // Grab the item at the currently selected inventory panel
    void GrabItemFromInventory(bool viaClick)
    {
        //convert selected panel to inventory index
        heldItemIndex = GetItemIndexFromPanel(selectedPanel);
        //get the itemUI
        if(heldItemIndex < InventoryBox.ItemIcons.Count && InventoryDisplay.Inventory[heldItemIndex].count > 0)
        {
            //only take the item if it exists
            HeldItem = InventoryBox.ItemIcons[heldItemIndex];
        } else
        {
            //otherwise just return
            return;
        }
        //show that we grabbed the item
        OnHeldItemChange(viaClick);
        //move to the ritual window if triggered via button
        if(viaClick == false) EnterRitualWindow();
    }

    // Grab the item at the currently selected ritual panel
    void GrabItemFromRitual(int ritualIndex, bool viaClick)
    {
        //get inventory index
        heldItemIndex = RitualItemIndices[ritualIndex];
        //get the itemUI
        HeldItem = InventoryBox.ItemIcons[heldItemIndex];
        //activate just in case
        HeldItem.SetActive(true);
        //increment its count since we grabbed the lost one
        InventoryDisplay.Inventory[heldItemIndex].count++;
        HeldItem.GetComponentInChildren<TMP_Text>().text = "" + InventoryDisplay.Inventory[heldItemIndex].count;
        //show that we grabbed the item
        OnHeldItemChange(viaClick);
        //clear it from the ritual panel
        RitualItemIndices[ritualIndex] = -1;
        RitualItemIds[ritualIndex] = 0;
        Destroy(RitualItemIcons[ritualIndex]);
    }

    // Update variables to match our new held item, and show its been grabbed in UI
    void OnHeldItemChange(bool viaClick)
    {
        //mark that we grabbed it
        itemHeld = true;
        heldItemOldPosition = HeldItem.GetComponent<RectTransform>().localPosition;
        //make it a child of the selector
        HeldItem.transform.SetParent(Selector.transform);
        if(viaClick == true)
        {
            //have the item follow our mouse if we clicked
            HeldItem.GetComponent<FollowPointer>().enabled = true;
            //and turn off masking so we can see it
            HeldItem.GetComponent<Image>().maskable = false;
            HeldItem.GetComponentInChildren<TMP_Text>().maskable = false;
        } else
        {
            //otherwise, set the item to follow the selector
            ResetHeldItemPosition();
        }
    }

    // Sets the position of the held item to just up-left of the selector
    void ResetHeldItemPosition()
    {
        //move it slightly to see underneath
        HeldItem.GetComponent<RectTransform>().localPosition = new Vector3(InventoryBox.xSpace / -4, InventoryBox.ySpace / -4, 0);
    }

    void PlaceItem(int ritualIndex)
    {
        //decrement item count
        InventoryDisplay.Inventory[heldItemIndex].count--;
        //place item
        RitualItemIcons[ritualIndex] = Instantiate(HeldItem, Panels[selectedPanel].transform);
        RitualItemIcons[ritualIndex].GetComponent<FollowPointer>().enabled = false;
        RitualItemIcons[ritualIndex].GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        //give help text
        RitualItemIcons[ritualIndex].GetComponent<HelpWindow>().info = HeldItem.GetComponent<HelpWindow>().info;
        //turn off text to represent only one
        RitualItemIcons[ritualIndex].GetComponentInChildren<TMP_Text>().enabled = false;
        //insert the item into our ritual internally
        RitualItemIndices[ritualIndex] = heldItemIndex;
        RitualItemIds[ritualIndex] = InventoryDisplay.Inventory[heldItemIndex].id;
        //diverge if that was the last one
        if(InventoryDisplay.Inventory[heldItemIndex].count == 0)
        {
            //if none remain, return to the inventory with no item held
            selectedPanel = lastInventoryPanel;
            Selector.transform.position = Panels[selectedPanel].transform.position;
            //drop the 0 count item, but make it inactive
            HeldItem.SetActive(false);
            DropItem();
        } else
        {
            //show the decrease in item count if multiple exist
            HeldItem.GetComponentInChildren<TMP_Text>().text = "" + InventoryDisplay.Inventory[heldItemIndex].count;
        }
    }

    // Return currently held item to the inventory
    void DropItem()
    {
        //return item to inventory
        HeldItem.transform.SetParent(InventoryBox.ItemsRoot);
        //move it to its correct position
        HeldItem.GetComponent<RectTransform>().localPosition = heldItemOldPosition;
        HeldItem.GetComponent<FollowPointer>().enabled = false;
        itemHeld = false;
    }

    // Remove the currently selected item from the inventory
    void DeleteCurrentItem()
    {
        //get the item index and delete
        int itemIndex = GetItemIndexFromPanel(selectedPanel);
        DeleteItemAt(itemIndex);
    }

    // Deletes an Item at a specific index
    void DeleteItemAt(int itemIndex)
    {
        //set count to 0 in the inventory
        InventoryDisplay.Inventory[itemIndex].count = 0;
        //for all inventory displays, set the count on this item to 0 and deactivate it
        foreach(RitualNavigator Navigator in RitualNavigators)
        {
            //all navigators first drop their held items
            if(Navigator.itemHeld == true) Navigator.DropItem();
            //the ritual window must remove all instances of this item as well
            if(Navigator.hasRitual == true)
            {
                //check for any instances of the item in the ritual window
                for(int i = 0; i < Navigator.RitualItemIndices.Length; i++)
                {
                    if(Navigator.RitualItemIndices[i] == itemIndex)
                    {
                        //destroy any matches
                        Destroy(Navigator.RitualItemIcons[i]);
                        Navigator.RitualItemIndices[i] = -1;
                        Navigator.RitualItemIds[i] = 0;
                    }
                }
            }
            //then we redraw the displays
            Navigator.InventoryBox.RedrawUI();
        }
    }

    // Remove the currently held item
    public void DeleteHeldItem()
    {
        int itemIndex = heldItemIndex;
        DropItem();
        DeleteItemAt(itemIndex);
    }

    // Changes our selection to the panel specified by
    public void SetSelectionTo(int newPanelIndex)
    {
        selectedPanel = newPanelIndex;
        Selector.transform.position = Panels[selectedPanel].transform.position;
    }

    // Turns on the help window of the item selected
    public void GetItemHelp(int panelIndex)
    {
        if(panelIndex > inventoryCutoff)
        {
            int ritualIndex = GetRitualIndex(panelIndex);
            if(ritualIndex < RitualItemIcons.Length && RitualItemIcons[ritualIndex] != null) RitualItemIcons[ritualIndex].GetComponent<HelpWindow>().OnMouseEnter();
        } else
        {
            //make sure item index is valid
            int itemIndex = GetItemIndexFromPanel(panelIndex);
            if(itemIndex >= InventoryBox.ItemIcons.Count || InventoryDisplay.Inventory[itemIndex].count == 0) return;
            //open the window if so
            InventoryBox.ItemIcons[itemIndex].GetComponent<HelpWindow>().OnMouseEnter();
        }
    }

    // Turns off item help window at panel index
    public void CloseItemHelp(int panelIndex)
    {
        if(panelIndex > inventoryCutoff)
        {
            int ritualIndex = GetRitualIndex(panelIndex);
            if(ritualIndex < RitualItemIcons.Length && RitualItemIcons[ritualIndex] != null) RitualItemIcons[ritualIndex].GetComponent<HelpWindow>().OnMouseExit();
        } else
        {
            //make sure item index is valid
            int itemIndex = GetItemIndexFromPanel(panelIndex);
            if(itemIndex >= InventoryBox.ItemIcons.Count) return;
            //close the window if so
            InventoryBox.ItemIcons[GetItemIndexFromPanel(panelIndex)].GetComponent<HelpWindow>().OnMouseExit();
        }
    }

    // Attempts to perform a summoning with the current configuration
    public void Summon()
    {
        //build array representing the state of the ritual
        int[] summoningKey = GetSummoningKey();
        //see what our configuration yeilds
        int result = RitualValidator.Instance.AttemptRitual(summoningKey);
        //print("summoning result = " + result);//test
        //check the result
        if(result < 0)
        {
            //if invalid, do nothing
            return;
        } else if(result > 0)
        {
            //if the result is a card id, give it to the player
            PlayerCharacter.Instance.PlayerDeck.AddNewCard(result);
            OnSummoningSuccess();
        } else
        {
            //if the result was exactly 0, perform a ressurection
            //WRITE THIS ONCE WE HAVE HERO ASSETS FOR HUMAN CATALYSTS AND STUFF
            OnSummoningSuccess();
        }
    }

    //returns an array of ints representing the summoning circle for use in verification
    private int[] GetSummoningKey()
    {
        List<int> summoningKey = new List<int>();
        summoningKey.Add(RitualItemIds[8]); //first add the catalyst
        if(catalystIsAlive == true) summoningKey[0] *= -1; //make negative to show its a card
        List<int> firstRing = new List<int>(); //add the first ring to a seperate array for sorting
        firstRing.Add(RitualItemIds[0]);
        firstRing.Add(RitualItemIds[3]);
        firstRing.Add(RitualItemIds[5]);
        firstRing.Sort();
        for(int i = 0; i < 3; i++)
        {
            summoningKey.Add(firstRing[i]);
        }
        List<int> secondRing = new List<int>(); //add the second ring similarly
        secondRing.Add(RitualItemIds[1]);
        secondRing.Add(RitualItemIds[4]);
        secondRing.Add(RitualItemIds[7]);
        secondRing.Sort();
        for(int i = 0; i < 3; i++)
        {
            summoningKey.Add(secondRing[i]);
        }
        //DO THIRD RING LATER
        return summoningKey.ToArray();
    }

    // Perform cleanup after a summon
    void OnSummoningSuccess()
    {
        //consume elements used in summoning
        if(catalystIsAlive == true)
        {
            //remove live catalyst from deck
            //WRITE THIS ONCE WE HAVE HERO ASSETS FOR HUMAN CATALYSTS AND STUFF
        }
        //clear the ritual window
        for(int i = 0; i < RitualItemIds.Length; i++)
        {
            //first try to clear the itemUI if count is 0
            if(RitualItemIndices[i] >= 0 && InventoryDisplay.Inventory[RitualItemIndices[i]].count == 0) InventoryBox.ItemIcons[RitualItemIndices[i]].SetActive(false);
            //then clear the ritual side
            RitualItemIndices[i] = -1;
            RitualItemIds[i] = 0;
            Destroy(RitualItemIcons[i]);
        }
        //drop our item and return to inventory
        OnCancel(0);
        //if this was our first ever summon, do some fancy stuff
        if(Calendar.currentDay == 1) OnFirstSummon();
    }

    //Trigger a cutscene, add popularity, and enable all tabs after our first summon
    void OnFirstSummon()
    {
        PlayerCharacter.Instance.popularity += 5;
        GameObject.FindGameObjectWithTag("Computer").GetComponent<TabManager>().EnableAllTabs();
        GameObject CutsceneObject = GameObject.FindGameObjectWithTag("CutsceneActivator").transform.GetChild(0).gameObject;
        CutsceneObject.GetComponent<TextLog>().sceneNumber = 1;
        CutsceneObject.SetActive(true);
    }

    // Returns the inventory index accessed via a given panel
    int GetItemIndexFromPanel(int panelIndex)
    {
        return panelIndex + (currentScroll * InventoryBox.columnCount);
    }

    // Returns the panel index associated with the given item index
    int GetPanelIndexFromItem(int itemIndex)
    {
        return itemIndex - (currentScroll * InventoryBox.columnCount);
    }
    
    // Returns the ritual index at our currently selected panel
    int GetRitualIndex(int panelIndex)
    {
        return panelIndex - inventoryCutoff - 1;
    }
}
