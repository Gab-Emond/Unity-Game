using System.Linq;
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
    private int aimCall = 0;
    private float timeLastShot = 0f;
    public float timeBetweenShots = 10f;
    
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
                    print("idle");
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
                if (_target != null){
                    if ((transform.position-_target.transform.position).sqrMagnitude < _attackRange*_attackRange && !IsPathBlocked(transform.position, _target.transform.position)){
                        Aim();//rotate towards shooting direction
                
                        //print(Time.time-timeLastShot);
                        if(aimCall == 1 && (Time.time - timeLastShot) > timeBetweenShots){//loaded + aim
                            Shoot(_target.transform.position);
                            aimCall = 0;
                            timeLastShot = Time.time;
                        }
                    }
                    else{
                        print("restarts chase");
                        _currentState = DroneShooterState.Chase;
                    }
                }	
                else{
                    _currentState = DroneShooterState.Wander;
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

    private void Aim(){
        
        //float Time.deltaTime;
        if(aimCall == 0){
            prevPos = _target.transform.position;
            aimCall++;
            return;
        }
        else if(aimCall == 1){
            currPos = _target.transform.position;
            Vector3 targetVelocity = (prevPos - currPos)/Time.deltaTime;
        /*
            ballistic.solve_ballistic_arc(projectileSpawn.position, projectileSpeed, randomPos, blockVelocity, gravity, shootingDir, shootingDir2, shootingDir3);
            Vector3 shootDir = shootScript(currPos, transform.position, targetVelocity, bulletVelocity)[0];
        */	//transform, snap to direction

            Vector3 _relPos = (_target.transform.position-projectileSpawn.position);

            double a = (double)(targetVelocity.sqrMagnitude- _projectileSpeed*_projectileSpeed); 
            double b = (double)(Vector3.Dot(_relPos, targetVelocity));
            double c = (double)(_relPos).sqrMagnitude;
            double tRes1 = -1d,tRes2 = -1d; 
            Fts.SolveQuadric(a, b, c, out tRes1, out tRes2);
            

            if (tRes1 > 0f){
                _direction = targetVelocity + (_relPos/(float)tRes1);

            }
            else if(tRes2 > 0f){
                _direction = targetVelocity + (_relPos/(float)tRes2);
            }


            
            prevPos = currPos;
        }
        transform.LookAt(transform.position + _direction);
        
        
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

    public override void GetIdlePath(){
            
        path = new Vector3[2];
        path[0] = transform.position + Vector3.up * 0.25f;
        path[1] = transform.position + Vector3.down * 0.25f;
        
    }


    /**/
    void TurnToFace(Vector3 lookTarget) {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;
        float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
        transform.eulerAngles = Vector3.up * angle;      
    }

    bool IsFacingTarget(Vector3 targetPoint){

        Vector3 dirToLookTarget = (targetPoint - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;
        
        return(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) <= 0.05f);
    }

    void FollowPath(float _speed, bool _loops= false, bool _turns = true) {
            
        Vector3 targetWaypoint = path[pathNodeIndex];
        
        
        if (transform.position == targetWaypoint){//to turn, must be at waypoint
            
            /////////////////////////Errror sets next target without having turned           
            
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


    public void TakeHit(){
        //print("hitTurret");
        if(health>1){
            health = health-1;
            print(health);
        }
        else{
            _currentState = DroneShooterState.Out;
            print("dead");
            EnableRagdoll();
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
    }
        
}

