using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardType = CardInfo.CardType;

public class Passive : MonoBehaviour
{
    public enum TriggerType {Summon, Turn, NewMember, OnDeath, Attack, None} //What triggers the passive?

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
        if(User.side == 0)
        {
            //does nothing if there are no entities in hand
            if(User.gated == false && CardPrompt.Instance.PromptPlayFromHand(CardType.Entity) == true)
            {
                Encounter.Instance.NextAllySoulbind = User;
            }
        } else
        {
            Encounter.Instance.AddAlly(new CardInfo(12), 1);
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
        if(User.side == 0) User.AbilityDisplay.UseAbility();
    }

    // Give each party member besides the User radiant hp equal to the user's max hp
    static void LightOnSummon(Permanent User)
    {
        foreach (Permanent Ally in Encounter.Instance.Parties[User.side])
        {
            if(Ally != User) Ally.radiantHp += User.maxHp;
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
        foreach (Permanent Member in Encounter.Instance.Parties[User.side])
        {
            Member.radiantHp -= User.maxHp;
        }
    }

    // Set the User to take damage for the party
    static void WardOnSummon(Permanent User)
    {
        Encounter.Instance.Wards[User.side].Add(User);
    }

    // Remove the user from the list of wards for their side
    static void WardOnDeath(Permanent User)
    {
        Encounter.Instance.Wards[User.side].Remove(User);
    }

    // Set the user as the target's grappler
    static void Grapple(Permanent User)
    {
        Targeting.Target.Grappler = User;
    }

    // Set the user's max ap and ap equal to its number of soulbound units + 1
    static void HolySupport(Permanent User)
    {
        User.maxAp = User.SoulboundEntities.Count + 1;
        User.ap = User.maxAp;
    }

    // Set the user's mirror flag, which will be used to trigger reflected abilities
    static void Mirror(Permanent User)
    {
        User.mirror = true;
    }

    // Use defend every turn
    static void Guard(Permanent User)
    {
        Ability.AbilityUsages[1](User);
    }

    // Add this unit's cost to the upkeep bonus (if ally)
    public static void Studied(Permanent User)
    {
        if(User.side == 0) Encounter.UpkeepBonus += User.Info.SummonCost;
    }

    // Do an extra 1 damage if the target has hp below the max
    public static void Bat(Permanent User)
    {
        Permanent Target = Targeting.Target;
        if(Target.hp < Target.maxHp) Target.TakeHit(1);
    }


    public static int[] TriggerPerPassive = new int[]
    {
        1, 0, 0, 0, 0, 2, 3, 1, 3, 4, 1, 0, 1, 1, 4
    };

    public static Usage[] PassiveUsages = new Usage[]
    {
        new Usage(Gate),
        new Usage(Flying),
        new Usage(Untargetable),
        new Usage(TriggerAbilityIfAlly),
        new Usage(LightOnSummon),
        new Usage(LightOnNewMember),
        new Usage(LightOnDeath),
        new Usage(WardOnSummon),
        new Usage(WardOnDeath),
        new Usage(Grapple),
        new Usage(HolySupport), //THIS SHOULD BE THE FIRST LISTED PASSIVE IN CARD[_].XML
        new Usage(Mirror),
        new Usage(Guard),
        new Usage(Studied),
        new Usage(Bat)
    };
}
