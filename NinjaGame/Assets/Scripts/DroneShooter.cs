using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*public enum Team
{
    Red,
    Blue
}*/

public enum DroneShooterState
{
    Wander,
    Chase,
    Attack,
    Out
}

public class DroneShooter : Enemy, IDamageable//make child objects with different weapon types?
{
    
    //public Fts ballistic;

    // Drone Shield
    public Shield shield;

    //projectile 
    public GameObject projectilePrefab;
    public Transform projectileSpawn;
    public float _projectileSpeed;

    /*public Team Team => _team;
    [SerializeField] private Team _team;*/
    [SerializeField] private LayerMask _layerMask;
    
    public float _attackRange = 6f;
    private Vector3 _destination;
    private Quaternion _desiredRotation;
    private Vector3 _direction;
    private GameObject _target;
    private DroneShooterState _currentState;
    //public Projectile _projectile;
    
    Vector3[] path;
    int pathNodeIndex = 0;
    bool isLookingForPath = false;
    bool isTurning = false;
    float timeSinceGotPath = 0;

    public float turnSpeed = 45;
    public float speed = 5f;
    //health = 3;

    //target movement
    private Vector3 prevPos;
    private Vector3 currPos;
    private float timeLastShot = 0f;
    public float timeBetweenShots = 10f;
    
    IEnumerator aiming;
    bool aimrunning;
    bool lockOn = false;
    public bool Lockon => lockOn;
    private Rigidbody rb;
    /**/
    private void Start() {

        rb = GetComponent<Rigidbody>();
        /*
        _target = GameObject.FindWithTag("Player");//GameObject.Find("1st-3rd Person Player");//slower than tag finder
        Alert(_target);
        */

    }
    

    private void Update()
    {
        switch (_currentState)
        {
            case DroneShooterState.Wander:
            {
                
                if(path == null){
                    pathNodeIndex = 0;
                    //print("idle");
                    GetIdlePath();
                    return;
                }


                //if(){}//sees player, starts chase

                FollowPath(0.25f, true, false);

                break;
            }
            case DroneShooterState.Chase:
            {
                if (_target == null)
                {
                    pathNodeIndex = 0;
                    path = null;
                    _currentState = DroneShooterState.Wander;
                    return;
                }
                

                bool needsPath = (path == null)||(path[path.Length-1]-_target.transform.position).sqrMagnitude > _attackRange*_attackRange || IsPathBlocked(path[path.Length-1], _target.transform.position);
                


                //var lastPos = seeTarget? target.pos: lastPos; //store last seen target pos
                //needs path = (path == null)||(at last path node index);
                //if at last node and cant see player,go check lastPos, then if not seen, back to idle

                

                

                if(needsPath){ //&& timeSinceLastCall > 1f){
                    pathNodeIndex = 0;
                    if(isLookingForPath){//||Time.time - timeSinceGotPath < 1f
                        return;
                    }
                    else if(IsPathBlocked(transform.position, _target.transform.position)){
                        PathRequestManager.RequestPath(transform.position,_target.transform.position, OnPathFound);
                        isLookingForPath = true;
                        // if path blocked completely, lose target, go back to wander
                        timeSinceGotPath = Time.time;
                        return;
                    }
                    else{//if not blocked, go straight to target
                        path = new Vector3[]{transform.position, _target.transform.position};
                        //print("got");
                        return;
                    }
                }


                if ((transform.position-_target.transform.position).sqrMagnitude < _attackRange*_attackRange && !IsPathBlocked(transform.position, _target.transform.position)){
                    _currentState = DroneShooterState.Attack;
                    return;
                }

                FollowPath(speed);
                break;
            }
            
            case DroneShooterState.Attack://could keep prev state, if different== first call
            {   
                path = null;
                Vector3 midLook = transform.forward;
                shield.turnOff();
                
                if ((transform.position-_target.transform.position).sqrMagnitude < _attackRange*_attackRange && !IsPathBlocked(transform.position, _target.transform.position)){
                    
                    if((Time.time - timeLastShot) > timeBetweenShots && isInShootArea(_target.transform.position,5f)){//if task.ended == true
                        
                        //use Invoke(); for time next shot
                        Shoot(transform.position+ midLook);
                        timeLastShot = Time.time;
                        
                    }
                    else{
                        //takes 1sec to reach, by definition
                        //or change to rotate towards, to limit max rotation speed
                        midLook = Vector3.Slerp(transform.forward, _target.transform.position-transform.position, Time.time - timeLastShot);
                        //transform.rotation = Quaternion.LookRotation(midLook);
                        transform.LookAt(transform.position+ midLook);
                        
                        
                        return;

                        
                    }
                    
                }
                else{
                    //stop aiming coroutine
                    shield.turnOn();
                    print("restarts chase");
                    _currentState = DroneShooterState.Chase;
                }
                
               
                
                break;
            }
            case DroneShooterState.Out://dead when health = 0
            {  
                break;
            }
        }
        //print(_currentState);
    }

