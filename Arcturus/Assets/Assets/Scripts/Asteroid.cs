using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour 
{
	public float 			xAxis, yAxis, zAxis;	//Manually change rotation
	public float			xVect, yVect, zVect;	//Manually change position
	
	//Random axis for rotation
	private float			randomXaxis;
	private float			randomYaxis;
	private float			randomZaxis;
	
	//Random rotations
	void Awake()
	{
		randomXaxis = Random.Range(-.5f, .5f);
		xAxis = randomXaxis;
		
		randomYaxis = Random.Range(-.5f, .5f);
		yAxis = randomYaxis;
		
		randomZaxis = Random.Range(-.5f, .5f);
		zAxis = randomZaxis;
	}
	
	//Rotate the asteroids randomly and move them along Z axis
	void Update() 
	{
		transform.Rotate(xAxis, yAxis, zAxis, Space.Self);
		GetComponent<Rigidbody>().velocity = new Vector3(xVect, yVect, zVect);
	}
	
	//If passes through scene and hits bounds, destroy asteroid
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "EnemyBound")
		{
			Destroy(this.gameObject);
		}
	}
}
