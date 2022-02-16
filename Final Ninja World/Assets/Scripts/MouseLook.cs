
using UnityEngine;

public class MouseLook : MonoBehaviour
{


    public Transform playerBody;
    float mouseX;
    float mouseY;
    float mouseSensitivity = 200f;

    float xRotation = 0f;

    float distanceRadius= 0f;

    // Start is called before the first frame update
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
        if((distanceRadius+ Input.mouseScrollDelta.y)>=0  &  (distanceRadius+ Input.mouseScrollDelta.y) <= 20){
        distanceRadius +=Input.mouseScrollDelta.y;
        }
        
        transform.localPosition = new Vector3(0, distanceRadius*Mathf.Sin(xRotation*Mathf.Deg2Rad)+0.5f, -distanceRadius*Mathf.Cos(xRotation*Mathf.Deg2Rad));

        transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
        playerBody.Rotate(Vector3.up* mouseX);
    



    }
}
