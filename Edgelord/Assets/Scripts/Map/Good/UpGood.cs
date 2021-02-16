using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpGood : Good
{
    public UpgradeShop Seller; //the shop selling this good

    private CardInfo UpInfo; //CradInfo for the upgraded unit this will create
    private int baseId; //the id of the upgradable unit
    private int cost = 1; //the $ cost of this upgrade
    private int baseIndex = -1; //index of the card to be upgraded, -1 if none

    // Initialize is called by the shop to set values in order
    public void Initialize(int id)
    {
        this.id = id;
        SetUpInfo();
        baseId = id/10; // JUST REMEMBER THE BASE IS ALWAYS THE ID / 10
        // SOMEHOW DETERMINE COST 
    }

    // Fill out UpInfo based on id and the player's existing cards matching baseId
    private void SetUpInfo()
    {
        UpInfo = new CardInfo(id);
        UpInfo.name = ""; //use this to tell if the player has a matching base card
        for(int i = 0; i < PlayerCharacter.Instance.PlayerDeck.Contents.Count; i++)
        {
            CardInfo Info = PlayerCharacter.Instance.PlayerDeck.Contents[i];
            if(Info.id == baseId)
            {
                baseIndex = i;
                //only make the upinfo if there's a matching base card
                UpInfo.name = Info.name;
                return;
            }
        }
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
        NameTextBox.text = UpInfo.name;
        CostTextBox.text = "$" + cost;
        OtherTextBox.text = "";
        Help.info = "Upgrade item for " + UpInfo.humanClass;
    }

    // Tell the seller to selectthis item
    public void Select()
    {
        Seller.SelectGoodAt(stockIndex);
        // display the upgraded card if the player can make an upgrade
        if(UpInfo.name == "")
        {
            Seller.CardDisplay.SetActive(false);
        } else
        {
            Seller.CardDisplay.GetComponent<HumanCard>().Reset(UpInfo);
            Seller.CardDisplay.SetActive(true);
        }
    }

    // Calls the seller's scroll function
    public void Scroll()
    {
        Seller.Scroll();
    }

    // Remove base card from player's deck, add the new one, and re-set up info for like upgoods
    public bool OnPurchase()
    {
        //make sure we have the requisite base card
        if(baseIndex < 0) return false;
        //return if we can't afford it
        if(cost > PlayerCharacter.Instance.money) return false;
        //replace the old card
        PlayerCharacter.Instance.PlayerDeck.Contents[baseIndex] = UpInfo;
        //pay
        PlayerCharacter.Instance.money -= cost;
        //re-SetUpInfo for goods with the same baseIndex
        Seller.ResetGoodInfoFor(baseIndex);
        return true;
    }

    // SetUpInfo if baseIndex is the given value
    public void ResetIfBase(int index)
    {
        if(baseIndex == index) SetUpInfo();
    }


}
