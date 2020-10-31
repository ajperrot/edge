using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentCard : Card
{
    public int associatedPermanentId = -1; //id of the permanent summoned via this card

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Fill in fields on the card
    public override void FillCardUI()
    {
        base.FillCardUI();
    }

  // Summon the associated permanent for the SummonCost


    // Summon the associated permanent for the SummonCost
    public override void Use()
    {
        //pay the summon cost
        Info.SummonCost.Pay();
        //GENERATE THE PERMANENT FROM ID
        //ADD THE PERMANENT TO THE PLAYER'S SIDE
        //REMOVE THIS CARD FROM THE PLAYER'S HAND (DO NOT DESTROY)
        //REGISTER UNDO IN THE TURNLOG SO THIS MAY BE UNDONE
    }

    // Summon the associated permanent for the SummonCost
    public override void Undo()
    {
        //give back the summon cost
        Info.SummonCost.Gain();
        //DESTROY GENERATED PERMANENT
        //ADD THIS CARD TO PLAYER'S HAND
    }
}
