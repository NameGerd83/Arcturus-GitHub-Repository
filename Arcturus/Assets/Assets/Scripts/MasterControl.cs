using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class MasterControl : MonoBehaviour 
	{
		//All game related objects
		public GameObject[]			shipObjects; //All enemy ships
		private GameObject[]		destroyShips = null; //All enemy ships to destroy
		private GameObject[]		destroyAsteroids = null; //All asteroids to destroy
		public GameObject[]			shipSpawnPoints; //Enemy ship spawnpoints
		public static GameObject[]	staticShipSpawnPoints;
		public GameObject			playerObject; //Player ship
		public GameObject			rocketObject; //Player rocket
		public GameObject			playerSpawn; //Player spawn point
		public GameObject			boundaryObject; //Game boundary
		
		//GUI
		public GameObject[]			gameGUI; //All game play GUI
		public static GameObject[]	staticGameGUI;
		public GameObject[]			mainMenuGUI; //All main menu GUI
		public GameObject[]			mainMenuParticles; //Main menu particles
		public GameObject[]			gameOverGUI; //Game Over/Won GUI
		public GameObject[]			infoGUIText; //Creation controls/credits GUI
		
		public GUITexture			playerAvatar; //Player ship symbol next to lives
		
		private float 				countDownFrom = 0; //Game waves timer
		private float				asteroidInterval = .5f; //Time between each asteroid spawn
		
		public static int			upgradeItem = 0;
		public static int			shieldUpgrade = 1; //Checks for wave 7 upgrade
		private int					menuChoice = 1; //Menu selection count
	 	public static int			gameWaves = 1; //Game play wave count
		public static int			miniBossCount = 3; //Count of mini boss ships 
		
		private bool 				isCounting = false; //count down check
		private bool				gameOver = false; //Game over check
		public static bool			canSpawn = true; //can spawn check
		private bool				menuControlOn = true; //Menu control check
	
		private PlayerControl		playerController; //Connect to player controller
		
		//Gamescreen enumerations
		public enum GameModes
		{
			StartScreen,
			GameScreen,
			EndScreen,
		}
		
		//Default screen
		public static GameModes			gameModes = GameModes.StartScreen;
		
		void Awake()
		{
			//Assign static objects
			staticShipSpawnPoints = shipSpawnPoints;
			staticGameGUI = gameGUI;
			
			//Unable to shoot at game menu
			LaserControl.cantShoot = true;
		}
		
		//Main menu start
		IEnumerator Start()
		{
			WaveShip.positionCheck = 1;
			
			yield return new WaitForSeconds(.1f);
			
			AudioControl.PlayAudio("MainMenu");
			
			playerController = GameObject.Find("PlayerShip").GetComponent<PlayerControl>();			
		}
	
		void Update() 
		{	
			//Menu controls
			StartCoroutine("GameModeSwitch");
		}
		
		IEnumerator GameModeSwitch()
		{
			switch(gameModes)
			{
				case GameModes.StartScreen:
				
				//If true, keypresses are active
				if(menuControlOn)
				{
					//Move particles up
					if(Input.GetKeyDown(KeyCode.W))
					{
						AudioControl.PlayAudio("MenuBeep");
						
						menuChoice--;
							
						if(menuChoice < 1)
						{
							menuChoice = 3;
						}
						
						//Menu particles control
						MenuParticles();
					}
					
					//Move particles down
					else if(Input.GetKeyDown(KeyCode.S))
					{
						AudioControl.PlayAudio("MenuBeep");
						
						menuChoice++;
						
						if(menuChoice > 3)
						{
							menuChoice = 1;
						}
						
						//Menu particles control
						MenuParticles();
					}
				}
				
				//Menu choice 1 = Start Game, choice 2 = Controls screen, choice 3 = Credits screen
				if(Input.GetKeyDown(KeyCode.Return))
				{
					if(menuControlOn)
					{
						if(menuChoice == 1)
						{
							//Place player in start position
							playerObject.transform.position = new Vector3(0, .55f, -.5f);
							
							//Play sound before starting game
							AudioControl.PlayAudio("PowerPickUp");
							
							//Enter can only be pressed once
							menuControlOn = false;
	
							yield return new WaitForSeconds(1f);
							
							//Start game
							gameModes = GameModes.GameScreen;
							
							//Begin spawn
							canSpawn = true;
							
							AudioControl.PlayAudio("GamePlay");
							
							//Turn off any menu GUI and turn on game play GUI
							ScreenControlOff();
							GameGUIOn();
							playerController.ContinueControl();
							
							//Spawn player ship
							playerObject.GetComponent<Renderer>().enabled = true;
							rocketObject.GetComponent<Renderer>().enabled = true;
							
							//Activate shooting
							LaserControl.cantShoot = false;
						}
					
						//Turn on Controls screen
						else if(menuChoice == 2)
						{
							ScreenControlOff();
							ControlsScreenOn();
							
							//Key presses disabled
							menuControlOn = false;
						}
						
						//Turn on Credits screen
						else if(menuChoice == 3)
						{
							ScreenControlOff();
							CreditsScreenOn();	
							
							//Key presses disabled
							menuControlOn = false;
						}
					}
					//If Enter is pressed, return to main menu
					//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
					else if(menuChoice == 4)
					{
						ScreenControlOn();
						CreditsScreenOff();
						
						//Enable menu particles and key presses
						MenuParticles();
						menuControlOn = true;
					}	
					
					else if(menuChoice == 5)
					{
						ScreenControlOn();
						ControlsScreenOff();
						
						MenuParticles();
						menuControlOn = true;
					}
					//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				}
				break;
			
			//Game play enum
			case GameModes.GameScreen:
				
				//Pause game
				if(Input.GetKeyDown(KeyCode.P))
					PauseGame();
	
				//Display player lives
				gameGUI[1].GetComponent<GUIText>().text = "X " + PlayerHealth.playerLives;
				
				//Begin spawning enemies
				if(!gameOver)
					StartCoroutine("GameWaveSwitch");	
				
				//Asteroid spawn
				if(gameWaves == 6 || gameWaves == 7 || gameWaves == 8)
				{
					asteroidInterval += Time.deltaTime;
					
					//Wait .5 seconds between each spawn and make them random across the screen
					if(asteroidInterval >= .5f)
					{
						asteroidInterval = 0;
		
						Instantiate(shipObjects[4], shipSpawnPoints[Random.Range(0, 11)].transform.position, 
							transform.rotation);
					}
				}
		
				//When wave 6 started, wait a moment then spawn mini boss
				if(gameWaves == 6 && miniBossCount <= 0)
				{	
					gameWaves = 7;
					shieldUpgrade = 1;
					
					yield return new WaitForSeconds(1f);
					
					canSpawn = true;
				}
				
				//If power up ship dead, move to next wave
				if(gameWaves == 7)
				{
					//Reset mini boss count to 6 for wave 8
					miniBossCount = 6;
					
					if(shieldUpgrade <= 0)
					{	
						gameWaves = 8;

						yield return new WaitForSeconds(1f);
					
						canSpawn = true;
					}
				}
				
				//If all 6 mini bosses dead, move to next wave
				if(gameWaves == 8 && miniBossCount <= 0)
				{	
					gameWaves = 9;
				
					yield return new WaitForSeconds(2f);
				}
				
				//If Player lives down to 0, Game Over
				if(PlayerHealth.playerLives <= 0)
					GameOver();
				
				//If Arcturus health down to 0, Game Won
				if(ArcturusShip.arcturusDead)
					GameWon();
					
				break;
				
			case GameModes.EndScreen:
				
				//If game is over, press Enter to go to Main Menu
				if(Input.GetKeyDown(KeyCode.Return))
				{
					ResetVars();
				}
				
				//Destroy everything if Game Over
				DestroyAllEnemies();
				
				break;
			}
		}
		
		//Game play spawn enum
		IEnumerator GameWaveSwitch()
		{
			yield return new WaitForSeconds(1f);
			
			switch(gameWaves)
			{
			case 1:
				
				//Wait a moment before each wave
				countDownFrom = 9;
				CountDown();
					
				if(canSpawn)
				{
					//Spawn 7 Wave ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[0], 7, shipSpawnPoints[5], null, 0, null, .75f));
				}
				break;
			
			case 2:
				
				//Wait 11 seconds before wave 3
				countDownFrom = 11;
				CountDown();
				
				if(canSpawn)
				{
					//Selective spawnpoints for wave ships
					WaveShip.positionCheck = 3;
					
					//Spawn 13 Wave ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[0], 13, shipSpawnPoints[5], null, 0, null, .75f));
					
					//Powerup ship
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[2], 1, shipSpawnPoints[5], 
						null, 0, null, .75f));
				}
				break;
			
			case 3:
				
				countDownFrom = 11;
				CountDown();
	
				if(canSpawn)
				{
					WaveShip.positionCheck = 7;
					
					//Spawn 6 Wave ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[0], 6, shipSpawnPoints[5], null, 0, null, .75f));
					
					yield return new WaitForSeconds(3);
					
					//Spawn 5 Dart ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[1], 5, shipSpawnPoints[5], null, 0, null, .75f));
					
					yield return new WaitForSeconds(3);
					
					WaveShip.positionCheck = 6;
					
					//Spawn 6 Wave ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[0], 6, shipSpawnPoints[5], null, 0, null, .75f));
					
				}
				break;
		
			case 4:
			
				countDownFrom = 13;
				CountDown();
				
				if(canSpawn)
				{	
					WaveShip.positionCheck = 1;
					
					//Spawn 10 Wave ships and an Upgrade ship
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[0], 10, shipSpawnPoints[5], 
						shipObjects[2], 1, shipSpawnPoints[5], .75f)); //Powerup
					
					yield return new WaitForSeconds(3);
					
					//Spawn two sets of 5 Blade ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[3], 5, shipSpawnPoints[2], 
						shipObjects[3], 5, shipSpawnPoints[9], .75f));
				}	
				break;
			
			case 5:
			
				countDownFrom = 16;
				CountDown();
			
				if(canSpawn)
				{
					WaveShip.positionCheck = 2;
					
					//Spawn 5 Dart ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[1], 5, shipSpawnPoints[2], null, 0, null, Random.Range(1, 2)));
				
					//Spawn 10 Wave ships and 5 Dart ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[0], 10, shipSpawnPoints[5], 
						shipObjects[1], 5, shipSpawnPoints[8], .75f));
				
					yield return new WaitForSeconds(2);
					
					//Powerup
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[2], 1, shipSpawnPoints[5], null, 0, null, 1f));		
				}
				break;
				
			case 6:
				
				yield return new WaitForSeconds(1f);
				
				if(canSpawn)
				{	
					//Spawn 3 Mini boss ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[5], 3, shipSpawnPoints[5], null, 0, null, .5f));
				}
				break;
			
			case 7:
		
				yield return new WaitForSeconds(1f);
				
				if(canSpawn)
				{
					//Powerup ship
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[2], 1, shipSpawnPoints[5], null, 0, null, 1f));
				}
				break;
			
			case 8:
				
				yield return new WaitForSeconds(2f);
				
				if(canSpawn)
				{	
					//Spawn 6 Mini boss ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[5], 6, shipSpawnPoints[5], null, 0, null, .5f));
				}
				break;
				
			case 9:
				
				//Wait a moment before the next phase of the game
				yield return new WaitForSeconds(2f);
				
				countDownFrom = 2f;
				CountDown();
				
				break;
				
			case 10:
				
				countDownFrom = 4;
				CountDown();
				
				if(canSpawn)
				{	
					WaveShip.positionCheck = 2;
					
					//Spawn Asteroid wall
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[6], 1, shipSpawnPoints[11], null, 0, null, .75f));
					
					//Spawn 10 Wave ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[0], 10, shipSpawnPoints[0], null, 0, null, .75f));
				}
				break;	
				
			case 11:
			
				countDownFrom = 9;
				CountDown();
				
				if(canSpawn)
				{	
					//Powerup ship
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[2], 1, shipSpawnPoints[5], null, 0, null, 1f));
					
					//Spawn 7 Dart ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[1], 7, shipSpawnPoints[3], null, 0, null, .75f));
					
					//Spawn 7 Dart ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[1], 7, shipSpawnPoints[7], null, 0, null, .75f));
				}
				break;
			
			case 12:
			
				countDownFrom = 8;
				CountDown();
				
				if(canSpawn)
				{	
					WaveShip.positionCheck = 4;
					
					//Spawn 5 Dart ships in 4 rows
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[1], 5, shipSpawnPoints[3], null, 0, null, .75f));
					
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[1], 5, shipSpawnPoints[4], null, 0, null, .75f));
					
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[1], 5, shipSpawnPoints[5], null, 0, null, .75f));
					
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[1], 5, shipSpawnPoints[6], null, 0, null, .75f));
					
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[1], 5, shipSpawnPoints[7], null, 0, null, .75f));
					
					yield return new WaitForSeconds(2);
					
					//Powerup ship
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[2], 1, shipSpawnPoints[5], null, 0, null, 1f));
					
					//Spawn 7 Wave ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[0], 7, shipSpawnPoints[5], null, 0, null, .75f));
				}
				break;
				
			case 13:
			
				countDownFrom = 26;
				CountDown();
				
				if(canSpawn)
				{	
					WaveShip.positionCheck = 2;
					
					//Spawn 15 Wave ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[0], 15, shipSpawnPoints[2], null, 0, null, .75f));
					
					//Spawn 5 Dart ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[1], 5, shipSpawnPoints[2], null, 0, null, .75f));
					
					yield return new WaitForSeconds(2);
					
					//Spawn 5 Dart ships
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[1], 5, shipSpawnPoints[8], null, 0, null, .75f));
				}
				break;
				
			case 14:
				
				if(canSpawn)
				{		
					//Spawn Arcturus ship
					StartCoroutine(SpawnEnemyObjects
						(shipObjects[7], 1, shipSpawnPoints[5], null, 0, null, .75f));
				}
				break;
			}
		}
		
		//Enemy spawn system
		public IEnumerator SpawnEnemyObjects(GameObject enemyShip1, int shipCount1, GameObject spawnPoints1, 
			GameObject enemyShip2, int shipCount2, GameObject spawnPoints2, float spawnTime)
		{
			canSpawn = false;
	
			for (var i = 0; i < shipCount1; i++)
			{
				yield return new WaitForSeconds(spawnTime);
				
				Instantiate(enemyShip1, spawnPoints1.transform.position, spawnPoints1.transform.rotation);
			}
			
			if(enemyShip2 != null)
			{
				for (var i = 0; i < shipCount2; i++)
				{
					yield return new WaitForSeconds(spawnTime);
					
					Instantiate(enemyShip2, spawnPoints2.transform.position, spawnPoints2.transform.rotation);
				}
			}
		}
		
		//Wave count down timer
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		void CountDown()
	    {
	        if (!isCounting)
	        {
	            StartCoroutine("Wait");
	        }
	    }
		
		IEnumerator Wait()
	    {
	        isCounting = true;
	
	        for(float i = countDownFrom; i >= 0; i--)
	        {
	            if (i <= 0)
	            {
					gameWaves++;
					canSpawn = true;
	            }
				
	            yield return new WaitForSeconds(1);
	        }
			
	        isCounting = false;
	    }
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		//Menu particle control
		void MenuParticles()
		{
			switch(menuChoice)
			{
				case 1:
					//Start Game button particles 
					mainMenuParticles[0].GetComponent<Renderer>().enabled = true;
					mainMenuParticles[1].GetComponent<Renderer>().enabled = false;
					mainMenuParticles[2].GetComponent<Renderer>().enabled = false;
					
					break;
					
				case 2:
					//Controls button particles
					mainMenuParticles[0].GetComponent<Renderer>().enabled = false;
					mainMenuParticles[1].GetComponent<Renderer>().enabled = true;
					mainMenuParticles[2].GetComponent<Renderer>().enabled = false;
					
					break;
					
				case 3:
					//Credits button particles
					mainMenuParticles[0].GetComponent<Renderer>().enabled = false;
					mainMenuParticles[1].GetComponent<Renderer>().enabled = false;
					mainMenuParticles[2].GetComponent<Renderer>().enabled = true;
					
					break;
			}
		}
		
		//Credits scene GUI on
		void CreditsScreenOn()
		{
			menuChoice = 4;
				
			infoGUIText[1].SetActive(true);
			mainMenuGUI[3].GetComponent<Renderer>().enabled = true;
			mainMenuGUI[3].transform.position = new Vector3(0, .55f, .4f);
		}
		
		//Credits scene GUI off
		void CreditsScreenOff()
		{
			menuChoice = 1;
				
			infoGUIText[1].SetActive(false);
			mainMenuGUI[3].transform.position = new Vector3(0, .55f, -.3f);
		}
		
		//Controls scene GUI on
		void ControlsScreenOn()
		{
			menuChoice = 5;
			
			infoGUIText[0].SetActive(true);
			mainMenuGUI[2].GetComponent<Renderer>().enabled = true;
			mainMenuGUI[2].transform.position = new Vector3(0, .55f, .4f);
		}
		
		//Controls scene GUI off
		void ControlsScreenOff()
		{
			menuChoice = 1;
			
			infoGUIText[0].SetActive(false);
			mainMenuGUI[2].transform.position = new Vector3(0, .55f, -.15f);
		}
		
		//Main Menu GUI on
		void ScreenControlOn()
		{
			mainMenuParticles[2].GetComponent<Renderer>().enabled = true;
	
			for(int i = 0; i < mainMenuGUI.Length; i++)
				mainMenuGUI[i].GetComponent<Renderer>().enabled = true;
		}
		
		//Main Menu GUI off
		void ScreenControlOff()
		{
			for(int i = 0; i < mainMenuParticles.Length; i++)
				mainMenuParticles[i].GetComponent<Renderer>().enabled = false;
	
			for(int i = 0; i < mainMenuGUI.Length; i++)
				mainMenuGUI[i].GetComponent<Renderer>().enabled = false;
		}
		
		//Game play GUI on
		void GameGUIOn()
		{
			for(int i = 0; i < gameGUI.Length; i++)
				gameGUI[i].GetComponent<GUIText>().enabled = true;
			
			//Activate player avatar texture
			playerAvatar.GetComponent<GUITexture>().enabled = true;
		}
		
		//Game play GUI off
		void GameGUIOff()
		{
			//Game GUI
			for(int i = 0; i < gameGUI.Length; i++)
				gameGUI[i].GetComponent<GUIText>().enabled = false;
			
			//End game GUI off
			for(int i = 0; i < gameOverGUI.Length; i++)
				gameOverGUI[i].SetActive(false);
			
			//Deactivate player avatar texture
			playerAvatar.GetComponent<GUITexture>().enabled = false;
		}
			
		//You win!
		public void GameWon()
		{
			gameModes = GameModes.EndScreen;
			
			//Stop all spawns
			StopAllCoroutines();
				
			//Display Game Won GUI
			gameOverGUI[0].SetActive(true);
				
			gameGUI[1].GetComponent<GUIText>().enabled = false;
			playerAvatar.GetComponent<GUITexture>().enabled = false;
			
			//Hide player ship
			playerObject.GetComponent<Renderer>().enabled = false;
			rocketObject.GetComponent<Renderer>().enabled = false;
		}
		
		//Game Over!
		public void GameOver()
		{
			gameModes = GameModes.EndScreen;
	
			//Stop all spawns
			StopAllCoroutines();
				
			//Display Game Over GUI
			gameOverGUI[1].SetActive(true);
			gameGUI[1].GetComponent<GUIText>().enabled = false;
			playerAvatar.GetComponent<GUITexture>().enabled = false;
			
			//Hide player ship
			playerObject.GetComponent<Renderer>().enabled = false;
			rocketObject.GetComponent<Renderer>().enabled = false;
		}
		
		//Reset all variables
		void ResetVars()
		{
			gameModes = GameModes.StartScreen;
			
			AudioControl.PlayAudio("MainMenu");
			
			StopCoroutine("Wait");
			StopCoroutine("GameWaveSwitch");
			
			countDownFrom = 0f;
			asteroidInterval = .5f;
		
			miniBossCount = 3;
			upgradeItem = 0;
			shieldUpgrade = 1;
			menuChoice = 1;
			gameWaves = 1;
			ScoreControl.totalScore = 0;
			WaveShip.positionCheck = 1;
	 		PlayerHealth.playerLives = 3;

			ScreenControlOn();
			GameGUIOff();
			MenuParticles();
			
			isCounting = false;
			gameOver = false;
			canSpawn = true;
			menuControlOn = true;
			AudioControl.bossMusic = false;
			ArcturusShip.arcturusDead = false;
			PlayerHealth.lifeTaken = false;
			
			PlayerControl.shieldsActive = false;
			PlayerControl.staticShield.emit = false;
			
			playerController.StopControl();
			
			LaserControl.cantShoot = true;
			LaserControl.multiShotSwitch = 1;
			
			//Reconnect collision between player and enemies
			Physics.IgnoreLayerCollision(8, 9, false);
			Physics.IgnoreLayerCollision(8, 11, false);
			Physics.IgnoreLayerCollision(10, 11, false);
			
			playerObject.transform.position = new Vector3(0, .55f, -2f);
			//boundaryObject.transform.position = new Vector3(5, 0, 0);
		}
		
		//If game is over, destroy everything immediately
		public void DestroyAllEnemies()
		{
			destroyShips = GameObject.FindGameObjectsWithTag("Enemy");
			destroyAsteroids = GameObject.FindGameObjectsWithTag("Asteroid");
			
			for(int i = 0; i < destroyShips.Length; i++)
			{
				if(destroyShips != null)
					Destroy(destroyShips[i]);
			}
		
			for(int i = 0; i < destroyAsteroids.Length; i++)
			{
				if(destroyAsteroids != null)
					Destroy(destroyAsteroids[i]);
			}
		}
		
		//Stop time and controls
		void PauseGame()
		{
			if(Time.timeScale == 1)
			{
				Time.timeScale = 0;
				
				//Player cant move or shoot
				PlayerControl.stopControls = true;
				
				if(!LaserControl.cantShoot)
					LaserControl.cantShoot = true;
				
				//Pause music
				AudioControl.PauseAudio();
			}	
			//Reset
			else
			{
				Time.timeScale = 1;
				
				//Allow player to move again
				playerObject.SendMessage("ContinueControl");
				
				//Player cant shoot
				if(LaserControl.cantShoot)
					LaserControl.cantShoot = false;
				
				//Resume music
				AudioControl.ResumeAudio();
			}
		}
	}
}
