using UnityEngine;
using UnityEngine.UI;

public class HUDClock : MonoBehaviour
{
	[Header("Children")]
	public Text _Text;

	public void SetTimeText(float time)
	{
		int mins = Mathf.FloorToInt(time / 60);
		int secs = Mathf.FloorToInt(time - mins * 60);
		_Text.text = "Time: " + mins.ToString("00") + ":" + secs.ToString("00");
	}

    private void Update()
    {
		SetTimeText(Config.Instance.TimeVal);
	}
}
