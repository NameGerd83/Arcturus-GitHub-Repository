//using UnityEngine;
//using System.Collections;
//
//public class BossShip : GamePlayWaves 
//{
//	public GameObject			leftPowerupParticle;
//	public GameObject			rightPowerupParticle;
//	public GameObject			centerPowerupParticle;
//	public GameObject			leftLaserParticle;
//	public GameObject			rightLaserParticle;
//	public GameObject			centerLaserParticle;
//	public GameObject			leftLaserCollider;
//	public GameObject			rightLaserCollider;
//	public GameObject			centerLaserCollider;
//	public GameObject			leftArmCollider;
//	public GameObject			rightArmCollider;
//	public GameObject			centerCollider;
//	public GameObject			bossBoxCollider;
//	
//	float 						randomTime;
//	float						zVect = -.1f;
//	
//	bool						timerCountDown = true;
//	bool						beginMovement = false;
//	public bool					leftArmAlive = true;
//	public bool					rightArmAlive = true;
//	
//	private NavigationAgentComponent			bossMovementEnable;
//	private Interaction_Wander					bossMovementSet;
//	
//	void Start()
//	{	
//		bossMovementEnable = this.gameObject.GetComponent<NavigationAgentComponent>();
//		bossMovementSet = this.gameObject.GetComponent<Interaction_Wander>();
//		
//		boundaryObject = GameObject.Find("BossBounds");
//		rigidbody.velocity = new Vector3(0, 0, zVect);
//	}
//	
//	void Update()
//	{
//		if(beginMovement)
//		{
//			if(timerCountDown)
//				randomTime -= Time.deltaTime;
//			
//			if(rightArmAlive || leftArmAlive)
//			{
//				if(randomTime <= 0)
//					StartCoroutine("ActivateParticles");
//			}
//			
//			if(!rightArmAlive && !leftArmAlive)
//			{
//				if(randomTime <= 0)
//					StartCoroutine("MainLaser");
//			}
//		}
//		
//		if(!timerCountDown)
//		{
//			randomTime = Random.Range(7, 11);
//			
//			timerCountDown = true;	
//		}
//	}
//	
//	IEnumerator ActivateParticles()
//	{
//		timerCountDown = false;
//		
//		StartCoroutine("PowerUpParticles");
//		
//		yield return new WaitForSeconds(2f);
//		
//		StartCoroutine("LaserParticles");
//		
//		yield return new WaitForSeconds(2f);
//		
//		AllParticlesOff();
//	}
//	
//	void OnTriggerEnter(Collider other)
//	{
//		if(other.gameObject.tag == "Boss Enabler")
//		{
//			bossMovementSet.enabled = true;
//			
//			randomTime = Random.Range(3, 7);
//			
//			boundaryObject.transform.position = new Vector3(0, 0, 0);
//			
//			beginMovement = true;
//			
//			zVect = 0;
//			
//			Destroy(GameObject.FindWithTag("Boss Enabler"));
//		}
//	}
//	
//	IEnumerator PowerUpParticles()
//	{
//		bossMovementEnable.CancelActiveRequest();
//		
//		yield return new WaitForSeconds(.25f);
//		
//		if(leftArmAlive)
//		{
//			leftPowerupParticle.renderer.enabled = true;
//			leftPowerupParticle.particleEmitter.emit = true;
//			leftArmCollider.collider.enabled = true;
//		}	
//		
//		if(rightArmAlive)
//		{
//			rightPowerupParticle.renderer.enabled = true;
//			rightPowerupParticle.particleEmitter.emit = true;
//			rightArmCollider.collider.enabled = true;
//		}	
//	}
//	
//	IEnumerator LaserParticles()
//	{
//		if(leftArmAlive)
//		{
//			leftArmCollider.collider.enabled = false;
//			leftLaserParticle.renderer.enabled = true;
//			leftLaserCollider.collider.enabled = true;
//		}
//				
//		if(rightArmAlive)
//		{
//			rightArmCollider.collider.enabled = false;
//			rightLaserParticle.renderer.enabled = true;
//			rightLaserCollider.collider.enabled = true;
//		}
//		
//		yield return new WaitForSeconds(2.5f);
//		
//		bossMovementSet.m_bNavRequestCompleted = true;
//		
//		yield return new WaitForSeconds(.5f);
//	}
//	
//	IEnumerator MainLaser()
//	{
//		timerCountDown = false;
//		
//		bossMovementEnable.CancelActiveRequest();
//		
//		centerPowerupParticle.renderer.enabled = true;
//		centerPowerupParticle.particleEmitter.emit = true;
//		centerCollider.collider.enabled = true;
//			
//		yield return new WaitForSeconds(1.25f);
//			
//		centerCollider.collider.enabled = false;
//		centerLaserParticle.renderer.enabled = true;
//		centerLaserCollider.collider.enabled = true;
//		
//		yield return new WaitForSeconds(2.5f);
//		
//		bossMovementSet.m_bNavRequestCompleted = true;
//		
//		yield return new WaitForSeconds(.5f);
//		
//		AllParticlesOff();
//	}
//	
//	public void AllParticlesOff()
//	{
//		if(leftArmAlive)
//		{
//			leftPowerupParticle.renderer.enabled = false;
//			leftPowerupParticle.particleEmitter.emit = false;
//			leftLaserParticle.renderer.enabled = false;
//			leftLaserCollider.collider.enabled = false;
//		}
//		
//		if(rightArmAlive)
//		{
//			rightPowerupParticle.renderer.enabled = false;
//			rightPowerupParticle.particleEmitter.emit = false;
//			rightLaserParticle.renderer.enabled = false;
//			rightLaserCollider.collider.enabled = false;
//		}
//		
//		centerPowerupParticle.renderer.enabled = false;
//		centerPowerupParticle.particleEmitter.emit = false;
//		centerLaserParticle.renderer.enabled = false;
//		centerLaserCollider.collider.enabled = false;
//		
//		StopCoroutine("ActivateParticles");
//		StopCoroutine("MainLaser");
//	}
//}
