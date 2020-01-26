using UnityEngine;
using System.Collections;

public class StarControl : MonoBehaviour 
{
	public ParticleEmitter[]			stars;

	private float						starSpeed = .08f;
	
	//Faster stars
	void SpeedUpStars()
	{
		stars[0].worldVelocity = new Vector3(0, 0, -.5f - starSpeed);
		stars[1].worldVelocity = new Vector3(0, 0, -.2f - starSpeed);
		stars[2].worldVelocity = new Vector3(0, 0, -.15f - starSpeed);
		stars[3].worldVelocity = new Vector3(0, 0, -.1f - starSpeed);
		
		starSpeed += starSpeed;
	}
	
	//Reset to default
	void ResetStars()
	{
		starSpeed = .08f;
		
		stars[0].worldVelocity = new Vector3(0, 0, -.5f);
		stars[1].worldVelocity = new Vector3(0, 0, -.2f);
		stars[2].worldVelocity = new Vector3(0, 0, -.15f);
		stars[3].worldVelocity = new Vector3(0, 0, -.1f);
	}
}
