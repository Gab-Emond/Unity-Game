using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.StateMachine;

namespace Enemies.GuardBotFSM
{
    public class SciAlive : BaseState{//hiarchical base state, for all non dead states
        
        Rigidbody rb;
        public SciAlive(string name, StateMachine stateMachine): base(name, stateMachine){
            rb=stateMachine.GetComponent<Rigidbody>();
        }// This constructor will call BaseClass.BaseClass()
        public override void Enter(){
            base.Enter();
            DisableRagdoll();
            
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

