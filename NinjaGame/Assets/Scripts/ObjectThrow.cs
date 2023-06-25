using UnityEngine;
//using Limb;
public class ObjectThrow : MonoBehaviour {
    Limb throwingArm;

    //Event throw;

    Transform objectThrown;

    void Throw(){
        if(!throwingArm.Active){
            throwingArm.Activate();
        }
        throwingArm.Retarget();
    }
}