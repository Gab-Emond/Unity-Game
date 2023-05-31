using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//not singleton anymore
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;//singleton; Garantir qu’une classe n’a qu’une seule instance et offre un point d’accès global(ift 2255,  ch 4.5)
    public GameObject[] pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledObjects = new GameObject[amountToPool];
        GameObject tmp;
        for(int i = 0; i < amountToPool; i++){
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects[i] = tmp;
        }
    }

    public GameObject GetPooledObject(){
        for(int i = 0; i < amountToPool; i++){
            if(!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    public void ReturnPooledObject(){

    }

}

//IN BULLET SCRIPT:
//replace instantiate (Instantiate(playerBullet, turret.transform.position, turret.transform.rotation);) with
/*

GameObject bullet = ObjectPool.SharedInstance.GetPooledObject(); 
  if (bullet != null) {
    bullet.transform.position = turret.transform.position;
    bullet.transform.rotation = turret.transform.rotation;
    bullet.SetActive(true);
  }

*/

//replace destroy (Destroy(gameObject);) with

/*
gameobject.SetActive(false);
*/

public interface ILoopPoolableObject {

    string Index
    {
        get;
        set;
    }

    void OnEnable() {
        
    }

    void OnDisable() {
        
    }
}