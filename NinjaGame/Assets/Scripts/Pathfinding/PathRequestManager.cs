using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;//to use "actions" // delegate

namespace Pathfinding
{
	
}

//adds priority queue, for several entities to pathfind at the "same" time
public class PathRequestManager : MonoBehaviour {

	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;

	static PathRequestManager instance;//A static variable: same for all instances of a class.(singleton, lets everyone access this single same class)
	Pathfinding3D pathfinding;

	bool isProcessingPath;

	void Awake() {
		instance = this;//single instance of pathrequest
		pathfinding = GetComponent<Pathfinding3D>();
	}

	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {//everyone using this class (static class cannot be instantiated, no "new")
		PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.TryProcessNext();
	}

	void TryProcessNext() {
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success) {//pathRequest3D calls it, and starts the next try; 
		currentPathRequest.callback(path,success);
		isProcessingPath = false;
		TryProcessNext();
	}


	/*A structure type (or struct type) is a value type that can encapsulate data and related functionality*/

	//how the pathrequest must be formulated
	struct PathRequest {
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;

		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback) {
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}

	}
}