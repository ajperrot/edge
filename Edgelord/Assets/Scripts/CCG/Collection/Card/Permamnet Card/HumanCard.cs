using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class HumanCard : PermanentCard
{
    public TMP_Text SanityField; //text field to display sanity

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
        SanityField.text = "" + Info.sanity;
        //then fill standard card fields
        base.FillCardUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
