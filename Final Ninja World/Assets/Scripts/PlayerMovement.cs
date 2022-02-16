
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public CharacterController controller;
    
    //jump+gravity
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.2f;

    public float jumpHeight = 20f;
    private float gravity = -9.81f;

    //1st person movement
    public float speed = 6f;
    private float x;
    private float z;
    private bool isGrounded;
    Vector3 velocity;
    Vector3 move;
    //3rd person

    /*
    public Transform cam;

    private float horizontal;
    private float vertical;
    float targetAngle;
    float angle;
    public float turnSmoothTime=0.1f;
    float turnSmoothVelocity;
    */
    //
    

    // Update is called once per frame
    void Update()
    {
        
    



        //1st person, assuming character rotation with camera
        
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        move = transform.right * x + transform.forward * z;//new Vector3(x,0,z);//to move in global coordinates

        controller.Move(move * speed * Time.deltaTime);
        
        
        
        
        //3rd person////nah

    /*
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal,0f, vertical).normalized;//new Vector3(x,0,z);//to move in global coordinates

        if(direction.magnitude>=0.1f){

            targetAngle = Mathf.Atan2(direction.x,direction.z)*Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f,angle,0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f)* Vector3.forward;

            controller.Move(direction * speed * Time.deltaTime);

        }
    */




        //Gravity

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //Debug.Log(isGrounded);
        //CollisionFlags.Below

        if(isGrounded && velocity.y<0) {
           
            velocity.y = -1f;
            if (Input.GetButtonDown("Jump")){
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                //Debug.Log(velocity.y);
            }   
        }
        else
        {
            velocity.y += gravity*Time.deltaTime; //gravity negative

        }
       //Debug.Log(velocity.y);


        controller.Move(velocity* Time.deltaTime);
    
    }
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere (groundCheck.position, groundDistance);

    }

}
