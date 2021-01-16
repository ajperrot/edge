using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardType = CardInfo.CardType;

public class Passive : MonoBehaviour
{
    public enum TriggerType {Summon, Turn, None} //What triggers the passive?

    public delegate void Usage(Permanent User);

    // use all the user's summon-triggered passives
    public static void TriggerPassives(Permanent User, int trigger)
    {
        foreach(int passive in User.Info.Passives)
        {
            if(TriggerPerPassive[passive] == trigger)
            {
                PassiveUsages[passive](User);
            }
        }
    }

    //pick an entity in hand to summon for free, soulbound to the user
    static void Gate(Permanent User)
    {
        //check if this is the ally's first gate
        if(User.isAlly == true)
        {
            //does nothing if there are no entities in hand
            if(User.gated == false && CardPrompt.Instance.PromptPlayFromHand(CardType.Entity) == true)
            {
                Encounter.Instance.NextAllySoulbind = User;
            }
        } else
        {
            Encounter.Instance.AddEnemy(new CardInfo(12));
        }
        User.gated = true;
    }

    // Take half damage when damaged
    static void Flying(Permanent User)
    {
        User.flying = true;
    }

    // Make the user untargetable
    static void Untargetable(Permanent User)
    {
        User.targetable = false;
    }

    // Use the User's first ability if they are an ally
    static void TriggerAbilityIfAlly(Permanent User)
    {
        if(User.isAlly) User.AbilityDisplay.UseAbility();
    }


    private static int[] TriggerPerPassive = new int[]
    {
        1, 0, 0, 0
    };

    public static Usage[] PassiveUsages = new Usage[]
    {
        new Usage(Gate),
        new Usage(Flying),
        new Usage(Untargetable),
        new Usage(TriggerAbilityIfAlly)
    };
}
