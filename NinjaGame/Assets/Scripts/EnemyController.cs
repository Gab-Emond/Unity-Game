using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float repeatRate = 1f;
    public int repeatNumTimes = 15;
    
    public bool alarmSounded = false;
    public Enemy[] enemies;
    public GameObject player;

    private IEnumerator alarmCoroutine;

    public delegate void AlarmDelegate(GameObject target);
    public static event AlarmDelegate alarmAnnouncement;//unity events, allow drag n drop from editor, but slower
    //event alert, static, no direct reference to enemy controller needed

    //event vs direct method call:

    /*Method call = "Do this specific thing"

    Event raise = "If anyone is listening and cares, this thing just happened."
    */

    void Start() {
        //find enemies within a range
        //Init();
    }

    void Init(){
        enemies = FindObjectsOfType<Enemy>();
    }

    public void SoundAlarm(){//async or ienum
        alarmCoroutine = SoundAlarm(repeatNumTimes, repeatRate);
        StartCoroutine(alarmCoroutine);
    }
    
    //use delegate + event system, for doors + enemies etc
    IEnumerator SoundAlarm(int repeatNumber, float repeatRate) {
        alarmSounded = true;
        int i = 0;
        while(i<repeatNumber) {
            i++;

            ///////// if event working, no need for this section

            //SearchForTarget(param1, param2, ...);
            foreach (Enemy enemy in enemies){
                enemy.Alert(player);
            }
            //////////

            //Alert event, subscribe all enemies to it (for all enemies, start by running findobjectoftype enemycontroller to add the script theyll subscribe to)
            if(alarmAnnouncement != null){
                alarmAnnouncement(player);//put player gameobject in alarm
            }
            
            print("alarm: "+ (repeatNumber-i));
            yield return new WaitForSeconds(repeatRate);
        }
        alarmSounded = false;


    }




    //https://answers.unity.com/questions/348680/invokerepeating-with-parameters.html

    // IEnumerator reRunFunction(param1, param2,..., repeatRate) {
    //     while(someCondition) {
    //         SearchForTarget(param1, param2, ...);
    //         yield return new WaitForSeconds(repeatRate);
    //     }
    // }

    //


// A delegate is a type that can contain any method of the same compatible type. Delegates are used to define callback methods and implement event handling. 
// Any method from any accessible class or struct that matches the delegate type can be assigned to the delegate. 

// An action is a premade delegate you can use without the need of making a custom one. 
// Action by itself has no parameters, but you can have those with the generic 



}

