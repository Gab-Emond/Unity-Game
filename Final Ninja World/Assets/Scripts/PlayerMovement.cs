
using UnityEngine;
using Utility.Math;

public class PlayerMovement : MonoBehaviour
{
    
    public CharacterController controller;
    PlayerInput playerInput;
    //grappleTest
    
    public Vector3 grappleTarget;

    private bool isGrappled = false;
    public bool IsGrappled => isGrappled;     // the Name property, getter
    private float grappleDistance;//grapple
    //jump+gravity
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.2f;

    public float jumpHeight = 20f;
    private float gravity = -9.81f;

    //1st person movement
    public float speed = 6f;
    
    Vector2 inputs;
    private bool isGrounded;
    public bool IsGrounded => isGrounded;     // the Name property, getter

    bool isNextToWall;
    Vector3 wallNormal;
    Vector3 velocity = Vector3.zero;
    Vector3 move;
    Vector3 v_0;
    private float timeOnWall = 0;
    //Vector3 position_0;

    //////////////////////////////////GrappleTestCode//////////////////////////////////////////////
    void Start() {
        
        playerInput = GetComponent<PlayerInput>();
        //position_0 = transform.position;
        //StartGrapple(grappleTarget);
        v_0 = velocity;
    }


    public void StartGrapple(Vector3 grPos) {   
        //line     
        grappleTarget = grPos;
        grappleDistance = Vector3.Distance(transform.position, grappleTarget);
        isGrappled = true;
        //make sure distance from point stays the same 
        
    }


    
    public void StopGrapple() {
        isGrappled = false;
        grappleTarget = Vector3.zero;
        grappleDistance = 0;
    }
/////////////////////////////////////////////////////////////////////////////////////////////////////////


    // Update is called once per frame
    void Update()
    {
        

        //if regular moving
        /*
        controller.Move(move * speed * Time.deltaTime);
        */

        inputs = playerInput.input;
        //Gravity

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //Debug.Log(isGrounded);
        //CollisionFlags.Below

        //if can climb/slide
		/**/
        isNextToWall = MovingTowardsWall(groundMask, out wallNormal, inputs);
        /*if(isNextToWall)
            print("wall" + wallNormal);
        */
    }

    private void FixedUpdate() {
        //position
        //v_0 = (transform.position-position_0)/Time.deltaTime;

        //////////////////////////////GRAPPLE CODE//////////////////////////////////
        

        if(isGrappled){
            grapple(grappleTarget, grappleDistance, inputs);
        }
		else if(isNextToWall && !isGrounded){
			wallSlide(wallNormal, inputs);
		}
        else{
            normal(inputs);
        }

        ////////////////////////////////////////////////////////////////////////////
        
        /////////////////////////////Acceleration test/////////////////////////////
        /*float sqrAcc = Vector3.SqrMagnitude((velocity-v_0)/Time.deltaTime);*/
        
        Debug.DrawLine (transform.position, transform.position+velocity, Color.cyan);
        //Debug.DrawLine (transform.position, transform.position+v_0, Color.cyan);

        controller.Move(velocity* Time.deltaTime);
        //print(Vector3.SqrMagnitude(velocity));

        
        //Vector3 deltaX = transform.position - (position_0 + v_0*Time.deltaTime);//3rd law?
        //Vector3 deltaV = v_0-(transform.position - position_0)/Time.deltaTime;
        
        ///////////////////////Prev Velocity from Position///////////////////////// 
       
       // v_0 = (transform.position-position_0)/Time.deltaTime;//inside fixed update, deltatime becomes fixedDeltaTime
        //velocity = v_0+Vector3.up*gravity*Time.deltaTime;
        //position_0 = transform.position;
        //v_0 = velocity;

    }
    void OnDrawGizmos() {//groundcheck
        Gizmos.color = Color.red;
        Gizmos.DrawSphere (groundCheck.position, groundDistance);

    }

    Vector3 quadFriction(Vector3 velocity, float coef = 0.5f){////cst = density diff* volume
        Vector3 frictVel = Vector3.zero;
        float frict = coef * Vector3.SqrMagnitude(velocity) * Time.deltaTime/2; 
        //F_r = c* v**2  
        frictVel = -frict*velocity/Vector3.Magnitude(velocity);

        return frictVel;
    }

    Vector3 linFriction(Vector3 velocity, float coef = 0.5f){
        Vector3 frictVel = Vector3.zero;
        //F_r = c* v   
        frictVel = -coef*velocity* Time.deltaTime;
        return frictVel;
    }

