using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.StateMachine;


namespace Enemies.GuardBotFSM{
    public class Alert : SciAlive {
        bool finishedAlert;
        float startTime;
        float alertTime;

        public Alert(GuardBotFSM stateMachine): base("Alert", stateMachine){
            alertTime = stateMachine.alertTime;
        }// This constructor will call BaseClass.BaseClass()

        public override void Enter(){//set idle targets
            base.Enter();
            finishedAlert = false;
            startTime = Time.time;
            //Invoke("FinishedAlert", 2.0f);
            //set light color: red
            GuardBotFSM gMachine = (GuardBotFSM)stateMachine;
            gMachine.ChangeLightColor(Color.red,Color.red);

        }

        public override void UpdateLogic(){
            base.UpdateLogic();

            if(Time.time-startTime > alertTime){//alert for a moment
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


