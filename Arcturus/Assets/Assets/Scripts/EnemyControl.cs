using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class EnemyControl : MonoBehaviour 
	{
		public ParticleEmitter			enemyExplosion; //Explosion particles
		
		public float					xVect = 0, yVect = 0, zVect = 0; //Manually change position
		
		void OnTriggerEnter(Collider other)
		{
			//Check for collision with player or player bullets
			if(other.gameObject.tag == "PlayerBullet" || other.gameObject.tag == "Player")		
			{
				ScoreControl.RaiseScore(100);
				
				AudioControl.PlayAudio("Explosion");
				
				Instantiate(enemyExplosion, this.transform.position, this.transform.rotation);
				
				Destroy(this.gameObject);
			}
			
			//Check if enemy has passed the camera and destroy it
			if(other.gameObject.tag == "EnemyBound")
			{
				Destroy(this.gameObject);
			}
		}
	}
}