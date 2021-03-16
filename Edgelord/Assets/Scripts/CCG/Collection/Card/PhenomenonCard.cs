using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhenomenonCard : Card
{
    // Start is called before the first frame update
    protected override void Start()
    {
        //randomly generate self if id is invalid
        if(Info.id < 0)
        {
            Info = new CardInfo(20);
        }
        base.Start();
    }

    // Summon the associated permanent for the SummonCost
    public override void Use()
    {
        //do not use if it is not your turn
        if(Encounter.Instance.yourTurn == false) return;
        //check if we can pay cost while paying
        if(Info.SummonCost.Pay() == false) return;
        //add the ability for this battle
        PlayerCharacter.Instance.AbilityDisplay.AddAbilityButton(Info.Abilities[0]);
        //remove card from hand
        PlayerCharacter.Instance.RemoveFromHand(transform.GetSiblingIndex());
        //use any necessary callbacks
        base.Use();
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
