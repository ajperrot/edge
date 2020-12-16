using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    public static Permanent Target; //permanents put themselves here when they are hovered over
    public static Targeting ActiveInstance = null; //are we in targeting mode

    public RectTransform Line; //stretches and rotates to point to the target

    // Called when entering targeting mode
    void OnEnable()
    {
        ActiveInstance = this;
    }

    // Called when leaving targeting mode
    void OnDisable()
    {
        ActiveInstance = null;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 MousePosition = Input.mousePosition;
        //stretch the distance to the mouse
        float distance = Vector2.Distance(MousePosition, transform.position);
        distance = distance / Screen.width * 1920;
        Line.offsetMax = new Vector2(distance, 10);
        //rotate to point at the mouse position
        float extraRotation = 0;
        if(MousePosition.x < transform.position.x) extraRotation = 180;
        Line.eulerAngles = new Vector3(0, 0, Mathf.Atan((MousePosition.y - transform.position.y) / (MousePosition.x - transform.position.x)) * Mathf.Rad2Deg + extraRotation);
    }

    // Set the target, set the skill to fire, and deactivate
    public void SetTarget(Permanent NewTarget)
    {
        Target = NewTarget;
        gameObject.SetActive(false);
        Ability.ActiveAbility.Use();
    }
}
