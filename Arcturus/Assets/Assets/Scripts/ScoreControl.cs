using UnityEngine;
using System.Collections;

namespace Arcturus
{
	public class ScoreControl : MonoBehaviour 
	{
		public static int 			totalScore = 0; //Score count
	
		//Set score to 0 at game start
		public void Awake()
		{
			totalScore = 0;
		}
	
		//Update/increment score
		void Update()
		{
			var prefix = "Score: ";
			MasterControl.staticGameGUI[0].GetComponent<GUIText>().text = prefix + totalScore;		
		}
		
		//Raise score
		public static void RaiseScore(int scoreAmount)
		{
			totalScore += scoreAmount;
		}
	}
}