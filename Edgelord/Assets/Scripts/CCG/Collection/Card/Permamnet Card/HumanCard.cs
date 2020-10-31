using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class HumanCard : PermanentCard
{
    public TMP_Text SanityField; //text field to display current sanity

    //current sanity of maximum
    private int _currentSanity = -1;
    public int currentSanity
    {
        get{return _currentSanity;}
        set
        {
            _currentSanity = value;
            //update UI to reflect change
            SanityField.text = "" + currentSanity;
        }

    }

    // Start is called before the first frame update
    protected override void Start()
    {
        //randomly generate self if id is invalid
        if(Info.id < 0)
        {
            Info = new CardInfo((int)UnityEngine.Random.Range(CardInfo.firstHumanId, CardInfo.lastHumanId));
        }
        base.Start();
    }

    public override void FillCardUI()
    {
        //fill our sanity, defaulting to max
        if(currentSanity < 0) currentSanity = Info.maxSanity;
        //then fill standard card fields
        base.FillCardUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
