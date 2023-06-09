using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.StateMachine;

namespace Enemies
{
    public class Alive : BaseState, IDamageable {//hiarchical base state, for all non dead states
        
        Rigidbody rb;
        public Alive(string name, StateMachine stateMachine): base(name, stateMachine){
            rb=stateMachine.GetComponent<Rigidbody>();
        }// This constructor will call BaseClass.BaseClass()
        public override void Enter(){
            base.Enter();
            DisableRagdoll();
            
        }
        public void TakeHit(Vector3 damageDir, Vector3 damagePos){
            //if(stateMachine.Health > 0){stateMachine.Health -= 1;}
            if(rb){
                if(rb.isKinematic){
                    EnableRagdoll();
                }
                rb.AddForceAtPosition(damageDir, damagePos, ForceMode.Impulse);

            }

            stateMachine.ChangeState(stateMachine.states[typeof(Dead)]);

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

    }

}

