using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ScoreCounter : MonoBehaviour
{

    [SerializeField] 
    TMP_Text score;
    public int currentScore = 0;
    public int onBoardScore = 0;
   
    void Update()
    {
        
        if(onBoardScore!=currentScore)
        {
            onBoardScore = currentScore;
            if(score.text != "your score " + onBoardScore.ToString())
            {
                score.text = "your score " + onBoardScore.ToString();
            }           
        }       
    }
}
