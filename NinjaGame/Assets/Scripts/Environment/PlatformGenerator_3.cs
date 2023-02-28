using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlatformGenerator_3 : MonoBehaviour
{

    //wave function collapse;

    //for grid of pos, containing possible states for said pos

    //iterate through grid, remove impossible states for pos
    //find pos with lowest num of states, >1
    //(backtrack alg, select state for pos randomly and keep selected state)
    //
    Vector3[] possibleNextDir = {new Vector3(1,0,0),new Vector3(-1,0,0),new Vector3(1,1,0),new Vector3(1,-1,0),new Vector3(-1,1,0),new Vector3(-1,-1,0),new Vector3(0,0,1),new Vector3(0,0,-1),new Vector3(0,1,1),new Vector3(0,-1,1),new Vector3(0,1,-1),new Vector3(0,-1,-1)};
    enum PossibleDir 
    {
    Forward,
    Right,
    Backward,
    Left,
    ForwardUp,
    RightUp,
    BackwardUp,
    LeftUp,
    ForwardDown,
    RightDown,
    BackwardDown,
    LeftDown
    }

    public GameObject platformPrefab;

    BitArray[,,] posNextStates;//bitArray, representing possible next direction
    bool [,,] spaceMatrix;//if false in [i][j][k], empty, if true, full

    Vector2[,] walls2d;//lines (origin+dir)
    //walls as lines v = v_0 + t*dir;
    //Plane[] wallPos;
    //Dictionary<char, float[]> wallsDict = new Dictionary<char, float[]>(){{'x', new float[2]},{'y', new float[2]},{'z', new float[2]}};//x[0] min, x[1] max

    //Vector3[] wallsAsBounds = {new Vector3(float.MaxValue,float.MaxValue,float.MaxValue),new Vector3(float.MinValue,float.MinValue,float.MinValue)};//wallsAsBounds[0] min, wallsAsBounds[1] max
    float[] wallsBounds = {float.MaxValue,float.MinValue,float.MaxValue,float.MinValue}; //min x, max x, min z, max z 

    public Transform walls;

    public float minTurnRadius = 1f;

    //path first sparsing second idea

    Vector3[] pathOfNodes;

    //////////////////////TestRegion

    Vector2[] circlesPosGlobal;
    float[] circlesRadiusGlobal;

    ///////////////////////EndTest

    public Transform startNode;
    public Transform endNode;

    float maxSlope = 1.732f;//slightly less than 60 deg
    //maxSope*deltaR = maxDeltaY //for linked path()
    //from point: make sure theta and phi to next point not too similar to "" to previous point
    //theta = baseTheta+gaussianRandom()
    //phi = basePhi+gaussianRandom


    void Start()
    {

        walls2d = new Vector2[walls.childCount,2];
		for (int i = 0; i< walls.childCount; i++){
            //assuming wall transform rotated; otherwise no need(just get min-max x and min-max z)
            walls2d[i,0]=  new Vector2(walls.GetChild(i).position.x,walls.GetChild(i).position.z);
            walls2d[i,1]= new Vector2(walls.GetChild(i).right.x,walls.GetChild(i).right.z);

            if(walls.GetChild(i).position.x<wallsBounds[0]){
                wallsBounds[0] = walls.GetChild(i).position.x;
            }
            if(walls.GetChild(i).position.x>wallsBounds[1]){
                wallsBounds[1] = walls.GetChild(i).position.x;
            }

            //y not needed, as walls = 2d

            if(walls.GetChild(i).position.z<wallsBounds[2]){
                wallsBounds[2]= walls.GetChild(i).position.z;
            }
            if(walls.GetChild(i).position.z>wallsBounds[3]){
                wallsBounds[3] = walls.GetChild(i).position.z;
            }

		}
        int lenX = ((int)wallsBounds[1])-((int)wallsBounds[0]);
        int lenZ = ((int)wallsBounds[3])-((int)wallsBounds[2]);
        int lenY = 100;
        posNextStates = new BitArray[lenX,lenY,lenZ];
        for(int i=0;i<posNextStates.GetLength(0);i++){
            for(int j = 0; j<posNextStates.GetLength(1);j++){
                for(int k = 0; k<posNextStates.GetLength(2);k++){
                    posNextStates[i,j,k] = new BitArray(12);//(int)Mathf.Pow(2,13)-1;
                    posNextStates[i,j,k].SetAll(true);
                }
            }
        }
        
        StartCoroutine(GenerateRandomPath(startNode.position, endNode.position));
    }


    IEnumerator GeneratePlatforms(Vector3[] nodes){
        print("startEnum");
        //NodesToPlatformMesh(nodes);
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
        
        for(int j = 0; j<posNextStates.GetLength(1);j++){
            //posNextStates[0,j,0]-=;010011
            for(int i=0;i<posNextStates.GetLength(0);i++){
                posNextStates[i,j,0].Set((int)PossibleDir.Backward,false);
                posNextStates[i,j,0].Set((int)PossibleDir.BackwardUp,false);
                posNextStates[i,j,0].Set((int)PossibleDir.BackwardDown,false);

                posNextStates[i,j,posNextStates.GetLength(2)-1].Set((int)PossibleDir.Forward,false);
                posNextStates[i,j,posNextStates.GetLength(2)-1].Set((int)PossibleDir.ForwardUp,false);
                posNextStates[i,j,posNextStates.GetLength(2)-1].Set((int)PossibleDir.ForwardDown,false);
            }
            for(int k = 0; k<posNextStates.GetLength(2);k++){
                posNextStates[0,j,k].Set((int)PossibleDir.Left,false);
                posNextStates[0,j,k].Set((int)PossibleDir.LeftUp,false);
                posNextStates[0,j,k].Set((int)PossibleDir.LeftDown,false);

                posNextStates[posNextStates.GetLength(0)-1,j,k].Set((int)PossibleDir.Right,false);
                posNextStates[posNextStates.GetLength(0)-1,j,k].Set((int)PossibleDir.RightUp,false);
                posNextStates[posNextStates.GetLength(0)-1,j,k].Set((int)PossibleDir.RightDown,false);
            }

        }
        
        return new Vector3[1];
        
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

    

    void OnDrawGizmos() {
		Vector3 startPosition = startNode.position;
		Vector3 previousPosition = startPosition;

        if(Application.isPlaying){
   
            // if(pathOfNodes !=null){
            //     foreach (Vector3 node in pathOfNodes) {
            //         Gizmos.color = Color.red;
            //         Gizmos.DrawSphere (node, .3f);

            //         Gizmos.color = Color.yellow;
            //         Gizmos.DrawLine (previousPosition, node);
            //         previousPosition = node;
            //     }
            // }

            if(circlesPosGlobal !=null){
                for(int i = 0; i<circlesPosGlobal.Length;i++){
                    Vector3 node = new Vector3(circlesPosGlobal[i].x,0,circlesPosGlobal[i].y);
                    float r_node = circlesRadiusGlobal[i];
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere (node, r_node);
                }
            }

            
            //Gizmos.DrawLine (previousPosition, startPosition);
        }
        if(walls2d!=null){
            for(int i = 0 ; i<walls2d.GetLength(0);i++){
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(new Vector3(walls2d[i,0].x,0,walls2d[i,0].y)-100*new Vector3(walls2d[i,1].x,0,walls2d[i,1].y),new Vector3(walls2d[i,0].x,0,walls2d[i,0].y)+100*new Vector3(walls2d[i,1].x,0,walls2d[i,1].y));
            }
        }
        Gizmos.DrawSphere (startPosition, minTurnRadius);
        Gizmos.DrawSphere (endNode.position, minTurnRadius);
	}



}
