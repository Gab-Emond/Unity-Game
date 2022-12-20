using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBot : Enemy
{
    
    public Mesh dashMesh;
    public Material dashMaterial;

    Vector3 _target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void Dash(float dashSpeed, Vector3 direction){
        //speed increase
        //particle trail
        //https://www.youtube.com/watch?v=7vvycc2iX6E
    }

    IEnumerator ActivateTrail(float activeTime){
        Graphics.DrawMesh(dashMesh, Vector3.zero, Quaternion.identity, dashMaterial, 0);
        yield return null;
    }
}
