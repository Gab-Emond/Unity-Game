using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Math;//own 
/// Modified from Dave's free bullet script
/// 
/// The code is fully commented but if you still have any questions
/// don't hesitate to write a yt comment
/// or use the #coding-problems channel of my discord server
/// 
/// Dave/GameDevelopment
public class Projectile : MonoBehaviour
{

    //Assignables
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEntities;
    
    //Stats
    [Range(0f,1f)]
    public float bounciness;
    public bool useGravity;

    //Damage
    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;

    //Lifetime
    public float maxLifetime = 60f;
    public float hitLifetime = 10f;
    public bool explode = false;
    public bool sticksToTarget;//todo;toggle if object stays in place or no


    PhysicMaterial physics_mat;


    private Vector3 prevPos;
    private RaycastHit hitInfo;
    public GameObject impactEffect;
    
    private Collider thisCollider;
    
    public string ignoreTag;
    bool hitSomething;
    
    
    //trailrenderer? selon si on veut

    //awake?

    private void Start()
    {
        prevPos = transform.position;
        Setup();
        //StartCoroutine(DestroyBulletAfterTime(gameObject, maxLifetime));
        Destroy(gameObject,maxLifetime);
        //error: invoked twice
        //Launch(100f);

    }
    // Update is called once per frame
    void Update()
    {   

        if(!hitSomething){
        
            //float step = speed*time.deltaTime;
            //transform.position = transform.position + transform.forward*step; new Vector3( * movementSpeed * Time.deltaTime, verticalInput * movementSpeed * Time.deltaTime, 0);
            if (Physics.Linecast(prevPos, transform.position, out hitInfo)){

                //projectile hit something

                //Don't count collisions with self(?)
                //if (hitInfo.collider.CompareTag("Bullet")) return;
                
                //Don<t count collisions with (own?) player:
                if (hitInfo.collider.CompareTag("Player")) return;
                /*
                if(hitInfo.collider.gameObject.GetComponent<DroneExplosive>() != null){
                    hitInfo.collider.gameObject.GetComponent<DroneExplosive>().TakeHit();
                }

                if(hitInfo.collider.gameObject.GetComponent<DroneShooter>() != null){
                    hitInfo.collider.gameObject.GetComponent<DroneShooter>().TakeHit();
                }
                */
                IDamageable damagedEntity = hitInfo.collider.gameObject.GetComponent<IDamageable>();
                if(damagedEntity != null){
                    damagedEntity.TakeHit(transform.position-prevPos, hitInfo.point);
                }
                //note: sendmessage("takehit") couldve worked too, without requiring idamageable, no error if doesnt have


                hitSomething = true;
                
                //rb.constraints = RigidbodyConstraints.FreezeAll;
                
                rb.isKinematic = true;
                rb.detectCollisions = false;
                //Destroy(rb);
                transform.position = hitInfo.point;
                
                //err with setparent in unity, if object scaled asymetrically, rotation will mess up
                this.transform.SetParent(hitInfo.transform, true); 
                

                if(explode){
                    Explode();
                }
                else{
                    //hitInfo
                    //StartCoroutine(DestroyBulletAfterTime(gameObject, hitLifetime));
                    Destroy(gameObject,hitLifetime);
                    //print("hit");
                }


            }
        }

    }

    private void FixedUpdate() {
        if(useGravity && !hitSomething){
            rb.AddForce(MathUtility.LinFriction(Vector3.Project(transform.up, rb.velocity)));    //, ForceMode.Acceleration vs Impulse
        }
    }

    

    void LateUpdate() 
    {
        prevPos = transform.position;//take late position to see if hit
    }

    //1-move projectile forwards (rigidbody physics / translate)
    
    //2-lateupdate, keep track of previous frame position

    //3-cast ray from previous position to current position

    //4-use ray to check if projectile hit something between last and current position
    
    //5-use hit.normal to reflect hit effect 
    
    //6-remove projectile

    public void Launch(float speed){
        
        
        rb.AddForce(speed*transform.forward, ForceMode.Impulse);
        //rb.AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

    }

    private void Explode()
    {
        //Instantiate explosion
        if (explosion != null) {
            Instantiate(explosion, transform.position, Quaternion.identity);

            //Check for enemies 
            Collider[] entities = Physics.OverlapSphere(transform.position, explosionRange, whatIsEntities);
            for (int i = 0; i < entities.Length; i++){
                //Get component of enemy and call Take Damage

                //Just an example!
                ///enemies[i].GetComponent<ShootingAi>().TakeDamage(explosionDamage);

                //Add explosion force (if enemy has a rigidbody)
                if (entities[i].GetComponent<Rigidbody>())
                    entities[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
            }
        }
        //Add a little delay, just to make sure everything works fine
        //StartCoroutine(DestroyBulletAfterTime(gameObject, 0.0625f));
        Destroy(gameObject,0.0625f);
    }
    
    

    private IEnumerator DestroyBulletAfterTime(GameObject other, float delay){
        //delete bullet
        yield return new WaitForSeconds(delay);
        Destroy(other);
    }
    
    


    private void Setup()
    {   
        rb = this.GetComponent<Rigidbody>();
        //Create a new Physic material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        //GetComponent<SphereCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;
        hitSomething = false;
    }

    /// Just to visualize the explosion range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }



}
