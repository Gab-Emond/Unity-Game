using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.StateMachine;

namespace Enemies
{
    public class Dead : BaseState, IDamageable {//hiarchical base state, for all dead states
        
        Rigidbody rb;
        GameObject gO;
        float cleanupTime = 5f;//destroys in 5 seconds


        public Dead(string name,StateMachine stateMachine): base(name, stateMachine){
            rb = stateMachine.GetComponent<Rigidbody>();
            gO = stateMachine.GetComponent<GameObject>();
        }// This constructor will call BaseClass.BaseClass()

        public override void Enter(){
            GameObject.Destroy(gO, cleanupTime);
        }
        //doesnt override anything other than logic
        public override void UpdateLogic(){
            base.UpdateLogic();
        }
        
        public void TakeHit(Vector3 damageDir, Vector3 damagePos){//kinematic disabled in alive state
            if(rb){
                rb.AddForceAtPosition(damageDir, damagePos, ForceMode.Impulse);
            }
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



