using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShop : Shop
{
    public GameObject CardDisplay; //card used to show the result of the upgrade

    private UpGood[] Stock; //all goods for sale
    private bool[] stockPurchased; //has a 1 for every index of stock that sold
    private int selection = 0; //the good we have selected to purchase

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        //fill out Stock
        FillStock();
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
            if(SelectPreviousGood() == false) SelectLastGood();
        } else
        {
            if(SelectNextGood() == false) SelectFirstGood();
        }
    }

    // Select the given good
    public void SelectGoodAt(int index)
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
            GoodsRoot.localPosition = GoodsRootOriginalPosition + (Vector3.down * (scrollIndex - 3) * goodSpacingY * -1);
            totalScroll -= (scrollIndex - 3) * goodSpacingY;
        }
        //then actually select it
        SelectGoodAt(index);
    }

    // Purchase the selected good
    public void Purchase(float axisValue)
    {
        //make sure selection exists
        if(selection < 0) return;
        UpGood SelectedGood = Stock[selection];
        //otherwise purchase (return if failed)
        if(SelectedGood.OnPurchase() == false) return;
        Destroy(Stock[selection].gameObject);
        stockPurchased[selection] = true;
        //commit to shopping here
        Commit();
        //move goods below this up a space
        for(int i = selection; i < Stock.Length; i++)
        {
            //check first if it's been purchased
            if(stockPurchased[i] == false)
            {
                //if not purchased, move it up
                Stock[i].transform.localPosition += (Vector3.up * goodSpacingY);
            }
        }
        //reduce maxScroll (absolute value)
         //maxScroll += goodSpacing;
        //select a new item
        SelectNewGood();
    }

    // Selects the next available item, and none if none exists
    private void SelectNewGood()
    {
        //select next available item
        if(SelectNextGood() == true || SelectPreviousGood() == true) return;
        //if no items are available, there can be no selection
        Destroy(Selector);
        selection = -1;
        PurchaseTextBox.text = "No Stock Remaining!";
    }

    // Select the next available item
    public bool SelectNextGood()
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
    private bool SelectPreviousGood()
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
    private bool SelectFirstGood()
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
    private bool SelectLastGood()
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
        Stock = new UpGood[(int)Random.Range(minStockCount, maxStockCount)];
        stockPurchased = new bool[Stock.Length];
        int highestPossibleGoodIndex = numberOfPossibleGoodsPerDay[Setting.currentDay];
        for(int i = 0; i < Stock.Length; i++)
        {
            //create and position the good
            GameObject GoodObject = Instantiate(GoodPrefab, GoodsRoot);
            GoodObject.transform.localPosition += (Vector3.right * goodSpacingX * (i%goodsPerRow));
            GoodObject.transform.localPosition += (Vector3.down * goodSpacingY * (i / goodsPerRow));
            //then set its UpGood info
            UpGood ThisGood = GoodObject.GetComponent<UpGood>();
            ThisGood.stockIndex = i;
            ThisGood.Seller = this;
            ThisGood.Initialize(possibleGoods[(int)Random.Range(0, highestPossibleGoodIndex)]);
            //and add it to the stock
            Stock[i] = ThisGood;
        }
        //set our max scroll
        maxScroll = Stock.Length / goodsPerRow * goodSpacingY * -1;
        //Select the first good for real
        Stock[0].Select();
    }

    // Reset info for goods with a baseIndex matching the given index
    public void ResetGoodInfoFor(int index)
    {
        for(int i = Stock.Length - 1; i >= 0; i--)
        {
            if(stockPurchased[i] == false)
            {
                Stock[i].ResetIfBase(index);
            }
        }
        Stock[selection].Select();
    }
}
