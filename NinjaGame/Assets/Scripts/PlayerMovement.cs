
using UnityEngine;
using Utility.Math;

public class PlayerMovement : MonoBehaviour
{
    //todo: hitbox "pinching": if lower ceiling, can go under, (or ledge stay at same leve?)
    
    public CharacterController controller;
    PlayerInput playerInput;

    /////////animator(temp?)
    PlayerController playerController; 
    ////

    //grappleTest

    //public Grapple playerGrapple;    
    public Vector3 grappleTarget;
    private bool isGrappled = false;
    public bool IsGrappled => isGrappled;     // the Name property, getter
    private float grappleDistance;//grapple
    //jump+gravity
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.5f;
    public float jumpHeight = 20f;
    private float gravity = -9.81f;
    public float speed = 6f;
    
    Vector2 inputs;
    bool jumpKeyDown;
    public bool JumpKeyDown => jumpKeyDown;

    bool jumpKeyHeld;

    public float shortJumpMultiplier = 2f;
    private bool isGrounded;
    public bool IsGrounded => isGrounded;     // the Name property, getter
    ///////////////////////////
    bool isMovingIntoWall;
    public bool IsMovingIntoWall => isMovingIntoWall;

    bool isNextToWall;
    public bool IsNextToWall => isNextToWall;
    ////////////////////////////Slope
    bool isOnWall;
    public bool IsOnWall => isOnWall;

    /////////////////////////////test
    bool isCrouching;
    public bool IsCrouching => isCrouching;     //getter
    Vector3 slopeNormal;
    Vector3 wallNormal;
    Vector3 velocity = Vector3.zero;
    public Vector3 Velocity => velocity;
    Vector3 move;
    Vector3 v_0;
    //private float timeOnWall = 0;
    //Vector3 position_0;

    //todo: use charInfo when using playercontroller for stamina
    public float maxStamina = 10f;
    float stamina = 0f;
    public float Stamina => stamina;//getter

    //////////////////////////////////GrappleTestCode//////////////////////////////////////////////
    void Start() {
        stamina = maxStamina;
        //playerGrapple = GetComponentInChildren<Grapple>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        //position_0 = transform.position;
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
        jumpKeyDown = playerInput.Jump;
        jumpKeyHeld = playerInput.JumpHeld;

        RaycastHit hitInfo;
        ///////////////////checksphere
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        //////////////////spherecast
        isGrounded = Physics.SphereCast(groundCheck.position, groundDistance, -transform.up, out hitInfo, 0.125f);

        isOnWall = isGrounded && hitInfo.normal.y < 0.375f;
        if(isOnWall){
            slopeNormal = hitInfo.normal;
            Debug.DrawLine(hitInfo.point,hitInfo.point+5*slopeNormal,Color.yellow);
            //print(hitInfo.normal.y);
        }
        ///////////////////////////////////////overlapsphere
        //Collider[] groundColliders = new Collider[1];//Todo;
        
        // int numColliders = Physics.OverlapSphereNonAlloc(groundCheck.position, groundDistance, groundColliders, groundMask);//OverlapSphereNonAlloc
        // if(numColliders>0){
        //     isGrounded = true;
        // }
        // else{
        //     isGrounded = false;
        //     isOnWall = false;
        // }


        

        //Debug.Log(isGrounded);

        //if can climb/slide

        isMovingIntoWall = MovingTowardsWall(groundMask, out wallNormal, inputs);

        /*if(isNextToWall)
            print("wall" + wallNormal);
        */
        isCrouching = playerInput.crouching;//tech dept, change to inputcrouching
        //iscrouching = inputcrouching&&grounded&&!grappled
    }

