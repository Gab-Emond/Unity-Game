using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
	public class Enemy : MonoBehaviour
	{
		public enum EnemyState
		{
			Wander,
			Chase,
			Attack
		}

		public Transform target;
		float speed = 5;
		private float speedWithAccel;
		float turnSpeed = 45;
		Vector3[] path;
		int targetIndex;
	    private EnemyState _currentState;

		//https://answers.unity.com/questions/669598/detect-if-player-is-in-range-1.html
		/*
		private void Update(){
			switch (_currentState){
				case EnemyState.Wander:
				{
					if (NeedsDestination())
					{
						GetDestination();
					}

					transform.rotation = _desiredRotation;

					transform.Translate(Vector3.forward * Time.deltaTime * 5f);

					var rayColor = IsPathBlocked() ? Color.red : Color.green;
					Debug.DrawRay(transform.position, _direction * _rayDistance, rayColor);

					while (IsPathBlocked())
					{
						//Debug.Log("Path Blocked");
						GetDestination();
					}

					var targetToAggro = CheckForAggro();
					if (targetToAggro != null)
					{
						_target = targetToAggro.GetComponent<DroneShooter>();
						_currentState = EnemyState.Chase;
					}
					
					break;
				}
				case EnemyState.Chase:
				{
					if (_target == null)
					{
						_currentState = EnemyState.Wander;
						return;
					}
					
					transform.LookAt(_target.transform);
					transform.Translate(Vector3.forward * Time.deltaTime * 5f);

					if (Vector3.Distance(transform.position, _target.transform.position) < _attackRange)
					{
						_currentState = EnemyState.Attack;
					}
					break;
				}
				case EnemyState.Attack:
				{
					if (_target != null)
					{
						Destroy(_target.gameObject);
					}
					
					// play laser beam
					
					_currentState = EnemyState.Wander;
					break;
				}
			}
    	}
		*/

		/*
		void Start() {
			PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
		}
		*/
		public void Alert(){
			PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
		}
		public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
			if (pathSuccessful) {		
				path = newPath;
				targetIndex = 0;
				_currentState = EnemyState.Chase;
				StopCoroutine(FollowPath());//"FollowPath" previously
				StartCoroutine(FollowPath());
			}
			else{
				_currentState = EnemyState.Wander;
				//path found failed,
			}
		}

		IEnumerator FollowPath() {
			
			Vector3 targetWaypoint = path[0];
			yield return StartCoroutine (TurnToFace (targetWaypoint));
			
			while (true) {
				if (transform.position == targetWaypoint) {
					targetIndex ++;
					if (targetIndex >= path.Length) {
						yield break;
					}
					targetWaypoint = path[targetIndex];
					yield return StartCoroutine (TurnToFace(targetWaypoint));
				}
				
				
				//transform.Rotate(Vector3.right, 30*speed*Time.deltaTime);//rotate while moving closer
				transform.position = Vector3.MoveTowards(transform.position,targetWaypoint, speed * Time.deltaTime);
				yield return null;

			}
		}

		IEnumerator TurnToFace(Vector3 lookTarget) {
			
			Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
			float targetAngle = 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

			while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) {

				float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
				transform.eulerAngles =new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);// Vector3.up * angle;
				yield return null;
			}
			//rotates only around y axis, so keeps original x and z

		}

		void IdlePath(){
			
			path = new Vector3[2];
			RaycastHit hitInfo;
			if(Physics.Linecast(transform.position, transform.position + Vector3.up*10f, hitInfo)){
				path[0] = transform.position + Vector3.up * (hitInfo.distance-1f);

			}
			else{
				path[0] = transform.position + Vector3.up * 8f;
			}
			
			if(Physics.Linecast(transform.position, transform.position + Vector3.down*10f, hitInfo)){
				path[1] = transform.position + Vector3.down * (hitInfo.distance-1f);
			}
			else{
				path[1] = transform.position + Vector3.down * 8f;
			}
			
			/*
			Random r = new Random();
			int rNodeNum = r.Next(1, 4); //for ints
			path = new Vector3[rNodeNum];
			path[0] = transform.position;
			for (int i = 0; i< rNodeNum; i++){				
				path[i] = (transform.position) + new Vector3(UnityEngine.Random.Range(-20f, 20f), 0f, UnityEngine.Random.Range(-20f, 20f));
				//free position around transform
			}
			*/
			
			
			
		}
		
		/*
		FindEmptyPointAroundPos(Vector3 pos){
			Vector3 possiblePos = pos + new Vector3(UnityEngine.Random.Range(-20f, 20f), 0f, UnityEngine.Random.Range(-20f, 20f)); 
			if(Physics.Linecast(pos, possiblePos)){
				
			}
			
		}
		*/
		
		public void OnDrawGizmos() {
			if (path != null) {

				for (int i = targetIndex; i < path.Length; i ++) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere(path[i], 1/4f);

					if (i == targetIndex) {
						Gizmos.DrawLine(transform.position, path[i]);
					}
					else {
						Gizmos.DrawLine(path[i-1],path[i]);
					}
				}
			}
		}
		

	}

}