using UnityEngine;
using System.Collections;

public class MenuButton : MonoBehaviour {
	
	/// <summary>
	/// Render the GUIs.
	/// </summary>
	void OnGUI()
	{
		// Render the menu button in the demo scene and test if it is pressed.
		if (GUI.Button(new Rect((Screen.width / 2) - 30, 1, 60, 45), "Menu"))
		{
			Application.LoadLevel("Demo Menu");
		}
		
#if UNITY_EDITOR
		// Common help
		GUI.Label(new Rect(5, 50, 200, 100), "Enable \"Display Debug\" on the DynamicDpad component and turn on Gizmos to view the d-pad wireframe graphics.");
		GUI.Label(new Rect(Screen.width - 205, 50, 200, 100), "The d-pad is rendered in the class \"ExampleDpadRendering\"");
#endif //UNITY_EDITOR

	}
	
}
