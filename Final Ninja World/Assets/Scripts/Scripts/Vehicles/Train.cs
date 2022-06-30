using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cart;
    public GameObject locomotive;

    public Transform track;
    public float trainSpeed;

    void Start()
    {
        
        /*last cart - cart(xn) - locomotive */
        float step = trainSpeed*Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {   
       
    }

    //see unity handles.drawbezier

    IEnumerator FollowPath(Vector3[] wayPoints) {

        //to follow bezierTrainPath
       /*float distanceTravelled +=trainSpeed*Time.deltaTime;
       distanceTravelled = distanceTravelled % pathLength;//to stay within bounds, value loops on closed path

       transform.position = pathCreator.path.getPointAtDistance(distanceTravelled);
       transform.rotation = pathCreator.path.getRotationAtDistance(distanceTravelled);*/
       yield return null;
    }

}
