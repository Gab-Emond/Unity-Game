using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.StateMachine;


namespace Enemies.GuardBotFSM{
    public class Alert : SciAlive {
        public Alert(GuardBotFSM stateMachine): base("Alert", stateMachine){}// This constructor will call BaseClass.BaseClass()
        bool finishedAlert;
        float startTime;
        float alertTime;

        public override void Enter(){//set idle targets
            base.Enter();
            finishedAlert = false;
            startTime = Time.time;
            //Invoke("FinishedAlert", 2.0f);
        }

        public override void UpdateLogic(){
            base.UpdateLogic();

            if(startTime-Time.time > alertTime){
                stateMachine.ChangeState(stateMachine.states[typeof(Patrol)]);
            }
        }

        public override void Exit(){//set idle targets
            base.Exit();
        }

        void FinishedAlert(){
            finishedAlert = true;
        }
        
    }
}


