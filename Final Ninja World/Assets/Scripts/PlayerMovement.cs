
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public CharacterController controller;
    PlayerInput playerInput;
    //grappleTest
    public Vector3 grappleTarget;
    private bool isGrappled = false;
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
    Vector3 velocity = Vector3.zero;
    Vector3 move;
    Vector3 v_0 ;
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


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
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

        //if climbing

        
    }

    private void FixedUpdate() {
        //position
        //v_0 = (transform.position-position_0)/Time.deltaTime;

        //////////////////////////////GRAPPLE CODE//////////////////////////////////
        

        if(isGrappled){
            grapple(grappleTarget, grappleDistance, inputs);
        }
        else{
            normal(inputs);
        }

        ////////////////////////////////////////////////////////////////////////////
        
        /////////////////////////////Acceleration test/////////////////////////////
        /*float sqrAcc = Vector3.SqrMagnitude((velocity-v_0)/Time.deltaTime);
        if(sqrAcc>100 && isGrappled)
            Debug.Log(sqrAcc);
        */
        


        controller.Move(velocity* Time.deltaTime);
        

        v_0 = velocity;

        ///////////////////////Prev Velocity from Position///////////////////////// 
        //v_0 = (transform.position-position_0)/Time.deltaTime;
        //velocity = v_0+Vector3.up*gravity*Time.deltaTime;
        //position_0 = transform.position;
    }
    void OnDrawGizmos() {//groundcheck
        Gizmos.color = Color.red;
        Gizmos.DrawSphere (groundCheck.position, groundDistance);

    }

    Vector3 quadFriction(Vector3 velocity){
        Vector3 frictVel = Vector3.zero;
        float coef = 1; 
        float frict = coef * Vector3.SqrMagnitude(velocity) * Time.deltaTime/2; 
        //F_r = c* v**2  
        frictVel = -frict*velocity/Vector3.Magnitude(velocity);

        return frictVel;
    }

    Vector3 linFriction(Vector3 velocity){
        Vector3 frictVel = Vector3.zero;
        float coef = 0.5f; 
        //F_r = c* v   
        frictVel = -coef*velocity* Time.deltaTime;
        return frictVel;
    }

    Vector3 CrouchSlide(Vector3 velocity, float mu, float g, float time){ 
        Vector3 v_current;
        Vector3 v_0 = velocity;
        if( v_0 != Vector3.zero){ 

            v_current = Vector3.Lerp(v_0,Vector3.zero,mu*g*time); //see vector3.smoothdamp
            return v_current;
        }
        else{
            return Vector3.zero;
        } 

    }

    async void normal(Vector2 inputs){
        //1st person, assuming character rotation with camera

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

            /*            */

            if(Vector2.SqrMagnitude(new Vector2(velocity.x,velocity.z))>speed*speed){

                //vector projection on velocity
                float parProjection = Vector3.Dot(move, velocity)/Vector3.SqrMagnitude(velocity);//just the length projected on the vector
                if(parProjection<=0){//to see if goes in opposite direction than vector
                    velocity += parProjection*velocity;
                }
                else{//if same direction, dont add

                }
                
                //vector projection on perpendicular to velocity

                //velocity += linFriction(new Vector3(velocity.x,0,velocity.z));
            }
            else{
                velocity.x = move.x;
                velocity.z = move.z;
            }


            //velocity += linFriction(new Vector3(velocity.x,0,velocity.z));

        }

        

    }

    void grapple(Vector3 target, float distance, Vector2 inputs){//move unused
        
        Vector3 ropeVect; 
        Vector3 sideVect;
        Vector3 pullDir;
        float pullSpeed = 0;//"attraction"
        float angle;
        float g = gravity;
        float l = distance;
        Vector3 correction;
        
        ropeVect = (target - transform.position);
        angle = Vector3.Angle(Vector3.up, ropeVect)* Mathf.Deg2Rad;


        if ((transform.position-target).sqrMagnitude < distance*distance) {//not in tension
            normal(inputs);
        
        }
        /*else if (angle > 1.5708f){

        }*/
        else{//in tension
            isGrounded = true;


            ropeVect = ropeVect.normalized;
            //charcontroller overrides position transform
            /**/
            controller.enabled = false;
            transform.position = target-ropeVect*distance;
            controller.enabled = true;
            

            

            
            
            //Debug.DrawLine (transform.position, target, Color.yellow);

            
            
            //theta' = -(g/l)sin(theta)*t (2D)
            //angle'' = -g/l * angle
            //angle'= omega = -g/l * angle * t (sketchy integration)
            //v_t = r*omega, r cancels out for pull speed


            if(angle<0.125){//small angle approx, sin theta => theta
                
                pullSpeed = -g*angle*Time.deltaTime;//*Time.deltaTime
            
            }
            else{
                pullSpeed = -g*Mathf.Sin(angle)*Time.deltaTime;//*Time.deltaTime       

                //print(pullSpeed);
                         
            }


            sideVect = Vector3.Cross(Vector3.up, ropeVect).normalized;//normalized, decide whether to optimize later
            pullDir =  Vector3.Cross(sideVect, ropeVect).normalized;//Quaternion.AngleAxis(-90, ropeVect)*sideVect;
          
            velocity +=pullDir*pullSpeed;
            
            //Debug.DrawLine (transform.position, transform.position+sideVect, Color.red);
           // Debug.DrawLine (transform.position, transform.position+velocity, Color.green);
            correction = (ropeVect*velocity.sqrMagnitude/distance)*Time.deltaTime;            //a_c=v**2/r, towards center
            //Debug.DrawLine (transform.position+velocity, transform.position+correction+velocity, Color.red);
            velocity +=correction;
            
            
            
            //player movement in relation to pulldir

            //basic slow down
            //velocity +=  (Vector3.Dot(transform.forward, pullDir)*pullDir/2 +Vector3.Dot(transform.forward, sideVect)*sideVect/4)*inputs.y;
            //velocity +=  (Vector3.Dot(transform.right, pullDir)*pullDir/2 +Vector3.Dot(transform.right, sideVect)*sideVect/4)*inputs.x;

            velocity +=  (Vector3.Project(transform.forward, pullDir)/2 +Vector3.Project(transform.forward, sideVect)/4)*inputs.y;
            velocity +=  (Vector3.Project(transform.right, pullDir)/2 +Vector3.Project(transform.right, sideVect)/4)*inputs.x;
                        

            
        }

    }



	
    
    void wallSlide(out float v_y, float t, float b, float m, float cst, Vector2 inputs){//cst = density diff* volume
        float g = gravity;

        v_y = (cst*g/b)*(1-Mathf.Exp(-b*t/m));//lerp((0,(cst*g/b),-b*t/m) //check which is better

    }

    public bool hasWallToSide(int dir, LayerMask layer){ // stolen from colanderp
        float radius = 0.5f;
        //Check for ladder in front of player
        Vector3 top = transform.position + (transform.right * 0.25f * dir);
        Vector3 bottom = top - (transform.up * radius);
        top += (transform.up * radius);

        return (Physics.CapsuleCastAll(top, bottom, 0.25f, transform.right * dir, 0.05f, layer).Length >= 1);
    }


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Vector3 wallNorm = hit.normal;

    }

}


//friction, assuming linear, F = -mu Fnorm
// F = m*a = -mu*m*g
// a = -mu*g
//v = v_0-mu*g*t

