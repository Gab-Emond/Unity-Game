using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour, IDamageable
{	
	public enum EntityState
	{
		Guarding,
		Searching,
		Alerting,
		Incapacitated
	}

	//event alert signal
	public EnemyController enemyController;
	public Transform pathHolder;
	Vector3[] wayPoints;
	public LayerMask viewMask;
	public Transform target;
	int targetWaypointIndex = 1;
	public float speed;
	public float waitTime;

	private Rigidbody rb;
	
	private float turnSpeed = 60;
	public Light spotlight;
	private Color originCol;
	float playerVisibleTimer = 0;
	float timeToSpotPlayer = 5f;
	IEnumerator move;
	bool moveRunning;
	private EntityState currentState;

	float rebootTime = 10f;
	float rebootTimer;
	bool standing = true;
	bool gettingUp = false;
	
	//for changing individual when multiple objects use same material:
	//https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html
	//http://thomasmountainborn.com/2016/05/25/materialpropertyblocks/

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rebootTimer = rebootTime;
		originCol = spotlight.color;
		

		wayPoints = new Vector3[pathHolder.childCount];
		
		for (int i = 0; i< wayPoints.Length; i++){
			wayPoints[i] = pathHolder.GetChild(i).position;
		}
		transform.position = wayPoints [0]; //puts guard at first waypoint
		currentState = EntityState.Guarding;
		
	}

	
	void Update() {
		//state machine
		//print(currentState);
		switch (currentState)
		{
			case EntityState.Guarding:
				//if not running: start coroutine
				if(!moveRunning){
					move = FollowPath(wayPoints);
					StartCoroutine(move);
					moveRunning = true;
				}
				if(spotlight.color != originCol){
					spotlight.color = originCol;
				}
				if(CanSeePlayer(15,38)){
			
					currentState = EntityState.Alerting;
					//playerVisibleTimer += Time.deltaTime*32;
					
					if(enemyController != null && enemyController.alarmSounded == false){
						
						enemyController.SoundAlarm();
						//stop moving
						//StopCoroutine(move);
						//isStopped = true;
					}
				}

				break;
			case EntityState.Searching:
				//addWayPoint
				//goto waypoint
				break;
			case EntityState.Alerting:
				if(moveRunning){
					StopCoroutine(move);
					moveRunning = false;
				}
				if(!CanSeePlayer(15,38)){
					if(enemyController == null){
						currentState = EntityState.Guarding;
					}
					else if( enemyController.alarmSounded == false){
						currentState = EntityState.Guarding;
					}		
					//playerVisibleTimer -= Time.deltaTime;
					
				}

				if(spotlight.color != Color.red){
					spotlight.color = Color.red;
				}


				/*You can clean up the materials explicitly using Resources.Unload( renderer.material ) when done, 
				instead of using Resources.UnloadUnusedAssets, which should be faster.*/


				break;
			case EntityState.Incapacitated:
				if(moveRunning){
					StopCoroutine(move);
					moveRunning = false;
				}
				if(spotlight.color != Color.black){
					spotlight.color = Color.black;
				}

				if(rebootTimer>0){
					//print("ko");
					rebootTimer -= Time.deltaTime;
					standing = false;
					gettingUp = false;
				}
				else{
					//lift up guard
					//point to Vector3.up
					if(standing){
						DisableRagdoll();
						rebootTimer = rebootTime;
						currentState = EntityState.Guarding;
					}
					else if(!gettingUp){
						StartCoroutine(StandUp());
						gettingUp = true;
					}
					
				}
				break;
			default:
				break;
		}

	}

	void EnableRagdoll()
    {   /**/
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.useGravity = true;
        
    }
	
	void DisableRagdoll()
    {   /**/
        rb.isKinematic = true;
        //rb.detectCollisions = false;
        rb.useGravity = false;
        
    }
	

	IEnumerator StandUp(){
		Vector3 lookTarget = Vector3.up;
		float standUpTime = 0.25f;
		Vector3 lookInit = transform.forward;
		float startTime = Time.time;
		Vector3 dirToLookTarget = Vector3.forward;//(lookTarget - transform.position).normalized;
		float angularDragInit = rb.angularDrag;
		rb.angularDrag = 10f;
		//while more than 1 degree off being straight up
		while (Vector3.Angle(transform.up, Vector3.up) > 1f ) {//(transform.up != Vector3.up){
			//float fracComplete = (Time.time - startTime) / standUpTime;

			//Vector3 midLook = Vector3.Slerp(lookInit,dirToLookTarget, Mathf.SmoothStep(0f,1f,fracComplete));	
			
			//transform.rotation = Quaternion.LookRotation(midLook);
			
			//rb.MoveRotation(Quaternion.LookRotation(midLook));
			
			rb.AddTorque(Vector3.Cross(transform.up,Vector3.up)*75f, ForceMode.Force);

			yield return null;
		}
		rb.angularDrag = angularDragInit;
		gettingUp = false;
		standing = true;
		yield return null;
	}
	IEnumerator FollowPath(Vector3[] waypoints) {//A coroutine is a function that can suspend its execution (yield) until the given YieldInstruction finishes.
		Vector3 currentVelocity = Vector3.zero;
		

		
		Vector3 targetWaypoint = waypoints [targetWaypointIndex];
		//transform.LookAt (targetWaypoint);
		yield return StartCoroutine (TurnToFace (targetWaypoint));

		while (true) {
			
			if (transform.position == targetWaypoint){//(transform.position == targetWaypoint) {
				targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;					
				targetWaypoint = waypoints [targetWaypointIndex];
				yield return new WaitForSeconds (waitTime);
				yield return StartCoroutine (TurnToFace (targetWaypoint));
			}
			else if((transform.position-targetWaypoint).sqrMagnitude <= 0.125f){
				transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, Vector3.SqrMagnitude(currentVelocity) * Time.deltaTime);
			}
			else{
				transform.position = Vector3.SmoothDamp(transform.position, targetWaypoint, ref currentVelocity, 1.25f , speed);//smoother than movetowards, sorta gauss func				}
			}
			yield return null;
		}
	}


	IEnumerator TurnToFace(Vector3 lookTarget) {
		Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
		float targetAngle = 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

		while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) {
			float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
			transform.eulerAngles = Vector3.up * angle;
			if(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) <= 0.05f){
				Debug.DrawLine(transform.position,transform.position+20f*transform.forward, Color.green);
			}
			yield return null;
		}
	}
	

	bool CanSeePlayer(float viewDistance, float viewAngle) {
		RaycastHit hitInfo;
		Vector3 vectResult;
		bool canSee = false;
		if ((transform.position-target.position).sqrMagnitude < viewDistance*viewDistance) {//compares distance faster, see https://docs.unity3d.com/ScriptReference/Vector3-sqrMagnitude.html
			
			Vector3 dirToPlayer = (target.position - transform.position);//find the vector pointing from our position to the target
			float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
			if(angleBetweenGuardAndPlayer< viewAngle / 2f){
				
				vectResult = Vector3.Cross(Vector3.up,dirToPlayer);
				if(!Physics.Linecast(transform.position, target.position,viewMask)){//if nothing is blocking the player (center)
					canSee= true;
					//Debug.DrawLine (transform.position, target.position, Color.red);
				}
				//Debug.DrawLine (target.position-vectResult, target.position, Color.yellow);
				
				else if(Physics.Linecast(target.position-vectResult, target.position, out hitInfo)){//if nothing is blocking the player (left)
					
					if(!Physics.Linecast(transform.position, hitInfo.point,viewMask)){
						canSee = true;
						//Debug.DrawLine(transform.position, hitInfo.point, Color.green);
					}
				}
				else if(Physics.Linecast(target.position+vectResult, target.position, out hitInfo)){//if nothing is blocking the player (right)
					
					if(!Physics.Linecast(transform.position, hitInfo.point,viewMask)){
						canSee = true;
						//Debug.DrawLine(transform.position, hitInfo.point, Color.green);
					}
				}
				
				
			
			}
		}
		return canSee;
		
	}

	public void TakeHit(Vector3 damageDir, Vector3 damagePos){
		//turn on rigidbody
		currentState = EntityState.Incapacitated;
		if(rb.isKinematic){
			EnableRagdoll();
		}
		rb.AddForceAtPosition(damageDir, damagePos, ForceMode.Impulse);
    }


	void OnDrawGizmos() {
		Vector3 startPosition = pathHolder.GetChild(0).position;
		Vector3 previousPosition = startPosition;

		foreach (Transform waypoint in pathHolder) {
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (waypoint.position, .3f);

			Gizmos.color = Color.yellow;
			Gizmos.DrawLine (previousPosition, waypoint.position);
			previousPosition = waypoint.position;
		}
		Gizmos.DrawLine (previousPosition, startPosition);
	}

}
