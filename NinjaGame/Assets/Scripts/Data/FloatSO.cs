using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//extra about scriptable objects and events
//https://www.raywenderlich.com/2826197-scriptableobject-tutorial-getting-started


[CreateAssetMenu(fileName = "FloatSO", menuName = "SharedNumber/FloatSO")]
public class FloatSO : ScriptableObject {
    
    [SerializeField]
    private float _value;
    public float Value{
        get{return _value;}
        set{_value = value;}
    }
}
