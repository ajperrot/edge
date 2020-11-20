﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Permanent : MonoBehaviour
{
    public Slider HpBar; //used to display hp
    public TMP_Text HpText; //displays hp as a number
    public TMP_Text MaxHpText; //displays maxHP as a number
    public ApRoot ApDisplay; //displays and stores ap
    public Slider SanityBar; //used to display sanity
    public TMP_Text SanityText; //prints sanity to screen
    public TMP_Text MaxSanityText; //prints maxSanity to screen
    public AbilitiesRoot AbilityDisplay; //displays buttons for each ability
    public Slider RadiantHpBar; //used to display radiantHp
    public TMP_Text RadiantHpText; //displays radiantHp as a number

    // Our maximum hp
    public int maxHp
    {
        get {return (int)HpBar.maxValue;}
        set
        {
            if(value < hp) hp = value;
            HpBar.maxValue = value;
            RadiantHpBar.maxValue = value;
            MaxHpText.text = value.ToString();
        }
    }

    // Our current hp
    public int hp
    {
        get {return (int)HpBar.value;}
        set
        {
            if(value > maxHp) value = maxHp;
            HpBar.value = value;
            HpText.text = value.ToString();

        }
    }

    // Extra hp used when at 0 hp
    public int radiantHp
    {
        get {return (int)RadiantHpBar.value;}
        set
        {
            if(value > 0)
            {
                RadiantHpBar.value = value;
                RadiantHpText.text = value.ToString();
                RadiantHpBar.gameObject.SetActive(true);
            } else
            {
                RadiantHpBar.gameObject.SetActive(false);
            }
        }
    }

    // Every Action consumes 1 ap, this is the max
    public int maxAp
    {
        get {return ApDisplay.maxAp;}
        set {ApDisplay.maxAp = value;}
    }

    // Current ap
    public int ap
    {
        get {return ApDisplay.ap;}
        set {ApDisplay.ap = value;}
    }

    // Max Psychological hp
    public int maxSanity
    {
        get {return (int)SanityBar.maxValue;}
        set
        {
            if(value < sanity) sanity = value;
            SanityBar.maxValue = value;
            MaxSanityText.text = value.ToString();
        }
    }

    // Psychological hp
    public int sanity
    {
        get {return (int)SanityBar.value;}
        set
        {
            if(value > maxSanity) value = maxSanity;
            SanityBar.value = value;
            SanityText.text = value.ToString();
        }
    }

    // Called on mouse hover
    public void OnPointerEnter()
    {
        AbilityDisplay.AddHover(0);
    }

    // Mouse leaves
    public void OnPointerExit()
    {
        AbilityDisplay.RemoveHover(0);
    }

    // Initialize with card info
    public void Initialize(CardInfo Info, bool isEnemy = false)
    {
        maxHp = Info.hp;
        hp = maxHp;
        if(isEnemy == false)
        {
            maxSanity = Info.sanity;
            sanity = sanity;
            AbilityDisplay.InitializeAbilityButtons(Info.Abilities);
            maxAp = Info.ap;
            ap = maxAp;
        }
    }
}