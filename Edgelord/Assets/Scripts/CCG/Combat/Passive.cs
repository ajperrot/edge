using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardType = CardInfo.CardType;

public class Passive : MonoBehaviour
{
    public enum TriggerType {Summon, Turn, NewMember, None} //What triggers the passive?

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

    // Give each party member besides the User radiant hp equal to the user's max hp
    static void LightOnSummon(Permanent User)
    {
        if(User.isAlly)
        {
            foreach (Permanent Ally in Encounter.Instance.Allies)
            {
                if(Ally != User) Ally.radiantHp += User.maxHp;
            }
        } else
        {
            foreach (Permanent Enemy in Encounter.Instance.Enemies)
            {
                if(Enemy != User) Enemy.radiantHp += User.maxHp;
            } 
        }
    }

    // Give a newly summoned party member rhp equal to the user's max hp
    static void LightOnNewMember(Permanent User)
    {
        Targeting.Target.radiantHp += User.maxHp;
    }

    // Take away rhp from each party member equal to the user's max hp
    static void LightOnDeath(Permanent User)
    {
        List<Permanent> Party;
        if(User.isAlly)
        {
            Party = Encounter.Instance.Allies;
        } else
        {
            Encounter.Instance.Enemies
        }
        foreach (Permanent Member in Party)
        {
            Member.radiantHp -= User.maxHp;
        }
    }


    public static int[] TriggerPerPassive = new int[]
    {
        1, 0, 0, 0, 0, 2, 3
    };

    public static Usage[] PassiveUsages = new Usage[]
    {
        new Usage(Gate),
        new Usage(Flying),
        new Usage(Untargetable),
        new Usage(TriggerAbilityIfAlly),
        new Usage(LightOnSummon),
        new Usage(LightOnNewMember),
        new Usage(LightOnDeath)
    };
}
