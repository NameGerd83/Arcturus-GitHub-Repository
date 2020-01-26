using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Arcturus
{
	public class AudioControl : MonoBehaviour 
	{
		public static GameObject		audioController; //Audio Control object
		private static GameObject 		mainCamera;	
		
		public AudioListener			audioListener; //Audio listener object
		
		private string[]				audioClipString; //String array of sounds
		public static List<AudioClip> 	allAudioClips; //List for string
		
		public float					audio1Volume; //Volume for sounds
		public float					audio2Volume; //Volume for music
		
		public static bool				bossMusic = false; //Check for boss wave 
		
		//Assign objects
		void Awake()
		{
			audioController = this.transform.gameObject;
			mainCamera = GameObject.FindWithTag("MainCamera");
		}
		
		//Assign list of sounds
		void Start() 
		{
			audioClipString = new string[8];
			allAudioClips = new List<AudioClip>(8);
				
			//Add audio to string array
			audioClipString[0] = "MainMenu";
			audioClipString[1] = "GamePlay";
			audioClipString[2] = "Boss";
			audioClipString[3] = "MenuBeep";
			audioClipString[4] = "PowerPickUp";
			audioClipString[5] = "Laser";
			audioClipString[6] = "Explosion";
			audioClipString[7] = "Shield";
			
			//Add audio to list
			int clips = 0;
			
			foreach(string audioType in audioClipString)
			{
				allAudioClips.Add((AudioClip)Resources.Load("SFX/" + audioType));
				clips++;
			}
		}
		
		void Update()
		{
			//Once to wave 14, fade out game music and fade in boss music
			if(MasterControl.gameModes == MasterControl.GameModes.GameScreen)
			{
				if(MasterControl.gameWaves == 14)
					StartBossMusic();
			}
			
			if(MasterControl.gameWaves == 1)
			{
				audio1Volume = 1;
				audio2Volume = 0;
			}
		}
		
		//If paused, resume music 
		public static void ResumeAudio()
		{
			audioController.GetComponent<AudioSource>().Play();
		}
		
		//Not in used, but just in case
		public static void StopAudio()
		{
			audioController.GetComponent<AudioSource>().Stop();
		}
		
		//Pause game, pause audio
		public static void PauseAudio()
		{
			audioController.GetComponent<AudioSource>().Pause();
		}
		
		//Fade in boss music
		void fadeIn() 
		{
		    if (audio2Volume < 1f) 
			{
		        audio2Volume += 0.4f * Time.deltaTime;
		        audioListener.GetComponent<AudioSource>().volume = audio2Volume;
		    }
		}
	 
		//Fade out music for boss
		void fadeOut() 
		{
		    if(audio1Volume > 0.1f)
		    {
		        audio1Volume -= 0.4f * Time.deltaTime;
		        audioListener.GetComponent<AudioSource>().volume = audio1Volume;
		    }
		}
		
		//Start boss music after level fade out
		void StartBossMusic()
		{
			fadeOut();
	 
	    	if (audio1Volume <= 0.1) 
			{
		        if(bossMusic == false)
		        {
		          bossMusic = true;
		          audioListener.GetComponent<AudioSource>().clip = allAudioClips[2];
		          audioListener.GetComponent<AudioSource>().Play();
		        }
				
	        	fadeIn();		
			}
	    }
		
		//Audio list
		public static void PlayAudio(string clip)
		{ 
			switch(clip)       
	      	{	
				//Main menu music
		        case "MainMenu": 
				
					audioController.GetComponent<AudioSource>().Stop();
					audioController.GetComponent<AudioSource>().clip = allAudioClips[0];
					audioController.GetComponent<AudioSource>().loop = true;
					audioController.GetComponent<AudioSource>().Play();
					
		            break;
				//Game play music
		        case "GamePlay":
				
					audioController.GetComponent<AudioSource>().Stop();
					audioController.GetComponent<AudioSource>().clip = allAudioClips[1];
					audioController.GetComponent<AudioSource>().loop = true;
					audioController.GetComponent<AudioSource>().Play();
				
					break;
				//Boss music
				case "Boss":
				
					audioController.GetComponent<AudioSource>().Stop();
					audioController.GetComponent<AudioSource>().clip = allAudioClips[2];
					audioController.GetComponent<AudioSource>().loop = true;
					audioController.GetComponent<AudioSource>().Play();
				
				break;
				//Main menu scroll
				case "MenuBeep":
				
					audioController.GetComponent<AudioSource>().PlayOneShot(allAudioClips[3]);
					
					break;
				//Powerup collision/Game start 
				case "PowerPickUp":
				
					audioController.GetComponent<AudioSource>().PlayOneShot(allAudioClips[4]);
					
					break;
				//Player laser
				case "Laser":
				
					mainCamera.GetComponent<AudioSource>().PlayOneShot(allAudioClips[5]);
					
					break;
				//Object explosion
				case "Explosion":
				
					audioController.GetComponent<AudioSource>().PlayOneShot(allAudioClips[6]);
					
					break;
				//Shield/Boss hit
				case "Shield":
				
					audioController.GetComponent<AudioSource>().PlayOneShot(allAudioClips[7]);
					
					break;     	
		        default:            
		            Debug.Log("Invalid selection.");            
		            break; 
	       	}
		}
	}
}
