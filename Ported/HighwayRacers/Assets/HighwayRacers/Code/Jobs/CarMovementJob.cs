using Aspects;
using HighwayRacers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Profiling;
using UnityEngine;

namespace Jobs
{
    [WithAll(typeof(CarData))]
    [BurstCompile]
    public partial struct CarMovementJob : IJobEntity
    {
        [ReadOnly]
        public int frameCount;

        [ReadOnly]
        public float DeltaTime;

        [ReadOnly]
        public Config config;

        [ReadOnly]
        public NativeArray<CarData> allCars;

        [ReadOnly]
        public NativeArray<LocalTransform> allCarTransforms;
        
#if USE_HIGHWAY
        [ReadOnly]
        public NativeArray<float3> allHighwayPieces;
#endif

        
        [BurstCompile]
        private void Execute(ref CarAspect car)
        {
            CarData other;
            CarData nearestFrontCar = default;
            bool foundFrontCar = false;

            float distanceToFrontCar = float.MaxValue;
            float desiredSpeed = car.CruisingSpeed;

            // while changing lane, don't do anything else
            if (car.CurrentLane < car.DesiredLane)
            {
                car.CurrentLane = math.min(car.CurrentLane + config.SwitchLanesSpeed * DeltaTime, car.DesiredLane);
            }
            else if (car.CurrentLane > car.DesiredLane)
            {
                car.CurrentLane = math.max(car.CurrentLane - config.SwitchLanesSpeed * DeltaTime, car.DesiredLane);
            }
            else if (car.OvertakeModeCountdown > 0)
            {
                // in overtake mode, move faster and tick down the timer
                desiredSpeed = car.OvertakeSpeed;
                car.OvertakeModeCountdown = math.max(car.OvertakeModeCountdown - DeltaTime, 0);
                if (car.OvertakeModeCountdown == 0)
                {
                    car.DesiredLane = car.OvertakeModeReturnToLane;
                }
            }
            else if (car.TEMP_NextLaneChangeCountdown <= 0)
            {
                // in regular cruising mode, randomly change lanes
                if (frameCount % 2 == 1)
                    car.DesiredLane = math.min(car.CurrentLane + 1, config.NumLanes - 1);
                else
                    car.DesiredLane = math.max(car.CurrentLane - 1, 0);

                if (car.DesiredLane != car.CurrentLane)
                {
                    car.OvertakeModeCountdown = config.OvertakeMaxDuration;
                    car.OvertakeModeReturnToLane = car.CurrentLane;
                }
                car.TEMP_NextLaneChangeCountdown = 3;
            }
            else
            {
                car.TEMP_NextLaneChangeCountdown -= DeltaTime;
            }

           
            for (int i = 0; i < allCars.Length; i++)
            {
                other = allCars[i];
                
                if(car.CurrentLane == other.CurrentLane && other.DesiredLane == other.CurrentLane) ;
                {
                    var distToOtherCarInLane = other.Distance - car.Distance;

                    if (distToOtherCarInLane > 0.0f && distToOtherCarInLane < distanceToFrontCar)
                    {
                        distanceToFrontCar = distToOtherCarInLane;
                        nearestFrontCar = other;
                        foundFrontCar = true;
                    }
                }
            }

            float minDistance = nearestFrontCar.Distance - car.Distance;

            if (foundFrontCar && minDistance < car.Length)
            {
                if (minDistance > 0.0f)
                    desiredSpeed = nearestFrontCar.Speed;
                else
                    desiredSpeed = 0.0f;
            }

            if (car.Speed < desiredSpeed)
                car.Speed = math.min(car.Speed + car.Acceleration, desiredSpeed);
            else
                car.Speed = math.max(car.Speed - car.Acceleration, desiredSpeed);

            if (desiredSpeed > car.OvertakeSpeed)
            {
                car.DesiredSpeed = 1.0f;
            }
            else
            {
                if (desiredSpeed == car.CruisingSpeed)
                    car.DesiredSpeed = 0.0f;
                else
                    car.DesiredSpeed = -1.0f;
                
            }

            car.Distance = (car.Distance + car.Speed * DeltaTime) % config.HighwayMaxSize;
            
#if USE_HIGHWAY
            // Translate position to track
            float x, y, rotation;
            GetPosition(car.Distance, car.CurrentLane, out x, out y, out rotation);
            //Debug.LogFormat("GetPosition( car.Distance={0}, car.CurrentLane={1}, out x={2}, out y={3}, out rotation={4}", car.Distance, car.CurrentLane, x, y, rotation);
            
            // Move on track
            car.Position = new float3(x, y, car.CurrentLane);
            car.Rotation = Quaternion.Euler(0, rotation * Mathf.Rad2Deg, 0);
#else
            car.Position = new float3(car.Distance, 0, car.CurrentLane);
#endif
            //Debug.LogFormat("car.Position={0}", car.Position);
        }
        
#if USE_HIGHWAY
        /// <summary>
        /// Gets position of a car based on its lane and distance from the start in that lane.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="lane"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="rotation">y rotation of the car, in radians.</param>
        public void GetPosition(float distance, float lane, out float x, out float z, out float rotation)
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

            for (int i = 0; i < allHighwayPieces.Length; i++)
            {
                pieceStartDistance = pieceEndDistance;
                pieceEndDistance += length(lane);
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
                RotateAroundOrigin(localX, localZ, allHighwayPieces[i].y, out x, out z);
                x += allHighwayPieces[i].x;
                z += allHighwayPieces[i].z;
                rotation += allHighwayPieces[i].y;
                break;                
            }
            
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
            x = MID_RADIUS-Mathf.Cos(angle) * radius;
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
         
         public const int NUM_LANES = 4;
         public const float LANE_SPACING = 1.9f;
         public const float MID_RADIUS = 31.46f;
         public const float CURVE_LANE0_RADIUS = MID_RADIUS - LANE_SPACING * (NUM_LANES - 1) / 2f;
         //public const float MIN_HIGHWAY_LANE0_LENGTH = CURVE_LANE0_RADIUS * 4;
         //public const float MIN_DIST_BETWEEN_CARS = .7f;
         
         /// <summary>
         /// Length of the innermost lane.
         /// </summary>
         //public float lane0Length { get; private set; }
         public float length(float lane)
         {
             return straightPieceLength * 4 + curvePieceLength(lane) * 4;
         }
         public float straightPieceLength
         {
             get
             {
                 return (250 - CURVE_LANE0_RADIUS * 4) / 4; //lane0Length
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
#endif
    }
}
