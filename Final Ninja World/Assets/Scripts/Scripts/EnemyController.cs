using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy{
    public class EnemyController : MonoBehaviour
    {
        public float repeatRate = 1f;
        public int repeatNumTimes = 15;
        
        public bool alarmSounded = false;
        public Enemy[] enemies;

        private IEnumerator alarmCoroutine;

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
        
        IEnumerator SoundAlarm(int repeatNumber, float repeatRate) {
            alarmSounded = true;
            int i = 0;
            while(i<repeatNumber) {
                i++;
                //SearchForTarget(param1, param2, ...);
                foreach (Enemy enemy in enemies){
                    enemy.Alert();
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

    }

}