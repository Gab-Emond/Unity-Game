using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPoisson : MonoBehaviour {

	public float radius = 1;
	public Vector3 regionSize = Vector3.one;
	public int rejectionSamples = 30;
	public float displayRadius =1;
	Vector3 p_0 = new Vector3(-3,0,4);
	float r_0 = 1;
	float r_1  = 3;
	Vector3 p_1 = new Vector3(5,0,2);
	Vector3[] l_P = new Vector3[2];

	List<Vector3> points;

	void OnValidate() {
		// points = PoissonDiscSampling.GeneratePoints3D(radius, regionSize, rejectionSamples);
		// List<Vector3> temp = new List<Vector3>();
		// foreach (Vector3 point in points){
		// 	temp.Add(point+transform.position);
		// }
		// points = temp;

		
		// int i = 0;
		// Vector3[] temp = new Vector3[2];

		// foreach (var item in Utility.Math.MathUtility.LineBetweenCircles(new Vector2(p_0.x,p_0.z), r_0, new Vector2(p_1.x,p_1.z), r_1,-1))
		// {
		// 	temp[i] = new Vector3(item.x,0,item.y);
		// 	i++;
		// }
		// l_P = temp;
		
		// print("magnitude 0: "+(p_0-l_P[0]).magnitude);
		// print("magnitude 1: "+(p_1-l_P[1]).magnitude);


		//print(Utility.Math.MathUtility.DistancePointLine2d(v_0,v_dir,v_point));
		
		// foreach(var pos in Utility.Math.MathUtility.CirclesIntersect(p_0,2,p_1,3)){
		// 	print(pos);
		// }
		

	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(p_0,r_0);
		Gizmos.DrawSphere(p_1,r_1);
		if(l_P.Length==2){
			Gizmos.DrawLine(l_P[0],l_P[1]);
		}
		Gizmos.DrawWireCube(transform.position+regionSize/2,regionSize);
		if (points != null) {
			foreach (Vector3 point in points) {
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(point, displayRadius);
			}
		}
	}
}
