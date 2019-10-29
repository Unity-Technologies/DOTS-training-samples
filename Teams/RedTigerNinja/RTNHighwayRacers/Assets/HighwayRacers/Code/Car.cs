using Unity.Entities;
using UnityEngine;

namespace HighwayRacers
{

    public struct CarStateStruct : Unity.Entities.IComponentData
    {
        public float defaultSpeed;
        public float overtakePercent;
        public float leftMergeDistance;
        public float mergeSpace;

        public float overtakeEagerness;

        public float velocityPosition;

        public float velocityLane;

        public MergeState state;

        public int DEBUG_JobTester;
        public Entity ThisCarEntity;


        /// <summary>
        /// Distance in current lane.  Can change when switching lanes.
        /// </summary>
        public float distance;


        /// <summary>
        /// Ranges from [0, 4]
        /// </summary>
        public float lane;


        public float targetLane;

        public bool hidden;

        /// <summary>
        /// Car to pass before considering merging right
        /// </summary>
        //public CarStateStruct overtakeCar;
        public Entity overtakeCarEntity;
        public float timeOvertakeCarSet;

        public bool IsNotNull { get { return !this.IsNull; } }
        public bool IsNull { get { return (this.ThisCarEntity == Entity.Null); } }

        public static CarStateStruct NullCar {
            get
            {
                CarStateStruct ans = new CarStateStruct();
                ans.ThisCarEntity = Entity.Null;
                return ans;
            }
        }

        public float maxSpeed
        {
            get
            {
                return defaultSpeed * overtakePercent;
            }
        }


        /// <summary>
        /// Distance of the back of the car in the current lane.
        /// </summary>
        public float distanceBack
        {
            get
            {
                return (distance - GlobalShared.distanceToBack) + Mathf.Floor((distance - GlobalShared.distanceToBack) / Highway.instance.length(lane)) * Highway.instance.length(lane);
            }
        }

        /// <summary>
        /// Distance of the front of the car in the current lane.
        /// </summary>
        public float distanceFront
        {
            get
            {
                return (distance + GlobalShared.distanceToFront) + Mathf.Floor((distance + GlobalShared.distanceToFront) / Highway.instance.length(lane)) * Highway.instance.length(lane);
            }
        }

        public bool isMerging
        {
            get { return state == MergeState.MERGE_LEFT || state == MergeState.MERGE_RIGHT; }
        }


        public bool IsInLane(float lane)
        {
            return Highway.LanesOverlap(lane, this.lane);
        }

        public Car.CarShared GlobalShared
        {
            get { return Car.GlobalShared; }
        }
    }


    public enum MergeState
    {
        NORMAL,
        MERGE_RIGHT,
        MERGE_LEFT,
        OVERTAKING,
    }


    public class Car : MonoBehaviour
    {
        #region Constant for all cars:

        public class CarShared
        {

            [Tooltip("Distance from center of car to the front.")]
            public float distanceToFront = 1;
            [Tooltip("Distance from center of car to the back.")]
            public float distanceToBack = 1;

            public Color defaultColor = Color.gray;
            public Color maxSpeedColor = Color.green;
            public Color minSpeedColor = Color.red;
        }

        public static CarShared GlobalShared = new CarShared();
        public CarShared Shared { get { return GlobalShared; } }




            #endregion


            [Header("Children")]
        public MeshRenderer topRenderer;
        public MeshRenderer baseRenderer;
        public Transform cameraPos;

        public CarStateStruct CarData;

        /*
        public float defaultSpeed { get; set; }
		public float overtakePercent { get; set; }
		public float leftMergeDistance { get; set; }
		public float mergeSpace { get; set; }

		public float overtakeEagerness { get; set; }




        #region Moved up state


        public float velocityPosition { get; set; }

        public float velocityLane { get; set; }

        public State state { get; private set; }


        /// <summary>
        /// Distance in current lane.  Can change when switching lanes.
        /// </summary>
        public float distance { get; set; }


        /// <summary>
        /// Ranges from [0, 4]
        /// </summary>
        public float lane { get; set; }


        private float targetLane = 0;

        private bool hidden = false;

        /// <summary>
        /// Car to pass before considering merging right
        /// </summary>
        private Car overtakeCar = null;
        private float timeOvertakeCarSet = 0;

        #endregion
        */



        public Color color
        {
            get
            {
                return topRenderer.material.color;
            }
            set
            {
				topRenderer.material.color = value;
                baseRenderer.material.color = value;
            }
        }

