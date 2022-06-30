using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//raycast, mostly just work on instantaneous shots, trail renderer cant simulate travel time perfectly, if target moves
public class RayWeapon : MonoBehaviour
{
    private ParticleSystem ImpactParticleSystem;
    private TrailRenderer bulletTrail;

    public float range = 500f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shoot()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray (transform.position, transform.forward);

        if (Physics.Raycast (ray, out hitInfo, 100)) {
			//print (hitInfo.collider.gameObject.name);
			//Destroy (hitInfo.transform.gameObject);
			//Debug.DrawLine (ray.origin, hitInfo.point, Color.red);

			//line renderer


		}
        
    }
}