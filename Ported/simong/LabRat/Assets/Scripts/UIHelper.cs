using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour
{
    public static UIHelper Instance;

    public Text[] Scores;
    public Image[] Cursors;
    public Text RoundTime;
    
    public Canvas GameOverCanvas;
    public Text GameOverText;
    
    private int[] m_ScoreValues;
    private int m_RemainingTime;

    public void Awake()
    {
        Instance = this;
        Debug.Log("UIHelper Available");

        m_ScoreValues = new int[Scores.Length];
    }

    public void Start()
    {
        var constantData = ConstantData.Instance;
        if (constantData != null)
        {
            for (int i = 0; i < Scores.Length; i++)
            {
                int colorIndex = i % constantData.PlayerColors.Length;
                Scores[i].color = constantData.PlayerColors[colorIndex];
                Cursors[i].color = constantData.PlayerColors[colorIndex];
            }
        }
    }

    public void SetScore(int playerId, int score)
    {
        if (playerId < 0 || playerId >= m_ScoreValues.Length)
        {
            return;
        }

        if (m_ScoreValues[playerId] != score)
        {
            m_ScoreValues[playerId] = score;
            Scores[playerId].text = score.ToString();
        }
    }

    public void SetCursorPosition(int playerId, int2 position)
    {
        if (playerId < 0 || playerId >= m_ScoreValues.Length)
        {
            return;
        }

        Cursors[playerId].rectTransform.anchoredPosition = new Vector2(position.x, position.y);
    }

    public void SetRemainingTime(float time)
    {
        var newRemainingTime = Mathf.CeilToInt(time);
        if (newRemainingTime != m_RemainingTime)
        {
            m_RemainingTime = newRemainingTime;

            var minutes = m_RemainingTime / 60;
            var seconds = m_RemainingTime % 60;

            RoundTime.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void ShowGameOverScreen(string gameOverText)
    {
        GameOverCanvas.enabled = true;
        GameOverText.text = gameOverText;
    }
}