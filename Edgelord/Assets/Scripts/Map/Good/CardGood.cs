using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGood : Good
{
    public CardShop Seller; //the shop selling this good
    public CardInfo Info; //CradInfo for the upgraded unit this will create
    
    private int cost = 1; //the $ cost of this upgrade

    // Initialize is called by the shop to set values in order
    public void Initialize(int id)
    {
        this.id = id;
        cost = Costs[id];
        Info = new CardInfo(id);
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        //fill out the UI
        FillUI();
    }

    // Fill out the various fields to express the associated item
    void FillUI()
    {
        NameTextBox.text = Info.name;
        CostTextBox.text = "$" + cost;
        Help.info = "This card, once purchased, can grant you a new ability when played.";
    }

    // Tell the seller to selectthis item
    public void Select()
    {
        Seller.SelectGoodAt(stockIndex);
        // display the card
        Seller.CardDisplay.GetComponent<PhenomenonCard>().Reset(Info);
        Seller.CardDisplay.SetActive(true);
    }

    // Calls the seller's scroll function
    public void Scroll()
    {
        Seller.Scroll();
    }

    // Remove base card from player's deck, add the new one, and re-set up info for like CardGoods
    public bool OnPurchase()
    {
        //return if we can't afford it
        if(cost > PlayerCharacter.Instance.money) return false;
        //add the card
        PlayerCharacter.Instance.PlayerDeck.Contents.Add(Info);
        //pay
        PlayerCharacter.Instance.money -= cost;
        return true;
    }

    Dictionary<int, int> Costs = new Dictionary<int, int>()
    {
        {20, 30},
        {21, 30},
        {22, 30}
    };
}
