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

        float timeToAlert = 4f;
        float spotTime;
        //Transform targetPos;
        GuardBotFSM gMachine;
        public Chase(GuardBotFSM stateMachine): base("Chase", stateMachine){
            gMachine = (GuardBotFSM)stateMachine;
            transform = gMachine.transform;
        }// This constructor will call BaseClass.BaseClass()

        public override void Enter(){//set 
            base.Enter();
            //Debug.Log("spotted something");
            spotTime = 0;
        }

        public override void UpdateLogic(){
            base.UpdateLogic();

            //Color.Lerp(startColor, Color.red, time-startTime);
            if(gMachine.targetsInView.Count==0){
                spotTime-=4f*Time.deltaTime;
            }
            else{
                
                foreach (var possibleTargets in gMachine.targetsInView){
                    Debug.Log(possibleTargets.name);
                    Transform targetForm = possibleTargets.transform;
                    Vector3 rayToTarget = transform.position - targetForm.position;//Vector3.ProjectOnPlane(transform.position - targetPos.position, transform.up);
                    //switchcase, (closest) distance between ray from eyes and target
                    Debug.Log(rayToTarget);
                    switch(rayToTarget.sqrMagnitude){
                        case(<4f):
                            spotTime+=8f*Time.deltaTime;
                        //fast notice 
                        //     //red
                        //     //alert
                            break;
                        case(<16f):
                            spotTime+=4f*Time.deltaTime;
                        //medium notice
                        //     //turn and go red
                            break;
                        case(<32f):
                            spotTime+=3f*Time.deltaTime;
                        //slow notice
                        //     //turn
                            break;
                        default:
                            spotTime+=2f*Time.deltaTime;
                            break;

                    }
                }
            }
            

            

            if(spotTime>=timeToAlert){
                stateMachine.ChangeState(stateMachine.states[typeof(Alert)]);
            }
            else if(spotTime<=0){
                stateMachine.ChangeState(stateMachine.states[typeof(Patrol)]);
            }

            // bool targetOutOfSightForTime = false;
            // bool atLastSeenNode = false;
            // if(targetOutOfSightForTime && atLastSeenNode){
            //     stateMachine.ChangeState(stateMachine.states[typeof(Patrol)]);
            // }
            

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



