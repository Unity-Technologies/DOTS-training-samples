using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighwayRacers
{

    public class Car : MonoBehaviour
    {
		#region Constant for all cars:

        [Tooltip("Distance from center of car to the front.")]
        public float distanceToFront = 1;
        [Tooltip("Distance from center of car to the back.")]
        public float distanceToBack = 1;

		public Color defaultColor = Color.gray;
		public Color maxSpeedColor = Color.green;
		public Color minSpeedColor = Color.red;




		#endregion

		public float defaultSpeed { get; set; }
		public float overtakePercent { get; set; }
		public float leftMergeDistance { get; set; }
		public float mergeSpace { get; set; }

		public float overtakeEagerness { get; set; }

		public float maxSpeed {
			get {
				return defaultSpeed * overtakePercent;
			}
		}



        [Header("Children")]
		public MeshRenderer topRenderer;
        public MeshRenderer baseRenderer;
		public Transform cameraPos;

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
			if (!hidden)
				return;
			topRenderer.enabled = true;
			baseRenderer.enabled = true;
			hidden = false;
		}

		public void Hide() {
			if (hidden)
				return;
			topRenderer.enabled = false;
			baseRenderer.enabled = false;
			hidden = true;

		}

		public void SetRandomPropeties()
		{
			defaultSpeed = Random.Range(Game.instance.defaultSpeedMin, Game.instance.defaultSpeedMax);	
			overtakePercent = Random.Range(Game.instance.overtakePercentMin, Game.instance.overtakePercentMax);
			leftMergeDistance = Random.Range(Game.instance.leftMergeDistanceMin, Game.instance.leftMergeDistanceMax);
			mergeSpace = Random.Range(Game.instance.mergeSpaceMin, Game.instance.mergeSpaceMax);
			overtakeEagerness = Random.Range(Game.instance.overtakeEagernessMin, Game.instance.overtakeEagernessMax);

		}

        public float velocityPosition { get; set; }

        public float velocityLane { get; set; }

		public enum State {
			NORMAL,
			MERGE_RIGHT,
			MERGE_LEFT,
			OVERTAKING,
		}
		public State state { get; private set; }
		public bool isMerging {
			get { return state == State.MERGE_LEFT || state == State.MERGE_RIGHT; }
		}

        /// <summary>
        /// Distance in current lane.  Can change when switching lanes.
        /// </summary>
        public float distance { get; set; }

        /// <summary>
        /// Distance of the back of the car in the current lane.
        /// </summary>
        public float distanceBack
        {
            get
            {
                return (distance - distanceToBack) + Mathf.Floor((distance - distanceToBack) / Highway.instance.length(lane)) * Highway.instance.length(lane);
            }
        }

        /// <summary>
        /// Distance of the front of the car in the current lane.
        /// </summary>
        public float distanceFront
        {
            get
            {
                return (distance + distanceToFront) + Mathf.Floor((distance + distanceToFront) / Highway.instance.length(lane)) * Highway.instance.length(lane);
            }
        }

        /// <summary>
        /// Ranges from [0, 4]
        /// </summary>
        public float lane { get; set; }

        /// <summary>
        /// Changes lanes, also setting distance to the appropriate value.
        /// </summary>
        /// <param name="newLane"></param>
        public void ChangeLane(float newLane)
        {
			float roundLane = Mathf.Round(newLane);
			if (Mathf.Abs (roundLane - newLane) < .0001f) {
				newLane = roundLane;
			}

            distance = Highway.instance.GetEquivalentDistance(distance, lane, newLane);
            lane = newLane;
        }
        
        public bool IsInLane(float lane)
        {
            return Highway.LanesOverlap(lane, this.lane);
        }

        public void UpdatePosition()
        {
            float x, z, rotation;
            Highway.instance.GetPosition(distance, lane, out x, out z, out rotation);

            transform.localPosition = new Vector3(x, transform.position.y, z);
            transform.localRotation = Quaternion.Euler(0, rotation * Mathf.Rad2Deg, 0);
        }

        private void Awake()
        {

        }


        private void Update()
        {
			float dt = Time.deltaTime;
			if (dt == 0) // possible when the game is paused
				return;

			float targetSpeed = defaultSpeed;
			Car carInFront = Highway.instance.GetCarInFront(this);
			float distToCarInFront = carInFront == null ? float.MaxValue : Highway.instance.DistanceTo(distanceFront, lane, carInFront.distanceBack, carInFront.lane);

			switch (state) {
			case State.NORMAL:

				targetSpeed = defaultSpeed;
				velocityLane = 0;

				// if won't merge, match car in front's speed
				if (distToCarInFront < leftMergeDistance) {
					targetSpeed = Mathf.Min(targetSpeed, carInFront.velocityPosition);
				}

				break;

			case State.MERGE_LEFT:

				targetSpeed = defaultSpeed;

				velocityLane = Game.instance.switchLanesSpeed;
				// detect ending merge
				if (lane + velocityLane * dt >= targetLane) {
					// set veloicty to not overshoot lane
					velocityLane = (targetLane - lane) / dt;
					if (lane >= targetLane) { // end when frame started in the target lane
						state = State.OVERTAKING;
					}
				}

				break;

			case State.OVERTAKING:

				targetSpeed = maxSpeed;
				velocityLane = 0;

				break;

			case State.MERGE_RIGHT:

				targetSpeed = defaultSpeed;

				velocityLane = -Game.instance.switchLanesSpeed;
				// detect ending merge
				if (lane + velocityLane * dt <= targetLane) {
					// set veloicty to not overshoot lane
					velocityLane = (targetLane - lane) / dt;
					if (lane <= targetLane) { // end when frame started in the target lane
						state = State.NORMAL;
					}
				}

				break;
			}

			// detect overtaking car taking too long
			if (overtakeCar != null && Time.time - timeOvertakeCarSet >= Game.instance.overtakeMaxDuration) {
				overtakeCar = null;
			}

			// detect merging
			if (!isMerging) {

				// detect merging to left lane
				if (lane + 1 < Highway.NUM_LANES // left lane exists
					&& distToCarInFront < leftMergeDistance // close enough to car in front
					&& overtakeEagerness > carInFront.velocityPosition / defaultSpeed // car in front is slow enough		
				) {

					if (Highway.instance.CanMergeToLane(this, lane + 1)) { // if space is available
						// start merge to left
						state = State.MERGE_LEFT;
						targetLane = Mathf.Round(lane + 1);
						overtakeCar = carInFront;
						timeOvertakeCarSet = Time.time;
					}
				}

				// detect merging to right lane
				bool tryMergeRight = false;
				if (overtakeCar == null) {
					// if overtake car got destroyed
					overtakeCar = null;
					tryMergeRight = true;
				} else {
					// if passed overtake car
					if (Highway.instance.DistanceTo(distance, lane, overtakeCar.distance, overtakeCar.lane) > Highway.instance.length(lane) / 2) {
						tryMergeRight = true;
					}
				}

				if (lane - 1 < 0) { // right lane must exist
					tryMergeRight = false;
				}

				if (tryMergeRight) {
					// don't merge if just going to merge back
					Car rightCarInFront = Highway.instance.GetCarInFront(Highway.instance.GetEquivalentDistance(distanceFront, lane, lane - 1), lane - 1);
					float distToRightCarInFront = rightCarInFront == null ? float.MaxValue : Highway.instance.DistanceTo(Highway.instance.GetEquivalentDistance(distanceFront, lane, lane - 1), lane - 1, rightCarInFront.distanceBack, rightCarInFront.lane);
					// condition for merging to left lane
					if (distToRightCarInFront < leftMergeDistance// close enough to car in front
					    && overtakeEagerness > rightCarInFront.velocityPosition / defaultSpeed // car in front is slow enough
					){
						tryMergeRight = false;
					}

				}


				if (!isMerging && // not currently merging
					tryMergeRight // overtook target car (or overtake car doesn't exist)
					) { 

					if (Highway.instance.CanMergeToLane(this, lane - 1)) { // if space is available
						// start merge to right
						overtakeCar = null;
						state = State.MERGE_RIGHT;
						targetLane = Mathf.Round(lane - 1);
					}

				}


			}


			// increase to speed
			if (targetSpeed > velocityPosition) {
				velocityPosition = Mathf.Min(targetSpeed, velocityPosition + Game.instance.acceleration * dt);
			} else {
				velocityPosition = Mathf.Max(targetSpeed, velocityPosition - Game.instance.brakeDeceleration * dt);
			}

			// crash prevention failsafe
			if (carInFront != null && dt > 0) {
				float maxDistanceDiff = Mathf.Max(0, distToCarInFront - Highway.MIN_DIST_BETWEEN_CARS);
				velocityPosition = Mathf.Min(velocityPosition, maxDistanceDiff / dt);
			}

			// update position
			distance += velocityPosition * dt;
			ChangeLane(lane + velocityLane * dt);
            UpdatePosition();

			UpdateColor();
        }

		private void UpdateColor() {

			if (velocityPosition > defaultSpeed) {
				color = Color.Lerp (defaultColor, maxSpeedColor, (velocityPosition - defaultSpeed) / (maxSpeed - defaultSpeed));
			} else if (velocityPosition < defaultSpeed) {
				color = Color.Lerp (minSpeedColor, defaultColor, velocityPosition / defaultSpeed);
			} else {
				color = defaultColor;
			}

		}

		private float targetLane = 0;

		private bool hidden = false;

		/// <summary>
		/// Car to pass before considering merging right
		/// </summary>
		private Car overtakeCar = null;
		private float timeOvertakeCarSet = 0;


    }

}