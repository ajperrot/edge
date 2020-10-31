using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	//track press status of inputs
	public static bool[] inputInUse = new bool[(int)AxisEnum.Count];
	public delegate void InputDelegate(float axisValue);
	public static InputDelegate[] OnInputHit = new InputDelegate[(int)AxisEnum.Count]; //used for initial press
	public static InputDelegate[] OnInput = new InputDelegate[(int)AxisEnum.Count]; //triggers all frames pressed
	public static InputDelegate[] OnInputRelease = new InputDelegate[(int)AxisEnum.Count]; //triggers first frame not pressed
	//SAVED EVENTS TO REVERT TO
	public static Stack<InputDelegate[]> SavedInputHits = new Stack<InputDelegate[]>(); //used for initial press
	public static Stack<InputDelegate[]> SavedInputs = new Stack<InputDelegate[]>(); //triggers all frames pressed
	public static Stack<InputDelegate[]> SavedInputReleases = new Stack<InputDelegate[]>(); //triggers first frame not pressed
	//enumeration for input names
	public enum AxisEnum {Horizontal, Vertical, Confirm, Cancel, Tab, Inventory, Trash, Count};
	private string[] axisEnumNames = System.Enum.GetNames(typeof(AxisEnum));
	
	// FixedUpdate is called before update
	void FixedUpdate () {
		//check if axes are in use
		for (int i = 0; i < (int)AxisEnum.Count; i++) {
			float axisValue = Input.GetAxis(axisEnumNames[i]);
			if (axisValue != 0) {
				if (inputInUse[i] == false) {
					inputInUse[i] = true;
					//trigger hit event if this is the first frame
					if (OnInputHit[i] != null) {
						OnInputHit[i](axisValue);
					}
				}
				//trigger standard input regardless
				if (OnInput[i] != null) {
					OnInput[i](axisValue);
				}
			} else if (inputInUse[i] == true) {
				inputInUse[i] = false;
				if (OnInputRelease[i] != null) {
					OnInputRelease[i](axisValue);
				}
			}
		}
	}

	// Save state of current event handlers and clear them
	public static void SaveAndClearInputEvents()
	{
		InputDelegate[] SavedInputHit = new InputDelegate[(int)AxisEnum.Count];
		InputDelegate[] SavedInput = new InputDelegate[(int)AxisEnum.Count];
		InputDelegate[] SavedInputRelease = new InputDelegate[(int)AxisEnum.Count];
		//save inputs
		for(int i = 0; i < (int)AxisEnum.Count; i++)
		{
			SavedInputHit[i] = OnInputHit[i];
			SavedInput[i] = OnInput[i];
			SavedInputRelease[i] = OnInputRelease[i];
		}
		SavedInputHits.Push(SavedInputHit);
		SavedInputs.Push(SavedInput);
		SavedInputReleases.Push(SavedInputRelease);
		//clear inputs
		OnInputHit = new InputDelegate[(int)AxisEnum.Count];
		OnInput = new InputDelegate[(int)AxisEnum.Count];
		OnInputRelease = new InputDelegate[(int)AxisEnum.Count];
	}

	// Restore input state from before Save and clear
	public static void RestoreInputState()
	{
		OnInputHit = SavedInputHits.Pop();
		OnInput = SavedInputs.Pop();
		OnInputRelease = SavedInputReleases.Pop();
	}
}