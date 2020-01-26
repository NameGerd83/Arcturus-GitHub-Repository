using UnityEngine;
using System.Collections;

public class SideScrollerController : MonoBehaviour {
	
	// Public
	//-------
	public float movementSpeed = 10.0f;
	public float turnSpeed = 10.0f;
	
	
	// Private
	//--------
	private DynamicDpad dpad;					// The dynamic d-pad
	
	private CharacterController controller;		// Character controller for movement and collision detection
	
	private bool isMoving = false;				// Indicates if the character is moving
	
	
	// Methods
	//--------
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start()
	{
		// Get a reference to the d-pad
		dpad = gameObject.GetComponent<DynamicDpad>();
		
		controller = gameObject.GetComponent<CharacterController>();
		
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
	/// Update is called once per frame
	/// </summary>
	void Update()
	{
		float dt = Time.deltaTime;
	
		bool moved = false;
		
		if (dpad != null)
		{
			// Does the d-pad have input?
			if ((dpad.HasNonZeroInput) && 
				(controller != null))
			{
				Vector3 direction = new Vector3(dpad.Axes.x * movementSpeed * dt, 0.0f, dpad.Axes.y * movementSpeed * dt);
				if (direction.sqrMagnitude != 0.0f)
				{
					controller.Move(direction);
				
					// Rotate in the direction of the movement
					Quaternion rotation = Quaternion.LookRotation(direction);
					transform.rotation = Quaternion.Slerp(transform.rotation, rotation, dt * turnSpeed);
					
					moved = true;
				}
			}
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
	/// GUI rendering.
	/// </summary>
	void OnGUI()
	{
		// Instructions
		GUI.Label(new Rect(5, 5, 200, 100), "Touch and drag on the left side of the screen to move.");
	}
}
