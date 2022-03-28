using UnityEngine;
using UnityEngine.UI;

namespace JumpTheGun
{

    public class PauseButton : MonoBehaviour
    {
        [Header("Children")]
        public Text text;

        public void ButtonPressed()
        {
            Game.instance.isPaused = !Game.instance.isPaused;
            text.text = Game.instance.isPaused ? "Paused" : "Pause";
        }

    }

}