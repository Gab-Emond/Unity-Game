using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

//ripped from dani, to convert from rigidbody
public class Grapple : MonoBehaviour {

    private Vector3 currentGrapplePosition;

    public PlayerMovement playerMov;
    private LineRenderer lRenderer;
    
    private Transform grappledObject;
    private Vector3 grappledDecal;
    private Vector3 grapplePoint;

    //future warning: grapplehook changes rotation for hand ik
    public Transform grappleHook;   //note: see launchRope() comment
    TwoBoneIKConstraint animRigIk;

    public Transform headAim;
    //public Transform chestAim;
    //public MultiAimConstraint headAvoidIk;
    public MultiRotationConstraint spineRotIk;

    public LayerMask whatIsGrappleable;
    public Transform gunTip, player;
    Transform camera;
    public float ropeSpeed = 150f;
    private float maxDistance = 100f;
    private bool grappling;


    IEnumerator launchRoutine;

    void Awake() {
        playerMov = transform.parent.GetComponent<PlayerMovement>();
        lRenderer = GetComponent<LineRenderer>();
        camera = this.transform;

    }

    private void Start() {
        animRigIk = grappleHook.GetComponent<TwoBoneIKConstraint>();
        animRigIk.data.targetPositionWeight = 0;
        animRigIk.data.targetRotationWeight = 0;
        
        //headAvoidIk.weight = 0;
        spineRotIk.weight = 0;

        //to get rid of stop null coroutine error
        launchRoutine = LaunchRope();
        
    }
    

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(1)||playerMov.JumpKeyDown) {
            StopGrapple();
        }
        
    }

    private void FixedUpdate() {
        if(grappling){//or change by making child of object attached to
            grapplePoint = grappledObject.position + grappledDecal;
            playerMov.grappleTarget = grapplePoint;
            grappleHook.position = grapplePoint;
            
            grappleHook.LookAt(2*grapplePoint-gunTip.position);

            //float minAngle = 70f;

            //simply edit rotation in late update otherwise
            
            // if(Vector3.Angle(Vector3.up,grapplePoint-gunTip.position)<minAngle){
                
            //     // Vector3 sideVect = Vector3.Cross(Vector3.up,grapplePoint-gunTip.position);
            //     // headAim.position = Quaternion.AngleAxis(60, sideVect) * (grapplePoint-gunTip.position);
                
            //     //headAim.Rotate(chestAim.forward,minAngle-Vector3.Angle(headAim.up,grapplePoint-gunTip.position));
            //     //headAvoidIk.weight = .425f;
            //     spineRotIk.weight = 0.28f;

            // }
            // else{

            //     //headAvoidIk.weight = 0f;
            //     spineRotIk.weight = 0f;

            // }

            //headAimTarget = Vector3.Lerp(normal,)
            //ik target = grapplePoint
        }
    }

    //Called after Update
    void LateUpdate() {
        DrawRope();
        
        if(grappling){
            float minAngle = 60f;//make as class definition at some point
            //float headTurnSpeed = 60f;//degrees per sec, to cap at some point
            Vector3 armDir = grapplePoint-gunTip.position;
            float angleHeadArm = Vector3.SignedAngle(headAim.up, Vector3.ProjectOnPlane(armDir,headAim.forward),headAim.forward); 
            
            //Debug.DrawRay(headAim.position,player.forward,Color.red);
            
            if(Mathf.Abs(angleHeadArm)<minAngle){
                float amount =  (Mathf.PI)*angleHeadArm/minAngle;//(should be) linear value between PI and -PI 

                //print(angleHeadArm/minAngle);

                //smaller angle should give smaller difference, down to zero, with max at half point between min angle
                float angle = (-minAngle/2)*Mathf.Sin(amount);

                Quaternion anglRotation = Quaternion.Euler(0,0,angle);//headAim.rotation*

                headAim.rotation = headAim.rotation*anglRotation;

                // get original rotation in Update()
                

                // var step = headTurnSpeed * Time.deltaTime;

                // Quaternion rotateTarget = headAim.rotation*anglRotation;

                // headAim.rotation = Quaternion.RotateTowards(headAim.rotation, rotateTarget, step);
                
                //todo: headAim moves head for now
                //add headBone transform, that moves head instead, and slerp to headAim at certain maxSpeed to ease movement

            }
        }
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    //Todo: add aim assist:
    //- possibility; instead of raycast, use spherecast
    //-look through several hits instead of one, dont choose harmful cast unless only hit option
    /// </summary>
    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable, QueryTriggerInteraction.Ignore)) {
            grappling = true;
            grapplePoint = hit.point;
            grappledObject = hit.transform;
            grappledDecal = grapplePoint-grappledObject.position;
            //line
            //float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //make sure distance from point stays the same 

            lRenderer.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            playerMov.StartGrapple(grapplePoint);
            
            launchRoutine = LaunchRope();
            StartCoroutine(launchRoutine);

            //ik setWeight = 1
            animRigIk.data.targetPositionWeight = 1;
            animRigIk.data.targetRotationWeight = 1;
            
            //headAvoidIk.enabled = true;
            spineRotIk.enabled = true;
            spineRotIk.weight = 0.28f;

            
            
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    public void StopGrapple() {
        grappling = false;
        lRenderer.positionCount = 0;

        playerMov.StopGrapple();
        StopCoroutine(launchRoutine);
        
        //ik setWeight = 0;
        animRigIk.data.targetPositionWeight = 0f;
        animRigIk.data.targetRotationWeight = 0f;
        //headAvoidIk.weight = 0f;
        spineRotIk.weight = 0f;
        //headAvoidIk.enabled = false;
        spineRotIk.enabled = false;
        //use delegate and event to join playermovement stop grapple
    }

    IEnumerator LaunchRope(){

        //note: player hand target is grapplePoint, not currentGrapplePos; ik always points at end target, not going target
        //(not bad if throw is desired)
        float mover = 0;
        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
        float timeTot = distanceFromPoint/ropeSpeed;

        while (currentGrapplePosition != grapplePoint)
        {
            currentGrapplePosition = Vector3.Lerp(gunTip.position, grapplePoint, mover);
            mover= Mathf.Max(1f, (mover+Time.deltaTime)/timeTot);
            yield return null;
        }

        
    }
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!grappling){return;} 
        //currentGrapplePosition = grapplePoint;
        
        lRenderer.SetPosition(0, gunTip.position);
        lRenderer.SetPosition(1, currentGrapplePosition);
    }

}