using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.StateMachine
{
    public class BaseState {

        public string stateName;

        protected StateMachine stateMachine;        

        public BaseState(string _name, StateMachine _stateMachine){
            this.stateName = _name;
            this.stateMachine = _stateMachine;
        }

        public virtual void Enter(){}//things to happen when entering state (start method)
        
        public virtual void UpdateLogic(){}
        
        public virtual void UpdatePhysics(){}

        //public virtual void CheckTransitions(){}

        public virtual void Exit(){}//things to happen when exiting state (cleanup)
        
    }
}

