using System.Collections;
using System.Collections.Generic;
using Utility.StateMachine;
using UnityEngine;

namespace Enemies.SwatBotFSM
{
    public class SwatBotFSM : SightLineFSM {
        //initState = setInitState;

        //state machine has every class initialized

        //public Dictionary<string, State> states = new Dictionary<string, State>();//can't get element directly in hashset, setting element as own key in Dict gives similar result


        private void Awake() {

            /*
            foreach (State state in folder)
            {
                states.Add((string) statename,state);
            }
            */
            Idle idle = new Idle(this);
            states.Add(typeof(Idle), idle);
            Attack attack = new Attack(this);
            states.Add(typeof(Attack), attack);
            Chase chase = new Chase(this);
            states.Add(typeof(Chase), chase);

        }
        //start calls init state
        protected override BaseState GetInitState(){//each state machine implemented changes this method to return desired starting state,
        
            return states[typeof(Idle)];
        }

        
    }
}
