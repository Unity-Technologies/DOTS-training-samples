using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighwayRacers
{

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

        private static CarShared GlobalShared = new CarShared();
        public CarShared Shared { get { return GlobalShared; } }




            #endregion


            [Header("Children")]
        public MeshRenderer topRenderer;
        public MeshRenderer baseRenderer;
        public Transform cameraPos;

        public struct CarStateStruct
        {
            public float defaultSpeed;
            public float overtakePercent;
            public float leftMergeDistance;
            public float mergeSpace;

            public float overtakeEagerness;

            public float velocityPosition;

            public float velocityLane;

            public State state;


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
            public Car overtakeCar;
            public float timeOvertakeCarSet;


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
                get { return state == State.MERGE_LEFT || state == State.MERGE_RIGHT; }
            }


            public bool IsInLane(float lane)
            {
                return Highway.LanesOverlap(lane, this.lane);
            }
        }

        public CarStateStruct CarData;

        public Color color
        {
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

		public enum State {
			NORMAL,
			MERGE_RIGHT,
			MERGE_LEFT,
			OVERTAKING,
		}


        /// <summary>
        /// Changes lanes, also setting distance to the appropriate value.
        /// </summary>
        /// <param name="newLane"></param>
        public static void ChangeLane(ref CarStateStruct data, float newLane)
        {
			float roundLane = Mathf.Round(newLane);
			if (Mathf.Abs (roundLane - newLane) < .0001f) {
				newLane = roundLane;
			}

            data.distance = Highway.instance.GetEquivalentDistance(data.distance, data.lane, newLane);
            data.lane = newLane;
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

        private void Update()
        {
            this.UpdateCarData(ref this.CarData);
        }

        private void UpdateCarData(ref CarStateStruct car)
        {
			float dt = Time.deltaTime;
			if (dt == 0) // possible when the game is paused
				return;

			float targetSpeed = car.defaultSpeed;
			Car carInFront = Highway.instance.GetCarInFront(this);
			float distToCarInFront = carInFront == null ? float.MaxValue : Highway.instance.DistanceTo(car.distanceFront, car.lane, carInFront.CarData.distanceBack, carInFront.CarData.lane);

			switch (car.state) {
			case State.NORMAL:

				targetSpeed = car.defaultSpeed;
                    car.velocityLane = 0;

				// if won't merge, match car in front's speed
				if (distToCarInFront < car.leftMergeDistance) {
					targetSpeed = Mathf.Min(targetSpeed, carInFront.CarData.velocityPosition);
				}

				break;

			case State.MERGE_LEFT:

				targetSpeed = car.defaultSpeed;

                    car.velocityLane = Game.instance.switchLanesSpeed;
				// detect ending merge
				if (car.lane + car.velocityLane * dt >= car.targetLane) {
                        // set veloicty to not overshoot lane
                        car.velocityLane = (car.targetLane - car.lane) / dt;
					if (car.lane >= car.targetLane) { // end when frame started in the target lane
                            car.state = State.OVERTAKING;
					}
				}

				break;

			case State.OVERTAKING:

				targetSpeed = car.maxSpeed;
                    car.velocityLane = 0;

				break;

			case State.MERGE_RIGHT:

				targetSpeed = car.defaultSpeed;

                    car.velocityLane = -Game.instance.switchLanesSpeed;
				// detect ending merge
				if (car.lane + car.velocityLane * dt <= car.targetLane) {
                        // set veloicty to not overshoot lane
                        car.velocityLane = (car.targetLane - car.lane) / dt;
					if (car.lane <= car.targetLane) { // end when frame started in the target lane
                            car.state = State.NORMAL;
					}
				}

				break;
			}

			// detect overtaking car taking too long
			if (car.overtakeCar != null && Time.time - car.timeOvertakeCarSet >= Game.instance.overtakeMaxDuration) {
                car.overtakeCar = null;
			}

			// detect merging
			if (!car.isMerging) {

				// detect merging to left lane
				if (car.lane + 1 < Highway.NUM_LANES // left lane exists
					&& distToCarInFront < car.leftMergeDistance // close enough to car in front
					&& car.overtakeEagerness > carInFront.CarData.velocityPosition / car.defaultSpeed // car in front is slow enough		
				) {

					if (Highway.instance.CanMergeToLane(this, car.lane + 1)) { // if space is available
                                                                               // start merge to left
                        car.state = State.MERGE_LEFT;
                        car.targetLane = Mathf.Round(car.lane + 1);
                        car.overtakeCar = carInFront;
                        car.timeOvertakeCarSet = Time.time;
					}
				}

				// detect merging to right lane
				bool tryMergeRight = false;
				if (car.overtakeCar == null) {
                    // if overtake car got destroyed
                    car.overtakeCar = null;
					tryMergeRight = true;
				} else {
					// if passed overtake car
					if (Highway.instance.DistanceTo(car.distance, car.lane, car.overtakeCar.CarData.distance, car.overtakeCar.CarData.lane) > Highway.instance.length(car.lane) / 2) {
						tryMergeRight = true;
					}
				}

				if (car.lane - 1 < 0) { // right lane must exist
					tryMergeRight = false;
				}

				if (tryMergeRight) {
					// don't merge if just going to merge back
					Car rightCarInFront = Highway.instance.GetCarInFront(Highway.instance.GetEquivalentDistance(car.distanceFront, car.lane, car.lane - 1), car.lane - 1);
					float distToRightCarInFront = rightCarInFront == null ? float.MaxValue : Highway.instance.DistanceTo(Highway.instance.GetEquivalentDistance(car.distanceFront, car.lane, car.lane - 1), car.lane - 1, rightCarInFront.CarData.distanceBack, rightCarInFront.CarData.lane);
					// condition for merging to left lane
					if (distToRightCarInFront < car.leftMergeDistance// close enough to car in front
					    && car.overtakeEagerness > rightCarInFront.CarData.velocityPosition / car.defaultSpeed // car in front is slow enough
					){
						tryMergeRight = false;
					}

				}


				if (!car.isMerging && // not currently merging
					tryMergeRight // overtook target car (or overtake car doesn't exist)
					) { 

					if (Highway.instance.CanMergeToLane(this, car.lane - 1)) { // if space is available
                                                                               // start merge to right
                        car.overtakeCar = null;
                        car.state = State.MERGE_RIGHT;
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
			if (carInFront != null && dt > 0) {
				float maxDistanceDiff = Mathf.Max(0, distToCarInFront - Highway.MIN_DIST_BETWEEN_CARS);
                car.velocityPosition = Mathf.Min(car.velocityPosition, maxDistanceDiff / dt);
			}

            // update position
            car.distance += car.velocityPosition * dt;
			ChangeLane(ref car, car.lane + car.velocityLane * dt);
            UpdatePosition(ref car);

			UpdateColor(ref car);
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