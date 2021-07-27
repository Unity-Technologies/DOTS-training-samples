using UnityEngine;
using UnityEngine.UI;

public class HUDPauseButton : MonoBehaviour
{
    [Header("Children")]
    public Text _Text;

    public void OnPress()
    {
        Config.Instance.Data.Paused = !Config.Instance.Data.Paused;
        _Text.text = Config.Instance.Data.Paused ? "Paused" : "Pause";
    }
}
