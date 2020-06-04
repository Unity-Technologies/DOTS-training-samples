using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour
{
    public static UIHelper Instance;

    public Text[] Scores;
    public int[] ScoreValues;
    
    public void Awake()
    {
        Instance = this;
        Debug.Log("UIHelper Available");
        
        ScoreValues = new int[Scores.Length];
    }
    
    public void SetScore(int playerId, int score)
    {
        if (playerId < 0 || playerId >= ScoreValues.Length)
        {
            return;
        }

        if (ScoreValues[playerId] != score)
        {
            ScoreValues[playerId] = score;
            Scores[playerId].text = score.ToString();
        }
    }
}
