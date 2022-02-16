using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicle : MonoBehaviour
{
    Transform cachedTransform;

    public float speed;
    private Vector3 velocity;
    //Boid?
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cachedTransform.position += velocity * Time.deltaTime;
    }
}