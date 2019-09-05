using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

namespace HighwayRacers
{
    public unsafe struct HighwayPiece1
    {
        public float startX;
        public float startZ;
        public float startRotation;
        public fixed float length[4];
    }
    
    public struct Highway1
    {
        public const int NumLanes = 4;
        public const float LaneSpacing = 1.9f;
        public const float MidRadius = 31.46f;
        public const float CuverLane0Radius = MidRadius - LaneSpacing * (NumLanes - 1) / 2f;
        public const float MinHighwayLane0Length = CuverLane0Radius * 4;
        public const float MinDistBetweenCars = 0.7f;

        public NativeArray<HighwayPiece1> pieces;
        public float innerLaneLength;

        public float length(float lane)
        {
            return GetStraightPieceLength(innerLaneLength) * 4.0f + GetCurvePieceLength(lane) * 4.0f;
        }

        public static float GetStraightPieceLength(float laneLength)
        {
            return (laneLength - CuverLane0Radius * 4.0f) / 4.0f;
        }

        public static float GetCurvePieceRadius(float lane)
        {
            return CuverLane0Radius + lane * LaneSpacing;
        }

        public static float GetCurvePieceLength(float lane)
        {
            return GetCurvePieceRadius(lane) * Mathf.PI / 2.0f;
        }

        private static void GetStraightPiecePosition(float localDistance, float lane, out float x, out float z, out float rotation)
        {
            x = LaneSpacing * ((NumLanes - 1) / 2f - lane);
            z = localDistance;
            rotation = 0;
        }

        private static void GetCurvePiecePosition(float localDistance, float lane, out float x, out float z, out float rotation)
        {
            float radius = GetCurvePieceRadius(lane);
            float angle = localDistance / radius;
            x = MidRadius - Mathf.Cos(angle) * radius;
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

        /// <summary>
        /// Gets position of a car based on its lane and distance from the start in that lane.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="lane"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="rotation">y rotation of the car, in radians.</param>
        public unsafe void GetPosition(float distance, float lane, out float x, out float z, out float rotation)
        {
            // keep distance in [0, length)
            distance -= Mathf.Floor(distance / length(lane)) * length(lane);

            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;

            float pieceStartDistance = 0;
            float pieceEndDistance = 0;
            x = 0;
            z = 0;
            rotation = 0;

            for (int i = 0; i < 8; i++)
            {
                var piece = pieces[i];
                pieceStartDistance = pieceEndDistance;
                pieceEndDistance += piece.length[(int)lane];
                if (distance >= pieceEndDistance)
                    continue;

                // inside piece i

                // position and rotation local to the piece
                float localX, localZ;
                if (i % 2 == 0)
                {
                    // straight piece
                    GetStraightPiecePosition(distance - pieceStartDistance, lane, out localX, out localZ, out rotation);
                }
                else
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
    }



    /// <summary>
    /// Singleton class containing math functions.
    /// </summary>
    public class Highway : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject carPrefab;
        public GameObject straightPiecePrefab;
        public GameObject curvePiecePrefab;

        public Entity carPrefabEntity;

        public static Highway instance { get; private set; }

        private HighwayPiece[] pieces = new HighwayPiece[8];

        public Highway1 highway1;
        private LinkedList<Car> cars = new LinkedList<Car>();

        public unsafe void CreateHighway(float lane0Length)
        {
            if (lane0Length < Highway1.MinHighwayLane0Length)
            {
                Debug.LogError("Highway length must be longer than " + Highway1.MinHighwayLane0Length);
                return;
            }

            int tempNumCars = GetNumCars();
            if (lane0Length < highway1.innerLaneLength) {
                ClearCars();
            }

            float straightPieceLength = Highway1.GetStraightPieceLength(highway1.innerLaneLength);

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

                    pos += curvePiece.startRotationQ * new Vector3(Highway1.MidRadius, 0, Highway1.MidRadius);
                    rot = Mathf.PI / 2 * (i / 2 + 1);
                }
            }

            highway1.innerLaneLength = Mathf.Max(Highway1.MinHighwayLane0Length, lane0Length);

            if (highway1.pieces.IsCreated)
            {
                highway1.pieces.Dispose();
            }

            highway1.pieces = new NativeArray<HighwayPiece1>(8, Allocator.Persistent);

            for (int i = 0; i < 8; ++i)
            {
                HighwayPiece1 piece1;

                piece1.startX = pieces[i].startX;
                piece1.startZ = pieces[i].startZ;
                piece1.startRotation = pieces[i].startRotation;

                for (int j = 0; j < 4; ++j)
                {
                    piece1.length[j] = pieces[i].length(j);
                }

                highway1.pieces[i] = piece1;
            }

            SetNumCars(tempNumCars);
        }

