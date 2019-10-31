using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace ECSExamples
{

    [RequireComponent(typeof(Text))]
    public class TimerUpdater : MonoBehaviour
    {
        GameManagementSystem m_gameManager;
        Text text;

        void OnEnable()
        {
            text = GetComponent<Text>();
            m_gameManager = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameManagementSystem>();
        }

        void Update()
        {
            var timeLeft = m_gameManager.NumSecodsLeftInGame;
            var t = TimeSpan.FromSeconds(timeLeft);
            var answer = string.Format("{1:D2}:{2:D2}",
                t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
            text.text = answer.ToString();
            text.color = timeLeft < 5f ? Color.red : Color.white;
        }
    }

}
