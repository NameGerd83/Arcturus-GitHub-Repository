using UnityEngine;
using System.Collections;

/// <summary>
/// An example of how to get the d-pad and d-pad knob positions for rendering.
/// </summary>
public class ExampleDpadRendering : MonoBehaviour {
	
	// Public
	//-------
	public DynamicDpad dynamicDpad;		// The dynamic d-pad
	public GameObject dpad;				// The d-pad graphic
	public GameObject dpadKnob;			// The d-pad knob graphic
	
	
	// Methods
	//--------
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update()
	{
		if (dynamicDpad != null)
		{
			// Is there a touch?
			if (dynamicDpad.HasTouch)
			{
				if (dpad != null)
				{
					// Get the world position
					// You can use GetScreen or GetScreenInvertY to get the screen position if your GUIs use screen space.
					dpad.transform.position = dynamicDpad.GetWorld(dpad.transform.position.z);
					
					// Show the d-pad
					if (dpad.gameObject.active != dynamicDpad.HasTouch)
					{
						dpad.SetActiveRecursively(dynamicDpad.HasTouch);
					}
				}
				if (dpadKnob != null)
				{
					// Get the world position
					// You can use GetKnobScreen or GetKnobScreenInvertY to get the screen position if your GUIs use screen space.
					dpadKnob.transform.position = dynamicDpad.GetKnobWorld(dpadKnob.transform.position.z);
					
					// Show the d-pad knob
					if (dpadKnob.gameObject.active != dynamicDpad.HasTouch)
					{
						dpadKnob.SetActiveRecursively(dynamicDpad.HasTouch);
					}
				}
			}
			else
			{
				if ((dpad != null) && (dpad.gameObject.active != dynamicDpad.HasTouch))
				{
					// Hide the d-pad
					dpad.SetActiveRecursively(dynamicDpad.HasTouch);
				}
				if ((dpadKnob != null) && (dpadKnob.gameObject.active != dynamicDpad.HasTouch))
				{
					// Hide the d-pad knob
					dpadKnob.SetActiveRecursively(dynamicDpad.HasTouch);
				}
			}
		}
	}

}
