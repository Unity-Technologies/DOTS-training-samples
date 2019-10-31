using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace ECSExamples
{
    public class IntroTextUpdater : MonoBehaviour
    {
        GameManagementSystem m_gameManager;
        Text m_introText;

        void OnEnable()
        {
            m_introText = GetComponent<Text>();
            m_gameManager = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameManagementSystem>();
        }

        void Update()
        {
            if (m_gameManager.TheGameState == GameManagementSystem.GameState.DisplayReadyText ||
                m_gameManager.TheGameState == GameManagementSystem.GameState.DisplaySetText ||
                m_gameManager.TheGameState == GameManagementSystem.GameState.DisplayGoText
                )
            {
                m_introText.enabled = true;
                m_introText.text = m_gameManager.IntroText;
            }
            else
            {
                m_introText.enabled = false;
            }
        }
    }

}
