
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define DPAD_SUPPORT_UNITY2D
#endif

using UnityEngine;
using System.Collections;

/// <summary>
/// A simple, responsive d-pad for games that use touch controls.
/// Author: Diorgo Jonkers
/// Version: 1.0.1
/// 
/// DISCLAIMER OF WARRANTY
/// THIS SOFTWARE IS PROVIDED "AS IS". THE AUTHOR DISCLAIMS ALL WARRANTIES, EXPRESSED OR IMPLIED, 
/// INCLUDING, WITHOUT LIMITATION, THE WARRANTIES OF MERCHANTABILITY AND OF FITNESS FOR ANY PURPOSE.
/// THE AUTHOR ASSUMES NO LIABILITY FOR DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
/// CONSEQUENTIAL DAMAGES, WHICH MAY RESULT FROM THE USE OF THE SOFTWARE, EVEN IF ADVISED OF THE 
/// POSSIBILITY OF SUCH DAMAGE. AUTHOR DOES NOT WARRANT THAT THE SOFTWARE WILL FUNCTION WITHOUT 
/// INTERRUPTION OR BE ERROR FREE, THAT AUTHOR WILL CORRECT ALL DEFICIENCIES, ERRORS, DEFECTS OR 
/// NONCONFORMITIES OR THAT THE SOFTWARE WILL MEET YOUR SPECIFIC REQUIREMENTS. THIS DISCLAIMER OF 
/// WARRANTY CONSTITUTES AN ESSENTIAL PART OF THIS AGREEMENT. NO USE OF THE SOFTWARE IS AUTHORIZED 
/// HEREUNDER EXCEPT UNDER THIS DISCLAIMER.
/// 
/// </summary>
public class DynamicDpad : MonoBehaviour
{
	// Const/Static
	//-------------
	// Minimum value of the deadzone after it has been scaled. This ensures there is at least a deadzone.
	private const float minDeadZoneScaled = 5.0f;
	
	// Minimum value of the radius after it has been scaled. This ensures there is at least a radius.
	private const float minRadiusScaled = 10.0f;
	
	// Minimum size of the input rect after it has been scaled. This ensures there is at least a rect for input.
	private const float minInputRectScaled = 100.0f;
	
	
	// Public
	//-------
	/// <summary>
	/// Indicates if debug info should be shown. (Turn this off in the release build.)
	/// </summary>
	public bool displayDebug = true;
	
	
	/// <summary>
	/// Colour to use for debug info.
	/// </summary>
	public Color displayDebugColour = Color.white;
	
	
	/// <summary>
	/// The camera used for GUI rendering. This is only needed if the GetWorld methods are used, or if displayDebug is used.
	/// If this is null then the first active orthographic camera will be used, or the main camera if there is no 
	/// orthographic camera (if the main camera is set).
	/// </summary>
	public Camera guiCamera = null;
	
	/// <summary>
	/// The screen height on which the pixel scale is 1:1 (e.g. if it is iPad 1 landscape then it will be 768).
	/// This allows the input to be automatically scaled for various screen sizes (e.g. radiusPixels will be scaled).
	/// </summary>
	public float baseScreenHeight = 768.0f;
	
	/// <summary>
	/// The deadzone radius (in pixels). There is no movement when the touch is inside the deadzone.
	/// This is overridden by deadZoneRadiusInches if Unity can detect the device's DPI.
	/// (A good value is 5 pixels.)
	/// </summary>
	public float deadZoneRadiusPixels = 5.0f;
	
	/// <summary>
	/// The deadzone radius (in inches). This overrides deadZoneRadiusPixels, but will only be used if Unity can detect the device's DPI.
	/// (A good value is 0.08 inches.)
	/// </summary>
	public float deadZoneRadiusInches = 0.08f;
	
	/// <summary>
	/// The d-pad radius (in pixels). A touch is active only within this radius.
	/// This is overridden by radiusInches if Unity can detect the device's DPI.
	/// (A good value is 40 pixels.)
	/// </summary>
	public float radiusPixels = 40.0f;
	
