using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpGood : Good
{
    public UpgradeShop Seller; //the shop selling this good

    private CardInfo UpInfo; //CradInfo for the upgraded unit this will create
    private int baseId; //the id of the upgradable unit
    private int cost = 1; //the $ cost of this upgrade

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
        foreach(CardInfo Info in PlayerCharacter.Instance.PlayerDeck.Contents)
        {
            if(Info.id == baseId)
            {
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
        Seller.SelectItemAt(stockIndex);
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


}
