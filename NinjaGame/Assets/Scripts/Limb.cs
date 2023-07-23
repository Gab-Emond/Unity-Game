using UnityEngine;

public class Limb : MonoBehaviour {
    public bool active;
    public float maxLength;

    public Vector3 CurrPos;

    public void Activate(){
        active = true;
    }
    public void Retarget(Transform _targ){

        //get last limb position, as old target pos
        //lerp between old target pos and new target pos
        //set speed
    }

}