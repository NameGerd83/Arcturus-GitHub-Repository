using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class Turret : EnemyControl 
	{
		public GameObject				enemyBullet; //Turrets bullet
		public GameObject				bulletSpawnPoint; //Spawnpoint of where bullets shoot
		private GameObject				playerObject; //player ship to shoot at
		
		private float 					fireRange = 2f; //Range between player and turret
		private float					fireRate = 3f; //Rate between each bullet
		private float					bulletSpeed = .6f; //Bullets speed
		private float					nextFire = .5f; //Timer between game time and rate
		
		//Assign player to shoot at
		void Awake() 
		{
			playerObject = GameObject.FindGameObjectWithTag("Player");
		}
		
		void Update() 
		{
			//Aim at player and shoot
			if(playerObject != null)
			{
				if(Vector3.Distance(this.transform.position, playerObject.transform.position) <= fireRange)
				{
					transform.LookAt(playerObject.transform);
					AttackPlayer();
				}
			}	
		}
		
		//Shoot timer and spawn
		public void AttackPlayer()
		{
			if(Time.time > nextFire)
			{
				nextFire = Time.time + fireRate;
				
				var clone = Instantiate(enemyBullet, bulletSpawnPoint.transform.position, 
				bulletSpawnPoint.transform.rotation) as GameObject;
				clone.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
			}
		}	
	}
}