using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class LaserControl : MonoBehaviour 
	{
		public GameObject			laserSpawn;
		public GameObject			bulletObject; //Ship bullets
		
		public static int			multiShotSwitch = 1;
		
		private float				bulletSpeed = 1.25f; //Adjustable bullet speed
		private float 				nextLaserFire; //Interval timer between bullets
		private float				fireRate = .2f; //How fast the intervals will iterate
		
		public static bool			cantShoot = true;
		
		void Update()
		{
			//If false, allow to shoot
			if(!cantShoot)
			{
				if(Input.GetKey (KeyCode.Space))
					Fire();
			}
		}
				
		void Fire()
		{
			//Switch through each weapon level up
			switch(multiShotSwitch)
			{
				//Single bullet
				case 1:
					if(Time.time > nextLaserFire)
					{
						nextLaserFire = Time.time + fireRate;
						
						var clone1 = Instantiate(bulletObject, laserSpawn.transform.position, laserSpawn.transform.rotation) as GameObject;
						clone1.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
						
						AudioControl.PlayAudio("Laser");
					}
				break;
				
				//Double bullets
				case 2:
					if(Time.time > nextLaserFire)
					{
						nextLaserFire = Time.time + fireRate;
						
						var clone1 = Instantiate(bulletObject, laserSpawn.transform.position, laserSpawn.transform.rotation) as GameObject;
						clone1.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
						clone1.GetComponent<Rigidbody>().AddForce(10, 0, 0);
					
						var clone2 = Instantiate(bulletObject, laserSpawn.transform.position, laserSpawn.transform.rotation) as GameObject;
						clone2.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
						clone2.GetComponent<Rigidbody>().AddForce(-10, 0, 0);
					
						AudioControl.PlayAudio("Laser");
					}
				break;
				
				//Triple bullets
				case 3:
					
					MultipleLasers();
				
				break;
			}
		}
		
		//Triple bullets
		void MultipleLasers()
		{
			if(Time.time > nextLaserFire)
			{
				nextLaserFire = Time.time + fireRate;
							
				var clone1 = Instantiate(bulletObject, laserSpawn.transform.position, laserSpawn.transform.rotation) as GameObject;
				clone1.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
			
				var clone2 = Instantiate(bulletObject, laserSpawn.transform.position, laserSpawn.transform.rotation) as GameObject;
				clone2.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
				clone2.GetComponent<Rigidbody>().AddForce(20, 0, 0);
			
				var clone3 = Instantiate(bulletObject, laserSpawn.transform.position, laserSpawn.transform.rotation) as GameObject;
				clone3.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
				clone3.GetComponent<Rigidbody>().AddForce(-20, 0, 0);
				
				AudioControl.PlayAudio("Laser");
			}	
		}
		
		//increment through upgrades upon object collision
		void UpgradeLaser()
		{
			++multiShotSwitch;
		}	
	}
}