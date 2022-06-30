using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwatBot : MonoBehaviour//make child objects with different weapon types?
{

    public enum SwatState
    {
        Wander,
        Chase,
        Attack,
        Out
    }

    public enum WeaponType{
        Explosive,
        Regular,
        Bursts
    }
    //cant fly, can jump?

    //navmesh using

    
    //projectile 
    public GameObject projectilePrefab;
    public Transform projectileSpawn;
    private SwatState _thisState;
    private WeaponType _thisWeapon;
    public WeaponType ThisWeapon    // the Name property
    {
        get => _thisWeapon;
        set => _thisWeapon = value;
    }

    private void Start() {
        if(_thisWeapon == null){//no weapon
            RandWeapon();
        }

    }

    void RandWeapon(){
        //_thisWeapon = (WeaponType)Random.Range(0, Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().Max());//returns a weapon from the amount available
        _thisWeapon = (WeaponType)UnityEngine.Random.Range(0, 2);
    }




}
