using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy{

    public enum SatState
    {
    Primed,
    Focusing,
    Shooting,
    Recharging
    }


    public class SkyLaser : MonoBehaviour
    {
   		public Light light;
        private Color originCol;
        private float originIntensity;

        public LayerMask mask;
        public Transform target;

        public float beamRadiusInit;
        public float beamRadiusFinal;
        private float beamRadius;
        public float chargeUpTimeDuration = 100f;
        public float rechargeTimeDuration = 100f;

        private float startTime = 0;
        //particle system laser, or mesh renderer

        Vector3 laserCenter = Vector3.zero;
        public float laserSpeed;

        private SatState currentState = SatState.Primed;
        bool shooting;

        //time.Time: returns clock time, delta time not needed
        private void Start() {
            if(target!=null)
                GiveTarget(target);
            originCol = light.color;
            originIntensity = light.intensity;
        }


        private void Update() {

            switch (currentState)
            {
                case SatState.Primed:{
                    if(target!=null){
                        light.intensity = originIntensity;
                        startTime = Time.time;
                        currentState = SatState.Focusing;
                    }
                    break;

                }
                case SatState.Focusing:{
                    FocusOnTarget(startTime, target.position);
                    if(beamRadius == beamRadiusFinal){
                        startTime = Time.time;
                        currentState = SatState.Shooting;
                    }                        
                    /*else if(!hasTarget){
                        currentState = SatState.Recharging;
                    }*/
                    /*else if(!focusing){

                        StartCoroutine(FocusOnTarget(startTime, target.position));

                    }
                    
                    
                    */

                    break;
                }
                case SatState.Shooting:{
                    //if done shooting, recharge state
                    //StartCoroutine(Shooting(targetPos))
                    Shooting(target.position);
                    if(!shooting){
                        startTime = Time.time;
                        currentState = SatState.Recharging;
                    }
                    

                    break;
                }
                case SatState.Recharging:{
                    Recharging();
                    
                    if(beamRadius == beamRadiusInit){
                        currentState = SatState.Primed;
                    }

                    break;
                }

            }
            //step 1 get target
            //if(remains shootable, still aggrod)

                //step 2 follow/focus

                //step 3 shoot
            //else(stop)

            //dissipate, back to 1
            
        }

        public void GiveTarget(Transform givenTarget){//setter
            target = givenTarget;
        }


        /*IEnumerator Shooting(Vector3 position){
            
			RaycastHit hitInfo;

            if(Physics.Raycast(new Vector3(position.x, 200,position.z), Vector3.down, out hitInfo, 100, mask, QueryTriggerInteraction.Ignore)){
            }

            yield return null;

        }*/

            
        void Shooting(Vector3 position){
            /*RaycastHit hitInfo;
            if(Physics.Raycast(new Vector3(position.x, 200,position.z), Vector3.down, out hitInfo, 100, mask, QueryTriggerInteraction.Ignore)){
                //damage
                //render blast
            }
            else{
                //render blast full length
            }*/
            float currTime = (Time.time - startTime);
            if(currTime<2f){
                shooting = true;
                light.color = Color.red;
            }
            else{
                shooting = false;
                light.color = originCol;
                light.intensity = 0.1f;
            }
                
            

            
        }

        void FocusOnTarget(float startTime, Vector3 position){
            
            /*
            RaycastHit hitInfo;

            if(Physics.Raycast(new Vector3(position.x, 200,position.z), Vector3.down, out hitInfo, 100, mask, QueryTriggerInteraction.Ignore)){
            }
            */


            float currTime = (Time.time - startTime) / chargeUpTimeDuration;
            beamRadius = Mathf.SmoothStep(beamRadiusInit, beamRadiusFinal, currTime); 

            //SmoothStep(float from, float to, float t); 
            //SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed = Mathf.Infinity, float deltaTime = Time.deltaTime); 
            
            float step = laserSpeed*Time.deltaTime;

            laserCenter = Vector3.MoveTowards(laserCenter, position, step);

            transform.position = new Vector3(laserCenter.x, beamRadius,laserCenter.z);
            //line renderer, use radius and center of beam

        }

        void Recharging(){
            
            float currTime = (Time.time - startTime) / rechargeTimeDuration;
            beamRadius = Mathf.SmoothStep(beamRadius, beamRadiusInit, currTime);
            transform.position = new Vector3(laserCenter.x, beamRadius,laserCenter.z);
            
        }

        void ReturnToInitial(){
         
        }

    }
}