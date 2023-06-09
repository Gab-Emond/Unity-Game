using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.StateMachine;

namespace Enemies
{
    public class AgentBotFSM : StateMachine, IParentingCollider, IDamageable {//parenting collider: viewcone

        int health = 3;
        Rigidbody rb;
        public Rigidbody Rb => rb; 

        public HashSet<GameObject> targetsInView = new HashSet<GameObject>();

        //todo: check what could be abstract/virtual classes, many states and properties look duplicate
        //check software engineer conception/modelisation/architecture slides
        //diagram
        //states
        //-patrol (or guard, if no patrol points?)
        //-search
        //-chase
        //-attack (substate of chase?)
        //slash (closerange), dash (long range)

        //movement coroutine; same as others? single "mover", with different speed&anim(legs layer or ik) by state?

        //for "as player" script, movement would count as input for a playerMovement script
        
        //instead of "mouselook", needs ai look, that decides ai rotation
        //robotInput.getKeyDown(shoot)

        //viewer abstract class, viewable tag(s), viewed objects hashset, IParentingCollider inheritance
        //state machine viewer would inherit from it
        //idamageable, as is?


        ////////////////////////IParentingCollider
        public void OnChildTriggerEnter(Collider collider){
            if(collider.tag == "Player"){
                targetsInView.Add(collider.gameObject);
                //targetsInViewCollider = true;
            }
        }

        public void OnChildTriggerExit(Collider collider){
            if(collider.tag == "Player"){
                targetsInView.Remove(collider.gameObject);
                //targetsInViewCollider = false;
            }


        }

        ///////////////////////////////

        ////////////////////////IDamageable

        public void TakeHit(Vector3 damageDir, Vector3 damagePos){
            if(this.health > 1){
                this.health -= 1;
                //todo: changeState? add force? when not dead but hit
            }//if health == 1, goes to zero, dead
            else{
                if(rb){
                    if(rb.isKinematic){
                        EnableRagdoll();
                    }
                    rb.AddForceAtPosition(damageDir, damagePos, ForceMode.Impulse);

                }
                //this.ChangeState(this.states[typeof(Unconscious)]);//todo: deadState
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
