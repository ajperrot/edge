using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShop : Shop
{
    private ItemGood[] Stock; //all goods for sale
    private bool[] stockPurchased; //has a 1 for every index of stock that sold
    private int selection = 0; //the good we have selected to purchase

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        //fill out Stock
        if(Calendar.currentDay == 0)
        {
            FillTutorialStock();
        } else
        {
            FillStock();
        }
        stockPurchased = new bool[Stock.Length];
    }

    // Enable event handlers
    void OnEnable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Vertical] += ScrollOne;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] += Purchase;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Cancel] += Exit;
    }

    // Disable event handlers
        void OnDisable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Vertical] -= ScrollOne;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] -= Purchase;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Cancel] -= Exit;
    }

    // React to up/down keys
    public void ScrollOne(float axisValue)
    {
        int sign = (axisValue > 0)? 1 : -1;
        //check the new index is valid
        if(sign > 0)
        {
            if(SelectPreviousItem() == false) SelectLastItem();
        } else
        {
            if(SelectNextItem() == false) SelectFirstItem();
        }
    }

    // Select the given good
    public void SelectItemAt(int index)
    {
        //select item
        Selector.transform.localPosition = Stock[index].transform.localPosition;
        selection = index;
    }

    // Select item and scroll to it
    private void SelectAndScrollTo(int index)
    {
        //calculate index of remaining items for scrolling
        int scrollIndex = 0;
        for(int i = 0; i < selection; i++)
        {
            if(stockPurchased[i] == false) scrollIndex++;
        }
        //scroll to item
        GoodsRoot.localPosition = GoodsRootOriginalPosition;
        totalScroll = 0;
        if(scrollIndex > 3)
        {
            GoodsRoot.localPosition = GoodsRootOriginalPosition + (Vector3.down * (scrollIndex - 3) * goodSpacing * -1);
            totalScroll -= (scrollIndex - 3) * goodSpacing;
        }
        //then actually select it
        SelectItemAt(index);
    }

    // Purchase the selected good
    public void Purchase(float axisValue)
    {
        //make sure selection exists
        if(selection < 0) return;
        //get the item
        Item SelectedItem = Stock[selection].ItemForSale;
        //return if we can't afford it
        if(SelectedItem.cost > PlayerCharacter.Instance.money) return;
        //otherwise add it to inventory, and remove it from stock
        PlayerCharacter.Instance.GetItem(SelectedItem);
        Destroy(Stock[selection].gameObject);
        stockPurchased[selection] = true;
        //and pay for it
        PlayerCharacter.Instance.money -= SelectedItem.cost;
        //commit to shopping here
        Commit();
        //move goods below this up a space
        for(int i = selection; i < Stock.Length; i++)
        {
            //check first if it's been purchased
            if(stockPurchased[i] == false)
            {
                //if not purchased, move it up
                Stock[i].transform.localPosition += (Vector3.up * goodSpacing);
            }
        }
        //reduce maxScroll (absolute value)
        maxScroll += goodSpacing;
        //select a new item
        SelectNewItem();
    }

    // Selects the next available item, and none if none exists
    private void SelectNewItem()
    {
        //select next available item
        if(SelectNextItem() == true || SelectPreviousItem() == true) return;
        //if no items are available, there can be no selection
        Destroy(Selector);
        selection = -1;
        PurchaseTextBox.text = "No Stock Remaining!";
    }

    // Select the next available item
    public bool SelectNextItem()
    {
        for(int i = selection + 1; i < Stock.Length; i++)
        {
            //check first if it's been purchased
            if(stockPurchased[i] == false)
            {
                //if not, select it and return
                SelectAndScrollTo(i);
                return true;
            }
        }
        //return false if no next item
        return false;
    }

    // Select previous available item
    private bool SelectPreviousItem()
    {
        for(int i = selection - 1; i >= 0; i--)
        {
            //check first if it's been purchased
            if(stockPurchased[i] == false)
            {
                //if not, select it and return
                SelectAndScrollTo(i);
                return true;
            }
        }
        //return false if no previous item exists
        return false;
    }

    //Select the first available item
    private bool SelectFirstItem()
    {
        for(int i = 0; i < Stock.Length; i++)
        {
            //check first if it's been purchased
            if(stockPurchased[i] == false)
            {
                //if not, select it and return
                SelectAndScrollTo(i);
                return true;
            }
        }
        //return false if no item exists
        return false;
    }

    //Select the last available item
    private bool SelectLastItem()
    {
        for(int i = Stock.Length - 1; i >= 0; i--)
        {
            //check first if it's been purchased
            if(stockPurchased[i] == false)
            {
                //if not, select it and return
                SelectAndScrollTo(i);
                return true;
            }
        }
        //return false if no item exists
        return false;
    }


    // Randomly generate a stock based on day, etc.
    private void FillStock()
    {
        Stock = new ItemGood[(int)Random.Range(minStockCount, maxStockCount)];
        stockPurchased = new bool[Stock.Length];
        int highestPossibleGoodIndex = numberOfPossibleGoodsPerDay[Calendar.currentDay];
        for(int i = 0; i < Stock.Length; i++)
        {
            //create and position the good
            GameObject GoodObject = Instantiate(GoodPrefab, GoodsRoot);
            GoodObject.transform.localPosition += (Vector3.down * goodSpacing * i);
            //then set its itemgood info
            ItemGood ThisGood = GoodObject.GetComponent<ItemGood>();
            ThisGood.id = possibleGoods[(int)Random.Range(0, highestPossibleGoodIndex)];
            ThisGood.stockIndex = i;
            ThisGood.Seller = this;
            //and add it to the stock
            Stock[i] = ThisGood;
        }
        //set our max scroll
        maxScroll = (Stock.Length - 4) * goodSpacing * -1;
    }

    // Adds the items specifically eeded on the first day
    private void FillTutorialStock()
    {
        Stock = new ItemGood[3];
        stockPurchased = new bool[Stock.Length];
        for(int i = 0; i < Stock.Length; i++)
        {
            //create and position the good
            GameObject GoodObject = Instantiate(GoodPrefab, GoodsRoot);
            GoodObject.transform.localPosition += (Vector3.down * goodSpacing * i);
            //then set its itemgood info
            ItemGood ThisGood = GoodObject.GetComponent<ItemGood>();
            ThisGood.id = i + 4;
            ThisGood.stockIndex = i;
            ThisGood.Seller = this;
            //and add it to the stock
            Stock[i] = ThisGood;
            //give the player their starting ingredients as well
            PlayerCharacter.Instance.Inventory.Add(new Item(i + 1));
        }
    }
}
