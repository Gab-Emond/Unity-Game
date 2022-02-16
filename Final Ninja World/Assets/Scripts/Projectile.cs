using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 prevPos;
    private RaycastHit hitInfo;
    public GameObject impactEffect;
    private GameObject projectile;

    //trailrenderer? selon si on veut

    // Update is called once per frame
    void Update()
    {
        //transform.position = transform.position + new Vector3( * movementSpeed * Time.deltaTime, verticalInput * movementSpeed * Time.deltaTime, 0);
        Vector3 direction = transform.position-prevPos;
        if (Physics.Raycast(prevPos, direction, out hitInfo, 5f)){
            //projectile hit something

            //hitInfo
            //Destroy(GameObject projectile);
        }

    }

    void LateUpdate() 
    {
        prevPos = transform.position;//take late position to see if hit
    }
    // Start is called before the first frame update
    void OnParticleCollision(GameObject other) {
    }
    

    private IEnumerator DestroyBulletAfterTime(GameObject other, float delay){
        //delete bullet
        yield return new WaitForSeconds(delay);
        Destroy(other);
    }
    //1-move projectile forwards (rigidbody physics / translate)
    
    //2-lateupdate, keep track of previous frame position

    //3-cast ray from previous position to current position

    //4-use ray to check if projectile hit something between last and current position
    
    //5-use hit.normal to reflect hit effect 
    
    //6-remove projectile
    


}
