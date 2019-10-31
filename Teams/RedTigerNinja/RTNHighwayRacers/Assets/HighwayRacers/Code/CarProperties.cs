using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighwayRacers {

	public class CarProperties : MonoBehaviour {

		[Header("Children")]
		public SliderProp defaultSpeed;
		public SliderProp overtakePercent;
		public SliderProp leftMergeDistance;
		public SliderProp mergeSpace;
		public SliderProp overtakeEagerness;

		public static CarProperties instance { get; private set; }

		public void Hide(){
			gameObject.SetActive(false);
			selectedCar = null;
		}

		public void Show(Car car) {
			gameObject.SetActive(true);
			selectedCar = car;

			preventUpdatingCar = true;
			SetSliderProperties();
			preventUpdatingCar = false;
		}

		public Car selectedCar { get; private set; }

		private void SetSliderProperties() {
			if (selectedCar == null)
				return;
			defaultSpeed.value = selectedCar.CarData.Settings.defaultSpeed;
			defaultSpeed.SetText("Default Speed: " + selectedCar.CarData.Settings.defaultSpeed.ToString("0.0") + " m/s");
			overtakePercent.value = selectedCar.CarData.Settings.overtakePercent;
			overtakePercent.SetText("Overtake Speed: " + Mathf.RoundToInt(selectedCar.CarData.Settings.overtakePercent * 100) + "%");
			leftMergeDistance.value = selectedCar.CarData.Settings.leftMergeDistance;
			leftMergeDistance.SetText("Distance to car in front before overtaking: " + selectedCar.CarData.Settings.leftMergeDistance.ToString("0.0") + " m");
			mergeSpace.value = selectedCar.CarData.Settings.mergeSpace;
			float combinedMergeSpace = selectedCar.CarData.Settings.mergeSpace * 2 + CarShared.distanceToFront + CarShared.distanceToBack;
			mergeSpace.SetText("Merge Space: " + combinedMergeSpace.ToString("0.0") + " m");
			overtakeEagerness.value = selectedCar.CarData.Settings.overtakeEagerness;
			overtakeEagerness.SetText("Overtake Eagerness: " + selectedCar.CarData.Settings.overtakeEagerness.ToString("0.0"));

		}

		public void SliderUpdated(float value){
			if (selectedCar == null)
				return;
			if (preventUpdatingCar)
				return;

			selectedCar.CarData.Settings.defaultSpeed = defaultSpeed.value;
			selectedCar.CarData.Settings.overtakePercent = overtakePercent.value;
			selectedCar.CarData.Settings.leftMergeDistance = leftMergeDistance.value;
			selectedCar.CarData.Settings.mergeSpace = mergeSpace.value;
			selectedCar.CarData.Settings.overtakeEagerness = overtakeEagerness.value;
            selectedCar.CarData.UpdateComponent(selectedCar.CarData.Settings);

			SetSliderProperties();
		}

		public void BackButtonPressed(){

			Game.instance.TopDownView();

		}

		void Awake() {
			if (instance != null) {
				Destroy (gameObject);
				return;
			}
			instance = this;

			defaultSpeed.slider.onValueChanged.AddListener(SliderUpdated);
			overtakePercent.slider.onValueChanged.AddListener(SliderUpdated);
			leftMergeDistance.slider.onValueChanged.AddListener(SliderUpdated);
			mergeSpace.slider.onValueChanged.AddListener(SliderUpdated);
			overtakeEagerness.slider.onValueChanged.AddListener(SliderUpdated);

		}

		// Use this for initialization
		void Start () {

			// setting slider bounds
			defaultSpeed.SetBounds(Game.instance.defaultSpeedMin, Game.instance.defaultSpeedMax);
			overtakePercent.SetBounds(Game.instance.overtakePercentMin, Game.instance.overtakePercentMax);
			leftMergeDistance.SetBounds(Game.instance.leftMergeDistanceMin, Game.instance.leftMergeDistanceMax);
			mergeSpace.SetBounds(Game.instance.mergeSpaceMin, Game.instance.mergeSpaceMax);
			overtakeEagerness.SetBounds(Game.instance.overtakeEagernessMin, Game.instance.overtakeEagernessMax);


			Hide();

		}
		
		// Update is called once per frame
		void Update () {
			
		}

		void OnDestroy() {
			if (instance == this) {
				instance = null;
			}
		}

		bool preventUpdatingCar = false;


	}

}