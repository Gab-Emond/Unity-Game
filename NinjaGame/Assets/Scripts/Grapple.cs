using UnityEngine;

//ripped from dani, to convert from rigidbody
public class Grapple : MonoBehaviour {

    private Vector3 currentGrapplePosition;

    public PlayerMovement playerMov;
    private LineRenderer lRenderer;
    
    public Transform grappledObject;
    public Vector3 grappledDecal;
    private Vector3 grapplePoint;
    
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private bool grappling;

    void Awake() {
        playerMov = transform.parent.GetComponent<PlayerMovement>();
        lRenderer = GetComponent<LineRenderer>();
        camera = this.transform;

    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(1)) {
            StopGrapple();
        }
        if(playerMov.PlayerJumped){
            StopGrapple();
        }
        
    }

    private void FixedUpdate() {
        if(grappling){//or change by making child of object attached to
            grapplePoint = grappledObject.position + grappledDecal;
            playerMov.grappleTarget = grapplePoint;
        }
    }

    //Called after Update
    void LateUpdate() {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable, QueryTriggerInteraction.Ignore)) {
            grappling = true;
            grapplePoint = hit.point;
            grappledObject = hit.transform;
            grappledDecal = grapplePoint-grappledObject.position;
            //line
            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //make sure distance from point stays the same 

            lRenderer.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            playerMov.StartGrapple(grapplePoint);

        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    public void StopGrapple() {
        grappling = false;
        lRenderer.positionCount = 0;
        playerMov.StopGrapple();

        //use delegate and event to join playermovement stop grapple
    }

    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!grappling){return;} 
        currentGrapplePosition = grapplePoint;
        //currentGrapplePosition = Vector3.Lerp(gunTip.position, grapplePoint, Time.deltaTime * 8f);
        
        lRenderer.SetPosition(0, gunTip.position);
        lRenderer.SetPosition(1, currentGrapplePosition);
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}