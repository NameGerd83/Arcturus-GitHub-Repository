/*///////////////////////////////////////////////
Name: Greg Clark
Date Started/Modified: 8/30/2012
Purpose/Changes: Rotate objects 
Attach To: Anything wishes to be rotated
///////////////////////////////////////////////*/
	
using UnityEngine;
using System.Collections;

public class ObjectMove : MonoBehaviour 
{
	public float 			xAxis, yAxis, zAxis;	//Manually change rotation
	public float			xVect, yVect, zVect;	//Manually change position
	
	private float			randomXaxis;
	private float			randomYaxis;
	private float			randomZaxis;
	
	public bool				isAsteroid;
	public bool				isWall;
	
	void Awake()
	{
		//Asteroids movement
		if(isAsteroid)
		{
			randomXaxis = Random.Range(-.5f, .5f);
			xAxis = randomXaxis;
			
			randomYaxis = Random.Range(-.5f, .5f);
			yAxis = randomYaxis;
			
			randomZaxis = Random.Range(-.5f, .5f);
			zAxis = randomZaxis;
		}
		
		//Asteroid wall movement
		if(isWall)
			zVect = -.2f;
	}
	
	//Manual rotate and velocity variables
	void Update() 
	{
		transform.Rotate(xAxis, yAxis, zAxis, Space.Self);
		GetComponent<Rigidbody>().velocity = new Vector3(xVect, yVect, zVect);
	}
}
