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
    Attack
}

public class DroneShooter : MonoBehaviour
{
    public Fts ballistic;

	//projectile 
    public GameObject projectilePrefab;
    public Transform projectileSpawn;
	
    /*public Team Team => _team;
    [SerializeField] private Team _team;*/
    [SerializeField] private LayerMask _layerMask;
    
    private float _attackRange = 3f;
    private float _rayDistance = 5.0f;
    private float _stoppingDistance = 1.5f;
    
    private Vector3 _destination;
    private Quaternion _desiredRotation;
    private Vector3 _direction;
    private GameObject _target;
    private DroneShooterState _currentState;
    public Projectile _projectile;
    float _projectileSpeed;
	
	//target movement
	private Vector3 prevPos;
	private Vector3 currPos;
	int aimCall = 0;
	float timeLastShot = 0f;
	float timeBetweenShots;
	
    private void Update()
    {
        switch (_currentState)
        {
            case DroneShooterState.Wander:
            {
                if (NeedsDestination())
                {
                    GetDestination();
                }

                transform.rotation = _desiredRotation;//snaps to rotation

                transform.Translate(Vector3.forward * Time.deltaTime * 5f);

                var rayColor = IsPathBlocked() ? Color.red : Color.green;
                Debug.DrawRay(transform.position, _direction * _rayDistance, rayColor);

                if(IsPathBlocked())
                {
                    //Debug.Log("Path Blocked");
                    GetDestination();
					break;
				}

                var targetToAggro = CheckForAggro();
                if (targetToAggro != null)
                {
                    _target = targetToAggro.gameObject;//.GetComponent<DroneShooter>();
                    _currentState = DroneShooterState.Chase;
                }
                
                break;
            }
            case DroneShooterState.Chase:
            {
                if (_target == null)
                {
                    _currentState = DroneShooterState.Wander;
                    return;
                }
                
                transform.LookAt(_target.transform);//IEnumerator TurnToFace(Vector3 lookTarget) {
                transform.Translate(Vector3.forward * Time.deltaTime * 5f);

                if (Vector3.Distance(transform.position, _target.transform.position) < _attackRange)
                {
                    _currentState = DroneShooterState.Attack;
                }
                break;
            }
			
            case DroneShooterState.Attack:
            {
                if (_target != null)
                {
					Aim();//rotate towards shooting direction
			
                    //Destroy(_target.gameObject);
                
					if(aimCall == 1 && (Time.time - timeLastShot) > timeBetweenShots){
						Shoot(_target.transform.position);
						aimCall = 0;
						timeLastShot = Time.time;
					}
					/*if(loaded && aimed){
						
					}*/
				}	
                // play laser beam
                else{
					_currentState = DroneShooterState.Wander;
                }
                
                break;
            }
        }
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
            ballistic.solve_ballistic_arc(arrow.position, projectileSpeed, randomPos, blockVelocity, gravity, shootingDir, shootingDir2, shootingDir3);
			Vector3 shootDir = shootScript(currPos, transform.position, targetVelocity, bulletVelocity)[0];
		*/	//transform, snap to direction
			prevPos = _target.transform.position;
		}
		
		
	}
	
	
	private void Shoot(Vector3 targetPos){
		
		Vector3 bulletPos = projectileSpawn.position;//projectile.transform.position
        Quaternion bulletDirection = Quaternion.LookRotation(projectileSpawn.transform.forward, Vector3.up);
		
		GameObject projectile = Instantiate(projectilePrefab, bulletPos, bulletDirection);
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), projectileSpawn.parent.GetComponent<Collider>());
		projectile.GetComponent<Projectile>().Launch(_projectileSpeed);

	}
	
    /*
    public void Alert(){
        PathRequestManager.RequestPath(transform.position,_target.position, OnPathFound);
        _currentState = DroneShooterState.Chase;
	}
    */
    
    private bool IsPathBlocked()
    {
        Ray ray = new Ray(transform.position, _direction);
        var hitSomething = Physics.RaycastAll(ray, _rayDistance, _layerMask);
        return hitSomething.Any();
    }

    private void GetDestination()
    {
        Vector3 testPosition = (transform.position + (transform.forward * 4f)) +
                               new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0f,
                                   UnityEngine.Random.Range(-4.5f, 4.5f));

        //returns random position

        _destination = new Vector3(testPosition.x, 1f, testPosition.z);

        _direction = Vector3.Normalize(_destination - transform.position);
        _direction = new Vector3(_direction.x, 0f, _direction.z);
        _desiredRotation = Quaternion.LookRotation(_direction);
    }

    private bool NeedsDestination()
    {
        if (_destination == Vector3.zero)
            return true;

        var distance = Vector3.Distance(transform.position, _destination);
        if (distance <= _stoppingDistance)
        {
            return true;
        }

        return false;
    }
    /*
    IEnumerator TurnToFace(Vector3 lookTarget) {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) {
            float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }
    */
    
    
    Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);
    
    private Transform CheckForAggro()
    {
        float aggroRadius = 5f;
        
        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        for(var i = 0; i < 24; i++)
        {
            if(Physics.Raycast(pos, direction, out hit, aggroRadius))
            {
                var DroneShooter = hit.collider.GetComponent<DroneShooter>();
                if(DroneShooter != null) //&& DroneShooter.Team != gameObject.GetComponent<DroneShooter>().Team)
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
                    return DroneShooter.transform;
                }
                else
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(pos, direction * aggroRadius, Color.white);
            }
            direction = stepAngle * direction;
        }

        return null;
    }
}

