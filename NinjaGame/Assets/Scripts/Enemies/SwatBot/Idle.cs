using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies.SwatBotFSM
{
    public class Idle : Alive {
    public Idle(SwatBotFSM stateMachine): base("Idle", stateMachine){}// This constructor will call BaseClass.BaseClass()

    public override void Enter(){//set idle targets
        base.Enter();

    }

    public override void UpdateLogic(){
        base.UpdateLogic();

        stateMachine.ChangeState(stateMachine.states[typeof(Chase)]);
    }

    public void IdlePathGen(){
        
    }
    
}

}


