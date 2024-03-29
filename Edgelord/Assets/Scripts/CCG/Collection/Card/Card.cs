﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;

[Serializable]
public class Card : MonoBehaviour
{
    public delegate void CardCallback();

    public static Dictionary<int, Sprite> LoadedCardArt = new Dictionary<int, Sprite>(); //cache of card art
   
    public CardInfo Info = null;

    public CardCallback OnUse;

    private TMP_Text[] TextFields = null;

    // Runs once at start
    protected virtual void Start()
    {
        //fill out the card's feilds 
        FillCardUI();
    }

    // Fills Card UI with card info
    public virtual void FillCardUI()
    {
        //get Text boxes
        if(TextFields == null) TextFields = gameObject.GetComponentsInChildren<TMP_Text>();
        Image[] Images = gameObject.GetComponentsInChildren<Image>();
        //add card art
        Images[0].sprite = GetCardArt(Info.id);
        //then the name
        TextFields[0].text = Info.name;
        //next three are Affinity costs
        int currentBox = 1;
        if(Info.SummonCost.radiant > 0)
        {
            //set text, accounting for variable cost
            if(Info.SummonCost.radiant == 100)
            {
                TextFields[currentBox].text = "X";
            }else TextFields[currentBox].text = "" + Info.SummonCost.radiant;
            //set icon
            Images[1 + currentBox].sprite = Resources.Load<Sprite>("Sprites/icon_radiant");
            currentBox++;
        }
        if(Info.SummonCost.lush > 0)
        {
            //set text, accounting for variable cost
            if(Info.SummonCost.lush == 100)
            {
                TextFields[currentBox].text = "X";
            }else TextFields[currentBox].text = "" + Info.SummonCost.lush;
            //set icon
            Images[1 + currentBox].sprite = Resources.Load<Sprite>("Sprites/icon_lush");
            currentBox++;
        }
        if(Info.SummonCost.crimson > 0)
        {
            //set text, accounting for variable cost
            if(Info.SummonCost.crimson == 100)
            {
                TextFields[currentBox].text = "X";
            }else TextFields[currentBox].text = "" + Info.SummonCost.crimson;
            //set icon
            Images[1 + currentBox].sprite = Resources.Load<Sprite>("Sprites/icon_crimson");
            currentBox++;
        }
        if(Info.SummonCost.free > 0)
        {
            TextFields[currentBox].text = "" + Info.SummonCost.free;
            //set icon
            Images[1 + currentBox].sprite = Resources.Load<Sprite>("Sprites/icon_basic");
            currentBox++;
        }
        //deactivate unused costs
        if(currentBox < 4)
        {
            transform.GetChild(5).gameObject.SetActive(false);
            if(currentBox < 3)
            {
                transform.GetChild(4).gameObject.SetActive(false);
                if(currentBox < 2)
                {
                    transform.GetChild(3).gameObject.SetActive(false);
                }
            }
        }
        //card text
        TextFields[4].text = Info.cardText;
        //type
        TextFields[5].text = Info.Type.ToString();
        if(Info.Type == CardInfo.CardType.Human)
        {
            TextFields[5].text += " - " + Info.humanClass;
        } else if(Info.Type == CardInfo.CardType.Entity)
        {
            TextFields[5].text += " - " + Info.EntityClass.ToString();
        }
        //next is attack, deactivate if unused
        if(Info.attack <= 0)
        {
            transform.GetChild(8).gameObject.SetActive(false);
        } else
        {
            TextFields[6].text = "" + Info.attack;
        }
        //next is hp, deactivate if unused
        if(Info.hp <= 0)
        {
            transform.GetChild(9).gameObject.SetActive(false);
        } else
        {
            TextFields[7].text = "" + Info.hp;
        }
    }

    // What the card does when played
    public virtual void Use()
    {
        if(OnUse != null) OnUse();
    }

    // How to undo what the card does when played
    public virtual void Undo()
    {
        
    }

    // Returns the card art sprite for a given card id
    public static Sprite GetCardArt(int id)
    {
        //return the art if already loaded
        if(LoadedCardArt.ContainsKey(id)) return LoadedCardArt[id];
        //otherwise load it as raw bytes from png
        byte[] pngBytes = System.IO.File.ReadAllBytes(Application.streamingAssetsPath + "/CardArt/" + id + ".png");
        //load bytes into texture
        Texture2D CardArt = new Texture2D(127, 97); //CHANGE SIZE ONCE WE AGREE ON ONE
        CardArt.LoadImage(pngBytes);
        //convert texture to sprite
        Sprite NewSprite = Sprite.Create(CardArt, new Rect(0.0f, 0.0f, CardArt.width, CardArt.height), new Vector2(0.5f, 0.5f));
        //register sprite in dictionary and return
        LoadedCardArt.Add(id, NewSprite);
        return LoadedCardArt[id];
    }

    
    // Change CardInfo and re-fill the ui
    public void Reset(CardInfo Info)
    {
        this.Info = Info;
        FillCardUI();
    }
}
