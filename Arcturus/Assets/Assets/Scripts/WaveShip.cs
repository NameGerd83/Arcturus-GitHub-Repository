using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class WaveShip : EnemyControl 
	{
		private float[]					twoWaves = {.1f, -.1f}; //Wave lengths
		private float					directionSpeed = 4; //ship speed
		private float					waveTimer = 4; //Time between each wave length
		private float 					timingOffset = 0; //Offset before each new wave
		private float					result; //Adjust speed of wave 
		private float 					zPos; //Move wave length in z axis
		
		private int 					myRandomIndex; //Indext the random range of integers
		public static int				positionCheck = 1; //Default position of spawn points
	
		private Vector3 				newStartPosition; //Random spawn point positions
		
		void Awake() 
		{
			//Cosine system to create wave system
			int numberofInts = twoWaves.Length;
			myRandomIndex = Random.Range(0, numberofInts);
			result = twoWaves[myRandomIndex];
		}
		
		//Preset each different position spawn point
		void Start()
		{
			if(positionCheck == 1)
				newStartPosition = MasterControl.staticShipSpawnPoints[Random.Range(1, 10)].transform.position;
			
			if(positionCheck == 2)
				newStartPosition = MasterControl.staticShipSpawnPoints[Random.Range(4, 7)].transform.position;
			
			if(positionCheck == 3)
				newStartPosition = MasterControl.staticShipSpawnPoints[Random.Range(6, 10)].transform.position;
			
			if(positionCheck == 4)
				newStartPosition = MasterControl.staticShipSpawnPoints[Random.Range(5, 8)].transform.position;
			
			if(positionCheck == 5)
				newStartPosition = MasterControl.staticShipSpawnPoints[Random.Range(5, 9)].transform.position;
			
			if(positionCheck == 6)
				newStartPosition = MasterControl.staticShipSpawnPoints[Random.Range(7, 10)].transform.position;
			
			if(positionCheck == 7)
				newStartPosition = MasterControl.staticShipSpawnPoints[Random.Range(1, 3)].transform.position;
		}
		
		void Update() 
		{
			//Face direction of movement along axis
			transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
			GetComponent<Rigidbody>().velocity = new Vector3(xVect, yVect, zVect);
			
			//Wave back and forth
	 		zPos -= Time.deltaTime / waveTimer;
		  	float offset = Mathf.Sin(Time.time * directionSpeed + timingOffset) * result;
			
			//Spawn point 
			transform.position = newStartPosition + new Vector3(offset, 0, zPos);
		}
	}
}