    public override void Alert(GameObject target){
        _target = target;
        _currentState = DroneShooterState.Chase;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        if (pathSuccessful) {		
            path = newPath;
            //print(path[path.Length-1]);
            //print(transform.position);

        }
        else{
            print("nope");
            path = null;
            _currentState = DroneShooterState.Wander;
            //path found failed,
        }
        isLookingForPath = false;
    }
    
    /**/
    IEnumerator Aim(Vector3 targetPos) {
        aimrunning = true;
        lockOn = false;
        double tRes1 = -1d,tRes2 = -1d; 
        float prevTime;
        //while (tRes1 <=0 && tRes2 <=0){}
        
        //print("aim");
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);
        prevPos = _target.transform.position;
        prevTime = Time.time;
        yield return new WaitForSeconds(0.05f);//breaks out the loop
        
        
        currPos = _target.transform.position;
        Vector3 targetVelocity = (currPos - prevPos)/(Time.time-prevTime);
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        //print("targetVel: " + (targetVelocity));

        Vector3 _relPos = (_target.transform.position-projectileSpawn.position);
        
        double a = (double)(targetVelocity.sqrMagnitude- _projectileSpeed*_projectileSpeed); 
        double b = (double)(Vector3.Dot(_relPos, targetVelocity));
        double c = (double)(_relPos).sqrMagnitude;
        
        Fts.SolveQuadric(a, b, c, out tRes1, out tRes2);
        //print(tRes1+" and"+tRes2);
        

        if (tRes1 > 0f){
            _direction = targetVelocity + (_relPos/(float)tRes1);
            //print(_direction);

            lockOn = true;
        }
        else if(tRes2 > 0f){
            _direction = targetVelocity + (_relPos/(float)tRes2);
            //print(_direction);
            lockOn = true;
        }
                
            
        
        
        yield return null;
    }

    bool isInShootArea(Vector3 targetPos, float gapLengthSqr){
        Vector3 rayToTarget = Vector3.ProjectOnPlane(projectileSpawn.position - targetPos, projectileSpawn.transform.forward);
        
        return (rayToTarget.sqrMagnitude <= gapLengthSqr); 
    }


    
    private void Shoot(Vector3 targetPos){//might need var later on, but not used rn
        
        Vector3 bulletPos = projectileSpawn.position;//projectile.transform.position
        Quaternion bulletDirection = Quaternion.LookRotation(projectileSpawn.transform.forward, Vector3.up);
        
        GameObject projectile = Instantiate(projectilePrefab, bulletPos, bulletDirection);
        //Physics.IgnoreCollision(projectile.GetComponent<Collider>(), projectileSpawn.parent.GetComponent<Collider>());
        projectile.GetComponent<Projectile>().Launch(_projectileSpeed);

    }
    
    
    private bool IsPathBlocked(Vector3 startPos, Vector3 endPos){
        bool hitSomething = Physics.Linecast(startPos, endPos, _layerMask);
        return hitSomething;
    }

