using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
	/*
	public enum EnemyState
	{
		Wander,
		Chase,
		Attack
	}
	*/

	public int health = 1;

	/*
	public virtual void TakeHit(){
		
	}
	*/
	public virtual void Alert(GameObject target){
		
	}

	public virtual void GetIdlePath(){
		
	}
	
	/*
	FindEmptyPointAroundPos(Vector3 pos){
		Vector3 possiblePos = pos + new Vector3(UnityEngine.Random.Range(-20f, 20f), 0f, UnityEngine.Random.Range(-20f, 20f)); 
		if(Physics.Linecast(pos, possiblePos)){
			
		}
		
	}
	*/

}

