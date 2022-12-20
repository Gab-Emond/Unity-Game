using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.StateMachine;


namespace Enemies.GuardBotFSM{
    public class Unconscious : BaseState {//seperate from dead

        Rigidbody rb;
        Transform transform;
        float wakeUpTime;
        float timeSinceLastHit;
        bool gettingUp;
        bool standing;
        float angularDragInit;

      	IEnumerator getUp;

        
        public Unconscious(GuardBotFSM stateMachine): base("Unconscious", stateMachine){
            rb = stateMachine.Rb;
            transform = stateMachine.GetComponent<Transform>();
            wakeUpTime = stateMachine.wakeUpTime;
            angularDragInit = rb.angularDrag;
        }// This constructor will call BaseClass.BaseClass()

        public override void Enter(){
            gettingUp = false;
            standing = false;
            timeSinceLastHit = Time.time;
            getUp = StandUp();
            
            //Color = Color.black;//texture offset?

        }
        public override void UpdateLogic(){
            base.UpdateLogic();
            
            if(Time.time-timeSinceLastHit > wakeUpTime){
                //get BackUp
                if(!gettingUp){
                    //Debug.Log("rigidbody exists: " + (rb !=null));
                    stateMachine.StartCoroutine(getUp);
                    //StartCoroutine(getUp);
					gettingUp = true;
                }
                else if(standing){
                    stateMachine.ChangeState(stateMachine.states[typeof(Patrol)]);

                }
            }

        }

        public override void Exit(){
            rb.angularDrag = angularDragInit;
            gettingUp = false;
            standing = true;
            stateMachine.StopCoroutine(getUp);
            //StopCoroutine(getUp);
            //DisableRagdoll();//error possible
            
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
            //float standUpTime = 0.25f;
            Vector3 lookInit = transform.forward;
            float startTime = Time.time;
            Vector3 dirToLookTarget = Vector3.forward;//(lookTarget - transform.position).normalized;
            rb.angularDrag = 10f;
            //while more than 1 degree off being straight up
            while (Vector3.Angle(transform.up, Vector3.up) > 1f ) {//(transform.up != Vector3.up){
                
                rb.AddTorque(Vector3.Cross(transform.up,Vector3.up)*75f, ForceMode.Force);

                yield return null;
            }
            rb.angularDrag = angularDragInit;
            gettingUp = false;
            standing = true;
            yield return null;
        }
        
    }
}



