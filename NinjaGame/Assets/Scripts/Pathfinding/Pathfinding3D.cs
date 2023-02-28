using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Utility.Math;

namespace Pathfinding
{
	
}
public class Pathfinding3D : MonoBehaviour
{
   PathRequestManager requestManager;
	Grid3D grid;
	
	void Awake() {
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid3D>();// new Grid3D( attributes)
	}
	
	
	public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
		StartCoroutine(FindPath(startPos,targetPos));
	}
	
	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;
		
		Node3D startNode = grid.NodeFromWorldPoint(startPos);
		Node3D targetNode = grid.NodeFromWorldPoint(targetPos);
		//Debug.Log("X: "+ targetNode.gridX + ", Z: "+targetNode.gridZ);
		
		
		if (startNode.walkable && targetNode.walkable) {
			Heap<Node3D> openSet = new Heap<Node3D>(grid.MaxSize);//quick to add/remove
			HashSet<Node3D> closedSet = new HashSet<Node3D>();//quick to see if contains, hashed position per element (set==hashtable)
			openSet.Add(startNode);
			
			while (openSet.Count > 0) {
				Node3D currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}
				
				foreach (Node3D neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closedSet.Contains(neighbour)) {
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}
		yield return null;

		//fct for simplyfing +++++ to create vector3 path from linked node list
		if (pathSuccess) {
			waypoints = RetracePath(startNode,targetNode);
		}
		requestManager.FinishedProcessingPath(waypoints,pathSuccess);
		
	}
	
	Vector3[] RetracePath(Node3D startNode, Node3D endNode) {
		List<Node3D> path = new List<Node3D>();
		Node3D currentNode = endNode;
		
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		
		return waypoints;
		
	}
	
	Vector3[] SimplifyPath(List<Node3D> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector3 directionOld = Vector3.zero;
		
		for (int i = 1; i < path.Count; i ++) {
			Vector3 directionNew = new Vector3(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY,path[i-1].gridZ - path[i].gridZ);
			if (directionNew != directionOld) {
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}
	
	int GetDistance(Node3D nodeA, Node3D nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);
		/*
		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY) ;
		return 14*dstX + 10 * (dstY-dstX);
		*/

		int[] distVect = new int[3]{10,14,17};
		int[] diff = differencesOfValues(dstX,dstY,dstZ);
		
		return MathUtility.DotProdInArray(distVect, diff);

	}
	
	int[] differencesOfValues(int a, int b, int c)	{
		

		int[] arr = new int[3]{a,b,c};
		Array.Sort(arr);//2 lazy to code
		//Debug.Log("" + arr[0]+","+arr[1]+","+arr[2]);

		int min = arr[0];
		int mid = arr[1];
		int max = arr[2];


		return new int[3]{max - mid , mid-min, min};

	}
	
}
