using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

namespace JumpTheGun
{
    public class TimerText : MonoBehaviour
    {
        public TextMeshProUGUI textWidget;
        private float _elapsedTime;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void ResetToZero()
        {
            _elapsedTime = 0;
        }

        // Update is called once per frame
        void Update()
        {
            _elapsedTime += Time.deltaTime;
            int seconds = (int) _elapsedTime;
            textWidget.text = $"{seconds / 3600:D2}:{seconds / 60:D2}:{seconds % 60:D2}";
        }
    }
}