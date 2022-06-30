using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//the brain

public enum Status { idle, walking, crouching, sprinting, sliding, climbing, wallRunning, vaulting, grabbedLedge, climbingLedge, grappling}
public class StatusEvent : UnityEvent<Status> {}//unity event constructor class(the one to go from class to object)

//If you wish to use a generic UnityEvent type you must override the class type.
//https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerControl : MonoBehaviour
{

    //note: headers: for in the unity editor, adds a title for things to control 
    public Status status;
    public StatusEvent onStatusChange;

    public PlayerInfo info;
    PlayerMovement movement;
    PlayerInput playerInput;

    MouseLook mouseLook;

    float[] inputDir = new float[2];

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        mouseLook = GetComponent<MouseLook>();

    }

    void Update()
    {
        
        

    }

    public void ChangeStatus(Status s)
    {
        if (status == s) return;
        status = s;
        if (onStatusChange != null)
            onStatusChange.Invoke(status);
    }
    /*
    public void ChangeStatus(Status s, Func<IKData> call)
    {
        if (status == s) return;
        status = s;
        if (onStatusChange != null)
            onStatusChange.Invoke(status, call);
    }
    */





}


public class PlayerInfo
{
    public float rayDistance;
    public float radius;
    public float height;
    public float halfradius;
    public float halfheight;

    public PlayerInfo(float r, float h)
    {
        radius = r; height = h;
        halfradius = r / 2f; halfheight = h / 2f;
        rayDistance =  halfheight + radius + .175f;
    }
}