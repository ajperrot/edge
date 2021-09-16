using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainScroll : MonoBehaviour
{   
    public Transform GoodsRoot; //moved opposite the rope
    public Shop associatedShop; //has info on how far to scroll

    private float xPos; //unchanging x position
    private float yMax; //the highest y position
    private float yMin = -30; //the lowest y position

    private float scale; //how much to move GoodsRoot relative to this

    //This will store the difference between where the mouse is clicked
    //on the Plane and where the origin of the object is
    private float offset;

    //This will be used to cache to main camera
    //You could also use a serialized field to accomplish the same thing
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        // Cache the camera at the start. 
        mainCamera = Camera.main; 
        // Get starting position
        xPos = transform.localPosition.x;
        yMax = transform.localPosition.y;
    }

    // Get/set info to begin moving the rope
    public void OnMouseDown()
    {
        //calculate offset
        offset = transform.localPosition.y - Input.mousePosition.y;
        // get scale from rope to shop
        scale = (associatedShop.yMax - associatedShop.yMin) / (yMax - yMin) * -1;
    }

    // Move this object along with the mouse, and scroll the opposite direction
    public void OnMouseDrag()
    {
        //get new position
        float y = Input.mousePosition.y + offset;
        if(y < yMin)
        {
            y = yMin;
        } else if(y > yMax) y = yMax;
        Vector3 newPosition = new Vector3(xPos, y, 0);
        //move the window accordingly
        GoodsRoot.localPosition += new Vector3(0, (y - transform.localPosition.y) * scale, 0);
        //move the rope itself
        transform.localPosition = newPosition;
    }
}
