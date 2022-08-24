using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateControl : MonoBehaviour
{
    public GameObject gate;
    Vector3 closedScale;
    Vector3 closedPosition;

    Vector3 openedScale;
    Vector3 openedPosition;
    
    float startTime;
    float doorTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        closedPosition = gate.transform.position;
        closedPosition = gate.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Open(){
        float t = (startTime - Time.time)/doorTime;
        gate.transform.position = Vector3.Lerp(closedPosition,openedPosition,t);        
    }

    void Close(){
        float t = (startTime - Time.time)/doorTime;
        gate.transform.position = Vector3.Lerp(openedPosition,closedPosition,t);
    }

}
