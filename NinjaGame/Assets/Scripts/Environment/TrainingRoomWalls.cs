using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrainingRoomWalls : MonoBehaviour {
    

    //from list of points and 4 walls
    //make list of initial starting points for each

    //endpoints in world space, aka turn into local space, according to center
    public List<Vector3> StartingPoints(List<Vector3> endPoints, Vector3 sampleRegionSize, Vector3 center){

        //array containing the center of the 4 surrounding walls
        //north = +z
        //wall order: north, south, east, west
        Vector3[] wallPos = new Vector3[]{center + Vector3.forward*sampleRegionSize.z/2,center + Vector3.back*sampleRegionSize.z/2,center+Vector3.right*sampleRegionSize.x/2,center+Vector3.left*sampleRegionSize.x/2};

        List<Vector3> startPoints = new List<Vector3>();

        foreach(Vector3 point in endPoints)
        {
            int wallClosest;

            Vector3 relPoint = point-center;
            //if x closest

            Vector3 startPoint;//in world space

            if(relPoint.z*relPoint.z>relPoint.x*relPoint.x){//north/south
                if(relPoint.z>0){//closest to north wall
                    wallClosest = 0;
                    startPoint = Vector3.ProjectOnPlane(relPoint,Vector3.back);
                    startPoint+=wallPos[0];
                }
                else{//closest to south
                    wallClosest = 1;
                    startPoint = Vector3.ProjectOnPlane(relPoint,Vector3.forward);
                    startPoint+=wallPos[1];
                }
            }
            else{//east/west
                if(relPoint.x>0){//closest to east
                    wallClosest = 2;
                    startPoint = Vector3.ProjectOnPlane(relPoint,Vector3.left);
                    startPoint+=wallPos[2];
                }
                else{//closest to west
                    wallClosest = 3;
                    startPoint = Vector3.ProjectOnPlane(relPoint,Vector3.right);
                    startPoint+=wallPos[3];
                }
            }

            startPoints.Add(startPoint);


        }

        return startPoints;
    }

    //Heap<Cube> cubes;//keep a tree of the cube positions

    bool[,,] occupiedSpace;//empty cube space, size of bounds of rectangle, true if occupied false if not (n**3 space)
    //new empty bool will be false
    public bool IsOccupied(Vector3 targetPoint){
        return occupiedSpace[(int)targetPoint.x,(int)targetPoint.y,(int)targetPoint.z];
    }  
    public void PathingPoints(Vector3[] startPoints, Vector3[] endPoints, Cube[] cubes,  Vector3 sampleRegionSize, Vector3 center){

            //go to spot

            //if at next space (pos%1==0)

            //if direct goto blocked 
            //move to empty space around
            //repeat
            //else,moving one space
            float cubeSpeed = 1;
            bool allCubesAtGoal = false;

            while(!allCubesAtGoal){
                int i  = 0;
                foreach (var cube in cubes)
                {
                    Vector3 targetPoint = cubes[i].nextNode;
                    //if(cube moving):do nothing
                    if(!IsOccupied(targetPoint)){//err will block when heading to point

                        //cubes[i] start moving towards next node
                        //startcoroutine(node.MoveToNext());
                        occupiedSpace[(int)targetPoint.x,(int)targetPoint.y,(int)targetPoint.z] = true;
                    }
                    else{
                        //Pathfinding(cube.currNode, cube.endNode);
                    }
                    
                    i++;
                }
                


            }


            // for(int i = 0; i<startPoints.Length;i++){
            //     Vector3 path = endPoints[i]-startPoints[i];
            //     Vector3 dir;
            //     if(path.x*path.x >= path.y*path.y&& path.x*path.x>=path.z*path.z){
            //         dir = Vector3.right*Mathf.Sign(path.x);
            //     }
            //     else if(path.y*path.y >= path.x*path.x && path.y*path.y >=path.z*path.z){
            //         dir = Vector3.up*Mathf.Sign(path.y);
            //     }
            //     else{
            //         dir = Vector3.forward*Mathf.Sign(path.z);
            //     }

            //     //if(Physics.Linecast(cubes[i].transform.position,cubes[i].transform.position+dir,))
            //     //ienum

            //     if(Vector3.Dot(cubes[i].transform.position,dir)-Mathf.Floor(Vector3.Dot(cubes[i].transform.position,dir))==0){
            //         //if at int position
            //     }

            //     cubes[i].transform.position += dir*cubeSpeed*Time.deltaTime; 
            // }


    }

    public class Cube{
        GameObject thisObject;

        public Transform transform;
        Stack<Vector3> pathNodes;

        public Vector3 nextNode{get; set;}
        public Vector3 currNode;
        Vector3 currPos;
        public Vector3 endNode;
        float speed;

        public Cube(Vector3 _endGoal, Vector3 _currNode, GameObject prefab){//constructor
            endNode = _endGoal;
            currNode = _currNode;
            currPos = currNode;
            thisObject = Instantiate(prefab,currNode,Quaternion.identity);
            transform = thisObject.transform;
        }

        public void SetNextNode(){
            currNode = nextNode;
            nextNode = pathNodes.Pop();
        }
        private void Update(float deltaTime) {
            
        }

        void MoveToNextNode(){//assumes next node correct
            
            if(currPos!=nextNode){

            }
            else{
                SetNextNode();
                //nextNode = Pathfinding(endGoal);
                //change 
            }
        }

        static Vector3[] FizzBuzzRaster(Vector3 start, Vector3 end){//assumes start&& end on whole numbers(int)
        // length=longest+mid+shortest-2 
            Vector3 deltaV = end-start;
            int distX = ((int)deltaV.x);
            int distY = ((int)deltaV.y);
            int distZ = ((int)deltaV.z); 

            int pathLength = (int)Vector3.Dot(deltaV,Vector3.one);

            Vector3[] path = new Vector3[pathLength];//dx+dy+dz = vectorDelta*vector(1,1,1)

            //get 2d vector3
            //vect forward, upward, side, ordered by size

            //aka; switch row, elementary matrix operation
            int maxLen=0;
            int midLen;
            int shortestLen;
            Vector3 moveDir;
            if(distX>=distY&&distX>=distZ){
                maxLen = distX;
                moveDir = Vector3.right;
            }
            else if(distZ>=distY&&distZ>=distX){
                maxLen = distZ;
                moveDir = Vector3.forward;
            }
            else{// if(distY>=distX&&distY>=distZ)
                maxLen = distY;
                moveDir = Vector3.up;
            }

            int j  = 0;
            for (int i = 0; i < maxLen; i++)
            {
                if(j==0){
                    path[j] = start;
                }
                else{
                    path[j] = path[j-1]+moveDir;
                }
                j++;

                
                if(i%distX==0){
                    
                }
                if(i%distY==0){

                }
                if(i%distZ==0){

                }


                //one for each of longest value

                //at ax for x = longestlen/otherlen
            }

            return path;
        }

    }

}



