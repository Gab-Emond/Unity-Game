//mesh generation
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://catlikecoding.com/unity/tutorials/procedural-grid/
//https://www.youtube.com/watch?v=64NblGkAabk
//https://youtu.be/WP-Bm65Q-1Y


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CloudGen : MonoBehaviour
{
    public int xSize, zSize;
    private Vector3[] vertices;
    int[] triangles;
    private Mesh mesh;

    Grid3D grid;
    private void Awake () {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
		Generate();
	}

	private void Generate () {
		vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        grid = new Grid3D();
    
    }
    
    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

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