using UnityEngine;

public class Limb : MonoBehaviour {
    public Transform targetPos;
    Transform targetGroundPos;// can keep wall normal here (transform.forward)
    bool isGrounded;
    public bool IsGrounded => isGrounded;
    bool isActive;//true: used for climb, false otherwise
    public bool IsActive{get; set;}    
    public float maxLength;

    public Vector3 CurrPos;

    Vector3 movementCenter;//relative to player COM
    public Vector3 MovementCenter => movementCenter;

    //note: keep movement center out of the way of other movement centers, depending on dir(legs or arms)

    float maxLength;
    public float MaxLength => maxLength;
    Transform limb_Origin;
    
    Vector3 wallNormal;
    //set bounds(min max that center can reach before having to move, depending on speed)
    
    TwoBoneIKConstraint limbIk;// the inverse kinematics reference
    float weight;//the ik importance

    Vector3 comSpeed; //speed of center of mass (player velocity, if moving not sliding)
    float limbSpeedSlow, limbSpeedFast;
    
    public AnimationCurve heightCurve;

    public void Activate(){
        isActive = true;
    }
    public void Retarget(Transform _targ){

        //get last limb position, as old target pos
        //lerp between old target pos and new target pos
        //set speed
    }

}






        