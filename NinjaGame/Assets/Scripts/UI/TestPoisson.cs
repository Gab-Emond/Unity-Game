using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPoisson : MonoBehaviour {

	public float radius = 1;
	public Vector3 regionSize = Vector3.one;
	public int rejectionSamples = 30;
	public float displayRadius =1;

	List<Vector3> points;

	void OnValidate() {
		points = PoissonDiscSampling.GeneratePoints3D(radius, regionSize, rejectionSamples);
		List<Vector3> temp = new List<Vector3>();
		foreach (Vector3 point in points){
			temp.Add(point+transform.position);
		}
		points = temp;

	}

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position+regionSize/2,regionSize);
		if (points != null) {
			foreach (Vector3 point in points) {
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(point, displayRadius);
			}
		}
	}
}
