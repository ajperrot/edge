using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class Deck
{
  const int startingHandSize = 3; //size of your initial hand
  const int maxHandSize = 6; //maximum number of card in hand

  public List<CardInfo> Contents = new List<CardInfo>(); //info representing the cards in the deck
  Stack<int> Order; //order of cards in deck, each entry is an index in Contents

  // Add a new card to the deck
  public void AddNewCard(int id)
  {
    Contents.Add(new CardInfo(id));
  }

  // Add a generated card to the deck
  public void AddCard(CardInfo NewCard)
  {
    Contents.Add(NewCard);
  }

  // Randomize Order
  public void Shuffle()
  {
    if(Order == null) InitializeOrder();
    Order.OrderBy(x => Guid.NewGuid()).ToList();
  }

  // Fill Order with the current order of cards
  public void InitializeOrder()
  {
    Order = new Stack<int>();
    for(int i = 0; i < Contents.Count; i++)
    {
      Order.Push(i);
    }
  }

  // Returns a hand's worth of cards, removing them from order
  public List<CardInfo> GetHand(int handSize = startingHandSize)
  {
    List<CardInfo> Hand = new List<CardInfo>();
    for(int i = 0; i < handSize; i++)
    {
      if(Order.Count > 0) Hand.Add(Contents[Order.Pop()]);
    }
    return Hand;
  }
}
