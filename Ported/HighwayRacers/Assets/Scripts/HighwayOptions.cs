using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace HighwayRacers
{
	public class HighwayOptions : MonoBehaviour
	{

		[Header("Children")]
		public Text numCarsText;
		public Slider numCarsSlider;
		public Text highwaySizeText;
		public Slider highwaySizeSlider;

		public static HighwayOptions instance { get; private set; }

		public void Show() {
			gameObject.SetActive(true);
		}
		public void Hide() {
			gameObject.SetActive(false);
		}

		public void UpdateButtonPressed()
		{
			var system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<UISystem>();

			system.Struct.UpdateTrackConfiguration(new()
			{
				highwaySize = Mathf.RoundToInt(highwaySizeSlider.value),
				numberOfCars = Mathf.RoundToInt(numCarsSlider.value)
			});
		}

		public void UpdateSliderValues(TrackConfigMinMax tcmm, TrackConfig tc)
		{
			// ideally we won't have components referenced directly in UI code but for now this works.
			// proper UI not a focus for this project probly

			numCarsSlider.minValue = tcmm.minNumberOfCars;
			numCarsSlider.maxValue = tcmm.maxNumberOfCars;
			highwaySizeSlider.minValue = tcmm.minHighwaySize;
			highwaySizeSlider.maxValue = tcmm.maxHighwaySize;

			numCarsSlider.value = tc.numberOfCars;
			highwaySizeSlider.value = tc.highwaySize;

		}

		// Use this for initialization
		void Awake ()
		{
			if (instance != null)
			{
				Destroy(gameObject);
				return;
			}
			instance = this;
			
			numCarsSlider.onValueChanged.AddListener(NumCarsSliderValueChanged);
			highwaySizeSlider.onValueChanged.AddListener(HighwaySizeSliderValueChanged);
		}

		void Start() { }

		private void NumCarsSliderValueChanged(float value)
		{
			numCarsText.text = "Number of Cars: " + Mathf.RoundToInt(value);
		}
		private void HighwaySizeSliderValueChanged(float value)
		{
			highwaySizeText.text = "Highway Size: " + Mathf.RoundToInt(value);
		}
		
		// Update is called once per frame
		void Update ()
		{
		}

		void OnDestroy() {

			if (instance == this) {
				instance = null;
			}

		}

	}

}