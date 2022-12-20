using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddedVelocity : MonoBehaviour {
    
    public Vector3 addedForce;
    Rigidbody rB;
    private void Start() {
        rB = GetComponent<Rigidbody>();
    }
    private void FixedUpdate() {
        if(rB!=null)
            rB.velocity += addedForce;    
    }

}