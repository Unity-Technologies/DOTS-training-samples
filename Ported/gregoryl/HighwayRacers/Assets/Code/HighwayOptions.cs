using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HighwayRacers
{

	public class HighwayOptions : MonoBehaviour {

		[Header("Children")]
		public Text numCarsText;
		public Slider numCarsSlider;
		public Text highwaySizeText;
		public Slider highwaySizeSlider;
		public Text lanesText;
		public Slider lanesSlider;

		public static HighwayOptions instance { get; private set; }

		public void Show() {
			gameObject.SetActive (true);
		}
		public void Hide() {
			gameObject.SetActive(false);
		}

		public void UpdateButtonPressed()
		{
			Highway.instance.CreateHighway(highwaySizeSlider.value);
			Highway.instance.SetNumCars(Mathf.RoundToInt(numCarsSlider.value));
			Highway.instance.NumLanes = (Mathf.RoundToInt(lanesSlider.value));
			numCarsSlider.value = Highway.instance.NumCars;
		}

		public void UpdateSliderValues(){

			numCarsSlider.minValue = 1;
			numCarsSlider.maxValue = Game.instance.maxNumCars;
			highwaySizeSlider.minValue = Mathf.Ceil(Highway.instance.MIN_HIGHWAY_LANE0_LENGTH);
			highwaySizeSlider.maxValue = Game.instance.highwayMaxSize;
            lanesSlider.minValue = 2;
            lanesSlider.maxValue = 16;

			numCarsSlider.value = Highway.instance.NumCars;
			highwaySizeSlider.value = Highway.instance.DotsHighway.Lane0Length;
		}

		// Use this for initialization
		void Awake () {

			if (instance != null) {
				Destroy (gameObject);
				return;
			}
			instance = this;

			numCarsSlider.onValueChanged.AddListener(NumCarsSliderValueChanged);
			highwaySizeSlider.onValueChanged.AddListener(HighwaySizeSliderValueChanged);
			lanesSlider.onValueChanged.AddListener(LanesSliderValueChanged);
		}

		void Start() {

			UpdateSliderValues();

		}

		private void NumCarsSliderValueChanged(float value) {
			numCarsText.text = "Number of Cars: " + Mathf.RoundToInt(value);
		}
		private void HighwaySizeSliderValueChanged(float value) {
			highwaySizeText.text = "Highway Size: " + Mathf.RoundToInt(value);
		}
		private void LanesSliderValueChanged(float value) {
			lanesText.text = "Number of Lanes: " + Mathf.RoundToInt(value);
		}

		void OnDestroy() {

			if (instance == this) {
				instance = null;
			}

		}

	}

}
