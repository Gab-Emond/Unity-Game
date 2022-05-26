﻿using UnityEngine;

public class PlayerFadeOut : MonoBehaviour
{
    public Material opaqueMat;
    public Material transparentMat;
    public Renderer rend;
    private bool transparentPlayer = false;
    float fadeSpeed = 85f;
    float fadeAmount;
    Color objectColor;
    
    void Start()
    {
        
    }

    // Toggle the Object's visibility each second.
    void Update()
    {

        if (Input.GetButton("Fire3")){
            
            transparentPlayer = true;
            UpdateMaterial(transparentPlayer);

            objectColor = rend.material.color;
            fadeAmount = objectColor.a - fadeSpeed*Time.deltaTime;
            //Debug.Log(objectColor.r + " " + fadeAmount);
            if(fadeAmount>0){
                rend.material.color = new Color(objectColor.r,objectColor.g,objectColor.b,fadeAmount);
            }



        }
        else if (Input.GetButtonUp("Fire3")){
            
            objectColor = rend.material.color;
            fadeAmount = objectColor.a + fadeSpeed*Time.deltaTime;
            if(fadeAmount<255){
                rend.material.color = new Color(objectColor.r,objectColor.g,objectColor.b,fadeAmount);
            }
            transparentPlayer = false;
            UpdateMaterial(transparentPlayer);
            
        }
        

    }



    
    void UpdateMaterial(bool transparent) {
        if (transparent) {
            rend.material = transparentMat;
        }
        else {
            rend.material = opaqueMat;
        }
    }


    public bool TransparentPlayer{
        get => transparentPlayer;
    }


}
