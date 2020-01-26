using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class PlayerHealth : MonoBehaviour 
	{
		private Transform				playerObject; //Player ship
		public GameObject				rocketObject; //Player rocket
		public GameObject[]				playerRespawn; //Player spawnpoint
		
		public ParticleEmitter			explosionParticle; //Player explosion particles
		
		public static int 				playerLives = 3; //Player lives

		private bool					respawnTimer = false; //Respawn timer check
		public static bool				lifeTaken = false; //Player dead check
		
		//Assign player ship
		void Awake()
		{
			playerObject = this.transform;
		}
	
		void Update() 
		{
			//Blink after respawning
			if(respawnTimer)
			{
				StartCoroutine(Blink(5, 0.2f, 0.4f));
	
				respawnTimer = false;
			}
		}
		
		void OnTriggerEnter(Collider other)
		{
			if(!lifeTaken)
			{
				//Collision with all enemy objects
				if(other.gameObject.tag == "Enemy" || other.gameObject.tag == "EnemyBullet" 
					|| other.gameObject.tag == "Asteroid")
				{
					//If player died, reset stars to default speed
					SendMessage("ResetStars");
					
					//If shields are active, count shield life
					if(PlayerControl.shieldsActive)
					{
						PlayerControl.shieldHealth--;
						
						AudioControl.PlayAudio("Shield");
						
						if(other.gameObject.tag == "Asteroid")
							PlayerControl.shieldHealth = 0;
						
						//If shield life has reached 0, hide shield
						if(PlayerControl.shieldHealth <= 0)
						{
							PlayerControl.staticShield.emit = false;
							PlayerControl.shieldsActive = false;
						}
					}
					//If shields are not active, destroy player and decrease life count
					else if(!PlayerControl.shieldsActive)
					{
						AudioControl.PlayAudio("Explosion");
						
						//Create explosion particle
						Instantiate(explosionParticle, this.transform.position, this.transform.rotation);
		
						if(playerLives > 1)
						{
							//Decrease lives by 1
							playerLives--;
							
							//Reset variables on player after death
							PlayerDead();

							//Reactivate player ship and being flash effect
							StartCoroutine("RespawnPlayer");
							
							MasterControl.upgradeItem = 0;
						}
						
						else
						{
							playerLives--;
							
							PlayerDead();
						}	
						
						//Deactivate collision while dead
					lifeTaken = true;
					}
				}
			}
		}

		public void PlayerDead()
		{
			//Turn off collision
			Physics.IgnoreLayerCollision(8, 9);
			Physics.IgnoreLayerCollision(8, 11);
			
			//Relocate to spawn point
			playerObject.transform.position = new Vector3(0, .55f, -2f);
			
			//Disable shooting and reset multishot to single
			LaserControl.cantShoot = true;
			LaserControl.multiShotSwitch = 1;
			
			//Turn off shield and reset shield health
			PlayerControl.shieldHealth = 3;
			PlayerControl.shieldsActive = false;
			
			//Hide player ship
			playerObject.GetComponent<Renderer>().enabled = false;
			rocketObject.GetComponent<Renderer>().enabled = false;
	
			//Disable controls
			SendMessage("StopControl");	
		}
			
		IEnumerator RespawnPlayer()
		{
			//Wait a brief moment then reactivate player 
			yield return new WaitForSeconds(1.25f);
			
			playerObject.GetComponent<Renderer>().enabled = true;
			rocketObject.GetComponent<Renderer>().enabled = true;
			
			//Move player back to spawn point
			playerObject.transform.position = new Vector3(0, .55f, -.5f);
			
			//Allow player to shoot and allow to begin blinking routine
			LaserControl.cantShoot = false;
			respawnTimer = true;
			
			SendMessage("ContinueControl");
		}
	 
		//Blink routine
		IEnumerator Blink(int nTimes, float timeOn, float timeOff)
		{
		    while (nTimes > 0)
		    {
		        playerObject.GetComponent<Renderer>().enabled = true;
				rocketObject.GetComponent<Renderer>().enabled = true;
				
		        yield return new WaitForSeconds(timeOn);
				
		        playerObject.GetComponent<Renderer>().enabled = false;
				rocketObject.GetComponent<Renderer>().enabled = false;
				
		        yield return new WaitForSeconds(timeOff);
				
		        nTimes--;
		    }
		 
			//Once blinking timer has ended, reactivate player collision
		   	playerObject.GetComponent<Renderer>().enabled = true;
			rocketObject.GetComponent<Renderer>().enabled = true;
			
			Physics.IgnoreLayerCollision(8, 9, false);
			Physics.IgnoreLayerCollision(8, 11, false);
			
			//Allow player to be killed
			lifeTaken = false;
		}
	}
}