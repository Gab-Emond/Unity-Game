using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WallMovementAnimRigging : MonoBehaviour {
    Transform leftLegTransf;
    Transform rightLegTransf;
    Transform playerBody;
    PlayerMovement playerMovement;
    public Vector3 wallNormal;
    Vector3 wallPos;

    bool leftHandOccupied;    
    bool rightHandOccupied;

    Limb leftArm;
    Limb rightArm;
    Limb leftLeg;
    Limb rightLeg;    

    IEnumerator main = mainController();
    bool noLimbsOnGround = true;
    
    private void Start() {
        //initialize limbs 
        leftArm = new Limb();
        rightArm = new Limb();
        leftLeg = new Limb();
        rightLeg = new Limb(); 
    }
    
    //todo: set for different limb bounds per direction faced (3d zone around limb movement center)
    IEnumerator mainController() {//set up in coroutine
        //set ArmLegsIK

        //check which limbs being used

        //arms should do opposite to leg, and both sets opposite to each other

        while (true)//loop that contains all actions, gives control of time between loop
        {

            yield return new WaitForSeconds(.1f);
 
        }

        //if no limbs used, at start, closest limbs reach first
        SetArmsLegsIk();
        //if(facing straight||facing back)
        if(rightArm.IsGrounded||leftLeg.IsGrounded){
            if(rightLeg.IsGrounded){StartCoroutine(rightLeg.TranslateBetweenGroundPoints());}
            if(leftArm.IsGrounded){StartCoroutine(leftArm.TranslateBetweenGroundPoints());}
        }
        else if(leftArm.IsGrounded||rightLeg.IsGrounded){//is grounded (and inner is active)
            if(leftLeg.IsGrounded){StartCoroutine(leftLeg.TranslateBetweenGroundPoints());}
            if(rightArm.IsGrounded){StartCoroutine(rightArm.TranslateBetweenGroundPoints());}
        }
        else{//nothing grounded, like at start
            //connect left arm
            //if not possible, connect right arm,
            //if not possible, connect left leg
            float startTime = Time.time;
            float totTime=.25f;//time to reach wall//(start limb pos - end limb pos(wallclosestpos).magnitude)/armVelocity
            
            Limb targetLimb;

            //Limb.StartPos = WallClosestPos
            //lerp ik weight to close distance
            if(leftArm.IsActive){//should left arm move ()
                
                
                leftArm.targetPos.position = WallClosestPos(leftArm.MovementCenter+rightArm.CurrentBoundsParal(.5)*playerMovement.velocity.normalized,5);
                targetLimb = leftArm;

            }
            else if(rightArm.IsActive){//should right arm move
                rightArm.targetPos.position = WallClosestPos(rightArm.MovementCenter+rightArm.CurrentBoundsParal(.5)*playerMovement.velocity.normalized,5);
                targetLimb = rightArm;
            }
            else{//check closest leg
                if((WallClosestPos(leftLeg.MovementCenter,5)-leftLeg.MovementCenter).sqrMagnitude>(WallClosestPos(rightLeg.MovementCenter,5)-rightLeg.MovementCenter).sqrMagnitude){
                    //if right leg closer to wall than left leg
                    rightLeg.targetPos.position = WallClosestPos(rightLeg.MovementCenter+rightLeg.CurrentBoundsParal(.5)*playerMovement.velocity.normalized,5);
                    targetLimb = rightLeg;
                }
                else{//left closer than right
                    leftLeg.targetPos.position = WallClosestPos(leftLeg.MovementCenter+leftLeg.CurrentBoundsParal(.5)*playerMovement.velocity.normalized,5);
                    targetLimb = leftLeg;
                }
            }
            while (noLimbsOnGround)
            {
                float timePassed = Time.time-startTime;     
                 
                //leftArm.weight = Mathf.Min(1,timePassed/totTime);//lerp(0,1,timePassed)
                if((timePassed/totTime) >=1){
                    noLimbsOnGround = false;
                }           
                yield return null;
            }
        }

        yield return new WaitForSeconds(.1f);

    }



    void SetArmsLegsIk(){
        float angle = WallPlayerAngle();//in degrees
        
        //todo: have leg ik hints constant pos/rot relative to body pos/rot (no code, simple transform)
        //todo(wip): when isactive change, use EaseIn/EaseOut coroutine
        //todo: check if wall is at limb height; when arriving at bottom or top of surface, shouldnt use limbs
        //todo(global): have inactive limbs compensate for weight; 
        //for opposite limbs, find pos to COM, nudge to equal opposite pos to those

        if(-160>angle&&angle>160){//if facing ~ opposite of wall
            if(!leftArm.IsActive){StartCoroutine(leftArm.EaseWeightIn(timeToIn));}
            if(!rightArm.IsActive){StartCoroutine(rightArm.EaseWeightIn(timeToIn));}
        }
            //either hands can connect to wall + legs
            //if ~isGrappled 
        else if(-5<angle&&angle<5){
            if(!leftArm.IsActive){StartCoroutine(leftArm.EaseWeightIn(timeToIn));}
            if(!rightArm.IsActive){StartCoroutine(rightArm.EaseWeightIn(timeToIn));}
        }//if facing ~ same dir as wall normal
            //either hands
            //if ~isGrappled 
        else if(-160<angle&&angle<-5){
            if(!leftArm.IsActive){StartCoroutine(leftArm.EaseWeightIn(timeToIn));}
            if(rightArm.IsActive){StartCoroutine(rightArm.EaseWeightOut(timeToOut));}
        }//if facing ~ left from wall normal
            //left hand needed
            //raycast (from: lefthandorigins, towards: wall normal )
        else if(5<angle&&angle<160){
            if(leftArm.IsActive){StartCoroutine(leftArm.EaseWeightOut(timeToOut));}
            if(!rightArm.IsActive){StartCoroutine(rightArm.EaseWeightIn(timeToIn));}
        }//if facing ~ right of wall normal
            //right hand needed
            //raycast (from: righthandorigins, towards: wall normal )
    }

    Vector3 WallClosestPos(Vector3 startPos, float checkDistance){//wall normal not as var, to change if crease, aka several planes/normals
        //, out bool inRange
        RaycastHit hitInfo;
        
        if(Physics.Linecast(startPos,startPos+wallNormal*checkDistance,out hitInfo)){//set check distance to max limb reach
            //inRange=true;
            return hitInfo.point;
        }
        else{
            //inRange=false;
            return startPos+(wallNormal*checkDistance*0.5f);//or set a default pos for the limb to stay in if cant reach
        }

        //inneficient, use project on plane, but for plane that doesnt pass through origin (simply substract offset from both, however must find offset)
        //

    }

    public Vector3 BoundOnWall(){
        Vector3 wallNormal;
    }

    //either travel cycloid, or travel circle around limb_origin, radius = dist(limb_origin, wall)
    //cycloid
    //x = r(t - sin(t))
    //y = r(1 - cos(t))

    //t = angle by which the circle has rotated
    

    //reducing r by factor for y? to be smaller -> works
    
    //x: velocity vector
    //y: wall normal

    //y=0 when x = xtarget

    //needs positions, vs simple circle vector slerp (or, smaller circle, lower center of rotation; curtate cycloid)

    //for slerp

    //slerp-unclamped

    //anglerotate for 120 degrees, rotate around each part () 

    //rotate around circle, 
    //wait for other limb to finish rotation (touch ground)
    //repeat

    private float WallPlayerAngle(){
        return Vector3.SignedAngle(wallNormal,playerBody.forward,Vector3.up);
    }

    void EnterWall(){
        
        StartCoroutine(main);
        //legs weighted in automatically
        //todo, when limbs deactivated (during rotation or use), ease them out
        //when limb reactivated (post use), ease them back in
        //subscribe to events (methods that use a limbs, event when start and when end)  

    }
    void ExitWall(){//set state change to events, and subscribe?
        // foreach (var item in limb)
        // {
            
        // }
        float timeToEase = .125f;

        if(leftLeg.IsActive){StartCoroutine(leftLeg.EaseWeightOut(timeToEase));}
        if(leftArm.IsActive){StartCoroutine(leftArm.EaseWeightOut(timeToEase));}
        if(rightArm.IsActive){StartCoroutine(rightArm.EaseWeightOut(timeToEase));}
        if(rightLeg.IsActive){StartCoroutine(rightLeg.EaseWeightOut(timeToEase));}

        StopCoroutine(main);
    }



    public class Limb {

        WallMovementAnimRigging parentMovingRig;
        
        public Transform targetPos;
        Vector3 targetGroundPos;
        bool isGrounded;
        public bool IsGrounded => isGrounded&&isActive;
        bool isActive;//true: used for climb, false otherwise
        public bool IsActive{get; set;}
        Vector3 movementCenter;//relative to player COM
        public Vector3 MovementCenter => movementCenter;

        float limbLength;
        Transform limb_Origin;
        
        Vector3 wallNormal;
        //set bounds(min max that center can reach before having to move, depending on speed)

        float boundParalSlow, boundParalFast;//furthest from moveCenter that can be reached, varies according to speed
        float boundPerpSlow, boundPerpFast;
        public float CurrentBoundsParal(float mag)=> Mathf.Lerp(boundParalSlow,boundParalFast,mag);
        float CurrentBoundsPerp(float mag)=> Mathf.Lerp(boundPerpSlow,boundPerpFast,mag);
        Limb oppositeLimb;//perhaps not necessary

        float innerPath;//0 to 1, .5 when back at center 
        
        Vector3 comSpeed; //speed of center of mass (player velocity, if moving not sliding)

        float limbSpeedSlow, limbSpeedFast;

        float CurrentLimbSpeed(float mag)=>Mathf.Lerp(limbSpeedSlow,limbSpeedFast,mag);
        
        //while touching ground, set in world space
        
        Vector3 startPos;//globalPos
        public Vector3 StartPos{get; set;}
        Vector3 endPos;

        TwoBoneIKConstraint limbIk;// the inverse kinematics reference

        float weight;//the ik importance
        
        public AnimationCurve heightCurve;

        public Limb(WallMovementAnimRigging wallMovementAnimRigging,AnimationCurve _hCurve, Vector3 _movCenter,float _limbSpeedSlow, float _limbSpeedFast){
            parentMovingRig = wallMovementAnimRigging;
            heightCurve = _hCurve;
            movementCenter = _movCenter;
        }
        
        //when opposite limb touching ground, can start moving
        public IEnumerator TranslateBetweenGroundPoints(){
            //get time.deltaTime

            Vector3 speedDir = comSpeed.normalized;
            float speedMag = CurrentLimbSpeed(comSpeed.magnitude);//the speed at which the limb will reach the target, regardless of speed changes afterwards
            endPos = movementCenter+speedDir*CurrentBoundsParal(speedMag);//+direction to wall
            targetGroundPos = startPos;

            float timeToReach = (endPos-startPos).magnitude/speedMag;
            float timeSinceStart = 0;
            while (targetGroundPos!=endPos)
            {   
                timeSinceStart+=Time.deltaTime;
                float iter = timeSinceStart/timeToReach;
                targetGroundPos = Vector3.Lerp(startPos,endPos,iter);
                
                float y = heightCurve.Evaluate(iter);//-height*iter*(iter-1);//parametric curve, height is how high it reaches

                targetPos.position = targetGroundPos + y*wallNormal;

                Vector3 newSpeedDir;
                yield return newSpeedDir = comSpeed.normalized;
                speedMag = CurrentLimbSpeed(comSpeed.magnitude);
                
                //if turn too fast, aka speed dir too different from prev speed dir, drop hand faster
                endPos = movementCenter+Vector3.Project(newSpeedDir,speedDir)*CurrentBoundsParal(speedMag);
                yield return timeToReach = (endPos-startPos).magnitude/speedMag;
                //yield return null;
            }
            //once at target,
            startPos = targetPos.position;
            isGrounded = true;
            //change velocity depending on speed

        }
        //x* velocity vector
        //y* wall normal
        //+startingPos: when touching ground/wall, change starting pos again
        IEnumerator TranslateFromIdle(){//needs a specified end Pos
            Vector3 speedDir = comSpeed.normalized;
            float speedMag = CurrentLimbSpeed(comSpeed.magnitude);//the speed at which the limb will reach the target, regardless of speed changes afterwards
            endPos = movementCenter+speedDir*CurrentBoundsParal(speedMag);
            float timeToReach = (endPos-startPos).magnitude/speedMag;
            float timeSinceStart = 0;
            while (targetPos.position!=endPos)
            {   
                timeSinceStart+=Time.deltaTime;
                float iter = timeSinceStart/timeToReach;
                targetPos.position = Vector3.Lerp(startPos,endPos,iter);
                
                Vector3 newSpeedDir;
                yield return newSpeedDir = comSpeed.normalized;
                speedMag = CurrentLimbSpeed(comSpeed.magnitude);
                
                //if turn too fast, aka speed dir too different from prev speed dir, drop hand faster
                
                yield return timeToReach = (endPos-startPos).magnitude/speedMag;
                //yield return null;
            }
            yield return null;
        }

        public IEnumerator EaseWeightIn(float totTime){//start of ik state (useful when limb gets used)
            float startTime = Time.time - totTime*Mathf.InverseLerp(0, 1, middleWeight);
            float timePassed;
            isActive = true;
            while (weight<1){
                timePassed = Time.time-startTime;
                weight = Mathf.Lerp(0,1,timePassed/totTime);
                yield return null;
            }
            isGrounded = true;
            
        }

        public IEnumerator EaseWeightOut(float totTime){//end of ik state(set tot time to small value, to not interfere with other)
            //todo set up to ease from middle, when activated/deactivated too quick
            //inverse(middleWeight) = timePassed/totTime
            //totTime*inverse(middleWeight) = timePassed = Time.time-startTime
            //startTime = Time.time - totTime*inverse(middleWeight);//middleWeight = CurrentWeight
            float startTime = Time.time - totTime*Mathf.InverseLerp(1,0,middleWeight);//takes into account having stopped halfway
            float timePassed;
            isGrounded = false;
            isActive = false;
            while (weight>0){
                timePassed = Time.time-startTime;
                weight = Mathf.Lerp(1,0,timePassed/totTime);
                yield return null;
            }
            
        }
    }

    private void OnDrawGizmos() {
        //draw the positions of limb targets
        Gizmos.color = Color.green;
        Gizmos.DrawSphere( leftArm.targetPos.position, 1);
        Gizmos.DrawSphere(rightArm.targetPos.position, 1);
        Gizmos.DrawSphere(leftLeg.targetPos.position, 1);
        Gizmos.DrawSphere(rightLeg.targetPos.position, 1);
    }
}