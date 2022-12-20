using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.StateMachine;

namespace Enemies.GuardBotFSM{
    public class Chase : SciAlive {
        
        int targetWaypointIndex = 0;
        Vector3[] wayPoints;
        Transform transform;
        float turnSpeed;

        Transform targetPos;

        public Chase(GuardBotFSM stateMachine): base("Chase", stateMachine){}// This constructor will call BaseClass.BaseClass()

        public override void Enter(){//set 
            base.Enter();
        }

        public override void UpdateLogic(){
            base.UpdateLogic();
            bool targetInSightForTime = false;

            //Color.Lerp(startColor, Color.red, time-startTime);

            Vector3 rayToTarget = Vector3.ProjectOnPlane(transform.position - targetPos.position, transform.forward);
            //switchcase, (closest) distance between ray from eyes and target

            switch(rayToTarget.sqrMagnitude){
                // case(<1f):
                //     //red
                //     //alert
                //     break;
                // case(<4f):
                //     //turn and go red
                //     break;
                // case(<9f):
                //     //turn
                //     break;

            }

            if(targetInSightForTime){
                stateMachine.ChangeState(stateMachine.states[typeof(Alert)]);
            }

            bool targetOutOfSightForTime = false;
            bool atLastSeenNode = false;
            if(targetOutOfSightForTime && atLastSeenNode){
                stateMachine.ChangeState(stateMachine.states[typeof(Patrol)]);
            }
            

        }
        

        IEnumerator FollowPath(Vector3[] waypoints) {//move faster, possible smooth curve instead of hard stop
            Vector3 currentVelocity = Vector3.zero;
            
            Vector3 targetWaypoint = waypoints [targetWaypointIndex];
            //transform.LookAt (targetWaypoint);
            yield return stateMachine.StartCoroutine (TurnToFace(targetWaypoint));

            while (true) {
                
                if (transform.position == targetWaypoint){
                    targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;					
                    targetWaypoint = waypoints [targetWaypointIndex];
                    //no wait time
                    yield return stateMachine.StartCoroutine (TurnToFace (targetWaypoint));
                }
                else if((transform.position-targetWaypoint).sqrMagnitude <= 0.125f){
                    transform.position = Vector3.Lerp(transform.position, targetWaypoint, Vector3.SqrMagnitude(currentVelocity) * Time.deltaTime);
                }
                yield return null;
            }
        }

        IEnumerator TurnToFace(Vector3 lookTarget) {//rotate faster
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



