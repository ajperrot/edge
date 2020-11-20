using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentCard : Card
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Fill in fields on the card
    public override void FillCardUI()
    {
        base.FillCardUI();
    }

    // Summon the associated permanent for the SummonCost
    public override void Use()
    {
        //do not use if it is not your turn
        if(Encounter.Instance.yourTurn == false) return;
        //pay the summon cost, return if unable
        if(Info.SummonCost.Pay() == false) return;
        //generate the permanent
        Encounter.Instance.AddAlly(Info);
        //remove card from hand
        PlayerCharacter.Instance.RemoveFromHand(transform.GetSiblingIndex());
        //REGISTER UNDO IN THE TURNLOG SO THIS MAY BE UNDONE
    }

    // UN-Summon the associated permanent for the SummonCost
    public override void Undo()
    {
        //give back the summon cost
        Info.SummonCost.Gain();
        //DESTROY GENERATED PERMANENT
        //ADD THIS CARD TO PLAYER'S HAND
    }
}