//////////////////////////todo: fix
    bool CanSeeTarget(float viewDistance, float viewAngle, Vector3 target) {
		RaycastHit hitInfo;
		Vector3 vectResult;
		bool canSee = false;
		if ((transform.position-target).sqrMagnitude < viewDistance*viewDistance) {//compares distance faster, see https://docs.unity3d.com/ScriptReference/Vector3-sqrMagnitude.html
			
			Vector3 dirToPlayer = (target - transform.position);//find the vector pointing from our position to the target
			float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
			if(angleBetweenGuardAndPlayer< viewAngle / 2f){
				
				vectResult = Vector3.Cross(Vector3.up,dirToPlayer);
				if(!Physics.Linecast(transform.position, target,_layerMask)){//if nothing is blocking the player (center)
					canSee= true;
					//Debug.DrawLine (transform.position, target.position, Color.red);
				}
				//Debug.DrawLine (target.position-vectResult, target.position, Color.yellow);
				
				else if(Physics.Linecast(target-vectResult, target, out hitInfo)){//if nothing is blocking the player (left)
					
					if(!Physics.Linecast(transform.position, hitInfo.point,_layerMask)){
						canSee = true;
						//Debug.DrawLine(transform.position, hitInfo.point, Color.green);
					}
				}
				else if(Physics.Linecast(target+vectResult, target, out hitInfo)){//if nothing is blocking the player (right)
					
					if(!Physics.Linecast(transform.position, hitInfo.point,_layerMask)){
						canSee = true;
						//Debug.DrawLine(transform.position, hitInfo.point, Color.green);
					}
				}
				
				
			
			}
		}
		return canSee;
		
	}

    public override void GetIdlePath(){
            
        path = new Vector3[2];
        path[0] = transform.position + Vector3.up * 0.25f;
        path[1] = transform.position + Vector3.down * 0.25f;
        
    }


    /**/
    void TurnToFace(Vector3 lookTarget) {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;


        /*Vector3 targetDir = lookTarget - transform.position;
        float angleBetween = Vector3.Angle(transform.forward, targetDir);
        */

        //print("targetAngle: "+ targetAngle);
        //print("newtargetAngle: " + Vector3.Angle(targetDir, transform.forward));
        float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
        transform.eulerAngles = Vector3.up * angle;      


        //other part, only height(x? component)

    }

    bool IsFacingTarget(Vector3 targetPoint){

        Vector3 dirToLookTarget = (targetPoint - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;
        
        return(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) <= 0.05f);
    }

    void FollowPath(float _speed, bool _loops= false, bool _turns = true) {
            
        Vector3 targetWaypoint = path[pathNodeIndex];
        
        
        if (transform.position == targetWaypoint){//to turn, must be at waypoint
            
            pathNodeIndex = (pathNodeIndex + 1)%path.Length;
            //transform.position = Vector3.MoveTowards(transform.position,targetWaypoint, speed * Time.deltaTime);
            isTurning = true;
            
        }
        else if(isTurning && _turns){
            if(IsFacingTarget(targetWaypoint)){
                isTurning = false;
                //transform.position = Vector3.MoveTowards(transform.position,targetWaypoint, speed * Time.deltaTime);
            }
            else{
                TurnToFace(targetWaypoint);
            }
        }
        else{
            transform.position = Vector3.MoveTowards(transform.position,targetWaypoint, _speed * Time.deltaTime);
        }
        
    }


    public void TakeHit(Vector3 damageDir, Vector3 damagePos){
        //print("hitTurret");
        if(health>1){
            health = health-1;
            print(health);
        }
        else{
            _currentState = DroneShooterState.Out;
            if(rb.isKinematic){
                print("dead");
			    EnableRagdoll();
            }
            rb.AddForceAtPosition(damageDir, damagePos, ForceMode.Impulse);
        }
    }

    // Let the rigidbody take control and detect collisions.
    void EnableRagdoll()
    {   /**/
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.useGravity = true;
        
    }


    public void OnDrawGizmos() {
        if (path != null) {

            for (int i = pathNodeIndex; i < path.Length; i ++) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(path[i], 1/4f);

                if (i == pathNodeIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i-1],path[i]);
                }
            }
        }
    
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position,transform.position + transform.forward*5);
     
    
    }


}

