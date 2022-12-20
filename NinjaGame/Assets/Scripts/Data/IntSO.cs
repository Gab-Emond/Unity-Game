using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//extra about scriptable objects and events
//https://www.raywenderlich.com/2826197-scriptableobject-tutorial-getting-started

[CreateAssetMenu(fileName = "IntSO", menuName = "SharedNumber/IntSO")]
public class IntSO : ScriptableObject {

    [SerializeField]
    private int _value;
    public int Value{
        get{return _value;}
        set{_value = value;}
    }

}
