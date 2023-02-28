using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pathfinding
{
	
}

public class Node3D : IHeapItem<Node3D> {
 	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;
	public int gridZ;

	public int gCost;//how far from starting node
	public int hCost;//how far from end node
	public Node3D parent;
	int heapIndex;
	
	public Node3D(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _gridZ) {//constructor
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		gridZ = _gridZ;
	}


	public int fCost {
		get {
			return gCost + hCost;//minimise fcost, to minimise distance from start and end 
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node3D nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
