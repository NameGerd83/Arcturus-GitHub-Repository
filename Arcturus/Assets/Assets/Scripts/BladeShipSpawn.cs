using UnityEngine;
using System.Collections;

public class BladeShipSpawn : MonoBehaviour 
{
	public GameObject				bladeShip; //Blade ship object
	public GameObject				spawnPoint; //Spawnpoint location
	
 	private float					nextFire = 1; //Interval of each shot
	
	void OnTriggerStay(Collider other)
	{
		//If player is in trigger, continue to shoot out ships
		if(other.gameObject.tag == "Player")
		{
			nextFire += Time.deltaTime;
			
			if(nextFire >= 1)
			{
				nextFire = 0;
				
				Instantiate(bladeShip, spawnPoint.transform.position, spawnPoint.transform.rotation);
			}
		}
	}
}
