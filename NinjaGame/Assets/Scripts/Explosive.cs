using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    float radius;
    float rayNum;

    float[] damageRange = new float[2];//reminder, array fixed size, list is linked


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void Explode() {
        var exp = GetComponent<ParticleSystem>();
        exp.Play();

        //https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html
        //https://docs.unity3d.com/ScriptReference/Rigidbody.AddExplosionForce.html
        
        //if(<radius*radius){}
        
    }


    void ExplosionDamage(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.SendMessage("AddDamage");
        }
    }
}