using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemies.SwatBotFSM
{
    public class Chase : Alive {
        public Chase(SwatBotFSM stateMachine): base("Chase", stateMachine){}// This constructor will call BaseClass.BaseClass()

        public override void Enter(){//set 
            base.Enter();
        }

        public override void UpdateLogic(){
            base.UpdateLogic();

            if(true)
            stateMachine.ChangeState(stateMachine.states[typeof(Attack)]);


            bool targetOutOfSightForTime = false;
            bool atLastSeenNode = false;
            if(targetOutOfSightForTime && atLastSeenNode){
                stateMachine.ChangeState(stateMachine.states[typeof(Idle)]);
            }
            

        }
        
    }


}
