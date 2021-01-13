using System.Collections.Generic;
using UnityEngine;

public class AI
{
    public enum AttackType {Intelligent, Neutral, Passive, Random, Ravenous, Vengeful}; //enumeration of attack types

    public delegate bool Decision(Permanent User);
    public delegate bool TargetSelector(Permanent User, List<Permanent> Options);

    // Decide who to attack based on AttackType
    public static bool Attack(Permanent User)
    {
        List<Permanent> Options;
        if(Encounter.Instance.FrontLines[0].Count > 0)
        {
            Options = Encounter.Instance.FrontLines[0];
        } else
        {
            Options = Encounter.Instance.Allies;
        }
        return TargetSelectors[User.Info.pattern](User, Options);
    }

    // Defend if total ally attack is higher than enemy defense
    public static bool Defend(Permanent User)
    {
        int potentialAttack = 0;
        foreach(Permanent Attacker in Encounter.Instance.Allies)
        {
            potentialAttack += Attacker.Info.attack;
        }
        if(potentialAttack > Encounter.Instance.enemyDefense) return true;
        return false;
    }

    // Give Devotion to the ally with the lowest combined hp and rhp, prioritizing the front line
    public static bool Devotion(Permanent User)
    {
        int lowestHp = 256;
        int targetIndex = 0;
        List<Permanent> PotentialTargets;
        if(Encounter.Instance.FrontLines[1].Count > 0)
        {
            PotentialTargets = Encounter.Instance.FrontLines[1];
        } else
        {
            PotentialTargets = Encounter.Instance.Enemies;
        }
        for(int i = 0; i < PotentialTargets.Count; i++)
        {
            Permanent PotentialTarget = PotentialTargets[i];
            int combinedHp = PotentialTarget.hp + PotentialTarget.radiantHp;
            if(combinedHp < lowestHp)
            {
                lowestHp = combinedHp;
                targetIndex = i;
            }
        }
        Targeting.Target = PotentialTargets[targetIndex];
        return true;
    }

    // Attck the leader, or the biggest threat(?)
    public static bool IntelligentSelection(Permanent User, List<Permanent> Options)
    {
        foreach(Permanent Option in Options)
        {
            if(Option.isLeader == true)
            {
                Targeting.Target = Option;
                return true;
            }
        }
        //OTHERWISE BE "SMART"???
        //RANDOM FOR NOW
        return RandomSelection(User, Options);
    }

    // Attack randomly if attacked
    public static bool NeutralSelection(Permanent User, List<Permanent> Options)
    {
        if(User.Attacker != null) return RandomSelection(User, Options);
        return false;
    }

    // Attack nobody
    public static bool PassiveSelection(Permanent User, List<Permanent> Options)
    {
        Targeting.Target = null;
        return false;
    }

    // Attack a random target
    public static bool RandomSelection(Permanent User, List<Permanent> Options)
    {
        Targeting.Target = Options[(int)Random.Range(0, Options.Count)];
        return true;
    }

    // Attack the option with the lowest hp
    public static bool RavenousSelection(Permanent User, List<Permanent> Options)
    {
        int lowestHp = 100;
        foreach(Permanent Option in Options)
        {
            if(Option.hp < lowestHp) Targeting.Target = Option;
        }
        return true;
    }

    // Attack the last option to attack you, otherwise randomly
    public static bool VengefulSelection(Permanent User, List<Permanent> Options)
    {
        if(User.Attacker != null && Options.Contains(User.Attacker))
        {
            Targeting.Target = User.Attacker;
            return true;
        } else
        {
            return RandomSelection(User, Options);
        }
    }

    public static TargetSelector[] TargetSelectors = new TargetSelector[]
    {
        new TargetSelector(IntelligentSelection),
        new TargetSelector(NeutralSelection),
        new TargetSelector(PassiveSelection),
        new TargetSelector(RandomSelection),
        new TargetSelector(RavenousSelection),
        new TargetSelector(VengefulSelection)
    };

    public static Decision[] AbilityDecisions = new Decision[]
    {
        new Decision(Attack),
        new Decision(Defend)
    };
}