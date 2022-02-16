using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {//designates nodes to be the type of class usable by heap
	
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;//how far from starting node
	public int hCost;//how far from end node
	public Node parent;
	int heapIndex;
	
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
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

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}