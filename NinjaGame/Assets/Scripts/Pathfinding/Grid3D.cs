using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	
}
public class Grid3D : MonoBehaviour
{
 
	public Transform player;
	public bool showGrid = false;
	public LayerMask unwalkableMask;
	public Vector3 gridWorldSize;
	public float nodeRadius;
	Node3D[,,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY, gridSizeZ;

	void Awake() {
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		gridSizeZ = Mathf.RoundToInt(gridWorldSize.z/nodeDiameter);
		CreateGrid();//places walkable and unwalkable node at empty object grid
	}

	/*
	public Grid3D(Vector3 _worldPos, int _gridX, int _gridY, int _gridZ) {//constructor works with methods inside
		transform.position = _worldPos;
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		gridSizeZ = Mathf.RoundToInt(gridWorldSize.z/nodeDiameter);
		CreateGrid();//places walkable and unwalkable node at empty object grid

	}
	*/
	public int MaxSize {
		get {
			return gridSizeX * gridSizeY * gridSizeZ;
		}
	}

	void CreateGrid() {
		grid = new Node3D[gridSizeX, gridSizeY, gridSizeZ];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.z/2 - Vector3.up * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				for(int z = 0; z < gridSizeZ; z ++) {
					Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius)+ Vector3.up* (y * nodeDiameter + nodeRadius);
					bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
					grid[x,y,z] = new Node3D(walkable,worldPoint, x, y, z);
				}
			}
		}
		
	}

	void SimplifyGrid(){

	}
	public List<Node3D> GetNeighbours(Node3D node) {
		List<Node3D> neighbours = new List<Node3D>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				for(int z = -1; z <= 1; z++) {
					if (x == 0 && y == 0 && z == 0)
						continue;

					int checkX = node.gridX + x;
					int checkY = node.gridY + y;
					int checkZ = node.gridZ + z;

					if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ) {
						neighbours.Add(grid[checkX,checkY,checkZ]);
					}
				}
			}
		}

		return neighbours;
	}
	


	/////////////////////Error: only works if grid at world center
	public Node3D NodeFromWorldPoint(Vector3 worldPosition){
		float percentX = worldPosition.x/gridWorldSize.x + 0.5f;//(worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x; to stay in the positives (50+50)/100=1 to (-50+50)/100=0
		float percentY = worldPosition.y/gridWorldSize.y + 0.5f;//(worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		float percentZ = worldPosition.z/gridWorldSize.z + 0.5f;

		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);//if outside of grid, no error
		percentZ = Mathf.Clamp01(percentZ);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		int z = Mathf.RoundToInt((gridSizeZ-1) * percentZ);
		
		return grid[x,y,z];
	}
		
	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,gridWorldSize.y,gridWorldSize.z));

		/*if (grid != null && showGrid) {
			
			Node3D playerNode = NodeFromWorldPoint(player.position);//indicates player pos
			
			foreach (Node3D n in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				if(playerNode == n){
					Gizmos.color = Color.cyan;
				}
				Gizmos.DrawSphere(n.worldPosition, (nodeRadius-.125f));
			}
		}*/
		/*
		if(Application.isPlaying)
		{
			Node3D playerNode = NodeFromWorldPoint(player.position);
			Gizmos.DrawSphere(playerNode.worldPosition, (nodeRadius-.125f));
		}
		

		*/	
	}
	

}
//TODO

//limit computation by changing grid size, according to pathfinder distance from target

//gridworldsize change at first
