using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class DartShip : EnemyControl 
	{
		//Move and face forward
		void Update() 
		{
			transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
			GetComponent<Rigidbody>().velocity = new Vector3(xVect, yVect, zVect);
		}
	}	
}
