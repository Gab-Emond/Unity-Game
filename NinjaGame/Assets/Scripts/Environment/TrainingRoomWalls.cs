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


    public void PathingPoints(Vector3[] startPoints, Vector3[] endPoints, GameObject[] cubes,  Vector3 sampleRegionSize, Vector3 center){

            //go to spot

            //if at next space (pos%1==0)

            //if direct goto blocked 
            //move to empty space around
            //repeat
            //else,moving one space
            float cubeSpeed = 1;
            

            for(int i = 0; i<startPoints.Length;i++){
                Vector3 path = endPoints[i]-startPoints[i];
                Vector3 dir;
                if(path.x*path.x >= path.y*path.y&& path.x*path.x>=path.z*path.z){
                    dir = Vector3.right*Mathf.Sign(path.x);
                }
                else if(path.y*path.y >= path.x*path.x && path.y*path.y >=path.z*path.z){
                    dir = Vector3.up*Mathf.Sign(path.y);
                }
                else{
                    dir = Vector3.forward*Mathf.Sign(path.z);
                }

                //if(Physics.Linecast(cubes[i].transform.position,cubes[i].transform.position+dir,))
                //ienum

                if(Vector3.Dot(cubes[i].transform.position,dir)-Mathf.Floor(Vector3.Dot(cubes[i].transform.position,dir))==0){
                    //if at int position
                }

                cubes[i].transform.position += dir*cubeSpeed*Time.deltaTime; 
            }


    }

    class Cube{
        GameObject thisCubeObject;
        Stack<Vector3> pathNodes;

        Vector3 nextNode;

        public Cube(){//constructor

        }
        private void Update(float deltaTime) {
            
        }

    }

}



