using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimizeBar : MonoBehaviour
{
    public RectTransform staminaRect;
    public Slider mainSlider;

    //public float mySliderFloat;
    float staminaMaxScale;

    public void Start()
	{
        staminaMaxScale = staminaRect.sizeDelta.x;
		//Adds a listener to the main slider and invokes a method when the value changes.
		mainSlider.onValueChanged.AddListener (delegate {ValueChangeCheck ();});
	}

    // Invoked when the value of the slider changes.
	public void ValueChangeCheck()
	{
        Vector2 temp = staminaRect.sizeDelta;
        temp.x = (staminaMaxScale/mainSlider.maxValue)*mainSlider.value;
        staminaRect.sizeDelta = temp;


	}
}
