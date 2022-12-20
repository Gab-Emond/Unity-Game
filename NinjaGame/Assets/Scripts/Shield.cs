using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IDamageable {
    //DroneShooter drone;
    Collider shieldCollider;
    Renderer shieldRenderer;
    Material shieldMat;

    // particles spawn on impact
    [SerializeField] private ParticleSystem collisionFXPrefab;
    float lifeTime = 2.5f;//just in case

    private void Start() {
        //drone = GetComponent<DroneShooter>();
        shieldCollider = GetComponent<Collider>();
        shieldRenderer = GetComponent<Renderer>();
    }

    public void turnOff(){
        shieldCollider.enabled = false;
        shieldRenderer.enabled = false;
    }
    public void turnOn(){
        shieldCollider.enabled = true;
        shieldRenderer.enabled = true;
    }

    public void TakeHit(Vector3 damageDir, Vector3 damagePos){
        ParticleCollision(damageDir, damagePos);
    }

    private void ParticleCollision(Vector3 damageDir, Vector3 damagePos){
        // instantiate collision particles at each intersection
        var collisionFX = Instantiate(collisionFXPrefab, damagePos, Quaternion.identity);

        // orient to face normal
        collisionFX.transform.rotation = Quaternion.LookRotation(damageDir);

        // destroy the collision effect after delay and increment
        Destroy(collisionFX.gameObject, lifeTime);

            
    }


    //todo, makewave in shield, like

    
}