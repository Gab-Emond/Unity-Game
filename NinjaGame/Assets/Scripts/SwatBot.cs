using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwatBot : MonoBehaviour//make child objects with different weapon types?
{

    //cant fly, can jump?

    //navmesh using

    
    //projectile 
    public GameObject projectilePrefab;
    public Transform projectileSpawn;
    private WeaponDataSO thisWeaponData;
    public WeaponDataSO ThisWeaponData    // the Name property
    {
        get => thisWeaponData;
        set => thisWeaponData = value;
    }

    bool aimRunning;
    bool lockOn;
    GameObject _target;
    Vector3 _direction;
    Vector3 prevPos;
    Vector3 currPos;
    int aimCall;
    float _projectileSpeed;
    IEnumerator aiming;

    private void Start() {

        if(thisWeaponData == null){//no weapon
            RandWeapon();
        }

    }

    private void Update() {
        
    }

    void RandWeapon(){
        //_thisWeapon = (WeaponType)Random.Range(0, Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().Max());//returns a weapon from the amount available
        //_thisWeapon = (WeaponType)UnityEngine.Random.Range(0, 2);
    }

    IEnumerator Aim(Vector3 targetPos) {
        aimRunning = true;
        lockOn = false;
        double tRes1 = -1d,tRes2 = -1d; 
        float prevTime;
        aimCall = 0;
        //while (tRes1 <=0 && tRes2 <=0){}
        
        //print("aim");
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);
        prevPos = _target.transform.position;
        prevTime = Time.time;
        yield return new WaitForSeconds(0.05f);//breaks out the loop
        
        
        currPos = _target.transform.position;
        Vector3 targetVelocity = (currPos - prevPos)/(Time.time-prevTime);
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        //print("targetVel: " + (targetVelocity));

        Vector3 _relPos = (_target.transform.position-projectileSpawn.position);
        
        double a = (double)(targetVelocity.sqrMagnitude- _projectileSpeed*_projectileSpeed); 
        double b = (double)(Vector3.Dot(_relPos, targetVelocity));
        double c = (double)(_relPos).sqrMagnitude;
        
        Fts.SolveQuadric(a, b, c, out tRes1, out tRes2);
        //print(tRes1+" and"+tRes2);
        

        if (tRes1 > 0f){
            _direction = targetVelocity + (_relPos/(float)tRes1);
            //print(_direction);

            lockOn = true;
        }
        else if(tRes2 > 0f){
            _direction = targetVelocity + (_relPos/(float)tRes2);
            //print(_direction);
            lockOn = true;
        }
                
            
        
        
        yield return null;
    }



}
