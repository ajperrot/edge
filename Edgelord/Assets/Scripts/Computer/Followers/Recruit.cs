using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recruit : MonoBehaviour
{
    public HumanCard OfferedCard;
    public GameObject AcceptButton;

    // Start is called before the first frame update
    void Start()
    {
        //close the requests if full on popularity
        CloseIfAtCapacity();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Get the card and change UI to reflect this
    public void Accept()
    {
        //add card to deck
        PlayerCharacter.Instance.PlayerDeck.AddCard(OfferedCard.Info);
        //deactivate button
        AcceptButton.SetActive(false);
        //add to folower count
        PlayerCharacter.Instance.followerCount++;
        //close the requests if full on popularity
        CloseIfAtCapacity();
    }

    // Deactivate the requests if popularity is full
    void CloseIfAtCapacity()
    {
        //check for popularity overflow
        if(PlayerCharacter.Instance.followerCount >= PlayerCharacter.Instance.popularity)
        {
            //deactivate the requests if there is no room for more
            transform.parent.gameObject.SetActive(false);
        }
    }
}
