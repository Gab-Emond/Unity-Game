using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFadeOut : MonoBehaviour
{
    public enum VisState
	{
		Invisible,
        Visible

	}

    public PlayerMovement playerMovement;
    public Material playerMat;
    //public Material transparentMat;
    //public Renderer rend;
    // private bool transparentPlayer = false;
    // public bool TransparentPlayer{
    //     get => transparentPlayer;
    // }

    public GameObject playerColliderObj;
    public int invis_Layer;
    private int init_Layer;
    bool layerIsChanged;

    Color objectColor;
    float fadeAmount;
    bool shouldFade = false;
    void Start()
    {   
        fadeAmount = 0;
        //playerMovement = GetComponent<PlayerMovement>();
        init_Layer = playerColliderObj.layer;
        layerIsChanged = false;

    }

    // Toggle the Object's visibility each second.
    /*
    void Update()
    {
        if(playerMovement.IsGrounded && !playerMovement.IsGrappled){

            if (Input.GetButtonDown("Fire3")){
            
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
        else if(fadeAmount<255){
            objectColor = rend.material.color;
            fadeAmount = objectColor.a + fadeSpeed*Time.deltaTime;
            rend.material.color = new Color(objectColor.r,objectColor.g,objectColor.b,fadeAmount);
            
            transparentPlayer = false;
            UpdateMaterial(transparentPlayer);
        }
        
        

    }
    */

    void Update()
    {
        
        if(playerMovement.IsGrounded && !playerMovement.IsGrappled){

            if (playerMovement.IsCrouching){
            
                //keyheld
                shouldFade = true;
            
            }
            else{
                //keyunpressed
                shouldFade = false;
            }

        }
        else{
            shouldFade = false;
        }

        if(shouldFade){
            fadeAmount = Mathf.Min(5, fadeAmount+2*Time.deltaTime);
            playerMat.SetFloat("Vector_Invis", fadeAmount);
            if(!layerIsChanged){
                playerColliderObj.layer = invis_Layer;
                layerIsChanged = true;
            }
            
        }
        else {
            if(fadeAmount!=0){
                fadeAmount = Mathf.Max(0, fadeAmount-3*Time.deltaTime);
                playerMat.SetFloat("Vector_Invis", fadeAmount);
            }
            if(layerIsChanged){
                playerColliderObj.layer = init_Layer;
                layerIsChanged = false;
            }
        }
        
        
        //else layer = init_layer
        
        
    }

    
    // void ChangeLayer(){
    //     //start: get initLayer = player.layer;
    //     //invis: player.layer = invisibleLayer;
    //     //end: player.layer = initLayer;
    // }

    // void UpdateMaterial(bool transparent) {
       
    // }


    

}
