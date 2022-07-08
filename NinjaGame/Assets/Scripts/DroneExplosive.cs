using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class DroneExplosive : MonoBehaviour //todo: change to enemy parenting
    {
        public GameObject target;

		////////////////explosion
	    public GameObject explosion;
		public float explosionRange = 2f;
		public float explosionForce = 10f;
		public LayerMask whatIsEnemies = 1;


		float speed = 5;
		private float speedWithAccel;
		float turnSpeed = 45;
		Vector3[] path;
		int targetIndex;

		private bool runningCoroutine = false;
		private IEnumerator coroutine;
		//Task t = new Task();//taskmanager

		public float _maxRange;



		/**/
		void Start() {
			//PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
			GetIdlePath();
			StartCoroutine(FollowPath());
		}
		

		/*
		private void Update(){
			switch (_currentState){
				case EnemyState.Wander:
				{
					if (!runningCoroutine)
					{
						GetIdlePath();
						runningCoroutine = true;
						coroutine = 
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

					bool needsPath = (path == null)||(path[path.Length-1]-_target.transform.position).sqrMagnitude > _maxRange*_maxRange || IsPathBlocked(path[path.Length-1], _target.transform.position);
                


					if(needsPath){ //&& timeSinceLastCall > 1f){
						pathNodeIndex = 0;
						if(isLookingForPath){//||Time.time - timeSinceGotPath < 1f
							return;
						}
						else if(IsPathBlocked(transform.position, _target.transform.position)){
							PathRequestManager.RequestPath(transform.position,_target.transform.position, OnPathFound);
							isLookingForPath = true;
							// if path blocked completely, lose target, go back to wander
							timeSinceGotPath = Time.time;
							return;
						}
						else{//if not blocked, go straight to target
							path = new Vector3[]{transform.position, _target.transform.position};
							//print("got");
							return;
						}
                	}

					
					
					if() //(Vector3.Distance(transform.position, _target.transform.position) < _attackRange)
					{
						//lostTarget?
					}
					
					break;
				}
			}
    	}
		*//**/

		void GetIdlePath(){
				
			path = new Vector3[2];
			/*path[0] = transform.position + Vector3.up * 0.25f;
			path[1] = transform.position + Vector3.down * 0.25f;*/
			RaycastHit hitInfo;
			if(Physics.Linecast(transform.position, transform.position + Vector3.up*10f, out hitInfo)){
				path[0] = transform.position + Vector3.up * (hitInfo.distance-1f);

			}
			else{
				path[0] = transform.position + Vector3.up * 10f;
			}
			
			if(Physics.Linecast(transform.position, transform.position + Vector3.down*10f, out hitInfo)){
				path[1] = transform.position + Vector3.down * (hitInfo.distance-1f);
			}
			else{
				path[1] = transform.position + Vector3.down * 10f;
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



		public void Alert(GameObject _target){
			target = _target;
			PathRequestManager.RequestPath(transform.position,target.transform.position, OnPathFound);
		}
		public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
			if (pathSuccessful) {		
				path = newPath;
				targetIndex = 0;
				StopCoroutine(FollowPath());//"FollowPath" previously
				StartCoroutine(FollowPath());
			}
		}

		IEnumerator FollowPath(){
			
			Vector3 targetWaypoint = path[0];
			yield return StartCoroutine (TurnToFace (targetWaypoint));
			
			while (true) {
				if (transform.position == targetWaypoint) {
					targetIndex = (targetIndex + 1) % path.Length;
					if (targetIndex >= path.Length) {
						yield break;
					}
					targetWaypoint = path[targetIndex];
					yield return StartCoroutine (TurnToFace(targetWaypoint));
				}
				
				//Vector3 direction = targetWaypoint-transform.position;//Vector3.ProjectOnPlane(Vector3.right, direction)
				transform.Rotate(Vector3.right, 30*speed*Time.deltaTime);//rotate while moving closer
				transform.position = Vector3.MoveTowards(transform.position,targetWaypoint,speed * Time.deltaTime);
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

		private void OnTriggerEnter(){//detects if player touches/inside
			print("hit");
			Explode();

		}

		public void TakeHit(){
			print("hit");
			Explode();
		}

		private void Explode()
		{
			//Instantiate explosion
			if (explosion != null) {
				
				Instantiate(explosion, transform.position, Quaternion.identity);

				//Check for enemies 
				Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
				for (int i = 0; i < enemies.Length; i++){
					//Get component of enemy and call Take Damage

					//Just an example!
					///enemies[i].GetComponent<ShootingAi>().TakeDamage(explosionDamage);

					//Add explosion force (if enemy has a rigidbody)
					if (enemies[i].GetComponent<Rigidbody>())
						enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
				}
			}
			//Add a little delay, just to make sure everything works fine
			Destroy(gameObject);
			//destroying object should stop coroutines; (stop moving)
		}




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