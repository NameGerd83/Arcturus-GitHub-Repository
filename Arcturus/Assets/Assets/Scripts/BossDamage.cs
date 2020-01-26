//using UnityEngine;
//using System.Collections;
//
//public class BossDamage : BossShip 
//{
//	public GameObject[]			bossExplosions;
//	public GameObject			bossObject;
//	public GameObject			bossRocketObject;
//	
//	public GameObject			leftSmokeParticle;
//	public GameObject			leftElectricParticle;
//	
//	public GameObject			rightSmokeParticle;
//	public GameObject			rightElectricParticle;
//	
//	public GameObject			electricParticle;
//	public GameObject			elecParticleCollider;
//	
//	public GameObject			leftExplosion;
//	public GameObject			rightExplosion;
//	public GameObject			endExplosion;
//
//	public AudioClip[]			bossSound;
//	
//	public int					centerHealth;
//	public int					leftHealth;
//	public int					rightHealth;
//	
//	private float				elecEnergyAmount;
//	
//	public bool					isCenterCollider;
//	public bool					leftCollider;
//	public bool					rightCollider;
//	private bool				blinkControl = true;
//	
//	public BossShip							bossShipComponent;
//	public NavigationAgentComponent			bossMovementControl;
//	public GamePlayWaves					gameWaveUpdate;
//	
//	void Start()
//	{
//		if(isCenterCollider)
//		{
//			gameWaveUpdate = GameObject.Find("WaveSpawnPoints").GetComponent<GamePlayWaves>();
//			bossMovementControl = GameObject.Find("BossShip").GetComponent<NavigationAgentComponent>();
//			bossShipComponent = GameObject.Find("BossShip").GetComponent<BossShip>();
//		}
//	}
//	
//	void Awake()
//	{
//		mainCamera = GameObject.Find("Camera");
//	}
//	
//	void Update()
//	{
//		if(leftCollider)
//		{
//			if(leftHealth <= 0)
//			{
//				leftArmCollider.collider.enabled = false;
//				leftPowerupParticle.particleEmitter.emit = false;
//				leftLaserParticle.renderer.enabled = false;
//				leftLaserCollider.collider.enabled = false;
//
//				elecEnergyAmount = electricParticle.particleEmitter.minEnergy - .5f;
//				elecEnergyAmount = electricParticle.particleEmitter.maxEnergy - .5f;
//				
//				electricParticle.particleEmitter.minEnergy = elecEnergyAmount; 
//				electricParticle.particleEmitter.maxEnergy = elecEnergyAmount; 
//				
//				leftSmokeParticle.particleEmitter.emit = true;
//				leftElectricParticle.particleEmitter.emit = true;
//				
//				mainCamera.audio.PlayOneShot(bossSound[0]);
//				Instantiate(leftExplosion, this.transform.position, this.transform.rotation);
//
//				bossShipComponent.leftArmAlive = false;
//				
//				leftCollider = false;
//			}	
//		}
//				
//		if(rightCollider)
//		{
//			if(rightHealth <= 0)
//			{
//				rightArmCollider.collider.enabled = false;
//				rightPowerupParticle.particleEmitter.emit = false;
//				rightLaserParticle.renderer.enabled = false;
//				rightLaserCollider.collider.enabled = false;
//				
//				elecEnergyAmount = electricParticle.particleEmitter.minEnergy - .5f;
//				elecEnergyAmount = electricParticle.particleEmitter.maxEnergy - .5f;
//				
//				electricParticle.particleEmitter.minEnergy = elecEnergyAmount; 
//				electricParticle.particleEmitter.maxEnergy = elecEnergyAmount; 
//				
//				rightSmokeParticle.particleEmitter.emit = true;
//				rightElectricParticle.particleEmitter.emit = true;
//				
//				mainCamera.audio.PlayOneShot(bossSound[0]);
//				Instantiate(rightExplosion, this.transform.position, this.transform.rotation);
//				
//				bossShipComponent.rightArmAlive = false;
//				
//				rightCollider = false;
//			}
//		}
//		
//		if(!rightCollider || !leftCollider)
//		{
//			if(electricParticle.particleEmitter.minEnergy == 0 && 
//				electricParticle.particleEmitter.maxEnergy == 0)
//				elecParticleCollider.SetActive(false);
//		}
//		
//		if(isCenterCollider)
//		{
//			if(centerHealth <= 0)
//			{	
//				leftSmokeParticle.particleEmitter.emit = false;
//				leftElectricParticle.particleEmitter.emit = false;
//				rightSmokeParticle.particleEmitter.emit = false;
//				rightElectricParticle.particleEmitter.emit = false;
//				
//				this.collider.enabled = false;
//				
//				if(blinkControl)
//				{
//					bossMovementControl.CancelActiveRequest();
//					
//					StartCoroutine(Blink(7, 0.1f, 0.2f));
//					
//					bossShipComponent.AllParticlesOff();
//					
//					blinkControl = false;
//				}
//			}
//		}
//	}
//	
//	IEnumerator Blink(int nTimes, float timeOn, float timeOff)
//	{
//		StartCoroutine(Explode(9, .2f));
//		
//	    while (nTimes > 0)
//	    {
//	        bossObject.renderer.enabled = true;
//			bossRocketObject.renderer.enabled = true;
//			
//	        yield return new WaitForSeconds(timeOn);
//			
//	        bossObject.renderer.enabled = false;
//			bossRocketObject.renderer.enabled = false;
//			
//	        yield return new WaitForSeconds(timeOff);
//			
//	        nTimes--;
//	    }
//	 
//	   	bossObject.renderer.enabled = true;
//		bossRocketObject.renderer.enabled = true;
//		
//		Physics.IgnoreLayerCollision(8, 9, false);
//		
//		Destroy(bossObject);
//		
//		gameWaveUpdate.GameWon();
//	}
//	
//	IEnumerator Explode(int spawnPoint, float TimeBetween)
//	{
//		while(spawnPoint > 0)
//		{
//			mainCamera.audio.PlayOneShot(bossSound[0]);
//			
//			Instantiate(endExplosion, bossExplosions[spawnPoint].transform.position, 
//				bossExplosions[spawnPoint].transform.rotation);
//			
//			yield return new WaitForSeconds(TimeBetween);
//	
//		    spawnPoint--; 
//		}
//	}
//	
//	void OnTriggerEnter(Collider other)
//	{
//		if(other.gameObject.tag == "PlayerBullet")		
//		{
//			mainCamera.audio.PlayOneShot(bossSound[1]);
//			
//			if(leftCollider)
//				leftHealth--;
//				
//			else if(rightCollider)
//				rightHealth--;
//			
//			else if(isCenterCollider)
//				centerHealth--;
//		}
//	}
//}
