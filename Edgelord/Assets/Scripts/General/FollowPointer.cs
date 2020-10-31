using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPointer : MonoBehaviour {
  
  // Update is called once per frame
  void Update()
  {
        gameObject.GetComponent<RectTransform>().position = Input.mousePosition;
    }
}