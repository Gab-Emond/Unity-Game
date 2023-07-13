using UnityEngine;
//using Limb;
public class ObjectThrow : MonoBehaviour {
    Limb throwingArm;

    //Event throw;
    Transform projectileSpawn;
    Projectile objectThrown;

    IKController playerIkController;

    void Throw(Vector3 velocity){//thrown velocity 
        
        //if was still throwing, end throw 
        //(or/and set time between throw to always larger than throw)?

        if(!throwingArm.Active){
            throwingArm.Activate();
        }
        
        throwingArm.Retarget(objectThrown.transform);
        //end throw naturally after time
        //or end throw once max exten



        // to find the progression needed to reach projectile before throw

        //knowing projectile direction and limb length+ limb origin, 
        //by the time or before having reached max point it needs to have reached
        //Vector3 endPos = Limb.CurrPos+projectileDir;
        //float timeToReachArmLength= (projectileSpawn.position-throwingArm.CurrPos).magnitude/(2*velocity.magnitude);

        Vector3 relVect = projectileSpawn.position-throwingArm.CurrPos;//dx
        //Vector3 dv = velocity;

        float timeToReachArmLength = throwingArm.MaxLength*throwingArm.MaxLength*(velocity.sqrMagnitude)/Vector3.Dot(relVect,velocity);
        float lerpTime=Time.time;


        while (lerpTime<timeToReachArmLength)
        {
            
        }

        //playerIkController.EndMovement(this);
        
        //ik controller script decides and sends start message movement 
        //stop targetting after, send end movement message (to global ik controller script)


    }
}

public class IKController {

    

    //changeMovement: 
    //public void EndMovement(IKMovement movement){}//message recieved telling it movement was ended

     //end the movement currently playing; also decide if need to start or end next movement
}

abstract class IKMovement {//general class for all movements done with ik
    //void Enter();//setup for start of movement
    //void Exit();//cleanup to end the movement
}


public class Limb {
    public bool Active;
    public float MaxLength;

    public Vector3 CurrPos;

    public void Activate(){

    }
    public void Retarget(Transform _targ){

    }

}

