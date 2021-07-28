using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    public static int goodsPerRow = 2; //goods displayed in one row of the menu
    public static float goodSpacingX = 350; //horizontal space between goods on the menu
    public static float goodSpacingY = 350; //vertical space between goods on the menu
    public static float scrollSpeed = 20; //scroll per frame scrolling

    public int shopId = -1; //indicates which shop this is
    public int minStockCount = 2; //minimum number of goods per day
    public int maxStockCount = 20; //maximum goods sold per day
    public int[] possibleGoods; //ids of every good that could appear in order
    public int[] numberOfPossibleGoodsPerDay; //the number of goods purchasable each day
    public TMP_Text ShopNameTextBox; //displays the name of the shop
    public TMP_Text PurchaseTextBox; //displays the prompt for the purchase button
    public GameObject GoodPrefab; //prefab to copy for each item in our inventory
    public Transform GoodsRoot; //all goods are children of this
    public GameObject Selector; //indicates selected good
    public bool committed = false; //have we bought anything yet?
    public string purchaseText = "Purchase"; //what to say on the purchase button once committed
    public float maxScroll = 0; //how far can we scroll
    public Vector3 GoodsRootOriginalPosition;
    public float totalScroll = 0; //how far have we scrolled
    public int purchaseCount;

    // Start is called before the first frame update
    public void Start()
    {
        GoodsRootOriginalPosition = GoodsRoot.localPosition;
    }

    // Return to map if uncommitted, otherwise go home
    public void Exit(float axisValue)
    {
        if(PlayerCharacter.Instance.movement == 0)
        {
            //go to next day if we had committed
            Setting.AdvanceDay();
        } else
        {
            //otherwise return to map
            gameObject.SetActive(false);
            Destination.ToggleMapIcons(true);
        }
    }

    // Change stats/UI to reflect locking inot this establishment
    public void Commit()
    {
        //return if already committed
        if(committed == true) return;
        //otherwise commit
        committed = true;
        PurchaseTextBox.text = purchaseText;
        //and spend movement
        PlayerCharacter.Instance.movement--;
    }

    // Scroll through goods
    public void Scroll()
    {
        //determine the scroll direction
        float sign = (Input.mouseScrollDelta.y > 0)? 1 : -1;
        float scrollAmmount = sign * scrollSpeed;
        //scroll the comments if necessary
        if( (sign > 0 && totalScroll < 0) || (sign < 0 && totalScroll > maxScroll) )
        {
            totalScroll += scrollAmmount;
            GoodsRoot.GetComponent<RectTransform>().localPosition += (Vector3.up * scrollAmmount * -1);
        }
    }
}
