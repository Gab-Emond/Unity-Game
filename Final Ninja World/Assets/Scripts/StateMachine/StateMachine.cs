using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StateMachine {
    public enum EntityState
	{
		Guarding,
        Searching,
        Alerting,
        Incapacitated
	}

    public void Tick(){
        
    }

    public void SetState(){

    }

    private class Transition{

    }

    private Transition GetTransitions(){
        Transition basic = new Transition();
        return basic;
    }

}