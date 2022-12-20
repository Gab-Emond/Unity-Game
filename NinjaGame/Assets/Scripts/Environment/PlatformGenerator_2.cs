using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlatformGenerator_2 : MonoBehaviour
{

    public GameObject platformPrefab;

    bool [][][] spaceMatrix;//if false in [i][j][k], empty, if true, full

    
    //path first sparsing second idea

    Vector3[] pathOfNodes;

    public Transform startNode;
    public Transform endNode;

    float maxSlope = 1.732f;//slightly less than 60 deg
    //maxSope*deltaR = maxDeltaY //for linked path()
    //from point: make sure theta and phi to next point not too similar to "" to previous point
    //theta = baseTheta+gaussianRandom()
    //phi = basePhi+gaussianRandom


    void Start()
    {

        StartCoroutine(GenerateRandomPath(startNode.position, endNode.position));
    }


    IEnumerator GeneratePlatforms(Vector3[] nodes){
        print("startEnum");
        NodesToPlatformMesh(nodes);
        print("endEnum");
        return null;
    }

    IEnumerator GenerateRandomPath(Vector3 _start, Vector3 _end){
        print("startEnum");
        pathOfNodes = RandomEnclosedPathGenerator(_start, _end);
        print("endEnum");
        return null;
    }


    Vector3[] RandomEnclosedPathGenerator(Vector3 start, Vector3 end){

        //somewhat like dfs random

        //fill space with nodes

        //random with minimum distance: poisson disc sampling

        //select node randomly; if node possible, add to path; remove impossible nodes


        //for node selected, 
        //if removing node, disconnects a neighbouring node from tree, 
        
        //dont remove node

        //for node_n in node_curr.neighbour
        //

        return new Vector3[1];
    }

    float PointSlope(Vector3 n_0, Vector3 n_1){//check if too steep
        Vector3 line_1 = n_1-n_0;
        float thetaInv = Vector3.Angle(line_1,Vector3.up);//theta = 90-thetaInv
        return thetaInv* Mathf.Rad2Deg;//<30f;
    }

    bool SmallAngleBetweenPoints(Vector3 n_0, Vector3 n_1, Vector3 n_2){ //checks n_0/n_1 not getting blocked by n_1/n_2
        //polar coords, check that theta not too similar
        //project on horizontal plane and check angle
        Vector3 line_1 = n_1-n_0;
        Vector3 line_2 = n_2-n_1;
        float deltaTheta = Vector2.Angle(new Vector2(line_1[0],line_1[2]),new Vector2(line_2[0],line_2[2])); 
        //if angle smaller than 15deg, too small

        return deltaTheta* Mathf.Rad2Deg<15f;
    }

    Vector3[] SmallAngleFix(Vector3 n_0, Vector3 n_1, Vector3 n_2){//returns two points, in order of position in path 

        //TODO: around spacing vector, check spacing constraints, from other possible points around
        //check angle to not make over steep

        Vector3 line_1 = n_1-n_0;
        Vector3 line_2 = n_2-n_1;

        //making line for extra point(using avgVect), to space n_1
        //randomly slope horizontally and slightly vertically
        Vector3 horizontalAvgVect = Vector3.ProjectOnPlane(AverageVect(line_1,line_2), Vector3.up); 
        Vector3 sideVect = Vector3.Cross(horizontalAvgVect,Vector3.up).normalized;

        float randomDeltaTheta = Random.Range(-5f,5f);//gauss? true rand? small variation
        float randomDeltaPhi = Random.Range(-5f,0f);//between 0 and (small angle for small slope)
        Vector3 spacingVector = Quaternion.AngleAxis(randomDeltaTheta, Vector3.up) * sideVect;
        spacingVector = Quaternion.AngleAxis(randomDeltaPhi, horizontalAvgVect) * spacingVector;

        float randomSpacingAmount = Random.Range(1f,2.5f);
        Vector3 n_a = n_1+spacingVector*randomSpacingAmount;
        Vector3 n_b = n_1-spacingVector*randomSpacingAmount;


        Vector3[] points;
        if(Vector3.Angle(Vector3.up,(n_a-n_2))>=Vector3.Angle(Vector3.up,(n_b-n_2))){
            points = new[] {n_a, n_b};
            //n_b goes after n_a
        }
        else{
            //n_a goes after n_b
            points = new[] {n_b, n_a};
        }

        //n_1 becomes n_a
        //n_b added between n_1 and n_2
        //add n_a, n_b to point chain

        return points;
    }

    Vector3 OverSteepSlopeFix(Vector3 n_0, Vector3 n_1){//, float phi
        
        Vector3 line_1 = n_1-n_0;
        float len = line_1.magnitude;

        Vector3 horizLine_1 = new Vector3(line_1[0],0,line_1[2]);
       
        //Vector3 n_a = midPoint;//n goes up
        
        Vector3 midPoint = AverageVect(n_0, n_1);
        Vector3 crossVect;

        if(Vector3.Cross(line_1,Vector3.up)==Vector3.zero){
            float randX = Random.Range(-1,1);
            float randZ = Mathf.Sqrt(1-randX*randX);
            crossVect = new Vector3(randX,0,randZ);
        }
        else{
            crossVect = Vector3.Cross(line_1,Vector3.up).normalized;
        }


        float r = midPoint.y/1.6f;//tan theta  = y/r, r = y/tan theta
        float z = Mathf.Sqrt(r-(new Vector2(midPoint.x,midPoint.z)).sqrMagnitude);

        Vector3 nA = midPoint + z*crossVect;
        Vector3 nB = midPoint - z*crossVect;
        float twoSide = Random.Range(-1,1);
        if(twoSide<0){
            return nA;
        }
        else{
            return nB;
        }

        //twoCircleInterSections(n_0,n_1)
    }

    Vector3 AverageVect(Vector3 line_1, Vector3 line_2){
        return (line_1+line_2)/2;
    }

    Vector3 ClosestPointOnLine(Vector3 n_0, Vector3 n_1, Vector3 point){
        Vector3 line_1 = n_1-n_0;
        Vector3 perpVect = Vector3.Cross(line_1,n_0-point).normalized;
        Vector3 dVect = Vector3.Cross(perpVect,line_1).normalized;

        return Vector3.zero;

    }

    //for array of points, return the platforms that connects them
    void NodesToPlatformMesh(Vector3[] points){
        //number of cubes = points.Length;
        
        for(int i = 0;i<points.Length-1;i++){
            List<Vector3> currPointsToAdd = BresenhamAlgo3D(points[i], points[i+1]);
            foreach(Vector3 currPoint in currPointsToAdd){
                Instantiate(platformPrefab, currPoint, Quaternion.identity);
            }
        }

        //only adds platforms at start and end of lines, add middle
        //Bresenham's line algorithm


        //possible direction: marching cubes (more angular)
    }

    List<Vector3> BresenhamAlgo3D(Vector3 n_1, Vector3 n_2){//float stepSize
        //note, does not add n_1 into list of points
        //used for chain of nodes not to spawn points twice (end last == new next)
        print("Bresenham");
        List<Vector3> listOfPoints = new List<Vector3>();
        
        int x_1 = (int)n_1.x;
        int y_1 = (int)n_1.y;
        int z_1 = (int)n_1.z;
        int x_2 = (int)n_2.x;
        int y_2 = (int)n_2.y;
        int z_2 = (int)n_2.z;
        
        int delta_x = (int)System.Math.Floor(System.Math.Abs(n_2.x - n_1.x));
        int delta_y = (int)System.Math.Floor(System.Math.Abs(n_2.y - n_1.y));
        int delta_z = (int)System.Math.Floor(System.Math.Abs(n_2.z - n_1.z));

        int x_s, y_s, z_s;
        int p_1, p_2;

        if (x_2 > x_1)
            x_s = 1;
        else
            x_s = -1;
        if (y_2 > y_1)
            y_s = 1;
        else
            y_s = -1;
        if (z_2 > z_1)
            z_s = 1;
        else
            z_s = -1;

        //listOfPoints.Add(new Vector3(x_1, y_1, z_1));//add n_1 into list of points

        // Driving axis is X-axis"
        if (delta_x >= delta_y && delta_x >= delta_z){
            p_1 = 2 * delta_y - delta_x;
            p_2 = 2 * delta_z - delta_x;
            while (x_1 != x_2){
                x_1 += x_s;
                if (p_1 >= 0){
                    y_1 += y_s;
                    p_1 -= 2 * delta_x;
                }
                if (p_2 >= 0){
                    z_1 += z_s;
                    p_2 -= 2 * delta_x;
                }
                p_1 += 2 * delta_y;
                p_2 += 2 * delta_z;
                listOfPoints.Add(new Vector3(x_1, y_1, z_1));
            }   
        }
        // Driving axis is Y-axis"
        else if (delta_y >= delta_x && delta_y >= delta_z){
            p_1 = 2 * delta_x - delta_y;
            p_2 = 2 * delta_z - delta_y;
            while (y_1 != y_2){
                y_1 += y_s;
                if (p_1 >= 0){
                    x_1 += x_s;
                    p_1 -= 2 * delta_y;
                }
                if (p_2 >= 0){
                    z_1 += z_s;
                    p_2 -= 2 * delta_y;
                }
                p_1 += 2 * delta_x;
                p_2 += 2 * delta_z;
                listOfPoints.Add(new Vector3(x_1, y_1, z_1));
            }   
        }
        // Driving axis is Z-axis"
        else{
            p_1 = 2 * delta_x - delta_z;
            p_2 = 2 * delta_y - delta_z;
            while (z_1 != z_2){
                z_1 += z_s;
                if (p_1 >= 0){
                    y_1 += y_s;
                    p_1 -= 2 * delta_z;
                }
                if (p_2 >= 0){
                    x_1 += x_s;
                    p_2 -= 2 * delta_z;
                }
                p_1 += 2 * delta_y;
                p_2 += 2 * delta_x;
                listOfPoints.Add(new Vector3(x_1, y_1, z_1));
            }   
        }
        

        //Vector3[] arrOfPoints = listOfPoints.ToArray();
        return listOfPoints;
    }

    void OnDrawGizmos() {
		Vector3 startPosition = startNode.position;
		Vector3 previousPosition = startPosition;

        if(Application.isPlaying){
   
            if(pathOfNodes !=null){
                foreach (Vector3 node in pathOfNodes) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere (node, .3f);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine (previousPosition, node);
                    previousPosition = node;
                }
            }
            Gizmos.DrawSphere (startPosition, .3f);
            Gizmos.DrawSphere (endNode.position, .3f);
            //Gizmos.DrawLine (previousPosition, startPosition);
        }
	}



}