    Vector3 Crouch(Vector3 velocity, float crouchSpeed, Vector2 inputs){ 
        Vector3 v_current;
        v_0 = velocity;
        float time = Time.deltaTime;
        float mu = 0.5f;

        if(Vector2.SqrMagnitude(new Vector2(velocity.x,velocity.z))>(crouchSpeed*crouchSpeed)){
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


        if( v_0 != Vector3.zero){ 

            v_current = Vector3.Lerp(v_0,Vector3.zero,mu*gravity*time); 
            return v_current;
        }
        else{
            return Vector3.zero;
        } 

    }

    void normal(Vector2 inputs){
        //1st person, character rotation with camera

        move = (transform.right * inputs.x + transform.forward * inputs.y)*speed;
        
        

        if(isGrounded && velocity.y<=0) {
           
            velocity.y = -1f;//-1f;
            if (Input.GetButtonDown("Jump")){
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

    


    void grapple(Vector3 target, float distance, Vector2 inputs){//shorten over time?
        
        Vector3 ropeVect; 
        Vector3 sideVect;
        Vector3 pullDir;
        float pullSpeed = 0;//"attraction"
        float angle;
        float l = distance;
        Vector3 correction;
        
        ropeVect = (target - transform.position);
        angle = Vector3.Angle(Vector3.up, ropeVect)* Mathf.Deg2Rad;

        if ((transform.position-target).sqrMagnitude < distance*distance ) {//not in tension
            normal(inputs);
            if (Input.GetButtonDown("Jump")){
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else if (angle > 1.5708f){//in tension (), above 90 degrees
			normal(inputs);

            ropeVect = ropeVect.normalized;
            if((transform.position-target).sqrMagnitude >= distance*distance){
                v_0 = velocity;
                controller.enabled = false;
                transform.position = target-ropeVect*distance;
                controller.enabled = true;
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
            
            controller.enabled = false;
            transform.position = target-ropeVect*distance;
            controller.enabled = true;
            
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
            if (Input.GetButtonDown("Jump")){
                velocity.y += Mathf.Sqrt(-2f* jumpHeight * gravity);//*ropeVect.y
                
            }    
            /*
            if(Input.GetButtonDown("Shift")){
                if(distance){

                }
            }
            if(Input.GetButtonDown("Ctrl")){
            }
            */
            
        }   

    }


	
    
    void wallSlide( Vector3 wallNormal ,Vector2 inputs){//just check initially, and keep wall dir until change?
        float gVel = gravity*Time.deltaTime;
		Vector3 gravDir = Vector3.ProjectOnPlane(Vector3.down, wallNormal);
        //float v_y = (cst*g/b)*(1-Mathf.Exp(-b*t/m));//lerp((0,(cst*g/b),-b*t/m) //check which is better
		
		float cst = Mathf.Lerp(0.75f, 0.5f, wallNormal.y);//Mathf.Max(wallNormal.y, 0.125f);
		
		velocity+= gVel*gravDir;
		velocity+= quadFriction(velocity, cst);//friction, due to movement gets larger than gravity
        
        //sigmoid derivative, for vel variation; slow then fast
		
        //inputs
		
        move = (transform.right * inputs.x + transform.forward * inputs.y)*speed;
		
		Vector3 sideDir = Vector3.Cross(wallNormal, gravDir);//already normalized, sin 90 = 1
        Vector3 sideMove = Vector3.Project(move, sideDir);

        //velocity += Vector3.Project(move,gravDir);
        velocity.x = sideMove.x;
        velocity.z = sideMove.z;

		//velocity +=inputs.x;

        //jump: diagonal between normal and plane

        if (Input.GetButtonDown("Jump")){
            Vector3 jumpDir = -gravDir + wallNormal;
            velocity += jumpDir*Mathf.Sqrt(-2f* jumpHeight * gravity)*0.7071f;
        } /**/   

		
		
    }

    public bool MovingTowardsWall(LayerMask layer, out Vector3 wallNormal, Vector2 inputs){ // heavily edited from colanderp "isnexttowall"
        float radius = 0.5f;//playerRadius, get
        move = (transform.right * inputs.x + transform.forward * inputs.y);
		
		Vector3 top = transform.position+Vector3.up*0.4375f;//-height/4 //to start lower
        Vector3 bottom = transform.position-Vector3.up*0.25f;
        RaycastHit hit;
		float maxDist = 0.25f;
		
        if(move != Vector3.zero){
            if(Physics.CapsuleCast(top, bottom, radius, move , out hit, maxDist, layer)){
                wallNormal = hit.normal;

                float projectWall = wallNormal.y;//normalized, hence angle directly
		
                if(projectWall <= 0.375f && projectWall >= -0.125f ){ 
			        //wallslide
                    return true; 

		        }
                /*else if(projectWall > 0.375f){
			        //slope
		        }*/
            }
            
        }
        

        wallNormal = Vector3.zero;
        return false;
		//if(Physics.SphereCast(startPos, radius, transform.up, out hit, maxDist, layer)){//throws sphere in direction indicated
        //return (Physics.CapsuleCastAll(top, bottom, radius, move , maxDist, layer, out hit).Length >= 1);//throws capsule in direction indicated
    }

    /*
    void OnControllerColliderHit(ControllerColliderHit hit){
    }
    */

}


//friction, assuming linear, F = -mu Fnorm
// F = m*a = -mu*m*g
// a = -mu*g
//v = v_0-mu*g*t

