using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//create parent gates, inherit control, plus change opening types
public class GateControl : MonoBehaviour
{
    public GameObject gate;
    Vector3 closedScale;
    Vector3 closedPosition;

    public Transform closedNode;

    Vector3 openedScale;
    Vector3 openedPosition;
    
    public Transform openedNode;
    
    //float startTime;
    public float doorTime;
    // Start is called before the first frame update
    float doorProgress;
    //public Collider collider;

    private int _count;

    void Start()
    {   
        _count = 0;
        doorProgress = 0;
        closedPosition = closedNode.position;
        closedScale = closedNode.localScale;
        openedPosition = openedNode.position;
        openedScale = openedNode.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isEmpty() && doorProgress<1){//if should open
            
            doorProgress = Mathf.Min(1, doorProgress+Time.deltaTime/doorTime);
            gate.transform.position = Vector3.Lerp(closedPosition, openedPosition, doorProgress);
        }
        else if((isEmpty() && doorProgress>0)){//if should close
            
            doorProgress = Mathf.Max(0, doorProgress-Time.deltaTime/doorTime);
            gate.transform.position = Vector3.Lerp(closedPosition, openedPosition, doorProgress);
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        _count++;
    }

    private void OnTriggerExit(Collider other) {
        _count--;
    }

    bool isEmpty() {
       return _count == 0;
    }


}
