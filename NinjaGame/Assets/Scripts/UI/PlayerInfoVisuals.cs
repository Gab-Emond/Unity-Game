using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoVisuals : MonoBehaviour
{

    public Slider healthSlider;
    public Slider staminaSlider;

    public RectTransform staminaRect;

    public Image staminaImg;

    //todo: use player controller
    public PlayerMovement playerMovement;

    public float maxOpacity = 255f;
    public float opacity;

    float opaciTime = 0;
    float maxStamina;
    float stamina;
    float prevStamina;
    
    //for two sided slider:
    //https://github.com/Unity-UI-Extensions/com.unity.uiextensions

    //calculate ortho size, responsive size of ui
    //https://www.youtube.com/watch?v=gFWQHordrtA&ab_channel=Tarodev
    private void Start() {
        maxStamina = playerMovement.maxStamina;
        SetMaxStamina((int)maxStamina);
        stamina =playerMovement.Stamina;
        prevStamina = stamina;
    }
    private void Update() {
        stamina = playerMovement.Stamina;
        //add fade in/out
        
        if(stamina >= prevStamina){
            
            if(stamina>maxStamina/2){
                opaciTime = Mathf.Max(0, opaciTime-Time.deltaTime/12);
                opacity = Mathf.SmoothStep(0f,255f, opaciTime);
                Color temp = staminaImg.color;
                temp.a = opacity;
                staminaImg.color = temp;
            }
        }
        else{
            
            opaciTime = Mathf.Min(1, opaciTime+Time.deltaTime/8); 
            opacity = Mathf.SmoothStep(0f,255f, opaciTime);
            Color temp = staminaImg.color;
            temp.a = opacity;
            staminaImg.color = temp;
        }
        
        
        if(stamina != prevStamina){
            SetStamina(stamina);
            //timeSinceLastChange = Time.time;
            
        }
        

        prevStamina = stamina;

    }

    public void SetMaxHealth(int h_value){
        healthSlider.maxValue = h_value;
        healthSlider.value = h_value;
    }

    public void SetMaxStamina(int s_value){
        staminaSlider.maxValue = s_value;
        staminaSlider.value = s_value;
    }
    public void SetHealth(float h_value){
        healthSlider.value = h_value;
    }
    public void SetStamina(float s_value){
        staminaSlider.value = s_value;
    }


}
