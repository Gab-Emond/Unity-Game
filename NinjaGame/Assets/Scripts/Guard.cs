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

	public EnemyController enemyController;
	public Transform pathHolder;
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
	
	

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rebootTimer = rebootTime;
		originCol = spotlight.color;
		

		Vector3[] wayPoints = new Vector3[pathHolder.childCount];
		
		for (int i = 0; i< wayPoints.Length; i++){
			wayPoints[i] = pathHolder.GetChild(i).position;
		}
		transform.position = wayPoints [0]; //puts guard at first waypoint, otherwise no turn towards next
		move = FollowPath(wayPoints);
		currentState = EntityState.Guarding;
		/*
		StartCoroutine(move);//FollowPath(wayPoints) would also work
		moveRunning = true;
		*/
	}

	/**/
	void Update() {
		//state machine
		print(currentState);
		switch (currentState)
		{
			case EntityState.Guarding:
				//if not running: start coroutine
				if(!moveRunning){
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
						rebootTimer = rebootTime;
						currentState = EntityState.Guarding;
					}
					else if(!gettingUp){
						DisableRagdoll();
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
		
		//&& Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) < 179.95f
		while (transform.forward != dirToLookTarget){//Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f ) {
			float fracComplete = (Time.time - startTime) / standUpTime;

			Vector3 midLook = Vector3.Slerp(lookInit,dirToLookTarget, Mathf.SmoothStep(0f,1f,fracComplete));	
			
			transform.rotation = Quaternion.LookRotation(midLook);
			yield return null;
		
		//TODO: fix to make sure turns in shortest dir, as backwards and forwards should both work as "facing"
		
		}
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
			yield return null;
		}
	}
	

	bool CanSeePlayer(float viewDistance, float viewAngle) {
		RaycastHit hitInfo;
		Vector3 vectResult;
		bool canSee = false;
		if ((transform.position-target.position).sqrMagnitude < viewDistance*viewDistance) {//compares distance faster, see https://docs.unity3d.com/ScriptReference/Vector3-sqrMagnitude.html
			
			Vector3 dirToPlayer = (target.position - transform.position).normalized;//find the vector pointing from our position to the target
			float angleBetweenGuardAndPlayer = Vector3.Angle (transform.forward, dirToPlayer);
			if(angleBetweenGuardAndPlayer< viewAngle / 2f){
				
				vectResult = Vector3.Cross(Vector3.up,dirToPlayer);
				if(!Physics.Linecast(transform.position, target.position,viewMask)){//LayerMask viewMask;
					canSee= true;
					//Debug.DrawLine (transform.position, target.position, Color.red);
				}
				//Debug.DrawLine (target.position-vectResult, target.position, Color.yellow);
				
				else if(Physics.Linecast(target.position-vectResult, target.position, out hitInfo)){//LayerMask viewMask;
					
					if(!Physics.Linecast(transform.position, hitInfo.point,viewMask)){
						canSee = true;
						//Debug.DrawLine(transform.position, hitInfo.point, Color.green);
					}
				}
				else if(Physics.Linecast(target.position+vectResult, target.position, out hitInfo)){//LayerMask viewMask;
					
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
