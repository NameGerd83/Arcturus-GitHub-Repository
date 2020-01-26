using UnityEngine;
using System.Collections;

public class SideScrollerCamera : MonoBehaviour {
	
	// Public
	//-------
	public GameObject target;
	public float distance = 10.0f;
	public float height = 10.0f;
	
	
	// Methods
	//--------
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start()
	{
		UpdateCamera();
	}
	
	
	/// <summary>
	/// Late update.
	/// </summary>
	void LateUpdate()
	{
		UpdateCamera();
	}
	
	
	/// <summary>
	/// Updates the camera.
	/// </summary>
	private void UpdateCamera()
	{
		if (target == null)
		{
			return;
		}
		
		transform.position = new Vector3(target.transform.position.x, target.transform.position.y + height, target.transform.position.z - distance);
		transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
	}
}
