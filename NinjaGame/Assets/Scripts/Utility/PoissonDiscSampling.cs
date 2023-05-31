using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiscSampling {

	public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30) {
		float cellSize = radius/Mathf.Sqrt(2);

		int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x/cellSize), Mathf.CeilToInt(sampleRegionSize.y/cellSize)];
		List<Vector2> points = new List<Vector2>();
		List<Vector2> spawnPoints = new List<Vector2>();

		spawnPoints.Add(sampleRegionSize/2);
		while (spawnPoints.Count > 0) {
			int spawnIndex = Random.Range(0,spawnPoints.Count);
			Vector2 spawnCentre = spawnPoints[spawnIndex];
			bool candidateAccepted = false;

			for (int i = 0; i < numSamplesBeforeRejection; i++)
			{
				float angle = Random.value * Mathf.PI * 2;
				Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2*radius);
				if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid)) {
					points.Add(candidate);
					spawnPoints.Add(candidate);
					grid[(int)(candidate.x/cellSize),(int)(candidate.y/cellSize)] = points.Count;
					candidateAccepted = true;
					break;
				}
			}
			if (!candidateAccepted) {
				spawnPoints.RemoveAt(spawnIndex);
			}

		}

		return points;
	}

	static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid) {
		if (candidate.x >=0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y) {
			int cellX = (int)(candidate.x/cellSize);
			int cellY = (int)(candidate.y/cellSize);
			int searchStartX = Mathf.Max(0,cellX -2);
			int searchEndX = Mathf.Min(cellX+2,grid.GetLength(0)-1);
			int searchStartY = Mathf.Max(0,cellY -2);
			int searchEndY = Mathf.Min(cellY+2,grid.GetLength(1)-1);

			for (int x = searchStartX; x <= searchEndX; x++) {
				for (int y = searchStartY; y <= searchEndY; y++) {
					int pointIndex = grid[x,y]-1;
					if (pointIndex != -1) {
						float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
						if (sqrDst < radius*radius) {
							return false;
						}
					}
				}
			}
			return true;
		}
		return false;
	}

	public static List<Vector3> GeneratePoints3D(float radius, Vector3 sampleRegionSize, int numSamplesBeforeRejection = 30) {
		float cellSize = radius/Mathf.Sqrt(2);

		int[,,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x/cellSize), Mathf.CeilToInt(sampleRegionSize.y/cellSize), Mathf.CeilToInt(sampleRegionSize.z/cellSize)];
		List<Vector3> points = new List<Vector3>();
		List<Vector3> spawnPoints = new List<Vector3>();

		spawnPoints.Add(sampleRegionSize/2);
		while (spawnPoints.Count > 0) {
			int spawnIndex = Random.Range(0,spawnPoints.Count);
			Vector3 spawnCentre = spawnPoints[spawnIndex];
			bool candidateAccepted = false;

			for (int i = 0; i < numSamplesBeforeRejection; i++)
			{
				
				//random dir
				Vector3 candidate = spawnCentre + Random.onUnitSphere*Random.Range(radius,2*radius);
				
				if (IsValid3D(candidate, sampleRegionSize, cellSize, radius, points, grid)) {
					points.Add(candidate);
					spawnPoints.Add(candidate);
					grid[(int)(candidate.x/cellSize),(int)(candidate.y/cellSize),(int)(candidate.z/cellSize)] = points.Count;
					candidateAccepted = true;
					break;
				}
			}
			if (!candidateAccepted) {
				spawnPoints.RemoveAt(spawnIndex);
			}

		}

		return points;
	}

	//add points and grid to variable, to be able to add empty points at start
	public static List<Vector3> GeneratePoints3DExtra(float radius, Vector3 sampleRegionSize,Vector3 sampleRegionCenter, Vector3[] addedPoints, int numSamplesBeforeRejection = 30) {
		float cellSize = radius/Mathf.Sqrt(2);

		int[,,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x/cellSize), Mathf.CeilToInt(sampleRegionSize.y/cellSize), Mathf.CeilToInt(sampleRegionSize.z/cellSize)];
		List<Vector3> points = new List<Vector3>();
		List<Vector3> spawnPoints = new List<Vector3>();

		//starts by adding added points
		// foreach (var aPoint in addedPoints)
		// {
			
		// 	Vector3 relPoint = aPoint - (sampleRegionCenter-sampleRegionSize);
		// 	points.Add(relPoint);
		// 	Debug.Log("point : " + (int)(relPoint.x/cellSize) +", " + (int)(relPoint.y/cellSize) +", " + (int)(relPoint.z/cellSize));
		// 	grid[(int)(relPoint.x/cellSize),(int)(relPoint.y/cellSize),(int)(relPoint.z/cellSize)] = points.Count;
		// 	spawnPoints.Add(relPoint);
		// }


		spawnPoints.Add(sampleRegionSize/2);
		while (spawnPoints.Count > 0) {
			int spawnIndex = Random.Range(0,spawnPoints.Count);
			Vector3 spawnCentre = spawnPoints[spawnIndex];
			bool candidateAccepted = false;

			for (int i = 0; i < numSamplesBeforeRejection; i++)
			{
				
				//random dir
				Vector3 candidate = spawnCentre + Random.onUnitSphere*Random.Range(radius,2*radius);
				
				if (IsValid3D(candidate, sampleRegionSize, cellSize, radius, points, grid)) {
					points.Add(candidate);//(sampleRegionCenter-(sampleRegionSize/2))
					spawnPoints.Add(candidate);
					grid[(int)(candidate.x/cellSize),(int)(candidate.y/cellSize),(int)(candidate.z/cellSize)] = points.Count;
					candidateAccepted = true;
					break;
				}
			}
			if (!candidateAccepted) {
				spawnPoints.RemoveAt(spawnIndex);
			}

		}

		return points;
	}

	static bool IsValid3D(Vector3 candidate, Vector3 sampleRegionSize, float cellSize, float radius, List<Vector3> points, int[,,] grid) {
		if (candidate.x >=0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y 
		&& candidate.z >= 0 && candidate.z < sampleRegionSize.z) {
			int cellX = (int)(candidate.x/cellSize);
			int cellY = (int)(candidate.y/cellSize);
			int cellZ = (int)(candidate.z/cellSize);
			int searchStartX = Mathf.Max(0,cellX -2);
			int searchEndX = Mathf.Min(cellX+2,grid.GetLength(0)-1);
			int searchStartY = Mathf.Max(0,cellY -2);
			int searchEndY = Mathf.Min(cellY+2,grid.GetLength(1)-1);
			int searchStartZ = Mathf.Max(0,cellZ -2);
			int searchEndZ = Mathf.Min(cellZ+2,grid.GetLength(2)-1);


			for (int x = searchStartX; x <= searchEndX; x++) {
				for (int y = searchStartY; y <= searchEndY; y++) {
					for(int z = searchStartZ; z <= searchEndZ; z++){
						int pointIndex = grid[x,y,z]-1;
						if (pointIndex != -1) {
							float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
							if (sqrDst < radius*radius) {
								return false;
							}
						}
					}
				}
			}
			return true;
		}
		return false;
	}


}
