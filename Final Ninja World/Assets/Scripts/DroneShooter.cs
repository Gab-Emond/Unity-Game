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
    /*public Team Team => _team;
    [SerializeField] private Team _team;*/
    [SerializeField] private LayerMask _layerMask;
    
    private float _attackRange = 3f;
    private float _rayDistance = 5.0f;
    private float _stoppingDistance = 1.5f;
    
    private Vector3 _destination;
    private Quaternion _desiredRotation;
    private Vector3 _direction;
    private DroneShooter target;
    private DroneShooterState _currentState;
    

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

                while (IsPathBlocked())
                {
                    //Debug.Log("Path Blocked");
                    GetDestination();
                }

                var targetToAggro = CheckForAggro();
                if (targetToAggro != null)
                {
                    target = targetToAggro.GetComponent<DroneShooter>();
                    _currentState = DroneShooterState.Chase;
                }
                
                break;
            }
            case DroneShooterState.Chase:
            {
                if (target == null)
                {
                    _currentState = DroneShooterState.Wander;
                    return;
                }
                
                transform.LookAt(target.transform);
                transform.Translate(Vector3.forward * Time.deltaTime * 5f);

                if (Vector3.Distance(transform.position, target.transform.position) < _attackRange)
                {
                    _currentState = DroneShooterState.Attack;
                }
                break;
            }
            case DroneShooterState.Attack:
            {
                if (target != null)
                {
                    Destroy(target.gameObject);
                }
                
                // play laser beam
                
                _currentState = DroneShooterState.Wander;
                break;
            }
        }
    }

    /*
    public void Alert(){
        PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
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

