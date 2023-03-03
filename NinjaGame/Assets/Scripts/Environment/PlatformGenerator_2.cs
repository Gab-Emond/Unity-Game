using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlatformGenerator_2 : MonoBehaviour
{

    public GameObject platformPrefab;

    public int platformSize = 1;

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
    
    //List<Vector3> tempArcNodes = new List<Vector3>();

    ///////////////////////EndTest

    public Transform startNode;
    public Transform endNode;

    float maxSlope = 1.192f;//tan(50 deg)=dy/dr //(r=x+z)
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

        //StartCoroutine(GenerateRandomPath(startNode.position, endNode.position));
        StartCoroutine(FullGen(startNode.position, endNode.position));
    }
    //todo: use async/await
    //learn about cancellation tokens (https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/cancel-an-async-task-or-a-list-of-tasks)
    IEnumerator FullGen(Vector3 _start, Vector3 _end){
        yield return StartCoroutine(GenerateRandomPath(_start,_end));
        yield return StartCoroutine(GeneratePlatforms(pathOfNodes));
    }

    IEnumerator GeneratePlatforms(Vector3[] nodes){
        NodesToPlatformMesh(nodes);
        yield return null;
    }

    IEnumerator GenerateRandomPath(Vector3 _start, Vector3 _end){
        pathOfNodes = RandomEnclosedPathGenerator(_start, _end);
        yield return null;
    }


    Vector3[] RandomEnclosedPathGenerator(Vector3 start, Vector3 end){

        //multiple circles in path, check distance of sum of arc length; change circles at connection points
        //circle minimum radius determined by max elevation (lets curve onto itself more)

        //1
        Vector2 diffVect = new Vector2(end.x,end.z)- new Vector2(start.x,start.z);
        float distHorizontal = diffVect.magnitude;
        Vector2 diffVectNorm = diffVect.normalized;
        
        Vector2 startPos2d = new Vector2(start.x,start.z);
        Vector2 endPos2d = new Vector2(end.x,end.z);
        

        float diffVert = end.y-start.y;
        float minimumHorizLength = diffVert/maxSlope;

        //print("minHorizLen: "+ minimumHorizLength);
        float pathTotalLength = 0;
        
       

        //////////////Test

        // circlesPosGlobal = circlesPos;

        // circlesRadiusGlobal = r_Circle;


        ///////////////////

        //iterate
        int iter = 0;
        int maxIteration = 20;
        Stack<Vector2> circleCenterStack = new Stack<Vector2>();
        Stack<float> circleRadiusStack = new Stack<float>();
        Stack<Vector2> circleStartPos = new Stack<Vector2>();
        Stack<Vector2> circleEndPos = new Stack<Vector2>();

        Stack<Vector2> pathAllPointPos = new Stack<Vector2>();

        bool clockWiseSide = false;//true, clockwise from start, false, counterclock from start
        
        float randR = Random.Range(minTurnRadius, Mathf.Min(wallsBounds[1]-wallsBounds[0],wallsBounds[3]-wallsBounds[2])/6);
        circleStartPos.Push(startPos2d);
        

        if(Mathf.Min(startPos2d.x-wallsBounds[0],wallsBounds[1]-startPos2d.x)<
        Mathf.Min(startPos2d.y-wallsBounds[2],wallsBounds[3]-startPos2d.y)){
            if(startPos2d.x-wallsBounds[0]<wallsBounds[1]-startPos2d.x){
                circleCenterStack.Push(startPos2d+Vector2.right*randR);
            }
            else{
                circleCenterStack.Push(startPos2d+Vector2.left*randR);
            }
        }
        else{
            if(startPos2d.y-wallsBounds[2]<wallsBounds[3]-startPos2d.y){
                circleCenterStack.Push(startPos2d+Vector2.up*randR);
            }
            else{
                circleCenterStack.Push(startPos2d+Vector2.down*randR);
            }
        }
        

        circleRadiusStack.Push(randR);

        while(pathTotalLength<minimumHorizLength&&iter<maxIteration){
            bool validPosFound = false;
            int inner_Iter = 0;
            Vector2 prevCenter=circleCenterStack.Peek();
            float prevRadius=circleRadiusStack.Peek();
            Vector2 samplePoint = new Vector2();
            while(!validPosFound&&inner_Iter<maxIteration){
                float randX = Random.Range(wallsBounds[0]+minTurnRadius,wallsBounds[1]-minTurnRadius);
                float randY = Random.Range(wallsBounds[2]+minTurnRadius,wallsBounds[3]-minTurnRadius);

                //possible next step: increase odds of further point
                //ex: find quadrant circle is in, increase chance of other quadrants


                samplePoint = new Vector2(randX,randY);
                if((samplePoint-prevCenter).sqrMagnitude<(prevRadius+minTurnRadius)*(prevRadius+minTurnRadius)){
                    validPosFound = false;
                }
                else{
                    validPosFound = true;
                }

                inner_Iter++;
            }

            //from pos, get possible radius

            // wall distances+sphere distance
            float[] wallDists = new float[walls2d.GetLength(0)];
            float closestDist = (samplePoint-prevCenter).magnitude-prevRadius;
            for(int j =0; j< walls2d.GetLength(0);j++){
                wallDists[j] = Utility.Math.MathUtility.DistancePointLine2d(walls2d[j,0],walls2d[j,1],samplePoint);            
                if(wallDists[j]<closestDist){
                    closestDist = wallDists[j];
                }
            }
            //error: radius can overlap, might be causing the line between circles issues

            float sampleRadius = Mathf.Min(Mathf.Min(wallsBounds[1]-wallsBounds[0],wallsBounds[3]-wallsBounds[2])/4,closestDist);//Random.Range(minTurnRadius,Mathf.Min(Mathf.Min(wallsBounds[1]-wallsBounds[0],wallsBounds[3]-wallsBounds[2])/3,closestDist));

            Vector2[] points = Utility.Math.MathUtility.LineBetweenCircles(prevCenter,prevRadius,samplePoint,sampleRadius,clockWiseSide?-1:1);
            
            circleEndPos.Push(points[0]);
            float pathOnPrevCircle = Utility.Math.MathUtility.ArcLength2D(circleStartPos.Peek()-circleCenterStack.Peek(),circleEndPos.Peek()-circleCenterStack.Peek(),circleRadiusStack.Peek());
            //pathOnPrevCircle = clockWiseSide?2*Mathf.PI*circleRadius.Last.Value-pathOnPrevCircle:pathOnPrevCircle;

            if(Utility.Math.MathUtility.Ccw(circleCenterStack.Peek(),circleStartPos.Peek(),circleEndPos.Peek())==clockWiseSide){//if not the same rotation dir
                pathOnPrevCircle = 2*Mathf.PI*circleRadiusStack.Peek()-pathOnPrevCircle;//larger side
            }
            //else same rotation dir; smaller side

            //todo: insert points between start and end on arc

            //getpointsinarc

            Vector2[] centerPoints = Utility.Math.MathUtility.GetPointsInArc(circleCenterStack.Peek(),circleRadiusStack.Peek(),circleStartPos.Peek(),circleEndPos.Peek(),clockWiseSide);
            pathAllPointPos.Push(circleStartPos.Peek());
            foreach (Vector2 point in centerPoints)
            {
                pathAllPointPos.Push(point);
            }
            pathAllPointPos.Push(circleEndPos.Peek());

            if(points.Length==2){
                
                float pathExtra = (points[1]-points[0]).magnitude;//space between the circles
                pathTotalLength+=pathExtra;
                circleStartPos.Push(points[1]);            
            }
            else{//touching, only one point
                circleStartPos.Push(points[0]);
            }
            //error, if touching uses center?


            circleCenterStack.Push(samplePoint);
            circleRadiusStack.Push(sampleRadius);

            pathTotalLength +=pathOnPrevCircle;
            //from possible radius, get random radius
            //from pos and random radius,and prev pos and prev radius, get new path length
            

            clockWiseSide = !clockWiseSide;
            iter++;

        }

        //section links final circle to end pos, two ways depending on if outside or inside
        
        
        if((circleCenterStack.Peek()-endPos2d).magnitude>circleRadiusStack.Peek()){
            //point to circle method
            Vector2[] points= Utility.Math.MathUtility.LineCirclePoint(circleCenterStack.Peek(),circleRadiusStack.Peek(),endPos2d,clockWiseSide?-1:1);
            circleEndPos.Push(points[0]);

            //////////////////////////////////////////////////
            Vector2[] centerPoints = Utility.Math.MathUtility.GetPointsInArc(circleCenterStack.Peek(),circleRadiusStack.Peek(),circleStartPos.Peek(),circleEndPos.Peek(),clockWiseSide);

            pathAllPointPos.Push(circleStartPos.Peek());
            foreach (Vector2 point in centerPoints)
            {
                pathAllPointPos.Push(point);
            }
            pathAllPointPos.Push(circleEndPos.Peek());
            pathAllPointPos.Push(points[1]);
            //////////////////////////////////////////////////
            
            //circleStartPos.Push(points[1]);
        }
        else{
            //tangent inside
            //center-endpos, get intersection points with circle
            //line = center + t*(center-endpos)
            //intersect = center +- r(center-endpos).normalized;
            Vector2 intersect = circleCenterStack.Peek()+circleRadiusStack.Peek()*(circleCenterStack.Peek()-endPos2d).normalized;
            //end.push intersect
            //start.push endpos2d
            circleEndPos.Push(intersect);
            //////////////////////////////////////////////////
            Vector2[] centerPoints = Utility.Math.MathUtility.GetPointsInArc(circleCenterStack.Peek(),circleRadiusStack.Peek(),circleStartPos.Peek(),circleEndPos.Peek(),clockWiseSide);

            pathAllPointPos.Push(circleStartPos.Peek());
            foreach (Vector2 point in centerPoints)
            {
                pathAllPointPos.Push(point);
            }
            pathAllPointPos.Push(circleEndPos.Peek());
            pathAllPointPos.Push(endPos2d);
            //////////////////////////////////////////////////
            
            //circleStartPos.Push(endPos2d);


        }


        //todo, make new stack, containing points from start to end on circles

        //for each circle

        //get angle between start and end
        //depending on angle, split into number of parts





        circlesPosGlobal = circleCenterStack.ToArray();
        circlesRadiusGlobal = circleRadiusStack.ToArray();

        //int numOfPoints = circleStartPos.Count+circleEndPos.Count;
        Vector3[] posTemp = new Vector3[pathAllPointPos.Count]; 
        float trueLen = 0;
        Vector2 prevPos = pathAllPointPos.Peek();
        for (int i = posTemp.Length - 1; i >= 0 ; i--)
        {
            

            Vector2 point = pathAllPointPos.Pop();
            posTemp[i] = new Vector3(point.x,0,point.y);
            
            trueLen += (point-prevPos).magnitude; 
            prevPos = point;
        }
        print("trueLen: "+trueLen);
        float currLen = 0;
        for (int i = 0; i < posTemp.Length; i++)
        {
            if(i>0){
                Vector2 prev = new Vector2(posTemp[i-1].x,posTemp[i-1].z);
                Vector2 curr = new Vector2(posTemp[i].x,posTemp[i].z);
                currLen += (curr-prev).magnitude;
            }
            posTemp[i].y=Mathf.Lerp(start.y,end.y,(currLen/trueLen));
            
        }

        // print("pathOfNode Start: "+posTemp[0]);
        // print("pathOfNode End: "+posTemp[posTemp.Length-1]);

        //print("endInsertLen"+i);
        

        //print("pathTotLength: "+pathTotalLength);


        

        //1. for distance (x,0,z) between start and end, seperate into spheres of diameters d > 2*r_min for according to max 

        //while(path<minimum path for reaching endpoint)

        //2.choose random circle to expand (change to r = r+random)
        //2a. choose random point in possible space (away from walls, further from before+after circle)
        
        //could pseudo hash, if within circles , send away from them
        //if within walls, modulo around other side

        //split space into grid, remove unplacable points

        //for(int i = center.x-radius;i<center.x+radius;i++)
        //float squareY = (int) Mathf.Sqrt(radius*radius-i*i);
        //for(int j  = center.y-squareY;j<center.y+squareY;j++)
        //grid[i,j] = false;

        //how to pick only from valid

        
        //if((pos-center).sqrMagnitude<r+r_min)
        //restart next iteration
        //push away by r amount, looping around inside space
        
        //if (close to end && path done)
        //end path


        //3.check path length
        //3a. save path lenght to lower check time; 
        //3b. only change part of path length from recent changed spheres

        //if path length>minimum path to reach end
        //return path
        //else
        //choose random to expand

        

        return posTemp;//new Vector3[1];
    }

   

    Vector3 AverageVect(Vector3 line_1, Vector3 line_2){
        return (line_1+line_2)/2;
    }

    //for array of points, return the platforms that connects them
    void NodesToPlatformMesh(Vector3[] points){
        //number of cubes = points.Length;
        
        for(int i = 0;i<points.Length-1;i++){
            List<Vector3> currPointsToAdd = BresenhamAlgo3D(points[i], points[i+1], platformSize);
            foreach(Vector3 currPoint in currPointsToAdd){
                Instantiate(platformPrefab, currPoint, Quaternion.identity);
            }
        }

        //only adds platforms at start and end of lines, add middle
        //Bresenham's line algorithm


        //possible direction: marching cubes (more angular)
    }

    List<Vector3> BresenhamAlgo3D(Vector3 n_1, Vector3 n_2,int stepSize=1){
        //note, does not add n_1 into list of points
        //used for chain of nodes not to spawn points twice (end last == new next)
        print("Bresenham");
        List<Vector3> listOfPoints = new List<Vector3>();
        
        int x_1 = Mathf.FloorToInt(n_1.x/stepSize);
        int y_1 = Mathf.FloorToInt(n_1.y/stepSize);
        int z_1 = Mathf.FloorToInt(n_1.z/stepSize);
        int x_2 = Mathf.FloorToInt(n_2.x/stepSize);
        int y_2 = Mathf.FloorToInt(n_2.y/stepSize);
        int z_2 = Mathf.FloorToInt(n_2.z/stepSize);
        
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

        listOfPoints.Add(new Vector3(x_1*stepSize, y_1*stepSize, z_1*stepSize));//add n_1 into list of points

        // Driving axis is X-axis"
        if (delta_x >= delta_y && delta_x >= delta_z){
            p_1 = 2 * delta_y - delta_x;
            p_2 = 2 * delta_z - delta_x;
            while (x_1*x_s < x_2*x_s){
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
                listOfPoints.Add(new Vector3(x_1*stepSize, y_1*stepSize, z_1*stepSize));
            }   
        }
        // Driving axis is Y-axis"
        else if (delta_y >= delta_x && delta_y >= delta_z){
            p_1 = 2 * delta_x - delta_y;
            p_2 = 2 * delta_z - delta_y;
            while (y_1*y_s < y_2*y_s){
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
                listOfPoints.Add(new Vector3(x_1*stepSize, y_1*stepSize, z_1*stepSize));
            }   
        }
        // Driving axis is Z-axis"
        else{
            p_1 = 2 * delta_x - delta_z;
            p_2 = 2 * delta_y - delta_z;
            while (z_1*z_s < z_2*z_s){
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
                listOfPoints.Add(new Vector3(x_1*stepSize, y_1*stepSize, z_1*stepSize));
            }   
        }
        

        //Vector3[] arrOfPoints = listOfPoints.ToArray();
        return listOfPoints;
    }

    List<Vector3> GridRaster(Vector3 n_1, Vector3 n_2,float stepSize=1){
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

        float t_delta_x=-1f/delta_x;
        float t_delta_y=-1f/delta_y;
        float t_delta_z=-1f/delta_z;



        return listOfPoints;
    }

    void OnDrawGizmos() {
		Vector3 startPosition = startNode.position;
		Vector3 previousPosition = startPosition;
        int prevI = 0;

        if(Application.isPlaying){
   
            if(pathOfNodes !=null){
                
                for (int i = 1; i < pathOfNodes.Length; i++)
                {
                    //print("prevI: "+prevI+", currI: "+i);
                    Vector3 node = pathOfNodes[i];
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine (previousPosition, node);
                    previousPosition = node;
                    prevI = i;
                    
                }
                
            }

            // if(circlesPosGlobal !=null){
            //     for(int i = 0; i<circlesPosGlobal.Length;i++){
            //         Vector3 node = new Vector3(circlesPosGlobal[i].x,0,circlesPosGlobal[i].y);
            //         float r_node = circlesRadiusGlobal[i];
            //         Gizmos.color = Color.red;
            //         Gizmos.DrawSphere (node, r_node);
            //     }
            // }

            
            
        }
        if(walls2d!=null){
            for(int i = 0 ; i<walls2d.GetLength(0);i++){
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(new Vector3(walls2d[i,0].x,0,walls2d[i,0].y)-100*new Vector3(walls2d[i,1].x,0,walls2d[i,1].y),new Vector3(walls2d[i,0].x,0,walls2d[i,0].y)+100*new Vector3(walls2d[i,1].x,0,walls2d[i,1].y));
            }
        }
        // if(wallsBounds!=null){
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawLine(new Vector3(wallsBounds[0],0,-200),new Vector3(wallsBounds[0],0,200));
        //     Gizmos.DrawLine(new Vector3(wallsBounds[1],0,-200),new Vector3(wallsBounds[1],0,200));
        //     Gizmos.DrawLine(new Vector3(-200,0,wallsBounds[2]),new Vector3(200,0,wallsBounds[2]));
        //     Gizmos.DrawLine(new Vector3(-200,0,wallsBounds[3]),new Vector3(200,0,wallsBounds[3]));

        // }
        Gizmos.DrawSphere (startPosition, minTurnRadius);
        Gizmos.DrawSphere (endNode.position, minTurnRadius);

        
	}



}
