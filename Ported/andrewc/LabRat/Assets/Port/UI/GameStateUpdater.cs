using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace ECSExamples
{
    public class GameStateUpdater : MonoBehaviour
    {
        GameManagementSystem m_gameManager;

        public GameObject m_introTextObject;
        public GameObject m_gameOverOverlayObject;
        public GameObject m_gameOverTextObject;

        Text m_introText;
        Text m_gameOverText;

        bool m_isGameOverActive = false;
        bool m_isIntroActive = false;

        void OnEnable()
        {
            m_introText = m_introTextObject.GetComponent<Text>();
            m_gameOverText = m_gameOverTextObject.GetComponent<Text>();
            m_gameManager = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameManagementSystem>();
        }

        string GetWinnerText()
        {
            int numWinners = 0;
            int maxScore = -1;
            int winningIndex = -1;
            for (int i = 0; i < m_gameManager.NumPlayers; ++i)
            {
                int score = (int)m_gameManager.GetPlayerScore(i);
                if (score > maxScore)
                {
                    maxScore = score;
                    winningIndex = i;
                }
            }

            for (int i = 0; i < m_gameManager.NumPlayers; ++i)
            {
                if (m_gameManager.GetPlayerScore(i) == maxScore)
                    numWinners++;
            }

            if (numWinners > 1)
            {
                return "TIE!";
            }

            return $"{m_gameManager.GetPlayerColor(winningIndex)} WINS!";
        }

        void Update()
        {
            if (m_gameManager.TheGameState == GameManagementSystem.GameState.DisplayReadyText ||
                m_gameManager.TheGameState == GameManagementSystem.GameState.DisplaySetText ||
                m_gameManager.TheGameState == GameManagementSystem.GameState.DisplayGoText)
            {
                if (m_isGameOverActive)
                {
                    m_gameOverOverlayObject.SetActive(false);
                    m_gameOverTextObject.SetActive(false);

                    m_isGameOverActive = false;
                }

                if (!m_isIntroActive || m_introText.text != m_gameManager.IntroText)
                {
                    m_introTextObject.SetActive(true);
                    m_introText.text = m_gameManager.IntroText;

                    m_isIntroActive = true;
                }
            }
            else if (m_gameManager.TheGameState == GameManagementSystem.GameState.GameOver)
            {
                if (!m_isGameOverActive)
                {
                    m_gameOverOverlayObject.SetActive(true);
                    m_gameOverTextObject.SetActive(true);
                    m_isGameOverActive = true;

                    m_gameOverText.text = GetWinnerText();
                }
            }
            else if (m_gameManager.TheGameState == GameManagementSystem.GameState.GamePlayInProgress)
            {
                if (m_isIntroActive)
                {
                    m_introTextObject.SetActive(false);
                    m_isIntroActive = false;
                }
            }
        }
    }

}
