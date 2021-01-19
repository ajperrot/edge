using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;

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

    public int partialSum
    {
        get {return radiant + lush + crimson;}
    }

    // +
    public static Affinity operator+ (Affinity a, Affinity b)
    {
        if(a == null) return b;
        if(b == null) return a;
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
        c.free = a.free;
        int remaining = b.free;
        while(c.radiant > 0 && remaining > 0)
        {
            remaining--;
            c.radiant--;
        }
        while(c.lush > 0 && remaining > 0)
        {
            remaining--;
            c.lush--;
        }
        while(c.crimson > 0 && remaining > 0)
        {
            remaining--;
            c.crimson--;
        }
        while(c.free > 0 && remaining > 0)
        {
            remaining--;
            c.free--;
        }
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

    // > int
    public static bool operator> (Affinity a, int b)
    {
        if(a.sum > b) return true;
        return false;
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

    // < int
    public static bool operator< (Affinity a, int b)
    {
        if(a.sum < b) return true;
        return false;
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
        if((object)b == null) return ((object)a == null);
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
        c.free = a.free - b.free;
        return c;
    }

    // >
    public static bool operator> (PlayerAffinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant > 0 && c.lush > 0 && c.crimson > 0 && c.partialSum > b.free)
        {
            return true;
        } else return false;
    }

    // <
    public static bool operator< (PlayerAffinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant < 0 || c.lush < 0 || c.crimson < 0 || c.partialSum < b.free)
        {
            return true;
        } else return false;
    }

    // >=
    public static bool operator>= (PlayerAffinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant >= 0 && c.lush >= 0 && c.crimson >= 0 && c.partialSum >= b.free)
        {
            return true;
        } else return false;
    }

    // <=
    public static bool operator<= (PlayerAffinity a, Affinity b)
    {
        Affinity c = a - b;
        if(c.radiant <= 0 && c.lush <= 0 && c.crimson <= 0 && c.partialSum <= b.free)
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

    // Fill out Fields based on XML data
    public void FillFromXml(XmlNode Root)
    {
        XmlNodeList Nodes = Root.ChildNodes;
        radiant = XmlConvert.ToInt32(Nodes[0].InnerText);
        lush = XmlConvert.ToInt32(Nodes[1].InnerText);
        crimson = XmlConvert.ToInt32(Nodes[2].InnerText);
        free = XmlConvert.ToInt32(Nodes[3].InnerText);
    }


    // OBJECT OVERRIDES

    // ToString
    public override string ToString()
    {
        string result = "";
        for(int i = 0; i < radiant; i++)
        {
            result += "<sprite=0>";
        }
        for(int i = 0; i < lush; i++)
        {
            result += "<sprite=1>";
        }
        for(int i = 0; i < crimson; i++)
        {
            result += "<sprite=2>";
        }
        for(int i = 0; i < free; i++)
        {
            result += "<sprite=3>";
        }
        return result;
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
