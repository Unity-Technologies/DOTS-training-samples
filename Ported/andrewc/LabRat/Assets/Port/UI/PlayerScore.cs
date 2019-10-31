using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace ECSExamples
{
    public class PlayerScore : MonoBehaviour
    {
        public int PlayerIndex;
        Text text;
        GameManagementSystem m_gameManager;

        void OnEnable()
        {
            text = GetComponent<Text>();
            m_gameManager = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameManagementSystem>();

            text.color = m_gameManager.GetPlayerColor(PlayerIndex);
        }

        void Update()
        {
            text.text = m_gameManager.GetPlayerScore(PlayerIndex).ToString();
        }
    }
}
