using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class ArcturusShip : EnemyControl 
	{
		public GameObject				playerObject; //Player ship
		public GameObject				nextPhase; //Each phase of the boss
		public GameObject				enemyBullet; //Enemy bullet object
		public GameObject[]				bossExplosions; //Boss death explosion array

		private float					nextFire = .5f; //Timer between fire rate
		private float 					fireRange = 8f; //Range between player and Arcturus
		private float					fireRate = 3f; //Rate of time between each bullet
		private float					bulletSpeed = .8f; //Bullet speed
	
		public int						arcturusHealth = 0; //Arcturus ship health
		private int 					explosionSpawnPoint; //Explosion particle spawn point
		private int						explodeLoopCount; //Count for array of explosions
		private static int				lastShipCount = 0; //Count before final phase
		
		private bool					startExplosion = true; //Explosion sequence check
		private bool					explosionProcess = false; //Explosion sequenec has begun check
		private bool					shipDead = false; //Ship is dead check
		private bool					blinkControl = false; //Blink sequence check
		public bool						roundOne = true; //Phase check
		public bool						isMainShip = true; //Is boss check
		public static bool				arcturusDead = false; //Final death check
		public bool						isLastShip = false; //is Final ship check
		private bool					lastPhaseDead = true; //Check for death
		
		public NavigationAgentComponent			bossMovementControl; //Arcturus naviation 
		private Interaction_Wander				bossMovementSet; //Arcturus wander 
		
		//Assign components and objects
		void Start () 
		{
			bossMovementControl = this.GetComponent<NavigationAgentComponent>();
			bossMovementSet = this.gameObject.GetComponent<Interaction_Wander>();
			
			playerObject = GameObject.FindGameObjectWithTag("Player");
		}
	
		void Update () 
		{
			//If ship is alive, face and attack player
			if(!shipDead)
			{
				if(playerObject != null)
				{
					if(Vector3.Distance(this.transform.position, playerObject.transform.position) <= fireRange)
					{
						transform.LookAt(playerObject.transform);
						AttackPlayer();
					}
				}
			}
			
			//If ship dies, play explosion sequence
			if(explosionProcess)
			{
				if(startExplosion)
				{
					StartCoroutine(Explode(.2f));
					
					startExplosion = false;
				}
				
				//Begin sequence
				if(explosionSpawnPoint >= 4)
				{
					startExplosion = true;
					explodeLoopCount++;
					explosionSpawnPoint = 0;
				}
				
				//Phase 1: If first round of ship has died, move ship up out of view and stop explosion
				if(explodeLoopCount >= 4)
				{
					if(roundOne)
					{
						GetComponent<Rigidbody>().velocity = new Vector3(0, 0, .5f);
						
						startExplosion = false;
						
						//Phase 2: Instantiate 3 smaller ships from one main ship
						for(int i = 1; i <= 3; i++)
						{
							Instantiate(nextPhase, this.transform.position, this.transform.rotation);
						}
						
						roundOne = false;
						explosionProcess = false;
					}
				}
			}
			
			//Phase 3: Check for second phase, 27 ships dead, final stage has begun
			if(lastShipCount >= 27)
			{
				//Reset health, fire rate and begin random movement
				if(isMainShip)
				{
					Physics.IgnoreLayerCollision(8, 11, false);
					Physics.IgnoreLayerCollision(10, 11, false);
					
					bossMovementSet.m_bNavRequestCompleted = true;
					shipDead = false;
					
					arcturusHealth = 25;
					lastShipCount = 0;
					fireRate = 5;
				}
			}
			
			//If final ship has been destroyed, begin blink sequence
			if(blinkControl)
			{
				shipDead = true;
					
				bossMovementControl.CancelActiveRequest();
				
				StartCoroutine(Blink(7, 0.1f, 0.2f));
				
				blinkControl = false;
			}
		}
		
		//If hit by player bullet, decrease health
		void OnTriggerEnter(Collider other)
		{
			if(other.gameObject.tag == "PlayerBullet")		
			{
				arcturusHealth--;
				
				AudioControl.PlayAudio("Shield");
				
				//If health has reached 0 
				if(arcturusHealth <= 0)
				{
					//Phase 1 ship collision
					if(isMainShip)
					{
						//Ignore collision while blink and explosion happen
						if(roundOne)
						{
							ScoreControl.RaiseScore(1000);
							
							Physics.IgnoreLayerCollision(8, 11);
							Physics.IgnoreLayerCollision(10, 11);
						}
						
						//If final phase and dead, begin blink and explode sequence
						else
						{
							ScoreControl.RaiseScore(5000);
							
							blinkControl = true;
							startExplosion = true;
						}
						
						Physics.IgnoreLayerCollision(8, 11);
						Physics.IgnoreLayerCollision(10, 11);
						
						explosionProcess = true;
						shipDead = true;	
						
						//Stop random movement
						bossMovementControl.CancelActiveRequest();
					}
					
					//Phase 2 ships collision
					else if(!isMainShip)
					{
						ScoreControl.RaiseScore(450);
						
						AudioControl.PlayAudio("Explosion");
						
						Instantiate(enemyExplosion, this.transform.position, this.transform.rotation);
	
						//Create 3 new ships upon each larger ship destroyed
						if(!isLastShip)
						{
							for(int i = 1; i <= 3; i++)
							{
								Instantiate(nextPhase, this.transform.position, this.transform.rotation);
							}
						}
						
						//Count down of smaller ships to reach before starting final phase
						if(isLastShip)
						{
							ScoreControl.RaiseScore(650);
							
							//Only increase by 1 then cut off
							if(lastPhaseDead)
							{
								lastShipCount++;
								
								lastPhaseDead = false;
							}
						}
						
						//Destroy each smaller ship
						Destroy(this.gameObject);
					}
				}
			}
			
			//When ship goes around of view and hits colliders, stop ship in its place for the time
			if(other.gameObject.tag == "BossStopper")
			{
				GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
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
		
		//Death blinking sequence
		IEnumerator Blink(int nTimes, float timeOn, float timeOff)
		{
			Physics.IgnoreLayerCollision(10, 11);
			
		    while (nTimes > 0)
		    {
		        this.GetComponent<Renderer>().enabled = true;
				
		        yield return new WaitForSeconds(timeOn);
				
		        this.GetComponent<Renderer>().enabled = false;
				
		        yield return new WaitForSeconds(timeOff);
				
		        nTimes--;
		    }
		 
		   	this.GetComponent<Renderer>().enabled = true;
			arcturusDead = true;
			
			Destroy(this.gameObject);
		}
		
		//Multiple explode sequences
		IEnumerator Explode(float TimeBetween)
		{
			while(explosionSpawnPoint < 4)
			{
				AudioControl.PlayAudio("Explosion");
				
				Instantiate(enemyExplosion, bossExplosions[explosionSpawnPoint].transform.position, 
					bossExplosions[explosionSpawnPoint].transform.rotation);
				
				yield return new WaitForSeconds(TimeBetween);
				
			    explosionSpawnPoint++; 			
			}
		}
	}
}