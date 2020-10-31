using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCard : PermanentCard
{
    public Payable[] UpkeepCost; //cost to keep entity on the field

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
