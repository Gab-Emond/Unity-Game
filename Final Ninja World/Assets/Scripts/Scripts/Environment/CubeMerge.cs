using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMerge : MonoBehaviour
{   
    Mesh cubeMesh;
    Vector3[] vertices;
    private void Awake() {
        //get instantiated mesh
        cubeMesh = GetComponent<MeshFilter>().mesh;
        vertices = cubeMesh.vertices;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
