using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Utility.StateMachine{
    public class StateMachine : MonoBehaviour
    {
        BaseState currentState;
        // Start is called before the first frame update

        public Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>();//could be hashset, but slightly easier readable access/remove with dictionary key

        void Start()
        {
            currentState = GetInitState();
            //print(currentState.stateName + " is" + (currentState !=null)); 
            if(currentState !=null){
                currentState.Enter();
            }  
        }

        // Update is called once per frame
        void Update()
        {
            if(currentState !=null){
                currentState.UpdateLogic();
            }
        }
        void LateUpdate()
        {
            if(currentState !=null){
                currentState.UpdatePhysics();
            }
        }

        public void ChangeState(BaseState newState){
            currentState.Exit();
            currentState = newState;
            currentState.Enter();
        }

        protected virtual BaseState GetInitState(){//each state machine implemented changes this method to return desired starting state,
            return null;
        }

        /////extra, for untimed behavior
        // public void RunCoroutines(IEnumerator corRun){
        //     StartCoroutine(corRun);
        // }

        // public void EndCoroutines(IEnumerator corRun){
        //     StopCoroutine(corRun);
        // }

        private void OnGUI() {
            GUILayout.BeginArea(new Rect(10f, 10f, 200f, 100f));
            string content = currentState != null? currentState.stateName : "no current state";
            GUILayout.Label($"<color = 'black'><size = 40>{content}</size></color>");
            GUILayout.EndArea();
        }

        //all transitions implied already added, vs jason state machine, has add transition function

    }
}




