#region Copyright
// ******************************************************************************************
//
// 							SimplePath, Copyright Â© 2011, Alex Kring
//
// ******************************************************************************************
#endregion

/*/////////////////////////////////////////////////////
Name: Greg Clark
Date Started/Modified: 1/22/2013
Purpose/Changes: Pathfinding/Chasing mod
Attach To: Patrol ship
/////////////////////////////////////////////////////*/
		
using UnityEngine;
using System.Collections;
using SimpleAI.Navigation;

[RequireComponent(typeof(NavigationAgentComponent))]
public class Interaction_Patrol : MonoBehaviour 
{
	#region Unity Editor Fields
	public GameObject[]						m_patrolNodes;
	public float							m_replanInterval = float.MaxValue;
	#endregion
	
	#region Fields
	private NavigationAgentComponent 		m_navigationAgent;
	private bool							m_bNavRequestCompleted;
	private int								m_currentPatrolNodeGoalIndex;
	
	//Additional Variables
	public GameObject						target; //Player object
	public GameObject						enemyBullet; //enemy ship's bullet
	
	private bool							isChasing; //Checks to see if enemy is chasing player
	private bool							isDamaged = false; //Checks to see if ship has been shot by player
	public float							chaseRange; //Checks to see if player is in range to chase
	private float 							nextFire; //Interval timer between bullets
	public float							fireRate; //How fast the intervals will iterate
	public float							bulletSpeed; //Speed of the bullet force
	
	#endregion
	
	#region MonoBehaviour Functions
	void Awake()
	{
		m_currentPatrolNodeGoalIndex = 0;
		m_bNavRequestCompleted = true;
		m_navigationAgent = GetComponent<NavigationAgentComponent>();
	}
	
	IEnumerator Start()
	{
		target = GameObject.FindGameObjectWithTag("Player"); //Assign player object on start
		//m_patrolNodes = GameObject.FindGameObjectsWithTag("PatrolNode"); //Assign all waypoints on start
		
		yield return new WaitForSeconds(.5f);
		StartCoroutine(PathFind()); //Begin pathfinding
	}
	
	// Update is called once per frame
	void Update() 
	{
		if ( m_patrolNodes == null || m_patrolNodes.Length == 0 )
		{
			Debug.LogError("No patrol nodes are set");
			return;
		}
		
		transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity); //Keeps ship facing forward on rotation
	}
	#endregion
	
	IEnumerator PathFind()
	{
		while(true)
		{
			//Can only detect player within 60 degrees in front of enemy ship
			Vector3 targetDir = target.transform.position - transform.position;
        	Vector3 forward = transform.forward;
        	float angle = Vector3.Angle(targetDir, forward);
			
        	if (angle < 60.0f)
			{
				//If ship is seen, begin chasing
				if(!isChasing)
				{
					isChasing = true;
					StartCoroutine(Chase());
				}
			}
			
			//If true, chase after player
			else if(isDamaged)
			{
				StartCoroutine(Chase());
			}
			
			//If ship breaks sight, go back to pathfinding
			else
			{
				if ( m_bNavRequestCompleted )
				{
					m_currentPatrolNodeGoalIndex = ( m_currentPatrolNodeGoalIndex + 1 ) % m_patrolNodes.Length;
					Vector3 destPos = GetPatrolNodePosition( m_currentPatrolNodeGoalIndex );
					m_navigationAgent.MoveToPosition(destPos, m_replanInterval);
					m_bNavRequestCompleted = false;
				}
			}
			yield return 0;
		}
	}
	
	IEnumerator Chase()
	{
		while(true)
		{
			//If ship is in range, shoot at player
			if(Vector3.Distance(this.transform.position, target.transform.position) <= chaseRange)
			{	
				SendMessage("AttackPlayer");
				
				if (isChasing)
				{
					//Move towards the player
					if ( m_navigationAgent.MoveToGameObject(target, m_replanInterval) )
					{
						m_bNavRequestCompleted = false;
					}
				}
			}
			else 
			{
				//If range is broken, go back to pathfinding and stop shooting at player
				if(isChasing)
				{
					isChasing = false;
					
					if ( m_bNavRequestCompleted )
					{
						m_currentPatrolNodeGoalIndex = ( m_currentPatrolNodeGoalIndex + 1 ) % m_patrolNodes.Length;
						Vector3 destPos = GetPatrolNodePosition( m_currentPatrolNodeGoalIndex );
						m_navigationAgent.MoveToPosition(destPos, m_replanInterval);
						m_bNavRequestCompleted = false;
					}
				}
				
				break;
			}
			
			yield return 0;
		}
	}
	
	private Vector3 GetPatrolNodePosition(int index)
	{
		if ( m_patrolNodes == null || m_patrolNodes.Length == 0 )
		{
			Debug.LogError("No patrol nodes are set");
			return transform.position;
		}
		
		if ( index < 0 || index >= m_patrolNodes.Length	)
		{
			Debug.LogError("PatrolNode index out of bounds");
			return transform.position;
		}
		
		Vector3 patrolNodePosition = m_patrolNodes[index].transform.position;
		
		return patrolNodePosition;
	}
	
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
