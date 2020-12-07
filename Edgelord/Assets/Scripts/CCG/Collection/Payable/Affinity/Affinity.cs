using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Affinity : Payable
{
    public int radiant = 0;
    public int lush = 0;
    public int crimson = 0;
    public int free = 0;

    public int sum
    {
        get {return radiant + lush + crimson + free;}
    }

    // +
    public static Affinity operator+ (Affinity a, Affinity b)
    {
        Affinity c = new Affinity();
        c.radiant = a.radiant + b.radiant;
        c.lush = a.lush + b.lush;
        c.crimson = a.crimson + b.crimson;
        c.free = a.free + b.free;
        return c;
    }

    // -
    public static Affinity operator- (Affinity a, Affinity b)
    {
        Affinity c = new Affinity();
        c.radiant = a.radiant - b.radiant;
        c.lush = a.lush - b.lush;
        c.crimson = a.crimson - b.crimson;
        return c;
    }

    // >
    public static bool operator> (Affinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant > 0 && c.lush > 0 && c.crimson > 0 && c.free > 0)
        {
            return true;
        } else return false;
    }

    // <
    public static bool operator< (Affinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant < 0 || c.lush < 0 || c.crimson < 0 || c.free < 0)
        {
            return true;
        } else return false;
    }

    // >=
    public static bool operator>= (Affinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant >= 0 && c.lush >= 0 && c.crimson >= 0 && c.free >= 0)
        {
            return true;
        } else return false;
    }

    // <=
    public static bool operator<= (Affinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant <= 0 && c.lush <= 0 && c.crimson <= 0 && c.free <= 0)
        {
            return true;
        } else return false;
    }

    // ==
    public static bool operator== (Affinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant == 0 && c.lush == 0 && c.crimson == 0 && c.free == 0)
        {
            return true;
        } else return false;
    }

    // !=
    public static bool operator!= (Affinity a, Affinity b)
    {
        return !(a == b);
    }

    // +
    public static Affinity operator+ (PlayerAffinity a, Affinity b)
    {
        Affinity c = new PlayerAffinity();
        c.radiant = a.radiant + b.radiant;
        c.lush = a.lush + b.lush;
        c.crimson = a.crimson + b.crimson;
        c.free = a.free + b.free;
        return c;
    }

    // -
    public static Affinity operator- (PlayerAffinity a, Affinity b)
    {
        Affinity c = new PlayerAffinity();
        c.radiant = a.radiant - b.radiant;
        c.lush = a.lush - b.lush;
        c.crimson = a.crimson - b.crimson;
        return c;
    }

    // >
    public static bool operator> (PlayerAffinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant > 0 && c.lush > 0 && c.crimson > 0 && c.free > 0)
        {
            return true;
        } else return false;
    }

    // <
    public static bool operator< (PlayerAffinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant < 0 || c.lush < 0 || c.crimson < 0 || c.free < 0)
        {
            return true;
        } else return false;
    }

    // >=
    public static bool operator>= (PlayerAffinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant >= 0 && c.lush >= 0 && c.crimson >= 0 && c.free >= 0)
        {
            return true;
        } else return false;
    }

    // <=
    public static bool operator<= (PlayerAffinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant <= 0 && c.lush <= 0 && c.crimson <= 0 && c.free <= 0)
        {
            return true;
        } else return false;
    }

    // ==
    public static bool operator== (PlayerAffinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant == 0 && c.lush == 0 && c.crimson == 0 && c.free == 0)
        {
            return true;
        } else return false;
    }

    // !=
    public static bool operator!= (PlayerAffinity a, Affinity b)
    {
        return !(a == b);
    }

    // Deduct this affinity from the player's working affinity
    public override bool Pay()
    {
        if(PlayerCharacter.Instance.PayableAffinity >= this)
        {
            PlayerCharacter.Instance.PayableAffinity = new PlayerAffinity(PlayerCharacter.Instance.PayableAffinity - this);
            return true;
        } else return false;
    }

    // Deduct this affinity from the player's working affinity
    public override void Gain()
    {
        PlayerCharacter.Instance.PayableAffinity = new PlayerAffinity(PlayerCharacter.Instance.PayableAffinity + this);
    }


    // OBJECT OVERRIDES

    // ToString
    public override string ToString()
    {
        return "" + radiant + "," + lush + "," + crimson + "," + free;
    }

    // Equals
    public override bool Equals(object o)
    {
        return o.Equals(this);
    }

    // GetHashCode
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
