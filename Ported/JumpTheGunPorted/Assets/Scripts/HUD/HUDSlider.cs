using UnityEngine;
using UnityEngine.UI;

public class HUDSlider : MonoBehaviour
{
	[Header("Children")]
	public Text _Text;
	public Slider _Slider;

	public void SetText(string text)
	{
		_Text.text = text;	
	}

	public void SetBounds(float min, float max)
	{
		_Slider.minValue = min;
		_Slider.maxValue = max;
	}

	public float Value
	{
		get => _Slider.value;
		set => _Slider.value = value;
	}
}
