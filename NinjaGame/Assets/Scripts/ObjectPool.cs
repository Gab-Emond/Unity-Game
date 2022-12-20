using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;//singleton; Garantir qu’une classe n’a qu’une seule instance et offre un point d’accès global(ift 2255,  ch 4.5)
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for(int i = 0; i < amountToPool; i++){
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
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