using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTab : MonoBehaviour
{
    public static InventoryTab Instance;
    public GameObject InventoryOverlay;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Activate input handlers when enabled
    void OnEnable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Inventory] += SpawnInventory;
    }

    // Deactivate input handlers when disnabled
    void OnDisable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Inventory] -= SpawnInventory;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Toggle inventory tab window
    public void OnClick()
    {
        if(InventoryOverlay.activeSelf == false)
        {
            SpawnInventory(0);
        } else
        {
            CloseInventory(0);
        }
    }

    // Spawns the Inventory Tab Window
    public void SpawnInventory(float axisValue)
    {
        //halt input outside the inventory
        InputManager.SaveAndClearInputEvents();
        //actually spawn the thing
        InventoryOverlay.SetActive(true);
    }

    // Closes the Inventory Tab Window
    public void CloseInventory(float axisValue)
    {
        InventoryOverlay.SetActive(false);
        //restore inputs from before the tab was open
        InputManager.RestoreInputState();

    }
}
