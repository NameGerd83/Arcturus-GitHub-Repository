using UnityEngine;
using System.Collections;

public class TimedObjectDestructor : MonoBehaviour 
{
	public ParticleEmitter		explosionParticle;
	
	public float 				timeOut = 1.0f;
	
	public bool	 				detachChildren = false;
	public bool					explosionParticleOn;
	
	void Start()
	{
		Invoke ("DestroyNow", timeOut);
	}
	
	void DestroyNow()
	{
		if (detachChildren) 
		{
			Instantiate(explosionParticle, this.transform.position, this.transform.rotation);
			transform.DetachChildren();
		}

		DestroyObject (gameObject);
	}
}

