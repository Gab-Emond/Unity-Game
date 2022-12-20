using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies.SwatBotFSM
{
    public class Attack : Alive {

        GameObject _target;
        public Attack(SwatBotFSM swatFSM): base("Attack", swatFSM){}// This constructor will call BaseClass.BaseClass()

        IEnumerator aiming;
        public override void Enter(){//set idle targets
            base.Enter();
            // aiming = Aim(_target.transform.position);
            // StartCoroutine(aiming);
        }

        public override void UpdateLogic(){
            base.UpdateLogic();

            bool targetOutOfSight = false;
            // if(checkedLastSeenPlace){
            //     if(targetOutOfSightForTime){

            //     }
            // }
            
            


            if(targetOutOfSight){
                stateMachine.ChangeState(stateMachine.states[typeof(Chase)]);
            }
        }

        /*
        IEnumerator Aim(Vector3 targetPos) {
            aimrunning = true;
            lockOn = false;
            double tRes1 = -1d,tRes2 = -1d; 
            float prevTime;
            aimCall = 0;
            //while (tRes1 <=0 && tRes2 <=0){}
            
            //print("aim");
            //Debug.Log("Started Coroutine at timestamp : " + Time.time);
            prevPos = _target.transform.position;
            prevTime = Time.time;
            yield return new WaitForSeconds(0.05f);//breaks out the loop
            
            
            currPos = _target.transform.position;
            Vector3 targetVelocity = (currPos - prevPos)/(Time.time-prevTime);
            //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
            //print("targetVel: " + (targetVelocity));

            Vector3 _relPos = (_target.transform.position-projectileSpawn.position);
            
            double a = (double)(targetVelocity.sqrMagnitude- _projectileSpeed*_projectileSpeed); 
            double b = (double)(Vector3.Dot(_relPos, targetVelocity));
            double c = (double)(_relPos).sqrMagnitude;
            
            Fts.SolveQuadric(a, b, c, out tRes1, out tRes2);
            //print(tRes1+" and"+tRes2);
            

            if (tRes1 > 0f){
                _direction = targetVelocity + (_relPos/(float)tRes1);
                //print(_direction);

                lockOn = true;
            }
            else if(tRes2 > 0f){
                _direction = targetVelocity + (_relPos/(float)tRes2);
                //print(_direction);
                lockOn = true;
            }
                    
                
            
            
            yield return null;
        }
        */
        



    }


}