        public void Destroy()
        {
            if (highway1.pieces.IsCreated)
            {
                highway1.pieces.Dispose();
            }
        }

        /// <summary>
        /// Adds a car to the highway in a random lane.  Won't add car if all lanes are full (returns null).
        /// </summary>
        public Car AddCar(){

            int lane = Random.Range(0, 4);

            for (int i = 0; i < 4; ++i)
            {
                var car = AddCar(lane);
                if (car != null)
                    return car;

                lane = (lane + 1) % 4;
            }

            return null;
        }

        /// <summary>
        /// Adds a car to the highway in the given lane.  Checks to make sure it's not on top of other cars.  Returns null if a car couldn't be placed.
        /// </summary>
        public Car AddCar(float lane)
        {
            return AddCarUnsafe(Random.Range(0, highway1.length(lane)), lane);
        }

        /// <summary>
        /// Adds a car to the highway, without checking if it would fit first.
        /// </summary>
        public Car AddCarUnsafe(float distance, float lane)
        {
            Car car = Instantiate(carPrefab, transform).GetComponent<Car>();
            car.SetRandomPropeties();
            car.distance = distance;
            car.lane = lane;
            car.velocityPosition = car.defaultSpeed;

            var type = new ComponentType[] { typeof(Lane0Tag), typeof(Lane1Tag), typeof(Lane2Tag), typeof(Lane3Tag) };

            var em = World.Active.EntityManager;

            var entity = em.Instantiate(carPrefabEntity);
            em.AddComponentData(entity, new Position {
                Pos = distance,
                Lane = (int)lane,
                MergingTime = 0.0f,
            });
            em.AddComponentData(entity, new Velocity {
                Value = car.defaultSpeed,
                Default = car.defaultSpeed
            });
            em.AddComponentData(entity, new State {
                DefaultSpeed = car.defaultSpeed,
                OvertakePercent = car.overtakePercent,
                LeftMergeDistance = car.leftMergeDistance,
                MergeSpace = car.mergeSpace,
                OvertakeEagerness = car.overtakeEagerness,
            });
            em.AddComponent(entity, type[(int)lane]);

            car.entity = entity;

            cars.AddLast(car);
            
            return car;
        }

        public void RemoveCar(Car car)
        {
            cars.Remove(car);
            Destroy (car.gameObject);

            var em = World.Active.EntityManager;
            em.DestroyEntity(car.entity);
        }

        public int GetNumCars()
        {
            return cars.Count;
        }

        public void SetNumCars(int numCars) {

            while (cars.Count > numCars) {
                RemoveCar(cars.First.Value);
            }
            while (cars.Count < numCars) {
                Car car = AddCar();
                if (car == null)
                    return;
            }

        }
        
        public void ClearCars()
        {
            foreach (Car car in cars)
            {
                Destroy(car.gameObject);
            }
            cars.Clear();
        }

        public Car GetCarAtScreenPosition(Vector3 screenPosition, float radius){

            foreach (Car car in cars) {
                Vector3 carScreenPos = Camera.main.WorldToScreenPoint(car.transform.position);
                carScreenPos.z = screenPosition.z;

                if (Vector3.Distance (screenPosition, carScreenPos) <= radius) {
                    return car;
                }

            }

            return null;
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
            carPrefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(carPrefab, World.Active);
        }

        private void Update()
        {
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
