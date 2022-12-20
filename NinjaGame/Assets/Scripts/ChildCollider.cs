using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChildCollider : MonoBehaviour {
    private IParentingCollider parentCollider;

    void Start()
    {
        parentCollider = transform.parent.GetComponent<IParentingCollider>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        parentCollider.OnChildTriggerEnter(other); // pass the own collider and the one we've hit
    }
    
    private void OnTriggerExit(Collider other) {
        parentCollider.OnChildTriggerExit(other);
    }
}
