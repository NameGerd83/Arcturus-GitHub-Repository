//private var motor : CharacterMotor;
//
//
//var graphics : GameObject;
//
//
//private var dpad : DynamicDpad;	// The dynamic d-pad used for movement
//
//
//// Use this for initialization
//function Awake () {
//	motor = GetComponent(CharacterMotor);
//	
//	if (graphics)
//	{
//		// Get a reference to the d-pad
//		dpad = graphics.GetComponent("DynamicDpad");
//	}
//}
//
//// Update is called once per frame
//function Update () {
//	// Get the input vector from keyboard or analog stick
//	var directionVector = Vector3.zero;
//	
//	if (dpad)
//	{
//		directionVector = new Vector3(dpad.Axes.x, 0, dpad.Axes.y);
//	}
//	
//	if (directionVector != Vector3.zero) {
//		// Get the length of the directon vector and then normalize it
//		// Dividing by the length is cheaper than normalizing when we already have the length anyway
//		var directionLength = directionVector.magnitude;
//		directionVector = directionVector / directionLength;
//		
//		// Make sure the length is no bigger than 1
//		directionLength = Mathf.Min(1, directionLength);
//		
//		// Make the input vector more sensitive towards the extremes and less sensitive in the middle
//		// This makes it easier to control slow speeds when using analog sticks
//		directionLength = directionLength * directionLength;
//		
//		// Multiply the normalized direction vector by the modified length
//		directionVector = directionVector * directionLength;
//	}
//	
//	// Apply the direction to the CharacterMotor
//	motor.inputMoveDirection = transform.rotation * directionVector;
//	motor.inputJump = Input.GetButton("Jump");
//}
//
//
//function OnGUI()
//{
//	// Instructions
//	GUI.Label(new Rect(5, 5, 200, 100), "Touch and drag on the left side of the screen to move.");
//	GUI.Label(new Rect(Screen.width - 205, 5, 200, 100), "Touch and drag on the right side of the screen to rotate the camera.");
//}
//
//// Require a character controller to be attached to the same game object
//@script RequireComponent (CharacterMotor)
//@script AddComponentMenu ("Character/FPS Input Controller")