	/// <summary>
	/// The d-pad radius (in inches). This overrides radiusPixels, but will only be used if Unity can detect the device's DPI.
	/// (A good value is 0.39 inches.)
	/// </summary>
	public float radiusInches = 0.39f;
	
	
	/// <summary>
	/// A touch must start within this screen rectangle for input to be processed. (Screen bottom-left = 0,0)
	/// </summary>
	public Rect inputScreenRect = new Rect(0, 0, 512, 768);
	
	
	/// <summary>
	/// Indicates if the d-pad is left-handed. This flips the d-pad to the other side of the screen.
	/// Use this if you want to position inputScreenRect relative to the bottom-right of the screen.
	/// You can also use this if your game uses two d-pads (e.g. dual stick), for the d-pad on the right side of the screen.
	/// Use "SetLeftHanded" to change it dynamically.
	/// </summary>
	public bool leftHanded = false;
	
	
	/// <summary>
	/// Indicates if the d-pad's centre can be dragged with the touch if the touch goes further than the radius.
	/// (A good value is "true". It makes it easier for the player to change direction.)
	/// </summary>
	public bool allowDraggingCentre = true;
	
	
	/// <summary>
	/// Indicates if precision movement must be used (e.g. if touch is dragged half-way along the X radius then the X axis value is 0.5).
	/// You get better results if the radius is bigger when you need precision movement.
	/// If this is false then the axes vector is normalized.
	/// (A good value is "false". It will be more responsive.)
	/// </summary>
	public bool usePrecision = false;
	
	
	/// <summary>
	/// Indicates if absolute values must be used. If this is true then the axes will be one of the following values: -1, 0, 1.
	/// If this is true then usePrecision will be ignored.
	/// (A good value is "true" if you want the game to feel like the NES d-pad, which uses 8 directions only.)
	/// </summary>
	public bool useAbsoluteValues = false;
	
	
	/// <summary>
	/// Indicates if axes bias must be used. This means that if the drag distance is greater on one axis then the other axis will 
	/// have a zero value. Essentially, it dynamically changes the deadzone per axis.
	/// (A good value is "true" if useAbsoluteValues=true. Or if your game requires horizontal or vertical movement more often 
	/// than diagonal movement. Such as a side-scrolling beat 'em up, or an FPS in which you move forward and strafe.)
	/// </summary>
	public bool useAxesBias = false;
	
	
	
	// Private
	//--------
	private Vector3 touchPos;		// Touch position.
	private Vector3 startPos;		// Start position of the touch. This is also the d-pad's centre.
	private int fingerId;			// ID of the finger that is down.
	private Vector3 knobPos;		// Knob position, used for rendering only.
	private Vector2 axes;			// Input axes.
	private float deadZone;			// Input deadzone. There is no movement when the touch is inside the deadzone.
	private float outerDeadzone;	// Outer deadzone used when useAxesBias=true.
	private float radius;			// Input radius. A touch is active only within this radius.
	private Rect rect;				// A touch must start within this screen rectangle for input to be processed. Screen bottom-left = 0,0.
	private bool didSetDynamicRect = false;	// Indicates if the rect was set vie SetInputScreenRect.
	private Rect dynamicInputScreenRect;	// The rect set via SetInputScreenRect.
	
#if UNITY_EDITOR
	private bool didWarnDeadZone = false;	// Indicates if the deadzone warning was displayed
	private bool didWarnRect = false;		// Indicates if the rect warning was displayed
#endif //UNITY_EDITOR
	
	
	
