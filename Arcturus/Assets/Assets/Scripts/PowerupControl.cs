using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class PowerupControl : MonoBehaviour 
	{
		public GameObject[]				upgradeObjects; //All upgrade objects
		
		public ParticleEmitter			enemyExplosion; //Explosion particles
		
		void OnTriggerEnter(Collider other)
		{
			//If power up ship is hit by player or player bullet, destroy ship
			if(other.gameObject.tag == "PlayerBullet" || other.gameObject.tag == "Player")		
			{
				//If game wave is 7, count for shield
				MasterControl.shieldUpgrade--;
				
				ScoreControl.RaiseScore(400);
				
				//Create upgrade object 
				Instantiate(upgradeObjects[MasterControl.upgradeItem], this.transform.position, this.transform.rotation);
				
				//Destroy ship
				StartCoroutine("DestroyObject");			
			}
		}
		
		//Death sequence
		IEnumerator DestroyObject()
		{
			yield return new WaitForSeconds(.02f);
			
			AudioControl.PlayAudio("Explosion");
			Instantiate(enemyExplosion, this.transform.position, this.transform.rotation);
			Destroy(this.gameObject);
		}
	}
}