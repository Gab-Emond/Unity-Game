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

        if(!throwingArm.IsActive){
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
        float lerpTime=0;


        while (lerpTime<timeToReachArmLength)
        {
            lerpTime+=Time.deltaTime;
            //yield return null;
        }

        //simple timer, arm retarget does throwing motion

        //playerIkController.EndMovement(this);
        
        //ik controller script decides and sends start message movement 
        //stop targetting after, send end movement message (to global ik controller script)


    }
}