	/// <summary>
	/// Gets the directional axes.
	/// </summary>
	/// <value>
	/// The directional axes.
	/// </value>
	public Vector2 Axes
	{
		get { return(axes); }
	}
	
	
	/// <summary>
	/// Gets the X value of the directional axes.
	/// </summary>
	/// <value>
	/// X.
	/// </value>
	public float X
	{
		get { return(axes.x); }
	}
	
	
	/// <summary>
	/// Gets the Y value of the directional axes.
	/// </summary>
	/// <value>
	/// Y.
	/// </value>
	public float Y
	{
		get { return(axes.y); }
	}
	
	
	/// <summary>
	/// Test if the d-pad has a touch in progress. Ideally, do Not render the d-pad when this returns false.
	/// </summary>
	/// <value>
	/// True/false.
	/// </value>
	public bool HasTouch
	{
		get { return(fingerId != -1); }
	}
	
	
	/// <summary>
	/// Test if the d-pad has non-zero input (i.e. touch is outside the deadzone).
	/// </summary>
	/// <value>
	/// True/false.
	/// </value>
	public bool HasNonZeroInput
	{
		get { return((HasTouch) && (axes.sqrMagnitude > 0.0f)); }
	}
	
	
	/// <summary>
	/// Gets the d-pad's radius (in pixels). This can be used for drawing your own d-pad.
	/// </summary>
	/// <value>
	/// The radius.
	/// </value>
	public float Radius
	{
		get { return(radius); }
	}
	
	
	/// <summary>
	/// Gets the d-pad's deadzone (in pixels). This can be used for drawing your own d-pad.
	/// </summary>
	/// <value>
	/// The radius.
	/// </value>
	public float DeadZone
	{
		get { return(deadZone); }
	}
	
	
	
