using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Deck
{
  List<CardInfo> Contents = new List<CardInfo>(); //info representing the cards in the deck

  //add a new entity to the deck
  public void AddNewCard(int id)
  {
    Contents.Add(new CardInfo(id));
  }

  public void AddCard(CardInfo NewCard)
  {
    Contents.Add(NewCard);
  }
}
