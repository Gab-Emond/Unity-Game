using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMerge : MonoBehaviour
{   

    Transform cubeHolder;
    GameObject[] Cubes;
    Mesh cubeMesh;
    Vector3[] vertices;
    int[] triangles;//vertice travel order, triangle:3, quad:6 (two triangles, opposite directions, 2 shared vertices)
    //(share more vertices on larger mesh)


    //keep meshes in data struct, array? by position and relation to each other

    //awake?
    
    // Start is called before the first frame update
    void Start()
    {
        //cubeholder get child== cubes
        //get instantiated mesh
        cubeMesh = GetComponent<MeshFilter>().mesh;
        vertices = cubeMesh.vertices;
    }

    //if non visible faces, delete face
    //if no visible faces to vertice, delete vertice

    /*
    once merged, use 

    AssetDatabase.CreateAsset( [mesh object here], [path to asset] );
    AssetDatabase.SaveAssets();//to save new mesh

    ////////////////////

     // make sure 
    EditorUtility.ReplacePrefab(template, prefab, ReplacePrefabOptions.ReplaceNameBased);
    // get rid of the temporary object (otherwise it stays over in scene)
    Object.DestroyImmediate(template);


    */



    
}
