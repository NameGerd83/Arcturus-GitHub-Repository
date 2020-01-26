using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class ItemUpgradeCheck : MonoBehaviour 
	{
		//If power up collides with player, increase count
		void OnTriggerEnter(Collider other)
		{
			if(other.gameObject.tag == "Player")
			{
				ScoreControl.RaiseScore(250);
				
				MasterControl.upgradeItem++;
			}
			
			//If passes through scene and hits bound, destroy
			if(other.gameObject.tag =="EnemyBound")
			{
				Destroy(this.gameObject);
			}
		}
	}
}
