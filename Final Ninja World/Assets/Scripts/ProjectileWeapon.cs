using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapons : MonoBehaviour
{
    
    public float speed;
    public float lifeTime;
    Vector3 direction;


    public GameObject projectilePrefab;
    public Transform projectileSpawn;
   
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
         if(Input.GetKeyDown("mouseKey1")){
           Shoot();
        }
        



    }
    
    //replace instantiate (Instantiate(playerBullet, turret.transform.position, turret.transform.rotation);)
    void Shoot(){
        
        Vector3 bulletPos = projectileSpawn.position;//projectile.transform.position
        Quaternion bulletDirection= projectileSpawn.rotation;
        //Vector3 v = bulletDirection.eulerAngles;


        GameObject projectile = Instantiate(projectilePrefab, bulletPos, bulletDirection);
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), projectileSpawn.parent.GetComponent<Collider>());

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        
        rb.AddForce(-speed*projectileSpawn.forward, ForceMode.Impulse);

        StartCoroutine(DestroyProjectileAfterTime(projectile, lifeTime));
        //startcor
    }

    

    private IEnumerator DestroyProjectileAfterTime(GameObject other, float delay){
        //delete bullet
        yield return new WaitForSeconds(delay);
        Destroy(other);
    }
    
    
    
    
    



}