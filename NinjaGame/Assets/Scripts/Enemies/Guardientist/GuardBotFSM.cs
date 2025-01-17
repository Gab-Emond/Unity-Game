using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.StateMachine;


namespace Enemies.GuardBotFSM{
    public class GuardBotFSM : StateMachine, IParentingCollider, IDamageable  {
        public LayerMask viewMask;

      	//public Light spotlight;

        public GameObject spotLight;
        public Material spotlightMaterial;
        public Material eyeMaterial;
        Color originEyeColor;
        Color originSpotLightColor;


        Rigidbody rb;
        public Rigidbody Rb => rb; 
        public Transform pathHolder;
        public float speed;
        public float turnSpeed;
        public float waitTime;
        public float wakeUpTime;
        public float alertTime = 5;
        bool canSeePlayer = false;
        // bool targetsInViewCollider = false;
        // public bool TargetsInViewCollider => targetsInViewCollider; 
        public HashSet<GameObject> targetsInView = new HashSet<GameObject>();
        
        //initState = setInitState;

        //state machine has every class initialized

        //public Dictionary<string, State> states = new Dictionary<string, State>();//can't get element directly in hashset, setting element as own key in Dict gives similar result


        private void Awake() {
            
            rb=this.GetComponent<Rigidbody>();
            /*
            foreach (State state in folder)
            {
                states.Add((string) statename,state);
            }
            */

            //ERROR: cannot instantiate monobehavior as "new"

            Patrol patrol = new Patrol(this);
            states.Add(typeof(Patrol), patrol);
            Chase chase = new Chase(this);
            states.Add(typeof(Chase), chase);
            Alert alert = new Alert(this);
            states.Add(typeof(Alert), alert);
            Unconscious unconscious = new Unconscious(this);
            states.Add(typeof(Unconscious), unconscious);

            // foreach (var kvp in states) {
            //     Debug.Log( kvp.Value.stateName);//kvp.Key+" "+
            // }
            
            spotlightMaterial = spotLight.GetComponent<MeshRenderer>().materials[1];//necessary, to get instance of this object only
            originEyeColor = eyeMaterial.color;
            originSpotLightColor = spotlightMaterial.color;
            
        }
        
        //start calls init state
        protected override BaseState GetInitState(){//each state machine implemented changes this method to return desired starting state,
            return this.states[typeof(Patrol)];
        }


        ////////////////////////IDamageable

        public void TakeHit(Vector3 damageDir, Vector3 damagePos){
            //if(stateMachine.Health > 0){stateMachine.Health -= 1;}
            if(rb){//check issue if remove changestate from rb condition; possibly change rb check into trycatch error
                if(rb.isKinematic){
                    EnableRagdoll();
                }
                rb.AddForceAtPosition(damageDir, damagePos, ForceMode.Impulse);

                this.ChangeState(this.states[typeof(Unconscious)]);
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

        ////////////////////////error: fix, check line of sight
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
        public void ChangeLightColor(Color _spotColor, Color _eyeColor){//change eyes+change spotlight(one? two methods)
            //todo: use  MaterialPropertyBlock.SetColor
            spotlightMaterial.SetColor("_Color", _spotColor);

            eyeMaterial.SetColor("_Color", _eyeColor);
        }

        public void SetOriginColor(){
            spotlightMaterial.SetColor("_Color", originSpotLightColor);
            eyeMaterial.SetColor("_Color", originEyeColor);
        }

        bool CanSeeTarget(Transform target) {//todo, see if way to invert ray
            RaycastHit hitInfo;
            Vector3 vectResult;
            bool canSee = false;
            //if (in range)
                //if (in angle)
            Vector3 dirToPlayer = (target.position - transform.position);

            vectResult = Vector3.Cross(Vector3.up, dirToPlayer);
            if(!Physics.Linecast(transform.position, target.position,viewMask)){//if nothing is blocking the player (center)
                canSee= true;
                //Debug.DrawLine (transform.position, target.position, Color.red);
            }
            //Debug.DrawLine (target.position-vectResult, target.position, Color.yellow);
            
            else if(Physics.Linecast(target.position-vectResult, target.position, out hitInfo)){
                
                if(!Physics.Linecast(transform.position, hitInfo.point,viewMask)){//if nothing is blocking the player (left)
                    canSee = true;
                    //Debug.DrawLine(transform.position, hitInfo.point, Color.green);
                }
            }
            else if(Physics.Linecast(target.position+vectResult, target.position, out hitInfo)){
                
                if(!Physics.Linecast(transform.position, hitInfo.point,viewMask)){//if nothing is blocking the player (right)
                    canSee = true;
                    //Debug.DrawLine(transform.position, hitInfo.point, Color.green);
                }
            }
            return canSee;
            
        }

        
    }
}

