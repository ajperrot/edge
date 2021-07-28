using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGood : Good
{
    public Item ItemForSale; //the item being sold
    public ItemShop Seller; //the shop selling this good

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        //initialize the item
        ItemForSale = new Item(id);
        //fill out the UI
        FillUI();
    }

    // Fill out the various fields to express the associated item
    void FillUI()
    {
        NameTextBox.text = ItemForSale.name;
        CostTextBox.text = "" + ItemForSale.cost;
        Help.info = ItemForSale.description;
    }

    // Tell the seller to selectthis item
    public void Select()
    {
        Seller.SelectItemAt(stockIndex);
    }

    // Calls the seller's scroll function
    public void Scroll()
    {
        Seller.Scroll();
    }


}
