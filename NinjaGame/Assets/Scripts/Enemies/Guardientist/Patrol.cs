using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.StateMachine;

namespace Enemies.GuardBotFSM{
    public class Patrol : SciAlive {

        float speed;
        float turnSpeed;
        Vector3[] wayPoints;
        int targetWaypointIndex = 0;//start at 0, figure out if constant
        float waitTime;
        bool moveRunning = false;
      	IEnumerator move;
        Transform transform;

        public Patrol(GuardBotFSM stateMachine): base("Patrol", stateMachine){
            wayPoints = new Vector3[stateMachine.pathHolder.childCount];
            speed = stateMachine.speed;
            turnSpeed = stateMachine.turnSpeed;
            waitTime = stateMachine.waitTime;
            transform = stateMachine.GetComponent<Transform>();
            for (int i = 0; i< wayPoints.Length; i++){
			    wayPoints[i] = stateMachine.pathHolder.GetChild(i).position;
		    }
        }// This constructor will call BaseClass.BaseClass()        

        public override void Enter(){//start patrol area
            base.Enter();
            move = FollowPath(wayPoints);
            stateMachine.StartCoroutine(move);
            //StartCoroutine(move);
        }

        public override void UpdateLogic(){
            base.UpdateLogic();

            GuardBotFSM gMachine = (GuardBotFSM)stateMachine;
            
            if(gMachine.TargetsInViewCollider){
                stateMachine.ChangeState(stateMachine.states[typeof(Chase)]);
            }
            
            //if(sawPlayerForTime)
            
        }

        public override void Exit(){//go back to patrol area
            base.Exit();
            //get Base Patrol Path
            stateMachine.StopCoroutine(move);
        }


        // public void IdlePathGen(){
            
        // }

        
        IEnumerator FollowPath(Vector3[] waypoints) {//A coroutine is a function that can suspend its execution (yield) until the given YieldInstruction finishes.
            Vector3 currentVelocity = Vector3.zero;
            
            Vector3 targetWaypoint = waypoints [targetWaypointIndex];
            //transform.LookAt (targetWaypoint);
            yield return stateMachine.StartCoroutine (TurnToFace(targetWaypoint));

            while (true) {
                
                if (transform.position == targetWaypoint){//(transform.position == targetWaypoint) {
                    targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;					
                    targetWaypoint = waypoints [targetWaypointIndex];
                    yield return new WaitForSeconds (waitTime);
                    yield return stateMachine.StartCoroutine (TurnToFace (targetWaypoint));
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

    }
}



