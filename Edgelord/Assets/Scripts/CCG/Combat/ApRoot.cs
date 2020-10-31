using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApRoot : MonoBehaviour
{
    public GameObject TicPrefab; //copied to create the tics we display AP with
    public float ticSpace; //space between tics

    private List<GameObject> Tics = new List<GameObject>(); //each represents 1 ap
    
    // Max number of Tics
    private int _maxAp;
    public int maxAp
    {
        get {return _maxAp;}
        set
        {
            if(value < _maxAp)
            {
                //if we're decreasing maxAp, decrease ap to match
                if(value < _ap) ap = value;
                //then destroy the excess tics
                for(int i = maxAp - 1; i >= value; i--)
                {
                    Destroy(Tics[i]);
                    Tics.RemoveAt(i);
                }
            } else
            {
                //otherwise add them
                for(int i = _maxAp; i < value; i++)
                {
                    Tics.Add(GameObject.Instantiate(TicPrefab, transform));
                    Tics[i].transform.localPosition -= new Vector3(ticSpace * i, 0, 0);
                    Tics[i].SetActive(false);
                }
            }
            //change the value
            _maxAp = value;
        }
    }

    // Number of Tics
    private int _ap;
    public int ap
    {
        get {return _ap;}
        set
        {
            if(value < _ap)
            {
                //if we're decreasing ap, remove tics
                for(int i = value; i < _ap; i++)
                {
                    Tics[i].SetActive(false);
                }
            } else
            {
                //otherwise add them
                for(int i = _ap; i < value; i++)
                {
                    Tics[i].SetActive(true);
                }
            }
            //change the actual value
            _ap = value;
        }
    }
}
