using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy{
	public class Guard : MonoBehaviour
	{	
		public EnemyController enemyController;
		public Transform pathHolder;
		public LayerMask viewMask;
		public Transform target;
		public float speed;
		public float waitTime;
		
		private float turnSpeed = 60;
		public Light spotlight;
		private Color originCol;

		float playerVisibleTimer = 0;
		float timeToSpotPlayer = 5f;
		IEnumerator move;
		void Start()
		{
			originCol = spotlight.color;

			Vector3[] wayPoints = new Vector3[pathHolder.childCount];
			
			for (int i = 0; i< wayPoints.Length; i++){
				wayPoints[i] = pathHolder.GetChild(i).position;
				
			}
			move = FollowPath(wayPoints);
			StartCoroutine(move);


		}


		IEnumerator FollowPath(Vector3[] waypoints) {//A coroutine is a function that can suspend its execution (yield) until the given YieldInstruction finishes.
			Vector3 currentVelocity = Vector3.zero;
			transform.position = waypoints [0]; //puts guard at first waypoint

			int targetWaypointIndex = 1;
			Vector3 targetWaypoint = waypoints [targetWaypointIndex];
			transform.LookAt (targetWaypoint);
			
			while (true) {
				
				if (transform.position == targetWaypoint){//(transform.position == targetWaypoint) {
					targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;					
					targetWaypoint = waypoints [targetWaypointIndex];
					yield return new WaitForSeconds (waitTime);
					yield return StartCoroutine (TurnToFace (targetWaypoint));
				}
				else if((transform.position-targetWaypoint).sqrMagnitude <= 0.125f){
					transform.position = Vector3.MoveTowards (transform.position, targetWaypoint, Vector3.SqrMagnitude(currentVelocity) * Time.deltaTime);
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

		void LateUpdate()
		{	
			//float playerVisibleTimer = 0;
			if(CanSeePlayer(15,38)){
				
				//playerVisibleTimer += Time.deltaTime*32;
				spotlight.color = Color.red;
				if(enemyController != null && enemyController.alarmSounded == false){
					enemyController.SoundAlarm();
					//stop moving
					//StopCoroutine(move);
					//isStopped = true;
				}
			}
			
			//else if recently changed
				//restart moving afterwards
				//return spotlight color

			else{
				//playerVisibleTimer -= Time.deltaTime;
				spotlight.color = originCol;
			}
			//playerVisibleTimer = Mathf.Clamp (playerVisibleTimer, 0, timeToSpotPlayer);
			
			//spotlight.color = Color.Lerp(originCol, Color.red, playerVisibleTimer/timeToSpotPlayer);


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
}