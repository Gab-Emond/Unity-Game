using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestRayCast02 : MonoBehaviour
{
    public LayerMask mask;
    public LineRenderer lRenderer;

   	public EnemyController enemyController;

	void Update () {

		Ray ray = new Ray (transform.position, transform.right);
		RaycastHit hitInfo;
		lRenderer.SetPosition(0, ray.origin);
		//lRenderer.positionCount = 3;
		
		if (Physics.Raycast (ray, out hitInfo, 100, mask, QueryTriggerInteraction.Ignore)) {
			//print (hitInfo.collider.gameObject.name);
			//Destroy (hitInfo.transform.gameObject);

			lRenderer.SetPosition(1, hitInfo.point);
			//edgeRay(ray);
			//lRenderer.SetPosition(2,hitInfo.point+edgeRay(ray)*10);
            
			if(hitInfo.collider.gameObject.layer == 10 && hitInfo.collider != null) {//player == 10
             	
				 if(enemyController != null && enemyController.alarmSounded == false){
					enemyController.SoundAlarm();
				}
            
         	}

		}
        else {
			//Debug.DrawLine (ray.origin, ray.origin + ray.direction * 100, Color.green);
			
			lRenderer.SetPosition(1, ray.origin + ray.direction * 100);
			

		}
		
	}

	Vector3 edgeRay(Ray ray){

		Vector3 topVect = transform.up;
		Vector3 vectResult = Vector3.Cross(topVect,ray.direction);

		return vectResult;

	}


}
