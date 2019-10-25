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

			preventUpdatingCar = true;
			SetSliderProperties();
			preventUpdatingCar = false;
		}

		public Entity selectedCar { get; private set; }

		bool preventUpdatingCar = false;

		private void SetSliderProperties()
        {
            var em = World.Active?.EntityManager;
            if (em == null || selectedCar == Entity.Null)
                return;
            var settings = em.GetComponentData<CarSettings>(selectedCar);
			defaultSpeed.value = settings.DefaultSpeed;
			defaultSpeed.SetText("Default Speed: " + settings.DefaultSpeed.ToString("0.0") + " m/s");
			overtakePercent.value = settings.OvertakePercent;
			overtakePercent.SetText("Overtake Speed: " + Mathf.RoundToInt(settings.OvertakePercent * 100) + "%");
			leftMergeDistance.value = settings.LeftMergeDistance;
			leftMergeDistance.SetText("Distance to car in front before overtaking: " + settings.LeftMergeDistance.ToString("0.0") + " m");
			mergeSpace.value = settings.MergeSpace;
			float combinedMergeSpace = settings.MergeSpace * 2 + Game.instance.distanceToFront + Game.instance.distanceToBack;
			mergeSpace.SetText("Merge Space: " + combinedMergeSpace.ToString("0.0") + " m");
			overtakeEagerness.value = settings.OvertakeEagerness;
			overtakeEagerness.SetText("Overtake Eagerness: " + settings.OvertakeEagerness.ToString("0.0"));
		}


		public void SliderUpdated(float value)
        {
			if (preventUpdatingCar)
				return;
            var em = World.Active?.EntityManager;
            if (em == null || selectedCar == Entity.Null)
                return;

            var settings = em.GetComponentData<CarSettings>(selectedCar);
			settings.DefaultSpeed = defaultSpeed.value;
			settings.OvertakePercent = overtakePercent.value;
			settings.LeftMergeDistance = leftMergeDistance.value;
			settings.MergeSpace = mergeSpace.value;
			settings.OvertakeEagerness = overtakeEagerness.value;
            em.SetComponentData(selectedCar, settings);

			SetSliderProperties();
		}

		public void BackButtonPressed()
        {
			Game.instance.TopDownView();
		}

		void Awake()
        {
			if (instance != null)
            {
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
		void Start ()
        {
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

		void OnDestroy()
        {
			if (instance == this)
				instance = null;
		}
	}
}
