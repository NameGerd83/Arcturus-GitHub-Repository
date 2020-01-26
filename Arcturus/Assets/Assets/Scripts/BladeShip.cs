using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class BladeShip : EnemyControl 
	{
		private GameObject				playerObject; //Player object to attack
		
		void Start() 
		{
			playerObject = GameObject.FindWithTag("Player");
			
			Debug.Log (playerObject);
			
			//Check if player is activate and attack
			if(playerObject != null)
			{
				transform.LookAt(playerObject.transform);
				AttackPlayer();
			}
		}
		
		//Move towards last player ship position
		void AttackPlayer()
		{
			this.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward);
		}
	}
}