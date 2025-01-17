﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    private Vector2 previous;
    private Vector2 _down;
    
    private int jumpTimer;
    private bool jump;
    
    public Vector2 input
    {
        //getter: function can get this input
        get
        {
            Vector2 i = Vector2.zero;
            i.x = Input.GetAxis("Horizontal");
            i.y = Input.GetAxis("Vertical");
            i *= (i.x != 0.0f && i.y != 0.0f) ? .7071f : 1.0f;//slows diagonally, quick normalization
            return i;
        }
    }

    public bool Jump{
        get{return Input.GetButtonDown("Jump");}
    }

    public bool JumpHeld{
        get{return Input.GetButton("Jump");}
    }

    public bool run
    {
        get { return Input.GetKey(KeyCode.LeftShift); }
    }

    public bool crouch
    {
        get { return Input.GetKeyDown(KeyCode.C); }
    }

    public bool crouching
    {
        get { return Input.GetKey(KeyCode.C); }
    }

    public KeyCode interactKey
    { 
        get { return KeyCode.E; }
    }

    public bool interact
    {
        get { return Input.GetKeyDown(interactKey); }
    }

    public bool reload
    {
        get { return Input.GetKeyDown(KeyCode.R); }
    }

    public bool aim
    {
        get { return Input.GetMouseButtonDown(1); }
    }

    public bool aiming
    {
        get { return Input.GetMouseButton(1); }
    }

    public bool shooting
    {
        get { return Input.GetMouseButton(0); }
    }

    public float mouseScroll
    { 
        get { return Input.GetAxisRaw("Mouse ScrollWheel"); }
    }


}
