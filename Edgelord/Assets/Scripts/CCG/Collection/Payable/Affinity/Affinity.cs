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

    // +
    public static Affinity operator+ (Affinity a, Affinity b)
    {
        Affinity c = new Affinity();
        c.radiant = a.radiant + b.radiant;
        c.lush = a.lush + b.lush;
        c.crimson = a.crimson + b.crimson;
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

    // Deduct this affinity from the player's working affinity
    public override void Pay()
    {
        PlayerCharacter.Instance.PayableAffinity -= this;
    }

    // Deduct this affinity from the player's working affinity
    public override void Gain()
    {
        PlayerCharacter.Instance.PayableAffinity += this;
    }
}
