using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Payable
{
    public abstract bool Pay();
    public abstract void Gain(); //inverse pay
}
