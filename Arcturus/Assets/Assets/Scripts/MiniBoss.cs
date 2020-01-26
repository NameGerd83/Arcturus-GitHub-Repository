using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class MiniBoss : MonoBehaviour 
	{
		private GameObject				playerObject; //Player ship
		public GameObject				enemyBullet; //Enemy's bullet
		
		public ParticleEmitter			enemyExplosion; //Explosion particles
		
		public float 					fireRange = 5f; //Range between player and enemy
		public float					fireRate = 3f; //Rate between each bullet
		public float					nextFire = .5f; //Timer to allow fire rate
		public float					bulletSpeed = .6f; //Speed of enemy bullet
		
		public int						miniBossHealth = 6; //Health of ship
		
		private bool					miniBossDead = true; //Check for death
		
		void Awake() 
		{
			playerObject = GameObject.FindGameObjectWithTag("Player");
		}
	
		void Update() 
		{
			//face toward and attack player
			if(playerObject != null)
			{
				if(Vector3.Distance(this.transform.position, playerObject.transform.position) <= fireRange)
				{
					transform.LookAt(playerObject.transform);
					AttackPlayer();
				}
			}
		}
		
		void OnTriggerEnter(Collider other)
		{
			//If hit by player or player bullet, decrease health by one
			if(other.gameObject.tag == "PlayerBullet" || other.gameObject.tag == "Player")		
			{
				miniBossHealth--;
				
				AudioControl.PlayAudio("Shield");
				
				//If health reaches 0, destroy and count down 1 boss 
				if(miniBossHealth <= 0)
				{
					//Only count down 1 and then cut off
					if(miniBossDead)
					{
						MasterControl.miniBossCount--;
						miniBossDead = false;
					}
					
					StartCoroutine("DestroyObject");				
				}
			}
		}
		
		//Shoot at player
		public void AttackPlayer()
		{
			if(Time.time > nextFire)
			{
				nextFire = Time.time + fireRate;
				
				var clone = Instantiate(enemyBullet, this.transform.position, this.transform.rotation) as GameObject;
				clone.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
			}
		}	
		
		//Destroy sequence
		IEnumerator DestroyObject()
		{
			yield return new WaitForSeconds(.02f);
			
			AudioControl.PlayAudio("Explosion");
			Instantiate(enemyExplosion, this.transform.position, this.transform.rotation);
			Destroy(this.gameObject);
		}
	}
}