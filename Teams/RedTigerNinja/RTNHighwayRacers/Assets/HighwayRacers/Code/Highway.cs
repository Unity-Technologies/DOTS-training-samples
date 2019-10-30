using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace HighwayRacers
{

	/// <summary>
	/// Singleton class containing math functions.
	/// </summary>
    public class Highway : MonoBehaviour
    {

        public const int NUM_LANES = 4;
        public const float LANE_SPACING = 1.9f;
        public const float MID_RADIUS = 31.46f;
        public const float CURVE_LANE0_RADIUS = MID_RADIUS - LANE_SPACING * (NUM_LANES - 1) / 2f;
        public const float MIN_HIGHWAY_LANE0_LENGTH = CURVE_LANE0_RADIUS * 4;
        public const float MIN_DIST_BETWEEN_CARS = .7f;

        [Header("Prefabs")]
        public GameObject carPrefab;
        public GameObject straightPiecePrefab;
        public GameObject curvePiecePrefab;

        public static Highway instance { get; private set; }


        private HighwayPiece[] pieces = new HighwayPiece[8];

		/// <summary>
		/// Length of the innermost lane.
		/// </summary>
        public float lane0Length { get; private set; }
        public float length(float lane)
        {
            return straightPieceLength * 4 + curvePieceLength(lane) * 4;
        }
        public float straightPieceLength
        {
            get
            {
                return (lane0Length - CURVE_LANE0_RADIUS * 4) / 4;
            }
        }
        public static float curvePieceRadius(float lane)
        {
            return CURVE_LANE0_RADIUS + lane * LANE_SPACING;
        }
        public static float curvePieceLength(float lane)
        {
            return curvePieceRadius(lane) * Mathf.PI / 2;
        }

        /// <summary>
        /// Gets position of a car based on its lane and distance from the start in that lane.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="lane"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="rotation">y rotation of the car, in radians.</param>
        public static void GetPosition(ref Highway.HighwayStateStruct highway, float distance, float lane, out float x, out float z, out float rotation)
        {
            // keep distance in [0, length)
            distance -= Mathf.Floor(distance / highway.length(lane)) * highway.length(lane);

            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;

            float pieceStartDistance = 0;
            float pieceEndDistance = 0;
            x = 0;
            z = 0;
            rotation = 0;

            for (int i = 0; i < highway.AllPieces.Length; i++)
            {
                HighwayPiece.HighwayPieceState piece = highway.AllPieces[i]; // TODO: TOTAL HACK wth the %4
                pieceStartDistance = pieceEndDistance;
                pieceEndDistance += piece.length(lane);
                if (distance >= pieceEndDistance)
                    continue;

                // inside piece i

                // position and rotation local to the piece
                float localX, localZ;
                if (i % 2 == 0)
                {
                    // straight piece
                    GetStraightPiecePosition(distance - pieceStartDistance, lane, out localX, out localZ, out rotation);
                } else
                {
                    // curved piece
                    GetCurvePiecePosition(distance - pieceStartDistance, lane, out localX, out localZ, out rotation);
                }
                // transform
                RotateAroundOrigin(localX, localZ, piece.startRotation, out x, out z);
                x += piece.startX;
                z += piece.startZ;
                rotation += piece.startRotation;
                break;                
            }
            
        }

        /// <summary>
        /// Gets distance in another lane that appears to be the same distance in the given lane.
        /// </summary>
        public float GetEquivalentDistance(float distance, float lane, float otherLane)
        {
            // keep distance in [0, length)
			distance = WrapDistance(distance, lane);

            float pieceStartDistance = 0;
            float pieceEndDistance = 0;
            float pieceStartDistanceOtherLane = 0;
            float pieceEndDistanceOtherLane = 0;

            for (int i = 0; i < 8; i++)
            {
                HighwayPiece piece = pieces[i];
                pieceStartDistance = pieceEndDistance;
                pieceStartDistanceOtherLane = pieceEndDistanceOtherLane;
                pieceEndDistance += piece.length(lane);
                pieceEndDistanceOtherLane += piece.length(otherLane);
                if (distance >= pieceEndDistance)
                    continue;

                // inside piece i

                if (i % 2 == 0)
                {
                    // straight piece
                    return pieceStartDistanceOtherLane + distance - pieceStartDistance;
                } else
                {
                    // curved piece
                    float radius = curvePieceRadius(lane);
                    float radiusOtherLane = curvePieceRadius(otherLane);
                    return pieceStartDistanceOtherLane + (distance - pieceStartDistance) * radiusOtherLane / radius;
                }
            }

            return 0;
        }



        private static void GetStraightPiecePosition(float localDistance, float lane, out float x, out float z, out float rotation)
        {
            x = LANE_SPACING * ((NUM_LANES - 1) / 2f - lane);
            z = localDistance;
            rotation = 0;
        }
        private static void GetCurvePiecePosition(float localDistance, float lane, out float x, out float z, out float rotation)
        {
            float radius = curvePieceRadius(lane);
            float angle = localDistance / radius;
            x = MID_RADIUS - Mathf.Cos(angle) * radius;
            z = Mathf.Sin(angle) * radius;
            rotation = angle;
        }

        private static void RotateAroundOrigin(float x, float z, float rotation, out float xOut, out float zOut)
        {
            float sin = Mathf.Sin(-rotation);
            float cos = Mathf.Cos(-rotation);

            xOut = x * cos - z * sin;
            zOut = x * sin + z * cos;
        }


        public void CreateHighway(float lane0Length)
        {
			
            if (lane0Length < MIN_HIGHWAY_LANE0_LENGTH)
            {
                Debug.LogError("Highway length must be longer than " + MIN_HIGHWAY_LANE0_LENGTH);
                return;
            }

			int tempNumCars = numCars;
			if (lane0Length < this.lane0Length) {
				ClearCars();
			}

            float straightPieceLength = (lane0Length - CURVE_LANE0_RADIUS * 4) / 4;

            Vector3 pos = Vector3.zero;
            float rot = 0;

            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                {
                    // straight piece
                    if (pieces[i] == null)
                    {
                        pieces[i] = Instantiate(straightPiecePrefab, transform).GetComponent<StraightPiece>();
                    }
                    StraightPiece straightPiece = pieces[i] as StraightPiece;
                    straightPiece.SetStartPosition(pos);
                    straightPiece.startRotation = rot;
                    straightPiece.SetLength(straightPieceLength);

                    pos += straightPiece.startRotationQ * new Vector3(0, 0, straightPieceLength);
                }
                else
                {
                    // curve piece
                    if (pieces[i] == null)
                    {
                        pieces[i] = Instantiate(curvePiecePrefab, transform).GetComponent<CurvePiece>();
                    }
                    CurvePiece curvePiece = pieces[i] as CurvePiece;
                    curvePiece.SetStartPosition(pos);
                    curvePiece.startRotation = rot;

                    pos += curvePiece.startRotationQ * new Vector3(MID_RADIUS, 0, MID_RADIUS);
                    rot = Mathf.PI / 2 * (i / 2 + 1);
                }
            }

            this.lane0Length = lane0Length;

			SetNumCars(tempNumCars);

        }

        /// <summary>
        /// Wraps distance to be in [0, l), where l is the length of the given lane.
        /// </summary>
        public float WrapDistance(float distance, float lane)
        {
            float l = length(lane);
            return distance - Mathf.Floor(distance / l) * l;
        }

        /// <summary>
        /// Gets the positive distance, wrapped in [0, length), from distance1 to distance2 in the given lane.
        /// </summary>
        public float DistanceTo(float distance1, float lane, float distance2)
        {
            return WrapDistance(distance2 - distance1, lane);
        }
        /// <summary>
        /// Gets the positive distance, wrapped in [0, length), from distance1 (in lane1) to distance2 (in lane2) in lane 1.
        /// </summary>
        public float DistanceTo(float distance1, float lane1, float distance2, float lane2)
        {
            return WrapDistance(GetEquivalentDistance(distance2, lane2, lane1) - distance1, lane1);
        }

        /// <summary>
        /// Returns if the given lanes overlap.
        /// Note that a car is considered to be in both floor(car.lane) and ceil(car.lane).
        /// </summary>
        public static bool LanesOverlap(float lane1, float lane2)
        {
            return Mathf.FloorToInt(lane1) == Mathf.FloorToInt(lane2) ||
                Mathf.CeilToInt(lane1) == Mathf.FloorToInt(lane2) ||
                Mathf.FloorToInt(lane1) == Mathf.CeilToInt(lane2) ||
                Mathf.CeilToInt(lane1) == Mathf.CeilToInt(lane2);
        }

        /// <summary>
        /// Returns if the given distances overlap, regardless of lane.
        /// If front distance is less than back distance, it's assumed that back is just behind the end of the track and front is just ahead of the beginning.
        /// </summary>
        public bool DistancesOverlap(float d1Lane, float d1Back, float d1Front, float d2Lane, float d2Back, float d2Front)
        {
            d1Back %= length(d1Lane);
            d1Front %= length(d1Lane);
            d2Back = GetEquivalentDistance(d2Back, d2Lane, d1Lane);
            d2Front = GetEquivalentDistance(d2Front, d2Lane, d1Lane);

            bool backContained = false;
            if (d1Back < d1Front)
            {
                backContained = d1Back <= d2Back && d2Back <= d1Front;
            } else
            {
                backContained = d1Back <= d2Back || d2Back <= d1Front;
            }
            if (backContained) return true;

            bool frontContained = false;
            if (d1Back < d1Front)
            {
                frontContained = d1Back <= d2Front && d2Front <= d1Front;
            }
            else
            {
                frontContained = d1Back <= d2Front || d2Front <= d1Front;
            }
            if (frontContained) return true;

            return false;
        }

        /// <summary>
        /// Gets if the two given areas overlap.  Areas are defined by a lane, a back distance, and a front distance.
        /// A float lane is considered to contain both floor(lane) and ceil(lane).
        /// </summary>
        public bool AreasOverlap(float lane1, float distance1Back, float distance1Front, float lane2, float distance2Back, float distance2Front)
        {
            if (!LanesOverlap(lane1, lane2)) return false;

            return DistancesOverlap(lane1, distance1Back, distance1Front, lane2, distance2Back, distance2Front);
        }

        /// <summary>
        /// Gets a list of all the cars in the given lane.
        /// </summary>
        public LinkedList<Car> GetCarsInLane(float lane)
        {
            LinkedList<Car> ret = new LinkedList<Car>();
            foreach (Car car in allCarsList)
            {
                if (car.CarData.IsInLane(lane))
                {
                    ret.AddLast(car);
                }
            }
            return ret;
        }

        /// <summary>
        /// Searches cars in the given lane to find the closest car in front of the given distance.  Returns null if there are no other cars in the lane.
        /// </summary>
        public CarStateStruct GetCarInFront(float distance, float lane)
        {
            Car ret = null;
            float diff = 0;
            LinkedList<Car> cars = GetCarsInLane(lane);
            foreach (Car car in cars)
            {
                float d = DistanceTo(distance, lane, car.CarData.distanceBack, car.CarData.lane);
                if (ret == null || d < diff)
                {
                    ret = car;
                    diff = d;
                }
            }
            if (ret == null)
            {
                return CarStateStruct.NullCar;
            }
            return ret.CarData;
        }

		/// <summary>
		/// Returns if the car can merge into the given lane.  Takes car's mergeSpace into consideration.
		/// </summary>
		/// <param name="car">Car merging.</param>
		/// <param name="mergeLane">The lane the car will merge to.</param>
		public bool CanMergeToLane(CarStateStruct car, float mergeLane)
		{
			float distanceBack = GetEquivalentDistance(car.distanceBack - car.mergeSpace, car.lane, mergeLane);
			float distanceFront = GetEquivalentDistance(car.distanceFront + car.mergeSpace, car.lane, mergeLane);

			foreach (Car otherCar in allCarsList)
			{
				if (car.ThisCarEntity == otherCar.CarData.ThisCarEntity)
					continue;
				if (AreasOverlap (mergeLane, distanceBack, distanceFront, otherCar.CarData.lane, otherCar.CarData.distanceBack, otherCar.CarData.distanceFront))
					return false;
			}
			return true;

		}

		/// <summary>
		/// Adds a car to the highway in a random lane.  Won't add car if all lanes are full (returns null).
		/// </summary>
		public Car AddCar(){

			Car car = null;

            var tryCount = 0;
            while (car == null)
            {
                car = this.AddCar(Random.Range(0, 3));

                tryCount++;
                if (tryCount > 100) break;
            }

            /*
			List<int> lanes = new List<int> (new int[]{ 0, 1, 2, 3 });
			while (lanes.Count > 0) {
				int index = Random.Range(0, lanes.Count);
				int lane = lanes [index];
				lanes.RemoveAt (index);
				car = AddCar(lane);
				if (car != null)
					return car;
			}
            */

			return car;

		}

        /// <summary>
        /// Adds a car to the highway in the given lane.  Checks to make sure it's not on top of other cars.  Returns null if a car couldn't be placed.
        /// </summary>
        public Car AddCar(float lane)
        {
            Highway.instance.EnsureUpdated();

            LinkedList<Car> cars = GetCarsInLane(lane);
            if (cars.Count == 0)
            {
                // add car wherever
                return AddCarUnsafe(Random.Range(0, length(lane)), lane);
            }
            
            foreach (Car car in cars)
            {
                var frontCar = GetCarInFront(car.CarData.distance, car.CarData.lane);
                float backDistance = GetEquivalentDistance(car.CarData.distanceFront, car.CarData.lane, lane);
                float frontDistance = GetEquivalentDistance(frontCar.distanceBack, frontCar.lane, lane);
                float space = DistanceTo(backDistance, lane, frontDistance);
                if (space > CarShared.distanceToFront + CarShared.distanceToBack + MIN_DIST_BETWEEN_CARS * 2)
                {
                    // enough space to add car
                    float distance = backDistance + (space - (CarShared.distanceToFront + CarShared.distanceToBack)) / 2 + CarShared.distanceToBack;
                    return AddCarUnsafe(distance, lane);
                }
            }

            // leweyg hackery:
            //Debug.Log("Adding random car...");
            return AddCarUnsafe(Random.Range(0.0f, Highway.MIN_HIGHWAY_LANE0_LENGTH), lane);

            return null;
        }

        /// <summary>
        /// Adds a car to the highway, without checking if it would fit first.
        /// </summary>
        public Car AddCarUnsafe(float distance, float lane)
        {
            var em = Unity.Entities.World.Active.EntityManager;

            Car car = Instantiate(carPrefab, transform).GetComponent<Car>();
			car.SetRandomPropeties();
            car.CarData.distance = distance;
            car.CarData.lane = lane;
            car.CarData.ThisCarEntity = em.Instantiate(CaptureNewEntity.CreatedEntity);// em.CreateEntity();
			car.CarData.velocityPosition = car.CarData.defaultSpeed;
            em.AddComponentData(car.CarData.ThisCarEntity, car.CarData);
            em.AddComponentData(car.CarData.ThisCarEntity, new CarSystem.CarNextState(car.CarData));
            allCarsList.Add(car);
            //this.UpdateCarList();
            car.UpdatePosition(ref car.CarData, ref Highway.instance.HighwayState);
            return car;
        }

		public void RemoveCar(Car car) {
			allCarsList.Remove(car);
            //this.UpdateCarList();
            if (car.CarData.ThisCarEntity != Entity.Null)
            {
                var ent = car.CarData.ThisCarEntity;
                car.CarData.ThisCarEntity = Entity.Null;
                Unity.Entities.World.Active.EntityManager.DestroyEntity(ent);
            }
            Destroy (car.gameObject);
		}

        public HighwayStateStruct HighwayState = new HighwayStateStruct();

        public struct HighwayStateStruct
        {
            [ReadOnly] public Unity.Collections.NativeArray<CarStateStruct> AllCars;
            [ReadOnly] public Unity.Collections.NativeArray<HighwayPiece.HighwayPieceState> AllPieces;
            public float lane0Length;

            //public HighwayStateStruct() { }

            static Unity.Profiling.ProfilerMarker CarMarker = new Unity.Profiling.ProfilerMarker("GetCarInFront");

            public CarStateStruct GetCarInFront(ref CarStateStruct startCar, float distance, float lane)
            {
                CarMarker.Auto();

                CarStateStruct ret = CarStateStruct.NullCar;
                float diff = 0;
                var nextIndex = startCar.SortedIndexNext;
                for (var ri =0; ri< AllCars.Length; ri++)
                {
                    var car = AllCars[ri];
                    /*
                    var curIndex = nextIndex;
                    var car = AllCars[curIndex];
                    nextIndex = car.SortedIndexNext;
                    */

                    if (!LanesOverlap(lane, car.lane))
                    {
                        continue;
                    }
                    /*
                    if ((car.distance - distance) > (Highway.MIN_HIGHWAY_LANE0_LENGTH * 0.5f))
                    {
                        //break;
                    }
                    */

                    float d = DistanceTo(distance, lane, car.distanceBack, car.lane);
                    if (ret.IsNull || d < diff)
                    {
                        ret = car;
                        diff = d;
                    }
                }

                if (ret.ThisCarEntity == CarStateStruct.NullCar.ThisCarEntity)
                {
                    return CarStateStruct.NullCar;
                }
                return ret;
            }


            /// <summary>
            /// Returns if the car can merge into the given lane.  Takes car's mergeSpace into consideration.
            /// </summary>
            /// <param name="car">Car merging.</param>
            /// <param name="mergeLane">The lane the car will merge to.</param>
            public bool CanMergeToLane(CarStateStruct car, float mergeLane)
            {
                float distanceBack = GetEquivalentDistance(car.distanceBack - car.mergeSpace, car.lane, mergeLane);
                float distanceFront = GetEquivalentDistance(car.distanceFront + car.mergeSpace, car.lane, mergeLane);

                //foreach (Car otherCar in allCarsList)
                for (var ci=0; ci<AllCars.Length; ci++)
                {
                    var otherCar = this.AllCars[ci];
                    if (car.ThisCarEntity == otherCar.ThisCarEntity)
                        continue;
                    if (AreasOverlap(mergeLane, distanceBack, distanceFront, otherCar.lane, otherCar.distanceBack, otherCar.distanceFront))
                        return false;
                }
                return true;

            }


            /// <summary>
            /// Returns if the given distances overlap, regardless of lane.
            /// If front distance is less than back distance, it's assumed that back is just behind the end of the track and front is just ahead of the beginning.
            /// </summary>
            public bool DistancesOverlap(float d1Lane, float d1Back, float d1Front, float d2Lane, float d2Back, float d2Front)
            {
                d1Back %= length(d1Lane);
                d1Front %= length(d1Lane);
                d2Back = GetEquivalentDistance(d2Back, d2Lane, d1Lane);
                d2Front = GetEquivalentDistance(d2Front, d2Lane, d1Lane);

                bool backContained = false;
                if (d1Back < d1Front)
                {
                    backContained = d1Back <= d2Back && d2Back <= d1Front;
                }
                else
                {
                    backContained = d1Back <= d2Back || d2Back <= d1Front;
                }
                if (backContained) return true;

                bool frontContained = false;
                if (d1Back < d1Front)
                {
                    frontContained = d1Back <= d2Front && d2Front <= d1Front;
                }
                else
                {
                    frontContained = d1Back <= d2Front || d2Front <= d1Front;
                }
                if (frontContained) return true;

                return false;
            }

            /// <summary>
            /// Gets if the two given areas overlap.  Areas are defined by a lane, a back distance, and a front distance.
            /// A float lane is considered to contain both floor(lane) and ceil(lane).
            /// </summary>
            public bool AreasOverlap(float lane1, float distance1Back, float distance1Front, float lane2, float distance2Back, float distance2Front)
            {
                if (!LanesOverlap(lane1, lane2)) return false;

                return DistancesOverlap(lane1, distance1Back, distance1Front, lane2, distance2Back, distance2Front);
            }


            public float length(float lane)
            {
                return straightPieceLength * 4 + curvePieceLength(lane) * 4;
            }
            public float straightPieceLength
            {
                get
                {
                    return (lane0Length - CURVE_LANE0_RADIUS * 4) / 4;
                }
            }


            /// <summary>
            /// Wraps distance to be in [0, l), where l is the length of the given lane.
            /// </summary>
            public float WrapDistance(float distance, float lane)
            {
                float l = length(lane);
                return distance - Mathf.Floor(distance / l) * l;
            }

            /// <summary>
            /// Gets the positive distance, wrapped in [0, length), from distance1 to distance2 in the given lane.
            /// </summary>
            public float DistanceTo(float distance1, float lane, float distance2)
            {
                return WrapDistance(distance2 - distance1, lane);
            }
            /// <summary>
            /// Gets the positive distance, wrapped in [0, length), from distance1 (in lane1) to distance2 (in lane2) in lane 1.
            /// </summary>
            public float DistanceTo(float distance1, float lane1, float distance2, float lane2)
            {
                return WrapDistance(GetEquivalentDistance(distance2, lane2, lane1) - distance1, lane1);
            }


            /// <summary>
            /// Gets distance in another lane that appears to be the same distance in the given lane.
            /// </summary>
            public float GetEquivalentDistance(float distance, float lane, float otherLane)
            {
                // keep distance in [0, length)
                distance = WrapDistance(distance, lane);

                float pieceStartDistance = 0;
                float pieceEndDistance = 0;
                float pieceStartDistanceOtherLane = 0;
                float pieceEndDistanceOtherLane = 0;

                for (int i = 0; i < this.AllPieces.Length; i++)
                {
                    var piece = this.AllPieces[i];
                    pieceStartDistance = pieceEndDistance;
                    pieceStartDistanceOtherLane = pieceEndDistanceOtherLane;
                    pieceEndDistance += piece.length(lane);
                    pieceEndDistanceOtherLane += piece.length(otherLane);
                    if (distance >= pieceEndDistance)
                        continue;

                    // inside piece i

                    if (i % 2 == 0)
                    {
                        // straight piece
                        return pieceStartDistanceOtherLane + distance - pieceStartDistance;
                    }
                    else
                    {
                        // curved piece
                        float radius = curvePieceRadius(lane);
                        float radiusOtherLane = curvePieceRadius(otherLane);
                        return pieceStartDistanceOtherLane + (distance - pieceStartDistance) * radiusOtherLane / radius;
                    }
                }

                return 0;
            }



            public static void UpdateFromHighway(ref HighwayStateStruct into, Highway hw)
            {
                into.AllCars = hw.allCarsNativeArray;
            }
        }

        public void UnitAnimate(float duration, System.Action<float> unitTime, System.Action onDone)
        {
            this.StartCoroutine(this.__innerUnitAnimate(duration, unitTime, onDone));
        }

        private IEnumerator __innerUnitAnimate(float duration, System.Action<float> onUnitTime, System.Action onDone)
        {
            var startTime = Time.time;
            onUnitTime(0.0f);
            yield return null;
            for (var curTime = Time.time; (curTime < (startTime + duration)); curTime = Time.time)
            {
                var unitTime = (curTime - startTime) / duration;
                onUnitTime(unitTime);
                yield return null;
            }
            onUnitTime(1.0f);
            if (onDone != null)
            {
                onDone();
            }

        }

        public void UpdateCarList()
        {
            /*
            if ((!this.allCarsNativeArray.IsCreated) || (this.allCarsNativeArray.Length != this.allCarsList.Count))
            {
                if (this.allCarsNativeArray.IsCreated)
                {
                    //Debug.Log("Cars realloc...");
                    var toDel = this.allCarsNativeArray;
                    this.UnitAnimate(0.5f, (dt) => { }, () => { toDel.Dispose(); });

                    //this.allCarsNativeArray.Dispose();
                }
                this.allCarsNativeArray = new Unity.Collections.NativeArray<CarStateStruct>(this.allCarsList.Count, Unity.Collections.Allocator.Persistent);
            }
            for (var i = 0; i < this.allCarsList.Count; i++)
            {
                this.allCarsNativeArray[i] = this.allCarsList[i].CarData;
            }
            this.HighwayState.AllCars = this.allCarsNativeArray;
            */

            this.UpdateHighwayPiecesState();

            this.HighwayState.lane0Length = this.lane0Length;

        }

        public void UpdateHighwayPiecesState()
        {
            /*
            if ((!this.allPiecesNativeArray.IsCreated) || (this.allCarsNativeArray.Length != this.pieces.Length))
            {
                if (this.allPiecesNativeArray.IsCreated)
                {
                    //Debug.Log("Pieces realloc...");
                    //this.allPiecesNativeArray.Dispose();
                    var toDel = this.allPiecesNativeArray;
                    this.UnitAnimate(0.5f, (dt) => { }, () => { toDel.Dispose(); });
                }
                var ar = this.pieces.Select(k => k.AsHighwayState).ToArray();
                this.allPiecesNativeArray = new Unity.Collections.NativeArray<HighwayPiece.HighwayPieceState>(ar, Unity.Collections.Allocator.Persistent);
            }
            for (var pi = 0; pi < this.pieces.Length; pi++)
            {
                this.allPiecesNativeArray[pi] = this.pieces[pi].AsHighwayState;
            }
            this.HighwayState.AllPieces = this.allPiecesNativeArray;
            */

        }

        public int numCars {
			get { return allCarsList.Count; }
		}

		public void SetNumCars(int numCars) {

			while (allCarsList.Count > numCars) {
				RemoveCar(allCarsList[0]);
			}
			while (allCarsList.Count < numCars) {
				Car car = AddCar();
				if (car == null)
					return;
			}

		}
        
        public void ClearCars()
        {
            foreach (Car car in allCarsList)
            {
                Destroy(car.gameObject);
            }
            allCarsList.Clear();

            if (this.allCarsNativeArray.IsCreated)
            {
                this.allCarsNativeArray.Dispose();
            }

        }

		public Car GetCarAtScreenPosition(Vector3 screenPosition, float radius){

			foreach (Car car in allCarsList) {
				Vector3 carScreenPos = Camera.main.WorldToScreenPoint(car.transform.position);
				carScreenPos.z = screenPosition.z;

				if (Vector3.Distance (screenPosition, carScreenPos) <= radius) {
					return car;
				}

			}

			return null;

		}

        
        private List<Car> allCarsList = new List<Car>();
        private Unity.Collections.NativeArray<CarStateStruct> allCarsNativeArray;
        private Unity.Collections.NativeArray<HighwayPiece.HighwayPieceState> allPiecesNativeArray;

        private void OnDisable()
        {
            this.ClearCars();
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

        private void Start()
        {
			CreateHighway(250);
        }


        private int mLastUpdatedFrame = -2;
        public void EnsureUpdated()
        {
            var curFrame = Time.frameCount;
            if ((this.mLastUpdatedFrame == curFrame))
            {
                return;
            }
            this.mLastUpdatedFrame = curFrame;

            this.UpdateCarList();
        }

        private void Update()
        {
            this.EnsureUpdated();

        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
            ClearCars();
        }

    }

}