    private void FixedUpdate() {
        //position
        //v_0 = (transform.position-position_0)/Time.deltaTime;

        //////////////////////////////STATE CODE//////////////////////////////////
        

        if(isGrappled){
            grapple(grappleTarget, inputs);
        }
		else if(isMovingIntoWall){//error: if moving into wall from ground without stamina, speed keep increasing && (!isGrounded||isOnWall)
			wallMove(wallNormal, inputs);//todo: put wallmove into normal?
		}


        //else if(isMovingIntoWall && isOnWall){}//is next to slope 
        //else if(isMovingIntoWall && isGrounded)//boost jump up wall, 
        
        //if ground or ok slope, no issue, if steep slope, slide
        else if(isCrouching && isGrounded){
            crouch(inputs);
        }
        else{
            normal(inputs);
            // if(velocity.sqrMagnitude==0){
            //     playerController.SetStatus(Status.idle);
            // }
            // else{
            //     playerController.SetStatus(Status.running);
            // }
        }

        ////////////////////////////////////////////////////////
        if(!isMovingIntoWall){
            stamina=Mathf.Min(maxStamina,stamina+Time.deltaTime);
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
        frictVel = -frict*velocity.normalized;

        return frictVel;
    }

    Vector3 linFriction(Vector3 velocity, float coef = 0.5f){
        Vector3 frictVel = Vector3.zero;
        //F_r = c* v   
        frictVel = -coef*velocity* Time.deltaTime;
        return frictVel;
    }

    void crouch(Vector2 inputs, float crouchSpeed =2f){ //only if grounded
        
        /*if(isGrounded){
            velocity.y = -.5f;
            if(velocity.sqrMagnitude > 0f){
                velocity += linFriction(velocity,5);
            }
        }
        else{
            velocity.y += gravity*Time.deltaTime;//gravity negative
            velocity += linFriction(new Vector3(velocity.x,0,velocity.z),5);
        }*/

        //if on steep slope
        if(isOnWall){

       		Vector3 gravSlope = -gravity*Vector3.ProjectOnPlane(Vector3.down, slopeNormal)*Time.deltaTime;//todo; change down to gravdir, if changes

            velocity += gravSlope;
            velocity += linFriction(velocity,5);
            

        }
        else{
            velocity.y = -1f;
            if(velocity.sqrMagnitude > 0f){
                velocity += linFriction(velocity,5);
            }
        }

    }

    void normal(Vector2 inputs){
        //1st person, character rotation with camera

        move = (transform.right * inputs.x + transform.forward * inputs.y)*speed;


         //if(on wall)
        //wallmove
        //if(crouched)
        //crouchmove?

        if(isOnWall){
            print("onWall");
            //sliding down hill
            Vector3 gravDir = gravity*Vector3.ProjectOnPlane(transform.up, slopeNormal)*Time.deltaTime;//todo; change down to gravdir, if changes

            velocity += gravDir;
            velocity += linFriction(Vector3.Project(velocity, gravDir),2f);
            
            //Debug.DrawLine(transform.position,transform.position+5*Vector3.Project(velocity,wallNormal), Color.red);
            //Debug.DrawLine(transform.position,transform.position+5*slopeNormal,Color.red);
            if(Vector3.Dot(velocity,slopeNormal)<0){//if velocity going into wall
                velocity -=Vector3.Project(velocity,slopeNormal);
            }
            if(Vector3.Dot(move,slopeNormal)>=0){//if not moving towards wall
                Vector3 velAcc = Vector3.Project(velocity, Vector3.ProjectOnPlane(transform.up, slopeNormal));
                velocity = move;
                velocity +=velAcc;
            }
            if (jumpKeyDown){
                Vector3 jumpDir = -gravDir*0.5f + slopeNormal*0.5f;
                velocity += jumpDir*Mathf.Sqrt(-2f* jumpHeight * gravity);
            }
        }
        else if(isGrounded && velocity.y<=0) {

            velocity.y = -1f;//-1f;
            if (jumpKeyDown){
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.x = move.x;
            velocity.z = move.z;

        }
       
        else
        {//if in the air

            //to add lower jump if key released before max
            if(!jumpKeyHeld && velocity.y>0){
                velocity.y += shortJumpMultiplier*gravity*Time.deltaTime;
            }
            else{
                velocity.y += gravity*Time.deltaTime; //gravity negative
            }
            

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
            if (jumpKeyDown){
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else if (angle > 1.5708f){//in tension (), above 90 degrees
			normal(inputs);

            ropeVect = ropeVect.normalized;
            if((transform.position-target).sqrMagnitude >= grappleDistance*grappleDistance){
                v_0 = velocity;
                controller.enabled = false;
                transform.position = target-ropeVect*grappleDistance;
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
            transform.position = target-ropeVect*grappleDistance;
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

            //basic slow down; slower at higher speed (input/speedindir**2)?
            
            //option: set maximum amount of velocity change (in pulling direction) per swing, resets at bottom


            velocity += (Vector3.Project(transform.forward, pullDir)/2 +Vector3.Project(transform.forward, sideVect)/4)*inputs.y;//6*Time.deltaTime*
            velocity += (Vector3.Project(transform.right, pullDir)/2 +Vector3.Project(transform.right, sideVect)/4)*inputs.x;//6*Time.deltaTime*
            if (jumpKeyDown){
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
            // else{//not that fun
            //     velocity.y += Mathf.Sqrt(-2f* jumpHeight * gravity);//*ropeVect.y
            //     playerGrapple.StopGrapple();
            // }
        }
        

    }

    public Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)//input toworldspace(=move), then to plane, then multiply by speed
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);//_characterUp);
        return Vector3.Cross(surfaceNormal, directionRight).normalized;
    }



	
    //(wallClimb slight, before fall; enter and leave state?)
    //include all cases in wallslide(done)
    //include stamina(done)
    //keep player attached (wallslide) for short time if no input towards wall

    void wallMove( Vector3 wallNormal ,Vector2 inputs){//input already towards wall, to be in wall slide
        float cst;//friction constant
        float gVel = gravity*Time.deltaTime;
		Vector3 gravDir = Vector3.ProjectOnPlane(Vector3.down, wallNormal);
        //float v_y = (cst*g/b)*(1-Mathf.Exp(-b*t/m));//lerp((0,(cst*g/b),-b*t/m) //air resistance

        if (stamina>0){//exhaustion mechanic
            //climb up
            //wallclimb
            //slowed by friction 
            velocity+= gVel*gravDir;
            //velocity-= (speed/2)*gravDir*Time.deltaTime;
            stamina=stamina-Time.deltaTime;
            cst = 5f;
            velocity+= linFriction(Vector3.Project(velocity, gravDir), cst);
        }

        else{
            //slide down
            //wallslide
            velocity-= gVel*gravDir;
            cst = 10f;//Mathf.Max(wallNormal.y, 0.125f);
            if(Vector3.Dot(gravDir,velocity) > 0){
                Debug.DrawLine(transform.position, transform.position+10*quadFriction(Vector3.Project(velocity, gravDir), cst), Color.magenta);
                velocity+= quadFriction(Vector3.Project(velocity, gravDir), cst);//Vector3.Project(velocity, gravDir)
            }
        }
        
		
		
		        
        //velocity+= quadFriction(Vector3.Project(velocity, gravDir), cst);
        
        /*//only necessary if velocity not overriden
        if(Vector3.Dot(wallNormal,velocity) < 0){
            velocity -= Vector3.Project(velocity, wallNormal);
        }
        */

        //sigmoid derivative, for vel variation; slow then fast
		
        
		
        Vector3 accel = Vector3.Project(velocity, gravDir);

        //inputs
        Vector3 _move = (transform.right * inputs.x + transform.forward * inputs.y)*speed;//;
		
		Vector3 sideDir = Vector3.Cross(wallNormal, gravDir);//already normalized, sin 90 = 1
        Vector3 sideMove = Vector3.Project(_move, sideDir);
        
        //Vector3 upMove = Vector3.Project(_move,wallNormal);
        

        velocity = sideMove;//move with player input + constand down speed

        velocity += accel;


        //velocity.x = sideMove.x;
        //velocity.z = sideMove.z;
        

        //jump: diagonal between normal and plane

        //todo; make less crazy
        
        if (jumpKeyDown){
            Vector3 jumpDir = -gravDir*0.5f + wallNormal*0.75f;
            velocity += jumpDir*Mathf.Sqrt(-2f* jumpHeight * gravity);
        } /**/   

		
		
    }

    public bool MovingTowardsWall(LayerMask layer, out Vector3 wallNormal, Vector2 inputs){ // heavily edited from colanderp "isnexttowall"
        float radius = 0.5f;//playerRadius, get
        
        Vector3 _move = (transform.right * inputs.x + transform.forward * inputs.y);
		
        //Vector3 sideDir = new Vector3(velocity.x,0,velocity.z);//velocity/velocity.sqrMagnitude;
		
        Vector3 top = transform.position+Vector3.up*0.4375f;//-height/4 //to start lower
        Vector3 bottom = transform.position-Vector3.up*0.45f;
        RaycastHit hit;
		float maxDist = 0.165f;
		
        if(_move != Vector3.zero){
            if(Physics.CapsuleCast(top, bottom, radius, _move , out hit, maxDist, layer)){
                wallNormal = hit.normal;

                float projectWall = wallNormal.y;//normalized, hence angle directly
                //print(projectWall);
                
                

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

    /**/
    void OnControllerColliderHit(ControllerColliderHit hit){

        if(isGrappled){
            //print("hit speed: "+ Vector3.Project(velocity,hit.normal));
            velocity -= Vector3.Project(velocity,hit.normal);
            //todo: fix floating issue while on ground

            //print("result: "+ velocity);
        }
    }
    

}


//friction, assuming linear, F = -mu Fnorm
// F = m*a = -mu*m*g
// a = -mu*g
//v = v_0-mu*g*t

