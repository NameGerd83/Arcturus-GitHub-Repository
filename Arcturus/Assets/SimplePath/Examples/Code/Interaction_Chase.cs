#region Copyright
// ******************************************************************************************
//
// 							SimplePath, Copyright © 2011, Alex Kring
//
// ******************************************************************************************
#endregion

/*/////////////////////////////////////////////////////
Name: Greg Clark
Date Started/Modified: 1/22/2013
Purpose/Changes: Follow player object
Attach To: Chasing Ships
/////////////////////////////////////////////////////*/
		
using UnityEngine;
using System.Collections;
using SimpleAI.Navigation;

[RequireComponent(typeof(NavigationAgentComponent))]
public class Interaction_Chase : MonoBehaviour 
{
	#region Unity Editor Fields
	public GameObject						m_chaseObject;
	public float							m_replanInterval = 0.5f;
	#endregion
	
	#region Fields
	private NavigationAgentComponent 		m_navigationAgent;
	private bool							m_bNavRequestCompleted;
	#endregion
	
	public GameObject						startingPos;
	
	public float							chaseRange;
	public float							fireRange;
	public float							guardSpeed;
	
	public bool								guardShip;
	public bool								homingShip;
	
	private bool							isChasing = false;
	#region MonoBehaviour Functions
	void Awake()
	{
		m_bNavRequestCompleted = true;
		m_navigationAgent = GetComponent<NavigationAgentComponent>();
	}
	
	void Start()
	{
		m_chaseObject = GameObject.FindGameObjectWithTag("Enemy"); //Assign to player ship on start
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.LookAt(m_chaseObject.transform);
		
		if(homingShip)
		{
			if ( m_bNavRequestCompleted )
			{
				if ( m_navigationAgent.MoveToGameObject(m_chaseObject, m_replanInterval) )
				{
					m_bNavRequestCompleted = false;
				}
			}
		}
		
		if(guardShip)
			GuardShipChase();
	}
	
	void GuardShipChase()
	{
	//If ship is in range, shoot at player
		if(Vector3.Distance(this.transform.position, m_chaseObject.transform.position) <= chaseRange)
		{
			isChasing = true;
		
			if(isChasing)
			{
				if ( m_bNavRequestCompleted )
				{
					if ( m_navigationAgent.MoveToGameObject(m_chaseObject, m_replanInterval) )
					{
						m_bNavRequestCompleted = false;
					}
				}
			}
			
			if(Vector3.Distance(this.transform.position, m_chaseObject.transform.position) <= fireRange)
			{
				isChasing = false;
				
				SendMessage("AttackPlayer");
			}
		}
		
		else if(Vector3.Distance(this.transform.position, m_chaseObject.transform.position) > chaseRange)
		{
			isChasing = false;
			
			//transform.position = Vector3.Lerp(transform.position, startingPos.transform.position, Time.deltaTime * guardSpeed);
			m_bNavRequestCompleted = true;
			
			if ( m_bNavRequestCompleted )
			{
				if ( m_navigationAgent.MoveToGameObject(startingPos, m_replanInterval) )
				{
					m_bNavRequestCompleted = false;
				}
			}
		}
	}
		
	#endregion
	
	#region Messages
	private void OnNavigationRequestSucceeded()
	{
		m_bNavRequestCompleted = true;
	}
	
	private void OnNavigationRequestFailed()
	{
		m_bNavRequestCompleted = true;
	}
	#endregion
}
