using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HighwayRacers {

	public class SliderProp : MonoBehaviour {

		[Header("Children")]
		public Text text;
		public Slider slider;

		public void SetText(string text)
		{
			this.text.text = text;	
		}

		public void SetBounds(float min, float max)
		{
			slider.minValue = min;
			slider.maxValue = max;
		}

		public float value {
			get {
				return slider.value;
			}
			set {
				slider.value = value;
			}
		}

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}

}