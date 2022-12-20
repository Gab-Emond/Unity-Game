using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRB : MonoBehaviour
{
    //Kinematic: more to do
    
    //Other
    private Rigidbody rb;

    
    //Movement

    private bool isGrappled = false;
    public bool IsGrappled => isGrappled;     // the Name property, getter
    private float grappleDistance;//grapple
    public bool isGrounded;
    public bool IsGrounded => isGrounded;
    bool isCrouching;
    public bool IsCrouching => isCrouching;     //getter
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundDistance = 0.2f;

    Vector3 velocity = Vector3.zero;
    Vector3 move;
    Vector3 v_0;

    //Wall/Slide
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormal;
    bool isNextToWall = false;
    
    public float maxSlopeAngle = 35f;

    //Jumping

    /*
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;
    */
    public float jumpHeight = 20f;
    private float gravity = -9.81f;
    public float speed = 6f;
    
    //Input
    Vector2 inputs;
    PlayerInput playerInput;
    
    


    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start() {
        rb.freezeRotation = true;
    }

    private void Update() {
        //player input
        inputs = playerInput.input;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);
        //rb.SweepTest();
    }
    
    private void FixedUpdate() {
        Movement();
    }


    private void Movement() {
        
    }

    private void Jump() {
        /*
        if (isGrounded && readyToJump) {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            
            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) 
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        */
    }
    //Invoke() calls method after certain time

    /*
    private void ResetJump() {
        readyToJump = true;
    }
    */


    void normal(Vector2 inputs){
        //1st person, character rotation with camera

        move = (transform.right * inputs.x + transform.forward * inputs.y)*speed;
        
        
        //isfloor,isSlope,isWall


        if(isGrounded && velocity.y<=0) {
           
            velocity.y = -1f;//-1f;
            if (playerInput.Jump){
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.x = move.x;
            velocity.z = move.z;

        }
        else
        {
            velocity.y += gravity*Time.deltaTime; //gravity negative
            

            if(Vector2.SqrMagnitude(new Vector2(velocity.x,velocity.z))>(speed*speed + 0.5f)){
                

                if(move.x*velocity.x < 0){
                    velocity.x += move.x;
                }
                else if(move.x*move.x>velocity.x*velocity.x){
                    velocity.x = move.x;
                }
                if(move.z*velocity.z < 0){
                    velocity.z += move.z;
                }
                else if(move.z*move.z>velocity.z*velocity.z){
                    velocity.z = move.z;
                }
                
            }
            else{
                velocity.x = move.x;
                velocity.z = move.z;
            }


            //velocity += linFriction(new Vector3(velocity.x,0,velocity.z));

        }

    }

    void grapple(Vector3 target, Vector2 inputs){//shorten over time?
        
        Vector3 ropeVect; 
        Vector3 sideVect;
        Vector3 pullDir;
        float pullSpeed = 0;//"attraction"
        float angle;
        float l = grappleDistance;
        Vector3 correction;
        
        ropeVect = (target - transform.position);
        angle = Vector3.Angle(Vector3.up, ropeVect)* Mathf.Deg2Rad;

        if ((transform.position-target).sqrMagnitude < grappleDistance*grappleDistance ) {//not in tension
            normal(inputs);
            if (playerInput.Jump){
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else if (angle > 1.5708f){//in tension (), above 90 degrees
			normal(inputs);

            ropeVect = ropeVect.normalized;
            if((transform.position-target).sqrMagnitude >= grappleDistance*grappleDistance){
                v_0 = velocity;
                //controller.enabled = false;
                rb.MovePosition(target-ropeVect*grappleDistance);
                //controller.enabled = true;
                velocity = v_0;
                
                if(Vector3.Dot(velocity,ropeVect)<0){
                    correction = -Vector3.Project(velocity,ropeVect);
                    velocity += correction;
                }

            }
            


			//projection of ropevect on x and z
			
			//if(position+movement*Time.deltaTime < rope dist){
			//	velocity = movement
			//}

			//away from pulldir = 0
			
        }
        else{//in tension
            isGrounded = true;


            ropeVect = ropeVect.normalized;
            //charcontroller overrides position transform
            /**/

            v_0 = velocity;
            
            //controller.enabled = false;
            rb.MovePosition(target-ropeVect*grappleDistance);
            //controller.enabled = true;
            
            velocity = v_0;
            
            //Debug.DrawLine (transform.position, target, Color.yellow);          
            
            //theta' = -(g/l)sin(theta)*t (2D)
            //angle'' = -g/l * angle
            //angle'= omega = -g/l * angle * t (sketchy integration)
            //v_t = r*omega, r cancels out for pull speed


            if(angle<0.125){//small angle approx, sin theta => theta
                
                pullSpeed = -gravity*angle*Time.deltaTime;//*Time.deltaTime
            
            }
            else{
                pullSpeed = -gravity*Mathf.Sin(angle)*Time.deltaTime;//*Time.deltaTime       

                //print(pullSpeed);
                         
            }

            sideVect = Vector3.Cross(Vector3.up, ropeVect).normalized;//normalized, decide whether to optimize later
            pullDir =  Vector3.Cross(sideVect, ropeVect).normalized;//Quaternion.AngleAxis(-90, ropeVect)*sideVect;
          
            velocity +=pullDir*pullSpeed;
            
            //Debug.DrawLine (transform.position, transform.position+sideVect, Color.red);
            //Debug.DrawLine (transform.position, transform.position+pullDir*pullSpeed, Color.green);
            
            if(Vector3.Dot(velocity,ropeVect)<0){/**/
                correction = -Vector3.Project(velocity,ropeVect);//centripedalish force
            }
            else{
                correction = Vector3.zero;
            }

            //correction = (ropeVect*velocity.sqrMagnitude/distance)*Time.deltaTime;       //a_c=v**2/r, towards center (v_t, tangential speed only)
            //Debug.DrawLine (transform.position+velocity, transform.position+correction+velocity, Color.red);
            velocity +=correction;
            
            //player movement in relation to pulldir

            //basic slow down
            //velocity +=  (Vector3.Dot(transform.forward, pullDir)*pullDir/2 +Vector3.Dot(transform.forward, sideVect)*sideVect/4)*inputs.y;
            //velocity +=  (Vector3.Dot(transform.right, pullDir)*pullDir/2 +Vector3.Dot(transform.right, sideVect)*sideVect/4)*inputs.x;

            velocity +=  (Vector3.Project(transform.forward, pullDir)/2 +Vector3.Project(transform.forward, sideVect)/4)*inputs.y;
            velocity +=  (Vector3.Project(transform.right, pullDir)/2 +Vector3.Project(transform.right, sideVect)/4)*inputs.x;
            if (playerInput.Jump){
                velocity.y += Mathf.Sqrt(-2f* jumpHeight * gravity);//*ropeVect.y
                //delegate jump for grapple script to connect
            }    

            
        }

        /**/   
        if(playerInput.crouching){
            grappleDistance += 5f*Time.deltaTime;
            
        }
        else if(playerInput.run){
            if(grappleDistance>0.5f){
                grappleDistance -= 5f*Time.deltaTime;
            }
        }
        

    }

    public Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)//input toworldspace(=move), then to plane, then multiply by speed
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);//_characterUp);
        return Vector3.Cross(surfaceNormal, directionRight).normalized;
    }

    private bool IsFloor(Vector3 v) {
        float projectFloor = v.y;
        //float angle = Vector3.Angle(Vector3.up, v);
        return v.y>0.325f;//angle < maxSlopeAngle;
    }

    private bool IsWall(Vector3 v) {
        float projectWall = v.y;
        //float angle = Vector3.Angle(Vector3.up, v);
        return -0125f< v.y && v.y<0.325f;//angle < maxSlopeAngle;
    }

    private string PlaneCheck(Vector3 v){
        //if floor or slope
        //return floor or slope, no movement problem
        //if wall
        //return wall, movement problem
        return "floor";
    }
    
    /// <summary>
    /// Handle -ground- wall detection+ moving platforms
    /// </summary>

    
    private void OnCollisionStay(Collision other){//for wall
        /**/
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsWall(normal)) {
                isNextToWall = true;
                wallNormal = normal;
                
                
                if(Vector3.Dot(wallNormal,velocity) < 0){
                    velocity -= Vector3.Project(velocity, wallNormal);
                }

            }
        }
    
    }

    private void OnCollisionExit(Collision other) {
        /**/
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsWall(normal)) {
                isNextToWall = false;
                wallNormal = Vector3.zero;
            }
        }
    }




    public bool MovingTowardsWall(LayerMask layer,  Vector2 inputs){ // heavily edited from colanderp "isnexttowall"
        
        Vector3 _move = (transform.right * inputs.x + transform.forward * inputs.y);
		if(isNextToWall && Vector3.Dot(wallNormal, _move)<0){
            return true;
        }
        return false;
		//if(Physics.SphereCast(startPos, radius, transform.up, out hit, maxDist, layer)){//throws sphere in direction indicated
        //return (Physics.CapsuleCastAll(top, bottom, radius, move , maxDist, layer, out hit).Length >= 1);//throws capsule in direction indicated
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     foreach (ContactPoint contact in collision.contacts)
    //     {
    //         Debug.DrawRay(contact.point, contact.normal, Color.white);
    //     }
    //     // if (collision.relativeVelocity.magnitude > 2)
    //     //     audioSource.Play();
    // }
    


}
