
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public LayerMask inTheWay;
    public Transform playerBody;
    float mouseX;
    float mouseY;
    float mouseSensitivity = 200f;

    float xRotation = 0f;

    float distanceRadius= 0f;
    public float maxRadius = 20f;

    // Start is called before the first frame update

    //3rd person

    /*
    private float horizontal;
    private float vertical;
    float targetAngle;
    float angle;
    public float turnSmoothTime=0.1f;
    float turnSmoothVelocity;
    */
    //

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        //transform.localPosition = new Vector3(0f, 1f, -5f);
        //transform.localRotation = Quaternion.Euler(3.3f,0f,0f);

        //print(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y")* mouseSensitivity * Time.deltaTime;//20
        
        xRotation -=mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        //localposition, camera relative to object
        //Debug.Log(transform.localPosition);

        //zoom in and out
        if((distanceRadius+ Input.mouseScrollDelta.y)>=0  &  (distanceRadius+ Input.mouseScrollDelta.y) <= maxRadius){
            distanceRadius +=Input.mouseScrollDelta.y;
        }
        
        transform.localPosition = new Vector3(distanceRadius/8, distanceRadius*Mathf.Sin(xRotation*Mathf.Deg2Rad)+0.5f, -distanceRadius*Mathf.Cos(xRotation*Mathf.Deg2Rad));//camera position,
                
        //x axis,  shifts to the side; distanceRadius/10
        transform.localRotation = Quaternion.Euler(xRotation,0f,0f);//rotate camera
        playerBody.Rotate(Vector3.up* mouseX);//rotate player, and camera by parenting
    
        //for more complex 3rd person, edit player rotate more smoothly, when condition met



        //move hands up with camera?

        
        //interact here?
        CheckOcclusionCollision();
    }

    


    private void CheckOcclusionCollision(){//or cinemachine

        RaycastHit hit;


        //Debug.DrawLine(transform.parent.position, transform.position, Color.green);
        //check if ray hits something
        if (distanceRadius > 0f && Physics.Linecast(transform.parent.position, transform.position, out hit, inTheWay)){
            distanceRadius = Mathf.Clamp(hit.distance, 0, maxRadius); 
            
        }

    }




}
