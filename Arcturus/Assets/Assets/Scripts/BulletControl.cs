using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class BulletControl : MonoBehaviour 
	{
		void Update()
		{
			//If player or Arcturus has died, destroy all bullets
			if(PlayerHealth.playerLives <= 0 || ArcturusShip.arcturusDead == true)
				Destroy(this.gameObject);
		}
		
		void OnTriggerEnter(Collider other)
		{
			//If player bullet collides with enemy, destroy bullet
			if(this.gameObject.tag == "PlayerBullet")
			{
				if(other.gameObject.tag == "Enemy") 
				{
					Destroy(this.gameObject);
				}
			}
			
			//If enemy bullet collides with player, destroy bullet
			if(this.gameObject.tag == "EnemyBullet")
			{
				if(other.gameObject.tag == "Player")
				{
					Destroy(this.gameObject);
				}
			}
			
			//If bullet hits asteroid or boundaries, destroy bullet
			if(other.gameObject.tag == "Boundary" || other.gameObject.tag == "Asteroid")
			{
				Destroy(this.gameObject);
			}
		}
	}
}