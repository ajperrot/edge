using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardType = CardInfo.CardType;

public class Passive : MonoBehaviour
{
    public enum TriggerType {Summon} //What triggers the passive?

    public delegate void Usage(Permanent User);

    // use all the user's summon-triggered passives
    public static void OnSummon(Permanent User)
    {
        foreach(int passive in User.Info.Passives)
        {
            if(TriggerPerPassive[passive] == 0)
            {
                PassiveUsages[passive](User);
            }
        }
    }

    //pick an entity in hand to summon for free, soulbound to the user
    static void Gate(Permanent User)
    {
        //does nothing if there are no entities in hand
        if(CardPrompt.Instance.PromptPlayFromHand(CardType.Entity) == true)
        {
            Encounter.Instance.NextAllySoulbind = User;
        }

    }

    private static int[] TriggerPerPassive = new int[]
    {
        0
    };

    public static Usage[] PassiveUsages = new Usage[]
    {
        new Usage(Gate)
    };
}
