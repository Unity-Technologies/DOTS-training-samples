using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
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
			selectedCar = Entity.Null;
		}

		public void Show(Entity car)
        {
			gameObject.SetActive(true);
			selectedCar = car;

//TODO			preventUpdatingCar = true;
			SetSliderProperties();
//TODO			preventUpdatingCar = false;
		}

		public Entity selectedCar { get; private set; }

		private void SetSliderProperties() {
#if false // TODO
			if (selectedCar == null)
				return;
			defaultSpeed.value = selectedCar.defaultSpeed;
			defaultSpeed.SetText("Default Speed: " + selectedCar.defaultSpeed.ToString("0.0") + " m/s");
			overtakePercent.value = selectedCar.overtakePercent;
			overtakePercent.SetText("Overtake Speed: " + Mathf.RoundToInt(selectedCar.overtakePercent * 100) + "%");
			leftMergeDistance.value = selectedCar.leftMergeDistance;
			leftMergeDistance.SetText("Distance to car in front before overtaking: " + selectedCar.leftMergeDistance.ToString("0.0") + " m");
			mergeSpace.value = selectedCar.mergeSpace;
			float combinedMergeSpace = selectedCar.mergeSpace * 2 + selectedCar.distanceToFront + selectedCar.distanceToBack;
			mergeSpace.SetText("Merge Space: " + combinedMergeSpace.ToString("0.0") + " m");
			overtakeEagerness.value = selectedCar.overtakeEagerness;
			overtakeEagerness.SetText("Overtake Eagerness: " + selectedCar.overtakeEagerness.ToString("0.0"));
#endif
		}

#if false // TODO
		bool preventUpdatingCar = false;

		public void SliderUpdated(float value){
			if (selectedCar == null)
				return;
			if (preventUpdatingCar)
				return;

			selectedCar.defaultSpeed = defaultSpeed.value;
			selectedCar.overtakePercent = overtakePercent.value;
			selectedCar.leftMergeDistance = leftMergeDistance.value;
			selectedCar.mergeSpace = mergeSpace.value;
			selectedCar.overtakeEagerness = overtakeEagerness.value;

			SetSliderProperties();
		}
#endif
		public void BackButtonPressed(){

			Game.instance.TopDownView();

		}

		void Awake() {
			if (instance != null) {
				Destroy (gameObject);
				return;
			}
			instance = this;
#if false // TODO
			defaultSpeed.slider.onValueChanged.AddListener(SliderUpdated);
			overtakePercent.slider.onValueChanged.AddListener(SliderUpdated);
			leftMergeDistance.slider.onValueChanged.AddListener(SliderUpdated);
			mergeSpace.slider.onValueChanged.AddListener(SliderUpdated);
			overtakeEagerness.slider.onValueChanged.AddListener(SliderUpdated);
#endif
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
		void Update () {}

		void OnDestroy() {
			if (instance == this) {
				instance = null;
			}
		}
	}
}
