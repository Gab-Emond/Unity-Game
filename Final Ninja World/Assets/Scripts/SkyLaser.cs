using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy{

    public class SkyLaser : MonoBehaviour
    {
        public LayerMask mask;
        Vector3 targetPos;
        public float beamRadiusInit;
        public float beamRadiusFinal;
        private float beamRadius;
        public float chargeUpTimeDuration = 10f;
        private float startTime = 0;
        //particle system laser, or mesh renderer

        Vector3 laserCenter = Vector3.zero;
        public float laserSpeed;

        bool hasTarget = false;
        bool canShootTarget;
        bool focused;
        bool shooting;
        bool ready;

        //time.Time: returns clock time, delta time not needed

        private void Update() {
            
            if(ready){
                
                if(hasTarget){
                              
                    if(focused && !shooting){
                        Shooting(targetPos);
                    }
                    else if(!focused){
                        FocusOnTarget(startTime, targetPos);
                    }
                    else{//shooting
                        //
                    }

                    
                }
                else{//lost target
                    if(focused){
                        
                        ReturnToInitial();
                        //change radius and dissipate light

                    }
                }
            
            }
            else{
                
                Recharging();
                //when just shot,
                //recharge timer goes down
                

            }
            //step 1 get target
            //if(remains shootable, still aggrod)

                //step 2 follow/focus

                //step 3 shoot
            //else(stop)

                //dissipate, back to 
        }

        public void GiveTarget(){
            startTime = Time.time;

            //targetPos = 

            hasTarget = true;
        }


        /*IEnumerator Shooting(Vector3 position){
            
			RaycastHit hitInfo;

            if(Physics.Raycast(new Vector3(position.x, 200,position.z), Vector3.down, out hitInfo, 100, mask, QueryTriggerInteraction.Ignore)){

            }

            
            yield return null;

        }*/

            
        void Shooting(Vector3 position){
            RaycastHit hitInfo;
            if(Physics.Raycast(new Vector3(position.x, 200,position.z), Vector3.down, out hitInfo, 100, mask, QueryTriggerInteraction.Ignore)){
                //damage
                //render blast
            }
            else{
                //render blast full length
            }

            
        }

        void FocusOnTarget(float startTime, Vector3 position){
            
            if(beamRadius != beamRadiusFinal){

                float currTime = (Time.time - startTime) / chargeUpTimeDuration;
                beamRadius = Mathf.SmoothStep(beamRadiusInit, beamRadiusFinal, currTime); 
                //SmoothStep(float from, float to, float t); 
                //SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed = Mathf.Infinity, float deltaTime = Time.deltaTime); 
                
                float step = laserSpeed*Time.deltaTime;

                laserCenter = Vector3.MoveTowards(laserCenter, position, step);
                //line renderer, use radius and center of beam



            }
            else{
                focused = true;
            }


        }

        void Recharging(){

        }

        void ReturnToInitial(){
            if(beamRadius != beamRadiusInit){
                float currTime = (Time.time - startTime) / chargeUpTimeDuration;
                beamRadius = Mathf.SmoothStep(beamRadiusInit, beamRadiusFinal, currTime); 
            }
            else{
                ready = true;
            }
        }

    }
}