	// Methods
	//--------
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Awake()
	{
		PositionControls();
		
		// Get the camera
		if (guiCamera == null)
		{
			if ((Camera.allCameras != null) && (Camera.allCameras.Length > 0))
			{
				// Use the first active orthographic camera
				foreach (Camera cam in Camera.allCameras)
				{
					if ((cam != null) && (cam.orthographic))
					{
						guiCamera = cam;
						break;
					}
				}
			}
			
			if (guiCamera == null)
			{
				// Use the main camera
				guiCamera = Camera.main;
			}
		}
		
		Reset();
	}
	
	
	/// <summary>
	/// Positions the controls.
	/// </summary>
	private void PositionControls()
	{
		float dpi = Screen.dpi;
		float pixelScale = (float)Screen.height / baseScreenHeight;
		
		// Could Unity detect the device's DPI?
		if (dpi > 0.0f)
		{
			// Scale input based on the DPI
			deadZone = Mathf.Max(deadZoneRadiusInches * dpi, minDeadZoneScaled);
			outerDeadzone = Mathf.Max((deadZoneRadiusInches * 2.0f) * dpi, minDeadZoneScaled);
			radius = Mathf.Max(radiusInches * dpi, minRadiusScaled);
		}
		else
		{
			// Scale input based on the screen height
			deadZone = Mathf.Max(deadZoneRadiusPixels * pixelScale, minDeadZoneScaled);
			outerDeadzone = Mathf.Max((deadZoneRadiusPixels * 2.0f) * pixelScale, minDeadZoneScaled);
			radius = Mathf.Max(radiusPixels * pixelScale, minRadiusScaled);
		}
		
		// Make sure the outer deadzone is Not too big (for axes bias)
		if (outerDeadzone > deadZone + ((radius - deadZone) / 2.0f))
		{
			outerDeadzone = deadZone + ((radius - deadZone) / 2.0f);
		}
		
		
		// Scale rect based on the screen height
		rect.x = inputScreenRect.x * pixelScale;
		rect.y = inputScreenRect.y * pixelScale;
		rect.width = Mathf.Max(inputScreenRect.width * pixelScale, minInputRectScaled);
		rect.height = Mathf.Max(inputScreenRect.height * pixelScale, minInputRectScaled);
		
		// Was the rect set via SetInputScreenRect?
		if (didSetDynamicRect)
		{
			rect = dynamicInputScreenRect;
		}
		
		if (leftHanded)
		{
			// Position the rect relative to the bottom-right of the screen
			rect.x = ((float)Screen.width - rect.x) - rect.width;
		}
		
#if UNITY_EDITOR
		// Test if the deadzone is bigger than the radius and display a warning in the editor
		if ((deadZone >= radius) && (didWarnDeadZone == false))
		{
			didWarnDeadZone = true;
			Debug.LogWarning("DYNAMIC D-PAD WARNING: The deadZone is greater than the radius. The d-pad will not work properly." + 
							" The d-pad is on the game object: " + name);
		}
		
		// Test if the input rect is outside the screen area
		if (((rect.x >= Screen.width) || (rect.y >= Screen.height) || 
			 (rect.x + rect.width <= 0.0f) || (rect.y + rect.height <= 0.0f)) &&
			(didWarnRect == false))
		{
			didWarnRect = true;
			Debug.LogWarning("DYNAMIC D-PAD WARNING: The input screen rect is outside the screen area. The d-pad will not work properly." + 
							" The d-pad is on the game object: " + name);
		}
#endif //UNITY_EDITOR
		
	}
	
	
	/// <summary>
	/// Reset the d-pad variables. This can be called at the start of a level and when the game pauses/unpauses.
	/// </summary>
	public void Reset()
	{
		touchPos = Vector3.zero;
		startPos = Vector3.zero;
		knobPos = Vector3.zero;
		fingerId = -1;
		axes = Vector2.zero;
	}
	
	
	/// <summary>
	/// Resets the touch's start X position. It moves the d-pad's centre X to the touch position X.
	/// It can be used to briefly stop the player moving on the X axis.
	/// This can be useful if you make a platformer game: call this method when the character starts climbing a 
	/// ladder so that he briefly stops moving horizontally (instead of moving forward and falling off the ladder).
	/// </summary>
	public void ResetStartX()
	{
		startPos.x = touchPos.x;
	}
	
	
	/// <summary>
	/// Resets the touch's start Y position. It moves the d-pad's centre Y to the touch position Y.
	/// It can be used to briefly stop the player moving on the Y axis.
	/// (See the ResetStartX comment for an example of use.)
	/// </summary>
	public void ResetStartY()
	{
		startPos.y = touchPos.y;
	}
	
	
	/// <summary>
	/// Sets the left handed.
	/// </summary>
	/// <param name='leftHanded'>
	/// Left handed.
	/// </param>
	public void SetLeftHanded(bool leftHanded)
	{
		this.leftHanded = leftHanded;
		PositionControls();
		Reset();
	}
	
	
	/// <summary>
	/// Dynamically set the input screen rect.
	/// For example, this can be used to make your rect half the screen size.
	/// </summary>
	/// <param name='newRect'>
	/// New rect.
	/// </param>
	public void SetInputScreenRect(Rect newRect)
	{
		didSetDynamicRect = true;
		dynamicInputScreenRect = newRect;
		rect = newRect;
		
		if (leftHanded)
		{
			// Position the rect relative to the bottom-right of the screen
			rect.x = ((float)Screen.width - rect.x) - rect.width;
		}
		
		Reset();
	}


#if DPAD_SUPPORT_UNITY2D
	/// <summary>
	/// Positions the d-pad's GUITexture.
	/// </summary>
	/// <param name="dpadGui">Dpad GUI.</param>
	public void PositionGUITexture(GUITexture dpadGui)
	{
		if ((guiCamera == null) || (dpadGui == null))
		{
			return;
		}

		Vector3 world = guiCamera.ScreenToViewportPoint(new Vector3(startPos.x, startPos.y, guiCamera.nearClipPlane + 0.0001f));
		dpadGui.transform.position = new Vector3(world.x, world.y, dpadGui.transform.position.z);
	}


