using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class PlayerControl : MonoBehaviour 
	{
		private GameObject				shieldObject; //Shield upgrade
		private GameObject				speedBoostObject; //Speed upgrade
		private GameObject				multiShotObject; //Multishot upgrade
		private GameObject[]			enemyObject; //Enemies
		
		public ParticleEmitter			rocketObject; //Player ship rocket
		public static ParticleEmitter	staticShield; //Player ship shield
		public ParticleEmitter			shield; //Player ship shield 
		
		private float 					speed = .4f; //Player ship speed
		private float					speedReset = .4f; //Player ship speed reset
		private float 					rotateSpeed = 5f; //Player ship turn rotate speed
	
		public static int				shieldHealth = 3; //Player ship's shield health
		
		private bool					rotateLeft = true; //Rotate left check
		private bool					rotateRight = true; //Rotate right check
		private bool					rocketForward = true; //Rocket forward check
		private bool					rocketBackward = true; //Rocket back check
		
		public static bool				stopControls = true; //Player control switch
		public static bool				shieldsActive = false; //Shield active check
		
		public Quaternion 				minAngle; //Rotation left
		public Quaternion 				maxAngle; //Rotation right
		
	    public CharacterController 		charController; //Player char control
		
	//	void OnGUI() 
	//	{ 
	//		hSliderValue = GUI.HorizontalSlider(new Rect (50, 25, 100, 30), hSliderValue, 0.0f, 10.0f); 
	//		GUI.Box(new Rect(0,0,200,50),"Volume"); 
	//		AudioListener.volume = 5 * hSliderValue; 
	//	}
		
		//Assign shield for namespace
		void Start()
		{	
			staticShield = shield;
		}
		
		//Check for new object upgrades
	    void Update() 
		{
			//Check if shield is available
			if(shieldObject == null)
			{
				shieldObject = GameObject.FindWithTag("Shield");
			}
			
			//Check is speed up is available
			if(speedBoostObject == null)
			{
				speedBoostObject = GameObject.FindWithTag("SpeedBoost");
			}
			
			//Check if multishot is available
			if(multiShotObject == null)
			{
				multiShotObject = GameObject.FindWithTag("MultiShot");
			}
			
			//Check if ship can be controlled
			if(!stopControls)
			{
				//Control player movement
	        	var moveDirection = new Vector3(Input.GetAxis("LeftAndRight"), 0, Input.GetAxis("ForAndBack"));
					charController.Move(moveDirection * Time.deltaTime * speed);   
			
				//Check if left
				if(rotateLeft)
				{
					//If 'A' is pressed, rotate the ship left
					if(Input.GetKey(KeyCode.A))
					{
						//While rotating left, cant rotate right
						rotateRight = false;
						
						//Rotate 40 degrees left
						maxAngle.z = 40;
					}
					
					//If released, reset
					if(Input.GetKeyUp(KeyCode.A))
					{
						rotateRight = true;
						rotateLeft = true;
					
						maxAngle.z = 0;
					}
				}
				
				//Check if right
				if(rotateRight)
				{
					//If 'D' is pressed, rotate the ship left
					if(Input.GetKey(KeyCode.D))
					{
						//While rotating right, cant rotate left
						rotateLeft = false;
						
						//Rotate 40 degrees right
						maxAngle.z = -40;	
					}
					
					//If released, reset
					if(Input.GetKeyUp(KeyCode.D))
					{
						rotateRight = true;
						rotateLeft = true;
					
						maxAngle.z = 0;
					}
				}
				
				if(rocketForward)
				{
					//While pressing forward, enlarge rocket particles
					if(Input.GetKey(KeyCode.W))
					{	
						//Cant shrink rocket particles
						rocketBackward = false;
						
						//Enlarge rocket particles
			    		rocketObject.GetComponent<ParticleEmitter>().minSize = .045f;
						rocketObject.GetComponent<ParticleEmitter>().maxSize = .045f;
						
						rocketObject.GetComponent<ParticleEmitter>().minEnergy = 1f;
						rocketObject.GetComponent<ParticleEmitter>().maxEnergy = 1f;
					}
						
					//If released, reset
					if(Input.GetKeyUp(KeyCode.W))
					{
						rocketForward = true;
						rocketBackward = true;
						
						rocketObject.GetComponent<ParticleEmitter>().minSize = .03f;
						rocketObject.GetComponent<ParticleEmitter>().maxSize = .03f;
						
						rocketObject.GetComponent<ParticleEmitter>().minEnergy = .75f;
						rocketObject.GetComponent<ParticleEmitter>().maxEnergy = .75f;
					}
				}
				
				//While pressing forward, enlarge rocket particles
				if(rocketBackward)
				{
					//While pressing back, shrink rocket particles
					if(Input.GetKey(KeyCode.S))
					{	
						//Cant enlarge rocket particles
						rocketForward = false;
						
						rocketObject.GetComponent<ParticleEmitter>().minSize = .025f;
						rocketObject.GetComponent<ParticleEmitter>().maxSize = .025f;
						
						rocketObject.GetComponent<ParticleEmitter>().minEnergy = .25f;
						rocketObject.GetComponent<ParticleEmitter>().maxEnergy = .25f;
					}
						
					//If released, reset default size
					if(Input.GetKeyUp(KeyCode.S))
					{
						rocketForward = true;
						rocketBackward = true;
						
						rocketObject.GetComponent<ParticleEmitter>().minSize = .03f;
						rocketObject.GetComponent<ParticleEmitter>().maxSize = .03f;
						
						rocketObject.GetComponent<ParticleEmitter>().minEnergy = .75f;
						rocketObject.GetComponent<ParticleEmitter>().maxEnergy = .75f;
					}
				}
				//Rotate gradually
				transform.rotation = Quaternion.Slerp(minAngle, maxAngle, Time.deltaTime * rotateSpeed);
			}
		}
		
		//Upgradable items
		void OnTriggerEnter(Collider otherObject)
		{	
			if(otherObject.gameObject.tag == "Shield")
			{
				//Shield power up collision
				AudioControl.PlayAudio("PowerPickUp");
				
				//Emit shield particles on ship
				staticShield.emit = true;
				shieldsActive = true;
				
				//Destroy shield powerup
				Destroy(shieldObject);
			}
		
			if(otherObject.gameObject.tag == "SpeedBoost")
			{
				AudioControl.PlayAudio("PowerPickUp");
				
				//Increase player speed
				speed += .15f;
				
				//Speed up stars
				SendMessage("SpeedUpStars");
				
				//Destory speedboost powerup
				Destroy(speedBoostObject);
			}
			
			if(otherObject.gameObject.tag == "MultiShot")
			{
				AudioControl.PlayAudio("PowerPickUp");
				
				//Multi bullets
				SendMessage("UpgradeLaser");
				
				//Destroy multishot powerup
				Destroy(multiShotObject);
			}
		}
		
		//Stop player from moving ship
		public void StopControl()
		{
			stopControls = true;
			speed = speedReset;
		}
		
		//Allow player to move again
		public void ContinueControl()
		{
			maxAngle.z = 0;
			
			rotateRight = true;
			rotateLeft = true;
			stopControls = false;	
		}
	}
}