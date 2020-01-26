using UnityEngine;
using System.Collections;

public class DemoMenu : MonoBehaviour {


	/// <summary>
	/// Render the GUIs.
	/// </summary>
	void OnGUI()
	{
		string[] demoLevels = {"Demo1 - SideScroller", "Demo2 - TopDown", "Demo3 - TopDown Dual Dpads", 
								"Demo4 - ThirdPerson", "Demo5 - FirstPerson"};
		string[] demoDescriptions = {"Demo 1 - Side Scroller", "Demo 2 - Top Down", "Demo 3 - Top Down Dual D-pads", 
								"Demo 4 - Third Person", "Demo 5 - First Person"};
		float pixelScale = (Screen.height / 320.0f);
		Vector2 buttonSize = new Vector2(220.0f, 40.0f * pixelScale);
		float spaceY = 10.0f;
		int i;
		Vector2 pos = new Vector2((Screen.width / 2) - (buttonSize.x / 2.0f), 
									(Screen.height / 2) - (((float)demoLevels.Length / 2.0f) * (buttonSize.y + spaceY)));
		
		// Title
		GUI.Label(new Rect(10.0f, 10.0f, 200.0f, 100.0f), "Dynamic D-pad");
		
		// Buttons
		i = 0;
		foreach (string demoLevel in demoLevels)
		{
			if (GUI.Button(new Rect(pos.x, pos.y, buttonSize.x, buttonSize.y), demoDescriptions[i]))
			{
				Application.LoadLevel(demoLevel);
				break;
			}
			pos.y += (buttonSize.y + spaceY);
			i ++;
		}
		
		// Tagline
		GUI.Label(new Rect(10.0f, Screen.height - 25, 400.0f, 100.0f), "A simple, responsive d-pad for games that use touch controls.");
	}

}
