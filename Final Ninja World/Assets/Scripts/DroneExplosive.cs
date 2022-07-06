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
		//https://answers.unity.com/questions/669598/detect-if-player-is-in-range-1.html



		/*
		void Start() {
			PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
		}
		*/
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

		private void OnTriggerEnter(){
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