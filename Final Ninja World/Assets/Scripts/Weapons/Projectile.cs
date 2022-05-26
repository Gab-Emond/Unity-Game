using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// Modded from Dave's free bullet script
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
    public LayerMask whatIsEnemies;
    
    //Stats
    [Range(0f,1f)]
    public float bounciness;
    public bool useGravity;

    //Damage
    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;

    //Lifetime
    public float maxLifetime = 1f;
    public bool explodeOnTouch = true;
    PhysicMaterial physics_mat;


    private Vector3 prevPos;
    private RaycastHit hitInfo;
    public GameObject impactEffect;

    //trailrenderer? selon si on veut
    private void Awake() {

        prevPos = transform.position; 
        
    }
    private void Start()
    {
        Setup();
        StartCoroutine(DestroyBulletAfterTime(gameObject, maxLifetime));   
    }
    // Update is called once per frame
    void Update()
    {
        //transform.position = transform.position + new Vector3( * movementSpeed * Time.deltaTime, verticalInput * movementSpeed * Time.deltaTime, 0);
        if (Physics.Linecast(prevPos, transform.position, out hitInfo)){
            //projectile hit something

            //Don't count collisions with other bullets(?)
            if (hitInfo.collider.CompareTag("Bullet")) return;
            //Don<t count collisions with player:

            //if has rigid body

            //hitInfo
            StartCoroutine(DestroyBulletAfterTime(gameObject, 0.0625f));
        }

    }

    private void Explode()
    {
        //Instantiate explosion
        if (explosion != null) {
            Instantiate(explosion, transform.position, Quaternion.identity);

            //Check for enemies 
            Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
            for (int i = 0; i < enemies.Length; i++){
                //Get component of enemy and call Take Damage

                //Just an example!
                ///enemies[i].GetComponent<ShootingAi>().TakeDamage(explosionDamage);

                //Add explosion force (if enemy has a rigidbody)
                if (enemies[i].GetComponent<Rigidbody>())
                    enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
            }
        }
        //Add a little delay, just to make sure everything works fine
        StartCoroutine(DestroyBulletAfterTime(gameObject, 0.0625f));
    }
    void LateUpdate() 
    {
        prevPos = transform.position;//take late position to see if hit
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
    


    private void Setup()
    {
        //Create a new Physic material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;
    }

    /// Just to visualize the explosion range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }



}
