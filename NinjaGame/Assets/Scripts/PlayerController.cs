using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//the brain


public enum Status { idle, walking, crouching, running, sliding, wallRunning, vaulting, grabbedLedge, climbingLedge}


//If you wish to use a generic UnityEvent type you must override the class type.
//https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{

    //note: headers: for in the unity editor, adds a title for things to control 

    public PlayerInfo info;
    PlayerMovement movement;
    PlayerInput playerInput;
    MouseLook mouseLook;

    Status currStatus;
    Grapple playerGrapple;

    [SerializeField] Animator animator;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        //mouseLook = GetComponent<MouseLook>();
        //playerGrapple = GetComponent<Grapple>();
        animator = GetComponentInChildren<Animator>();
        currStatus = Status.idle;
    }

    void Update()
    {
        //stateMachine, if 
        Vector2 inputs = playerInput.input;
        animator.SetFloat("inputX",inputs.x);
        animator.SetFloat("inputZ",inputs.y);

        if(movement.IsGrounded){
            animator.SetBool("grounded",true);
            //if(playerInput.Jump)
            
        }
        else{
            animator.SetBool("grounded",false);//jump vs fall, add transition bool (jump) before going to air

        }
        if(movement.IsCrouching && !movement.IsGrappled){//tech dept, to fix
            animator.SetBool("crouching",true);
        }
        else{
            animator.SetBool("crouching",false);
        }
        // if(movement.IsGrappled){//can be grounded and grappled

        // }

        //while crouching/landing, reduce collider size
    }

    //states
    //grounded (and/or slope)
    //falling?(!grounded)
    //on wall
    //grappled
    //

    //control ik here
    //fall to grounded; center of mass lower; (or animation layer, remake all anims as crouching, weight up-down-up on land)
    //vice versa for jump

    //wall climb ik


    void AnimationHandler(){

    }

    //if within range, melee attack not ranged?

    void ChangeAnimatorBool(string trigger)
    {
        animator.SetBool(trigger,true);
        //player input vector to get dir,
        //velocity? maybe
    }

    public void SetStatus(Status status, float velocity){
        
        if(status!=currStatus){
            animator.SetBool(currStatus.ToString(), false);
            //ChangeAnimatorBool(status.ToString());
            currStatus = status;
            animator.SetBool(currStatus.ToString(), true);

        }
    }






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