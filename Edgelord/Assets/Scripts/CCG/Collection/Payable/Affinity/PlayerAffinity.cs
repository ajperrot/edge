using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerAffinity : Affinity
{
    // Underlying values
    private int _radiant = 0;
    private int _lush = 0;
    private int _crimson = 0;


    // Properties
    public new int radiant
    {
        get {return _radiant;}
        set
        {
            if(_radiant != value)
            {
                _radiant = value;
                PlayerCharacter.Instance.AffinityDisplay[0].text = "" + value;
            }
        }
    }
    public new int lush
    {
        get {return _lush;}
        set
        {
            if(_lush != value)
            {
                _lush = value;
                PlayerCharacter.Instance.AffinityDisplay[1].text = "" + value;
            }
        }
    }
    public new int crimson
    {
        get {return _crimson;}
        set
        {
            if(_crimson != value)
            {
                _crimson = value;
                PlayerCharacter.Instance.AffinityDisplay[2].text = "" + value;
            }
        }
    }

    public string toString()
    {
        return "" + _radiant;
    }
}
