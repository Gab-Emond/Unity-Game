using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public IntSO scoreData;

    int startScore;
    int endScore;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void IncreaseScore(int num){
        scoreData.Value = scoreData.Value + num;
    }
}
