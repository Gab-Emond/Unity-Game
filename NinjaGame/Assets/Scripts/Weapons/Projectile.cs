using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
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
    
    //ObjectPool, either reference or an OnDisable event

    IObjectPool<Projectile> pool;

    public IObjectPool<Projectile> Pool
    {
        get
        {
            return pool;
        }
        set
        {
            this.pool = value;
        }
    }

    //Stats
    [Range(0f,1f)]
    //public float bounciness;
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


    PhysicMaterial physics_mat;//todo: single physics mat accross all SAME projectile
    
    [Range(0,1)]
    public float crossAirResistance = .5f;

    private Vector3 prevPos;
    private RaycastHit hitInfo;
    public GameObject impactEffect;
    
    private Collider thisCollider;
    
    public string ignoreTag;
    bool hitSomething;
    
    
    //trailrenderer? selon si on veut

    //awake?
    //private on enable

    private void Start()//Start will not run again if the component is re-enabled
    {
        Setup();
        //Destroy(gameObject,maxLifetime);
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
                
                IDamageable damagedEntity = hitInfo.collider.GetComponent<IDamageable>();//.gameObject
                if(damagedEntity != null){
                    damagedEntity.TakeHit(transform.position-prevPos, hitInfo.point);
                }
                //note: sendmessage("takehit") couldve worked too, without requiring idamageable, no error if doesnt have


                hitSomething = true;
                
                //rb.constraints = RigidbodyConstraints.FreezeAll;
                
                ///////////region/////////////if stuck in something
                if (!hitInfo.collider.CompareTag("Impenetrable")){
                    rb.isKinematic = true;
                    rb.detectCollisions = false;
                    //Destroy(rb);
                    transform.position = hitInfo.point;
                    //err with setparent in unity, if object scaled asymetrically, rotation will mess up
                    this.transform.SetParent(hitInfo.transform, true); 
                }
                //////////
                //else bounce(tag:)
                else{
                    //rb.detectCollisions = true;
                    Vector3 initVel = rb.velocity;
                    rb.velocity = Vector3.Reflect(rb.velocity,hitInfo.normal)/16;
                    rb.AddForceAtPosition(Vector3.ProjectOnPlane((Random.onUnitSphere*initVel.magnitude/8)+(initVel/8), hitInfo.normal), hitInfo.point,ForceMode.Impulse);
                }//*rb.velocity.magnitude/2

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

    protected virtual void FixedUpdate() {
        ExtraPhysicsBehaviour();
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

    protected virtual void ExtraPhysicsBehaviour(){
        if(useGravity && !hitSomething){
            rb.AddForce(MathUtility.LinFriction(Vector3.Project(transform.up, rb.velocity),crossAirResistance));    //, ForceMode.Acceleration vs Impulse
        }
    }

    //on collision also possible for collision, for slower projectiles
    protected virtual void OnHitSomething(){
        //projectile hit something

        //Don't count collisions with self(?)
        //if (hitInfo.collider.CompareTag("Bullet")) return;
        
        //Don<t count collisions with (own?) player:
        if (hitInfo.collider.CompareTag("Player")) return;
        
        IDamageable damagedEntity = hitInfo.collider.GetComponent<IDamageable>();//.gameObject
        if(damagedEntity != null){
            damagedEntity.TakeHit(transform.position-prevPos, hitInfo.point);
        }
        //note: sendmessage("takehit") couldve worked too, without requiring idamageable, no error if doesnt have


        hitSomething = true;
        
        ///////////region/////////////if stuck in something
        rb.isKinematic = true;
        rb.detectCollisions = false;
        //Destroy(rb);
        transform.position = hitInfo.point;
        
        //err with setparent in unity, if object scaled asymetrically, rotation will mess up
        this.transform.SetParent(hitInfo.transform, true); 
        //////////
        //else bounce(tag:)

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
                IDamageable damagedEntity = entities[i].GetComponent<IDamageable>();
                if(damagedEntity != null){
                    damagedEntity.TakeHit(transform.position-prevPos, hitInfo.point);
                }

                //Add explosion force (if enemy has a rigidbody)
                if (entities[i].GetComponent<Rigidbody>())
                    entities[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
            }
        }
        //Add a little delay, just to make sure everything works fine
        StartCoroutine(CleanupAfterTime(0.0625f));//could be an invoke
        //Destroy(gameObject,0.0625f);
        
    }
    

    // private IEnumerator DisableSelfAfterTime(GameObject self, float delay){
    //     //disable bullet and return to pool
    //     yield return new WaitForSeconds(delay);
    //     self.SetActive(false);
    // }

    private IEnumerator CleanupAfterTime(float delay){
        if(pool!=null){
            yield return new WaitForSeconds(delay);
            pool.Release(this);
        }
        else{
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }

    private void Setup()//add pools that contains it?
    {   
        rb = this.GetComponent<Rigidbody>();
        //Create a new Physic material//use scriptable objects
        // physics_mat = new PhysicMaterial();//no need should all have same
        // physics_mat.bounciness = bounciness;
        // physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        // physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;

        //Assign material to collider
        //GetComponent<SphereCollider>().material = physics_mat;
        
        rb.detectCollisions = true;
        //Set gravity
        rb.useGravity = useGravity;
        hitSomething = false;
    }

    private void OnEnable() {
        prevPos = transform.position;
        hitSomething = false;
        StartCoroutine(CleanupAfterTime(maxLifetime));

    }

    private void OnDisable() {//to stop any movement when pooled? verify if necessary
        rb.isKinematic = true;
        rb.isKinematic = false;
        StopAllCoroutines();
    }

    /// Just to visualize the explosion range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }



}
