using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    public static Permanent Target; //permanents put themselves here when they are hovered over

    public LineRenderer Line; //the line from user to target

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Line.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
