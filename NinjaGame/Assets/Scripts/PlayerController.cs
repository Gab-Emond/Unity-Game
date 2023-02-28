using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//the brain


public enum Status { idle, walking, crouching, sprinting, sliding, wallRunning, vaulting, grabbedLedge, climbingLedge, surfaceSwimming, underwaterSwimming }


//If you wish to use a generic UnityEvent type you must override the class type.
//https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerControl : MonoBehaviour
{

    //note: headers: for in the unity editor, adds a title for things to control 

    public PlayerInfo info;
    PlayerMovement movement;
    PlayerInput playerInput;
    MouseLook mouseLook;

    Grapple playerGrapple;

    Animator animator;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        mouseLook = GetComponent<MouseLook>();
        playerGrapple = GetComponent<Grapple>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //stateMachine, if 
        

    }




    void AnimationHandler(){

    }

    //if within range, melee attack not ranged?








    //on hit, lower player health
    //need single instance of player info
}




public class PlayerInfo
{
    public float rayDistance;
    public float radius;
    public float height;
    public float halfradius;
    public float halfheight;
    public float mass;

    public int health;
    public float stamina;

    //Dictionary<string, GameObject>;

    public PlayerInfo(float r, float h)
    {
        radius = r; height = h;
        halfradius = r / 2f; halfheight = h / 2f;
        rayDistance =  halfheight + radius + .175f;
    }
}