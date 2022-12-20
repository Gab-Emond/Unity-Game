using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingPlatform : MonoBehaviour
{
    public Transform platform;
	public Transform pathHolder;
    
    public float travelTime = 2f; 
    float startTime;
    public float waitTime;
    public float speed;
    
    private Vector3 velocity = Vector3.zero;
    int prevWaypointIndex;
    int targetWaypointIndex;
    Vector3[] waypoints;
    Vector3 targetWaypoint;

    //Vector3 collisionDecal;//keep 
    
    

    // Start is called before the first frame update
    void Start()
    {
    	waypoints = new Vector3[pathHolder.childCount];

        for (int i = 0; i< waypoints.Length; i++){
            waypoints[i] = pathHolder.GetChild(i).position;
        }
        

        targetWaypointIndex = 0;
        if(waypoints.Length >0){
            transform.position = waypoints[targetWaypointIndex];
        }
        startTime = Time.time;
       

    }

    private void FixedUpdate() {
        if(waypoints.Length >0){
            FollowPath();
        }
    }


    void FollowPath() {//Guard Coroutine, modified as fixed update function
        velocity = Vector3.zero;
        //transform.position = waypoints [0];

        targetWaypoint = waypoints[targetWaypointIndex];
        //transform.LookAt(targetWaypoint);
        
            
        if (transform.position == targetWaypoint){//(transform.position == targetWaypoint) {
            prevWaypointIndex = targetWaypointIndex;
            targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
            //targetWaypoint = waypoints[targetWaypointIndex];
            //yield return new WaitForSeconds (waitTime);
            //yield return StartCoroutine (TurnToFace (targetWaypoint));
            startTime = Time.time;
        }
        
        else{
            
            float t = (Time.time - startTime)/travelTime;
            
            transform.position = Vector3.Lerp(waypoints[prevWaypointIndex], targetWaypoint, Mathf.SmoothStep(0f,1f,t));//change to math.utility
            
        }

        
        
    }

    //character controller issue: https://docs.unity3d.com/ScriptReference/Physics-autoSyncTransforms.html
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player" || other.attachedRigidbody !=null){
            other.transform.SetParent(transform, true); 
        }
    }
    //without parenting;
    //changes position of player in update,
    //fixupdate move
    //cache new position in late update? or fixed update?

    /*
    private void OnTriggerStay(Collider other) {
        grapplePoint = grappledObject.position + grappledDecal;

    }
    
    private void LateUpdate() {
        grappledDecal = grappledObject.position - transform.position;
    }
    */
    private void OnTriggerExit(Collider other) {
         if(other.tag == "Player" || other.attachedRigidbody !=null){
            other.transform.SetParent(null);
        }
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