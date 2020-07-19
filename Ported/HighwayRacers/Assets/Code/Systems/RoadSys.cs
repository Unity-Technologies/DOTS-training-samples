using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class RoadSys : SystemBase
    {
        public static NativeArray<RoadSegment> roadSegments;
        public static NativeArray<float> thresholds;
        public static NativeArray<float> segmentLengths;
        
        public static CarBuckets CarBuckets;

        public const int nLanes = 4;
        public static int nSegments = 8;
        public const float lengthDivisor = 4000;    // bigger = fewer, larger segments
        public const float minDist = 4.0f;

        public const float mergeTime = laneWidth / 1.8f; // merge rate (meters per second)
        
        public const float mergeLookAhead = 14.0f;
        public const float mergeLookBehind = 7.5f;

        public const float decelerationRate = 3.0f; // m/s to lose per second
        public const float accelerationRate = 8.0f; // m/s to lose per second

        public const float carSpawnDist = 8.0f;
        public static int numCars = 10;

        public static float minLength = 400.0f; // 4 * curved length + 4 * min straight length
        public static float maxLength = 1000000;

        public const float laneWidth = 1.88f;
        public static float roadLength = minLength;

        public static float straightLength;
        public const float curvedLength = 48.69f;
        
        public const float overtakeTime = 6.0f;
        public const float overtakeTimeTryMerge = overtakeTime - 3.0f;

        private List<GameObject> straightRoadExtraPieces = new List<GameObject>(); // pieces of straight road that should be destroyed when we recreate the road

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<RoadInit>();
            var ent = EntityManager.CreateEntity(typeof(RoadInit));
            EntityManager.SetComponentData(ent,
                new RoadInit()
                {
                    Length = roadLength,
                    NumCars = numCars
                }
            );
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            Cleanup();
            base.OnDestroy();
        }

        private void Cleanup()
        {
            roadSegments.Dispose();
            thresholds.Dispose();
            segmentLengths.Dispose();
            CarBuckets.Dispose();
        }
        
        public static int NumCarsFitInStraightSegment(float straightLength)
        {
            var n = (int)(straightLength / RoadSys.carSpawnDist) - 2; // - 2 so cars can spawn comfortably within margin
            return n * nLanes; 
        }

        public static float GetLengthStraightSegment(float roadLength)
        {
            return (roadLength - RoadSys.curvedLength * 4) / (GetNumSegments(roadLength) - 4);
        }

        public static int GetNumSegments(float length)
        {
            return 4 + (4 * GetNumSegmentsPerStraightaway(length));
        }
        
        public static int GetNumSegmentsPerStraightaway(float length)
        {
            return (int) (length / lengthDivisor) + 1;  // todo: play with this number to experiment with segment size
        }

        public static int GetMaxCars(float roadLength)
        {
            var straightLength = GetLengthStraightSegment(roadLength);
            return GetNumSegmentsPerStraightaway(roadLength) * NumCarsFitInStraightSegment(straightLength) * 4;     
        }

        public static float4x4 GetRotYMatrix(float radians)
        {
            var mat = float4x4.identity;
            math.sincos(radians, out var s, out var c);
            mat.c0 = new float4(c, 0, -s, 1);
            mat.c1 = new float4(0, 1, 0, 1);
            mat.c2 = new float4(s, 0, c, 1);
            return mat;
        }

        protected override void OnUpdate()
        {
            var roadInit = GetSingleton<RoadInit>();
            EntityManager.DestroyEntity(GetSingletonEntity<RoadInit>());

            numCars = roadInit.NumCars;
            roadLength = roadInit.Length;
            
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<CameraSys>().ResetCamera();

            CarBucketSpawnSys.remakeBuckets = true;

            var transform = GameObject.Find("Road").transform;

            var rotFromCardinal = new Dictionary<Cardinal, float>();
            rotFromCardinal[Cardinal.UP] = 0;
            rotFromCardinal[Cardinal.DOWN] = 180;
            rotFromCardinal[Cardinal.LEFT] = -90;
            rotFromCardinal[Cardinal.RIGHT] = 90;

            var vecFromCardinal = new Dictionary<Cardinal, float3>();
            vecFromCardinal[Cardinal.UP] = Vector3.forward;
            vecFromCardinal[Cardinal.DOWN] = Vector3.back;
            vecFromCardinal[Cardinal.LEFT] = Vector3.left;
            vecFromCardinal[Cardinal.RIGHT] = Vector3.right;

            var transformFromCardinal = new Dictionary<Cardinal, float4x4>();
            transformFromCardinal[Cardinal.UP] = GetRotYMatrix(0);
            transformFromCardinal[Cardinal.DOWN] = GetRotYMatrix(math.radians(180));
            transformFromCardinal[Cardinal.LEFT] = GetRotYMatrix(math.radians(-90));
            transformFromCardinal[Cardinal.RIGHT] = GetRotYMatrix(math.radians(90));

            var rightRotFromCardinal = new Dictionary<Cardinal, quaternion>();
            rightRotFromCardinal[Cardinal.UP] = quaternion.EulerXYZ(0, math.radians(90), 0);
            rightRotFromCardinal[Cardinal.DOWN] = quaternion.EulerXYZ(0, math.radians(270), 0);
            rightRotFromCardinal[Cardinal.LEFT] = quaternion.EulerXYZ(0, 0, 0);
            rightRotFromCardinal[Cardinal.RIGHT] = quaternion.EulerXYZ(0, math.radians(180), 0);

            var leftVecFromCardinal = new Dictionary<Cardinal, float3>();
            leftVecFromCardinal[Cardinal.UP] = Vector3.left;
            leftVecFromCardinal[Cardinal.DOWN] = Vector3.right;
            leftVecFromCardinal[Cardinal.LEFT] = Vector3.back;
            leftVecFromCardinal[Cardinal.RIGHT] = Vector3.forward;

            const float baseStraightLength = 12.0f;

            int segmentsPerStraightaway = GetNumSegmentsPerStraightaway(roadLength);
            nSegments = GetNumSegments(roadLength);

            straightLength = GetLengthStraightSegment(roadLength);
            Debug.Log("straighLength: " + straightLength);
            float straightScale = straightLength / baseStraightLength;

            if (roadSegments.IsCreated)
            {
                roadSegments.Dispose();
            }

            if (thresholds.IsCreated)
            {
                thresholds.Dispose();
            }

            if (segmentLengths.IsCreated)
            {
                segmentLengths.Dispose();
            }
            
            if (CarBuckets.IsCreated)
            {
                CarBuckets.Dispose();
            }

            roadSegments = new NativeArray<RoadSegment>(nSegments, Allocator.Persistent);
            thresholds = new NativeArray<float>(nSegments, Allocator.Persistent);
            segmentLengths = new NativeArray<float>(nSegments, Allocator.Persistent);
            CarBuckets = new CarBuckets(nSegments, NumCarsFitInStraightSegment(straightLength));
            
            var segmentGOs = new GameObject[nSegments];

            // cleanup extra straight pieces
            foreach (var go in straightRoadExtraPieces)
            {
                GameObject.Destroy(go);
            }
            straightRoadExtraPieces.Clear();

            int newIdx = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                var segmentInfo = child.gameObject.GetComponent<SegmentAuth>();
                var cardinal = segmentInfo.direction;

                if (segmentInfo.radius > 0) // curved
                {
                    segmentGOs[newIdx] = child.gameObject;

                    roadSegments[newIdx] = new RoadSegment()
                    {
                        Transform = transformFromCardinal[cardinal],
                        Direction = cardinal,
                        DirectionVec = vecFromCardinal[cardinal],
                        RotDegrees = rotFromCardinal[cardinal],
                        DirectionLaneOffset = leftVecFromCardinal[cardinal] * laneWidth,
                        Length = curvedLength,
                        Radius = segmentInfo.radius,
                    };

                    newIdx++;
                }
                else // straight
                {
                    child.localScale = new Vector3(1, 1, straightScale);

                    for (int j = 0; j < segmentsPerStraightaway; j++)
                    {
                        if (j == 0)
                        {
                            segmentGOs[newIdx] = child.gameObject;
                        }
                        else
                        {
                            var newTrackPiece = GameObject.Instantiate(child.gameObject);
                            segmentGOs[newIdx] = newTrackPiece;
                            straightRoadExtraPieces.Add(newTrackPiece);
                        }

                        roadSegments[newIdx] = new RoadSegment()
                        {
                            Transform = transformFromCardinal[cardinal],
                            Direction = cardinal,
                            DirectionVec = vecFromCardinal[cardinal],
                            RotDegrees = rotFromCardinal[cardinal],
                            DirectionLaneOffset = leftVecFromCardinal[cardinal] * laneWidth,
                            Length = straightLength,
                            Radius = 0,
                        };

                        newIdx++;
                    }
                }
            }

            var pos = new float3();
            float lengthSum = 0;
            
            var baseBounds = new Bounds();
            baseBounds.extents = new Vector3(straightLength, straightLength, straightLength);

            // put the track pieces in place: end point of segment is start point of next
            for (int i = 0; i < roadSegments.Length; i++)
            {
                var segment = roadSegments[i];
                float length = 0;
                float3 endPos = new float3();

                if (segment.Radius > 0) // curved
                {
                    endPos = pos + vecFromCardinal[segment.Direction] * segment.Radius + leftVecFromCardinal[segment.Direction] * -segment.Radius;
                    length = curvedLength;
                    lengthSum += curvedLength;
                }
                else // straight
                {
                    endPos = pos + vecFromCardinal[segment.Direction] * straightLength;
                    length = straightLength;
                    lengthSum += straightLength;
                }

                segmentGOs[i].transform.position = pos;

                var tr = segment.Transform;
                tr.c3 = new float4(pos, 1);
                
                segment.Transform = tr;
                segment.Length = length;
                baseBounds.center = pos;
                segment.Bounds = baseBounds;
                
                thresholds[i] = lengthSum;
                segmentLengths[i] = length;
                roadSegments[i] = segment;

                pos = endPos;
            }
        }
    }

    // todo: use float2's instead
    public struct RoadSegment
    {
        public float4x4 Transform;   // transform for car in rightmost lane at start of segment
        public Cardinal Direction;
        public float3 DirectionVec;
        public float3 DirectionLaneOffset;
        public float RotDegrees;             
        public float Length;
        public float Radius; // 0 for straight segments 
        public Bounds Bounds;

        public bool IsCurved()
        {
            return Radius != 0;
        }
    }

    public enum Cardinal
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }
}