		public void Show() {
			if (!CarData.hidden)
				return;
			topRenderer.enabled = true;
			baseRenderer.enabled = true;
            CarData.hidden = false;
		}

		public void Hide() {
			if (CarData.hidden)
				return;
			topRenderer.enabled = false;
			baseRenderer.enabled = false;
            CarData.hidden = true;

		}

		public void SetRandomPropeties()
		{
            var data = CarData;
            data.defaultSpeed = Random.Range(Game.instance.defaultSpeedMin, Game.instance.defaultSpeedMax);
            data.overtakePercent = Random.Range(Game.instance.overtakePercentMin, Game.instance.overtakePercentMax);
            data.leftMergeDistance = Random.Range(Game.instance.leftMergeDistanceMin, Game.instance.leftMergeDistanceMax);
            data.mergeSpace = Random.Range(Game.instance.mergeSpaceMin, Game.instance.mergeSpaceMax);
            data.overtakeEagerness = Random.Range(Game.instance.overtakeEagernessMin, Game.instance.overtakeEagernessMax);
            CarData = data;
		}



        /// <summary>
        /// Changes lanes, also setting distance to the appropriate value.
        /// </summary>
        /// <param name="newLane"></param>
        public static void ChangeLane(ref CarStateStruct car, float newLane)
        {
			float roundLane = Mathf.Round(newLane);
			if (Mathf.Abs (roundLane - newLane) < .0001f) {
				newLane = roundLane;
			}

            car.distance = Highway.instance.GetEquivalentDistance(car.distance, car.lane, newLane);
            car.lane = newLane;
        }


        public void UpdatePosition(ref CarStateStruct car)
        {
            float x, z, rotation;
            Highway.instance.GetPosition(car.distance, car.lane, out x, out z, out rotation);

            transform.localPosition = new Vector3(x, transform.position.y, z);
            transform.localRotation = Quaternion.Euler(0, rotation * Mathf.Rad2Deg, 0);
        }

        private void Awake()
        {

        }


        public static CarStateStruct GetOtherCar(Entity id)
        {
            return CarStateStruct.NullCar;
        }

        private void Update()
        {
            UpdateCarData(ref this.CarData);
            UpdatePosition(ref this.CarData);
            UpdateColor(ref this.CarData);
        }

        public static void UpdateCarState_FromJob(ref CarStateStruct car)
        {
            //UpdateCarData(ref car);
        }

