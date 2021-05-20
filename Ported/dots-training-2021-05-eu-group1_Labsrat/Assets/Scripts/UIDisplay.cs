using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class UIDisplay : MonoBehaviour
{
    public static NativeArray<int> PlayerScores;
    public static NativeArray<float4> PlayerColors;

    public static float TimeLeft;
    public float TimerXPos, TimerYPos;

    public static bool EndGame;
    public static int Winner;

    private void Start()
    {
        EndGame = false;
    }

    private void OnGUI()
    {
        var timerPos = new Rect(TimerXPos, TimerYPos, Screen.height, Screen.width);
        GUI.color = Color.black;
        GUI.Label(timerPos, TimeLeft.ToString());

        for(int i = 0; i < PlayerScores.Length; i++)
        {
            var scoreColor = PlayerColors[i];
            
            GUI.color = new Color(scoreColor.x, scoreColor.y, scoreColor.z, 1);
            GUILayout.Label($"{i} - {PlayerScores[i]}");
        }

        if (EndGame)
        {
            // Display winner
            var winnerPos = new Rect(TimerXPos, TimerYPos + 20, Screen.height, Screen.width);
            var winnerColor = PlayerColors[Winner];
            
            GUI.color = new Color(winnerColor.x, winnerColor.y, winnerColor.z, 1);
            GUI.Label(winnerPos, $"Player {Winner} won");
        }
    }
}
