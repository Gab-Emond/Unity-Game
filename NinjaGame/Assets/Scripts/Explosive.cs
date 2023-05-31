using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    float radius;
    float rayNum;

    float[] damageRange = new float[2];//reminder, array fixed size, list is changeable but slower access


    void Explode() {
        var exp = GetComponent<ParticleSystem>();
        exp.Play();

        //https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html
        //https://docs.unity3d.com/ScriptReference/Rigidbody.AddExplosionForce.html
        
        //if(<radius*radius){}
        
    }


    void ExplosionDamage(Vector3 center, float radius,float forceAtCenter)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.SendMessage("AddDamage");//other way of calling class in other object
            if (hitCollider.attachedRigidbody)
            {
                float distanceSqr = Vector3.SqrMagnitude(hitCollider.transform.position-center);
                hitCollider.attachedRigidbody.AddExplosionForce(forceAtCenter/distanceSqr,center,radius,0,ForceMode.Impulse);
            }
        }
    }
}