        private static void UpdateCarData(ref CarStateStruct car)
        {
			float dt = Time.deltaTime;
			if (dt == 0) // possible when the game is paused
				return;

			float targetSpeed = car.defaultSpeed;
			CarStateStruct carInFront = Highway.instance.GetCarInFront(car.distance, car.lane);
			float distToCarInFront = (carInFront.IsNull) ? float.MaxValue : Highway.instance.DistanceTo(car.distanceFront, car.lane, carInFront.distanceBack, carInFront.lane);

			switch (car.state) {
			case MergeState.NORMAL:

				targetSpeed = car.defaultSpeed;
                    car.velocityLane = 0;

				// if won't merge, match car in front's speed
				if (distToCarInFront < car.leftMergeDistance) {
					targetSpeed = Mathf.Min(targetSpeed, carInFront.velocityPosition);
				}

				break;

			case MergeState.MERGE_LEFT:

				targetSpeed = car.defaultSpeed;

                    car.velocityLane = Game.instance.switchLanesSpeed;
				// detect ending merge
				if (car.lane + car.velocityLane * dt >= car.targetLane) {
                        // set veloicty to not overshoot lane
                        car.velocityLane = (car.targetLane - car.lane) / dt;
					if (car.lane >= car.targetLane) { // end when frame started in the target lane
                            car.state = MergeState.OVERTAKING;
					}
				}

				break;

			case MergeState.OVERTAKING:

				targetSpeed = car.maxSpeed;
                    car.velocityLane = 0;

				break;

			case MergeState.MERGE_RIGHT:

				targetSpeed = car.defaultSpeed;

                    car.velocityLane = -Game.instance.switchLanesSpeed;
				// detect ending merge
				if (car.lane + car.velocityLane * dt <= car.targetLane) {
                        // set veloicty to not overshoot lane
                        car.velocityLane = (car.targetLane - car.lane) / dt;
					if (car.lane <= car.targetLane) { // end when frame started in the target lane
                            car.state = MergeState.NORMAL;
					}
				}

				break;
			}

			// detect overtaking car taking too long
			if ((car.overtakeCarEntity != Entity.Null) && Time.time - car.timeOvertakeCarSet >= Game.instance.overtakeMaxDuration) {
                car.overtakeCarEntity = Entity.Null;
			}

			// detect merging
			if (!car.isMerging) {

				// detect merging to left lane
				if (car.lane + 1 < Highway.NUM_LANES // left lane exists
					&& distToCarInFront < car.leftMergeDistance // close enough to car in front
					&& car.overtakeEagerness > carInFront.velocityPosition / car.defaultSpeed // car in front is slow enough		
				) {

					if (Highway.instance.CanMergeToLane(car, car.lane + 1)) { // if space is available
                                                                               // start merge to left
                        car.state = MergeState.MERGE_LEFT;
                        car.targetLane = Mathf.Round(car.lane + 1);
                        car.overtakeCarEntity = carInFront.ThisCarEntity;
                        car.timeOvertakeCarSet = Time.time;
					}
				}

				// detect merging to right lane
				bool tryMergeRight = false;
				if (car.overtakeCarEntity == Entity.Null) {
                    // if overtake car got destroyed
                    car.overtakeCarEntity = Entity.Null;
					tryMergeRight = true;
				} else {
					// if passed overtake car
					if (Highway.instance.DistanceTo(car.distance, car.lane, GetOtherCar( car.overtakeCarEntity ).distance, GetOtherCar(car.overtakeCarEntity).lane) > Highway.instance.length(car.lane) / 2) {
						tryMergeRight = true;
					}
				}

				if (car.lane - 1 < 0) { // right lane must exist
					tryMergeRight = false;
				}

				if (tryMergeRight) {
					// don't merge if just going to merge back
					CarStateStruct rightCarInFront = Highway.instance.GetCarInFront(Highway.instance.GetEquivalentDistance(car.distanceFront, car.lane, car.lane - 1), car.lane - 1);
					float distToRightCarInFront = rightCarInFront.IsNull ? float.MaxValue : Highway.instance.DistanceTo(Highway.instance.GetEquivalentDistance(car.distanceFront, car.lane, car.lane - 1), car.lane - 1, rightCarInFront.distanceBack, rightCarInFront.lane);
					// condition for merging to left lane
					if (distToRightCarInFront < car.leftMergeDistance// close enough to car in front
					    && car.overtakeEagerness > rightCarInFront.velocityPosition / car.defaultSpeed // car in front is slow enough
					){
						tryMergeRight = false;
					}

				}


				if (!car.isMerging && // not currently merging
					tryMergeRight // overtook target car (or overtake car doesn't exist)
					) { 

					if (Highway.instance.CanMergeToLane(car, car.lane - 1)) { // if space is available
                                                                               // start merge to right
                        car.overtakeCarEntity = Entity.Null;
                        car.state = MergeState.MERGE_RIGHT;
                        car.targetLane = Mathf.Round(car.lane - 1);
					}

				}


			}


			// increase to speed
			if (targetSpeed > car.velocityPosition) {
                car.velocityPosition = Mathf.Min(targetSpeed, car.velocityPosition + Game.instance.acceleration * dt);
			} else {
                car.velocityPosition = Mathf.Max(targetSpeed, car.velocityPosition - Game.instance.brakeDeceleration * dt);
			}

			// crash prevention failsafe
			if (carInFront.IsNotNull && dt > 0) {
				float maxDistanceDiff = Mathf.Max(0, distToCarInFront - Highway.MIN_DIST_BETWEEN_CARS);
                car.velocityPosition = Mathf.Min(car.velocityPosition, maxDistanceDiff / dt);
			}

            // update position
            car.distance += car.velocityPosition * dt;
			ChangeLane(ref car, car.lane + car.velocityLane * dt);

        }

		private void UpdateColor(ref CarStateStruct car) {

			if (car.velocityPosition > car.defaultSpeed) {
				color = Color.Lerp (GlobalShared.defaultColor, GlobalShared.maxSpeedColor, (car.velocityPosition - car.defaultSpeed) / (car.maxSpeed - car.defaultSpeed));
			} else if (car.velocityPosition < car.defaultSpeed) {
				color = Color.Lerp (GlobalShared.minSpeedColor, GlobalShared.defaultColor, car.velocityPosition / car.defaultSpeed);
			} else {
				color = GlobalShared.defaultColor;
			}

		}

    }

}