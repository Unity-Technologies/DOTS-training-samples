using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace HighwayRacers
{

    public class Game : MonoBehaviour
    {
        public static Game instance { get; private set; }

        #region Inspector Properties

		[Header("Highway Bounds Properties")]
		public int maxNumCars = 100;
		public float highwayMaxSize = 500;

		[Header("Car Properties")]
		public float acceleration = 15;
		public float brakeDeceleration = 20;
		[Tooltip("\"Speed\" in lanes on how quickly the car changes lanes.")]
		public float switchLanesSpeed = 3;
		[Tooltip("Give up on overtaking a car if it takes this long or more.")]
		public float overtakeMaxDuration = 5;

		[Header("Car Bounds Properties")]
		public float defaultSpeedMin = 15;
		public float defaultSpeedMax = 25;
		[Tooltip("Min bound for a car's overtake percentage.  This is the percent of the car's default speed it will be at when attempting to overtake a car.")]
		public float overtakePercentMin = 1.2f;
		[Tooltip("Max bound for a car's overtake percentage.  This is the percent of the car's default speed it will be at when attempting to overtake a car.")]
		public float overtakePercentMax = 2f;
		[Tooltip("Min distance to slow car in front needed to start merge to the left.")]
		public float leftMergeDistanceMin = 1;
		[Tooltip("Max distance to slow car in front needed to start merge to the left.")]
		public float leftMergeDistanceMax = 3;
		[Tooltip("Min distance required between this car and cars in adjacent lanes to start a merge.")]
		public float mergeSpaceMin = 1;
		[Tooltip("Max distance required between this car and cars in adjacent lanes to start a merge.")]
		public float mergeSpaceMax = 3;
		[Tooltip("Min bound for eagerness; If eagerness > (car in front's speed) / (this car's speed), then this car will attempt to overtake the car in front.")]
		public float overtakeEagernessMin = .5f;
		[Tooltip("Max bound for eagerness; If eagerness > (car in front's speed) / (this car's speed), then this car will attempt to overtake the car in front.")]
		public float overtakeEagernessMax = 1.5f;

		[Header("Car Global Settings")]
        [Tooltip("Distance from center of car to the front.")]
        public float distanceToFront = 1;
        [Tooltip("Distance from center of car to the back.")]
        public float distanceToBack = 1;
        public Color defaultColor = Color.grey;
        public Color maxSpeedColor = Color.green;
        public Color minSpeedColor = Color.red;

		[Header("UI")]
		public float carSelectRadius = 7;

		[Header("Space Partition")]
		public float bucketDistance = 10;

        #endregion

		public enum ViewState
		{
			NONE,
			TOP_DOWN,
			CAR
		}
		public ViewState viewState { get; private set; }
		public Mesh entityMesh;
		public Material entityMaterial;

		public void TopDownView(){
			viewState = ViewState.TOP_DOWN;
			HighwayOptions.instance.Show();
			CarProperties.instance.Hide();
			CameraControl.instance.ToTopDownView();
		}

		public void CarView(Entity car)
        {
			viewState = ViewState.CAR;
			HighwayOptions.instance.Hide();
			CarProperties.instance.Show(car);
			CameraControl.instance.ToCarView(car);
		}

		public void SelectCarAtMousePosition()
        {
			if (CameraControl.instance.state != CameraControl.State.TOP_DOWN)
				return; // can only select in top down mode

			var car = Highway.instance.GetCarAtScreenPosition(Input.mousePosition, carSelectRadius);
			if (car != Entity.Null) {
				CarView(car);
			}
		}

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

        }

		void Start() {

			Highway.instance.CreateHighway(250);
			Highway.instance.SetNumCars(100);
			HighwayOptions.instance.UpdateSliderValues();

			TopDownView();
		}

        private void Update()
        {

			switch (viewState) {
			case ViewState.TOP_DOWN:

				break;
			case ViewState.CAR:
				break;
			}

			if (Input.GetKeyDown (KeyCode.Escape)) {
				Application.Quit();
			}

        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }

        }

    }

}
