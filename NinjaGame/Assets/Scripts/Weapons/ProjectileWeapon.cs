using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProjectileWeapon : MonoBehaviour
{
    public LayerMask hitableLayer;//3rdperson raycast


    //bullet 
    public GameObject projectilePrefab;
    public Transform projectileSpawn;
    public float maxDistance = 200f;

    //bullet force
    public float speed;
    Vector3 direction;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools (statemachine)
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera camera;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
    
    

    //bug fixing :D
    public bool allowInvoke = true;

    // Update is called once per frame
    void Update()
    {
        

        if(Input.GetKeyDown(KeyCode.Mouse0)){
           Shoot();
        }
    
    }

    private void MyInput(){
        //Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading 
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //Set bullets shot to 0
            bulletsShot =0;

            Shoot();
        }
    }
    
    //replace instantiate (Instantiate(playerBullet, turret.transform.position, turret.transform.rotation);)
    void Shoot(){
        /*
        Vector3 bulletDirection;

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxDistance, hitableLayer)){
            bulletDirection = projectileSpawn.position - hit.point;
        }
        else{
            bulletDirection;
        }
        */
        
        //Find the exact hit position using a raycast
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view (from Dave code, otherwise just raycast)
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, maxDistance))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(maxDistance); //Just a point far away from the player


        
        Vector3 spawnPos = projectileSpawn.position;//projectile.transform.position
        Quaternion bulletDirection = Quaternion.LookRotation(targetPoint-spawnPos, Vector3.up);//camera.transform.rotation;//projectileSpawn.rotation
        //Vector3 v = bulletDirection.eulerAngles;


        GameObject projectile = Instantiate(projectilePrefab, spawnPos, bulletDirection);
        //Physics.IgnoreCollision(projectile.GetComponent<Collider>(), projectileSpawn.parent.GetComponent<Collider>());

        /*
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        
        rb.AddForce(-speed*projectileSpawn.forward, ForceMode.Impulse);//speed on weapon insdead of projectile?
        rb.AddForce(camera.transform.up * upwardForce, ForceMode.Impulse);
        */
        projectile.GetComponent<Projectile>().Launch(speed);

    }

    private void ResetShot(){
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload(){
        reloading = true;
        Invoke("ReloadFinished", reloadTime); //Invoke ReloadFinished function with your reloadTime as delay
    }
    private void ReloadFinished(){
        //Fill magazine
        bulletsLeft = magazineSize;
        reloading = false;
    }
}