	/// <summary>
	/// Positions the d-pad knob's GUITexture.
	/// </summary>
	/// <param name="dpadKnobGui">Dpad knob GUI.</param>
	public void PositionKnobGUITexture(GUITexture dpadKnobGui)
	{
		if ((guiCamera == null) || (dpadKnobGui == null))
		{
			return;
		}
		
		Vector3 world = guiCamera.ScreenToViewportPoint(new Vector3(knobPos.x, knobPos.y, guiCamera.nearClipPlane + 0.0001f));
		dpadKnobGui.transform.position = new Vector3(world.x, world.y, dpadKnobGui.transform.position.z);
	}
#endif //DPAD_SUPPORT_UNITY2D

	
	/// <summary>
	/// Get the d-pad centre's screen position. (Screen bottom-left = 0,0)
	/// Use this to position your d-pad GUI when your GUI uses screen coordinates.
	/// Use HasTouch to determine if the d-pad should be rendered.
	/// </summary>
	/// <returns>
	/// The screen position.
	/// </returns>
	public Vector3 GetScreen()
	{
		return (startPos);
	}
	
	
	/// <summary>
	/// Get the d-pad centre's screen position with an inverted Y. (Screen top-left = 0,0)
	/// Use this to position your d-pad GUI when your GUI uses screen coordinates.
	/// Use HasTouch to determine if the d-pad should be rendered.
	/// </summary>
	/// <returns>
	/// The screen position.
	/// </returns>
	public Vector3 GetScreenInvertY()
	{
		return (new Vector3(startPos.x, Screen.height - startPos.y, startPos.z));
	}
	
	
	/// <summary>
	/// Get the d-pad knob's screen position. (Screen bottom-left = 0,0)
	/// Use this to position your d-pad knob GUI when your GUI uses screen coordinates.
	/// Use HasTouch to determine if the d-pad should be rendered.
	/// </summary>
	/// <returns>
	/// The screen position.
	/// </returns>
	public Vector3 GetKnobScreen()
	{
		return (knobPos);
	}
	
	
	/// <summary>
	/// Get the d-pad knob's screen position with an inverted Y (i.e. the touch position). (Screen top-left = 0,0)
	/// Use this to position your d-pad knob GUI when your GUI uses screen coordinates.
	/// Use HasTouch to determine if the d-pad should be rendered.
	/// </summary>
	/// <returns>
	/// The screen position.
	/// </returns>
	public Vector3 GetKnobScreenInvertY()
	{
		return (new Vector3(knobPos.x, Screen.height - knobPos.y, knobPos.z));
	}
	
	
	/// <summary>
	/// Get the d-pad centre's world position.
	/// Use this to position your d-pad GUI when your GUI uses world coordinates.
	/// Use HasTouch to determine if the d-pad should be rendered.
	/// PLEASE NOTE: If you are using a non-orthographic camera, then ideally this should be called from LateUpdate after all the 
	/// camera movement has been updated, or else it may be misaligned when the camera moves.
	/// </summary>
	/// <returns>
	/// The world position. Returns zero vector if no guiCamera has been defined.
	/// </returns>
	/// <param name='z'>
	/// The world Z position to return.
	/// </param>
	public Vector3 GetWorld(float z)
	{
		if (guiCamera == null)
		{
			return (Vector3.zero);
		}
		
		Vector3 world = guiCamera.ScreenToWorldPoint(new Vector3(startPos.x, startPos.y, guiCamera.nearClipPlane + 0.0001f));
		world.z = z;
		return (world);
	}
	
	
	/// <summary>
	/// Get the d-pad knob's world position.
	/// Use this to position your d-pad knob GUI when your GUI uses world coordinates.
	/// Use HasTouch to determine if the d-pad should be rendered.
	/// PLEASE NOTE: If you are using a non-orthographic camera, then ideally this should be called from LateUpdate after all the 
	/// camera movement has been updated, or else it may be misaligned when the camera moves.
	/// </summary>
	/// <returns>
	/// The world position. Returns zero vector if no guiCamera has been defined.
	/// </returns>
	/// <param name='z'>
	/// The world Z position to return.
	/// </param>
	public Vector3 GetKnobWorld(float z)
	{
		if (guiCamera == null)
		{
			return (Vector3.zero);
		}
		
		Vector3 world = guiCamera.ScreenToWorldPoint(new Vector3(knobPos.x, knobPos.y, guiCamera.nearClipPlane + 0.0001f));
		world.z = z;
		return (world);
	}
	

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	void Update()
	{
		if (fingerId != -1)
		{
			// Update the existing touch
			
			bool touchIsActive = false;
			
			foreach (Touch touch in Input.touches)
			{
				if (touch.fingerId == fingerId)
				{
					if ((touch.phase == TouchPhase.Moved) || (touch.phase == TouchPhase.Stationary))
					{
						UpdateTouch(touch.position);
						touchIsActive = true;
					}
					break;
				}
			}
			
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBPLAYER
			// Simulate a touch, using the mouse
			if ((Input.GetMouseButton(0)) || (Input.GetMouseButton(1)) || (Input.GetMouseButton(2)))
			{
				UpdateTouch(Input.mousePosition);
				touchIsActive = true;
			}
#endif
			
			if (touchIsActive == false)
			{
				fingerId = -1;
				axes = Vector2.zero;
			}
		}
		else
		{
			// Check for a new touch
			foreach (Touch touch in Input.touches)
			{
				if (touch.phase == TouchPhase.Began)
				{
					if (StartTouch(touch.position, touch.fingerId))
					{
						break;
					}
				}
			}
			
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBPLAYER
			// Simulate a touch, using the mouse
			if ((Input.GetMouseButtonDown(0)) || (Input.GetMouseButtonDown(1)) || (Input.GetMouseButtonDown(2)))
			{
				StartTouch(Input.mousePosition, 0);
			}
#endif
		}
		
	}
	
	
	/// <summary>
	/// Late update.
	/// </summary>
	void LateUpdate()
	{
		if (displayDebug)
		{
			DisplayDebug();
		}
	}
	
	
	/// <summary>
	/// Starts the touch.
	/// </summary>
	/// <returns>
	/// True if it started, false otherwise.
	/// </returns>
	/// <param name='position'>
	/// If set to <c>true</c> position.
	/// </param>
	/// <param name='fingerId'>
	/// If set to <c>true</c> finger identifier.
	/// </param>
	private bool StartTouch(Vector3 position, int fingerId)
	{
		// Check if the touch starts outside the input rect
		if ((position.x < rect.x) || (position.x > rect.x + rect.width) || 
			(position.y < rect.y) || (position.y > rect.y + rect.height))
		{
			return (false);
		}
		
		this.fingerId = fingerId;
		startPos = position;
		touchPos = startPos;
		knobPos = touchPos;
		
		return (true);
	}
	
	
	/// <summary>
	/// Updates the touch.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	private void UpdateTouch(Vector3 position)
	{
		Vector2 diff;
		
		touchPos = position;
		knobPos = touchPos;
		
		diff.x = touchPos.x - startPos.x;
		diff.y = touchPos.y - startPos.y;
		
		// Test if the centre is dragged
		if ((allowDraggingCentre) && (diff.magnitude > radius))
		{
			diff.Normalize();
			diff *= radius;
			startPos.x = touchPos.x - diff.x;
			startPos.y = touchPos.y - diff.y;
				
			diff.x = touchPos.x - startPos.x;
			diff.y = touchPos.y - startPos.y;
		}
		else if (diff.magnitude > radius)
		{
			// Keep the visual knob within the radius
			diff.Normalize();
			diff *= radius;
			knobPos.x = startPos.x + diff.x;
			knobPos.y = startPos.y + diff.y;
		}
		
		
		if (useAxesBias)
		{
			// Use separate deadzones for X and Y
			
			diff = Vector2.ClampMagnitude(diff, radius);
			
			// Test if X is still inside the deadzone OR
			// X is inside the outer deadzone and Y is outside the outer deadzone
			if ( (Mathf.Abs(diff.x) < deadZone) ||
				 ((Mathf.Abs(diff.x) < outerDeadzone) && (Mathf.Abs(diff.y) >= outerDeadzone)) )
			{
				axes.x = 0.0f;
			}
			else
			{
				// Convert axes to a percentage of the radius
				axes.x = diff.x / radius;
			}
			
			// Test if Y is still inside the deadzone OR
			// Y is inside the outer deadzone and X is outside the outer deadzone
			if ( (Mathf.Abs(diff.y) < deadZone) ||
				 ((Mathf.Abs(diff.y) < outerDeadzone) && (Mathf.Abs(diff.x) >= outerDeadzone)) )
			{
				axes.y = 0.0f;
			}
			else
			{
				// Convert axes to a percentage of the radius
				axes.y = diff.y / radius;
			}
		}
		else if (diff.sqrMagnitude < (deadZone * deadZone))
		{
			// Touch is inside the deadzone
			axes = Vector2.zero;
		}
		else
		{
			diff = Vector2.ClampMagnitude(diff, radius);
			// Convert axes to a percentage of the radius
			axes.x = diff.x / radius;
			axes.y = diff.y / radius;
		}
		
		
		if (useAbsoluteValues)
		{
			float deadZonePercent = deadZone / radius;
			if (axes.x <= -deadZonePercent)
			{
				axes.x = -1.0f;
			}
			else if (axes.x >= deadZonePercent)
			{
				axes.x = 1.0f;
			}
			else
			{
				axes.x = 0.0f;
			}
			
			if (axes.y <= -deadZonePercent)
			{
				axes.y = -1.0f;
			}
			else if (axes.y >= deadZonePercent)
			{
				axes.y = 1.0f;
			}
			else
			{
				axes.y = 0.0f;
			}
		}
		else if (usePrecision == false)
		{
			axes.Normalize();
		}

	}
	
	
	/// <summary>
	/// Display debug info.
	/// </summary>
	private void DisplayDebug()
	{
		if ((guiCamera == null) || (displayDebug == false))
		{
			return;
		}
		
		Vector3[] points = new Vector3[10];
		Vector3 vec;
		int i;
		float angle;
		float z = guiCamera.nearClipPlane + 0.0001f;
		
		
		// Active rectangle
		i = 0;
		points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(rect.x, rect.y, z));
		points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(rect.x + rect.width, rect.y, z));
		points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(rect.x + rect.width, rect.y + rect.height, z));
		points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(rect.x, rect.y + rect.height, z));
		
		Debug.DrawLine(points[0], points[1], displayDebugColour, 0.0f, false);
		Debug.DrawLine(points[1], points[2], displayDebugColour, 0.0f, false);
		Debug.DrawLine(points[2], points[3], displayDebugColour, 0.0f, false);
		Debug.DrawLine(points[3], points[0], displayDebugColour, 0.0f, false);
		
		if (HasTouch)
		{
			// Knob
			i = 0;
			points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(knobPos.x, knobPos.y - 30.0f, z));
			points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(knobPos.x, knobPos.y + 30.0f, z));
			points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(knobPos.x - 30.0f, knobPos.y, z));
			points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(knobPos.x + 30.0f, knobPos.y, z));
			Debug.DrawLine(points[0], points[1], displayDebugColour, 0.0f, false);
			Debug.DrawLine(points[2], points[3], displayDebugColour, 0.0f, false);
			
			// Touch
			i = 0;
			points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y - 10.0f, z));
			points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y + 10.0f, z));
			points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(touchPos.x - 10.0f, touchPos.y, z));
			points[i++] = guiCamera.ScreenToWorldPoint(new Vector3(touchPos.x + 10.0f, touchPos.y, z));
			Debug.DrawLine(points[0], points[1], displayDebugColour, 0.0f, false);
			Debug.DrawLine(points[2], points[3], displayDebugColour, 0.0f, false);
			
			// D-pad centre
			angle = 0.0f;
			for (i = 0; i < points.Length; i++)
			{
				vec = Quaternion.Euler(0.0f, 0.0f, angle) * (Vector3.up * radius);
				points[i] = guiCamera.ScreenToWorldPoint(new Vector3(startPos.x + vec.x, startPos.y + vec.y, z));
				if (i > 0)
				{
					Debug.DrawLine(points[i - 1], points[i], displayDebugColour, 0.0f, false);
				}
				angle += (360.0f / (points.Length - 1));
			}
		}

	}
	
}
