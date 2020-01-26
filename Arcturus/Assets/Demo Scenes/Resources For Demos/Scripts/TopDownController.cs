using UnityEngine;
using System.Collections;

public class TopDownController : MonoBehaviour {
	
	// Public
	//-------
	public Camera mainCamera;
	public float cameraHeight = 10.0f;
	
	public float movementSpeed = 10.0f;
	public float turnSpeed = 10.0f;
	
	public DynamicDpad rotationDpad;			// An additional d-pad for rotation
	
	
	// Private
	//--------
	private DynamicDpad dpad;					// The dynamic d-pad
	
	private CharacterController controller;		// Character controller for movement and collision detection
	
	private bool isMoving = false;				// Indicates if the character is moving
	
	
	/// <summary>
	/// Use this for initialization.
	/// </summary>
	void Start()
	{
		// Get a reference to the d-pad
		dpad = gameObject.GetComponent<DynamicDpad>();
		
		controller = gameObject.GetComponent<CharacterController>();
		
		UpdateCamera();
		
		PlayAnimation("idle");
	}
	
	
	/// <summary>
	/// Plays the animation.
	/// </summary>
	/// <param name='name'>
	/// Name.
	/// </param>
	private void PlayAnimation(string name)
	{
		if ((animation == null) || (animation[name] == null))
		{
			return;
		}
		animation.CrossFade(name, 0.2f);
	}
	
	
	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	void Update()
	{
		float dt = Time.deltaTime;
	
		bool moved = false;
		Vector3 direction;
		Quaternion rotation;
		
		if (dpad != null)
		{
			// Does the d-pad have input?
			if ((dpad.HasNonZeroInput) && 
				(controller != null))
			{
				direction = new Vector3(dpad.Axes.x * movementSpeed * dt, 0.0f, dpad.Axes.y * movementSpeed * dt);
				if (direction.sqrMagnitude != 0.0f)
				{
					controller.Move(direction);
				
					if (rotationDpad == null)
					{
						// Rotate in the direction of the movement
						rotation = Quaternion.LookRotation(direction);
						transform.rotation = Quaternion.Slerp(transform.rotation, rotation, dt * turnSpeed);
					}
					
					moved = true;
				}
			}
		}
		
		
		// Use a second d-pad for rotation
		if ((rotationDpad != null) && (rotationDpad.HasNonZeroInput))
		{
			// Rotate in the direction of the axes
			direction = new Vector3(rotationDpad.Axes.x, 0.0f, rotationDpad.Axes.y);
			rotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, dt * turnSpeed);
		}
		
		
		if (isMoving != moved)
		{
			// Play the relevant animation
			isMoving = moved;
			if (moved)
			{
				PlayAnimation("run");
			}
			else
			{
				PlayAnimation("idle");
			}
		}
		
	}
	
	
	/// <summary>
	/// Late update.
	/// </summary>
	void LateUpdate()
	{
		UpdateCamera();
	}
	
	
	/// <summary>
	/// Update the camera.
	/// </summary>
	private void UpdateCamera()
	{
		if (mainCamera == null)
		{
			return;
		}
		
		// Position camera directly above the player
		mainCamera.transform.position = transform.position + (Vector3.up * cameraHeight);
	}
	
	
	
	/// <summary>
	/// GUI rendering.
	/// </summary>
	void OnGUI()
	{
		// Instructions
		GUI.Label(new Rect(5, 5, 200, 100), "Touch and drag on the left side of the screen to move.");
		if (rotationDpad != null)
		{
			GUI.Label(new Rect(Screen.width - 205, 5, 200, 100), "Touch and drag on the right side of the screen to rotate.");
		}
	}

}
