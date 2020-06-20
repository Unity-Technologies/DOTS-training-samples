using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;
using Random = Unity.Mathematics.Random;


namespace HighwayRacer
{
    public class CarSpawnSys : SystemBase
    {
        private EntityQuery carQuery;
        private Entity carPrefab;
        
        public readonly static int nLanes = 4;

        public readonly static float minSpeed = 7.0f;
        public readonly static float maxSpeed = 20.0f; // max cruising speed

        public readonly static float minBlockedDist = 8.0f;
        public readonly static float maxBlockedDist = 15.0f;

        public readonly static float minOvertakeModifier = 1.2f;
        public readonly static float maxOvertakeModifier = 1.6f;

        protected override void OnCreate()
        {
            base.OnCreate();
            carQuery = GetEntityQuery(new EntityQueryDesc[]
            {
                new EntityQueryDesc()
                {
                    All = new ComponentType[] {typeof(RenderMesh)},
                    None = new ComponentType[] {typeof(Prefab)},
                },
            });
        }

        public static bool respawnCars = true;
        public static bool firstTime = true;

        protected override void OnUpdate()
        {
            if (respawnCars)
            {
                respawnCars = false;

                if (firstTime)
                {
                    firstTime = false;
                    
                    carPrefab = carQuery.GetSingletonEntity();    
                    var types = new ComponentType[]
                    {
                        typeof(Prefab), typeof(Speed), typeof(TrackPos), typeof(TrackSegment), typeof(TargetSpeed),
                        typeof(DesiredSpeed), typeof(Lane), typeof(Blocking), typeof(Translation), typeof(Rotation), typeof(Color)
                    };
                    EntityManager.AddComponents(carPrefab, new ComponentTypes(types));
                }
                
                // destroy all cars except for prefab
                EntityManager.DestroyEntity(carQuery);

                int nCars = RoadInit.numCars;
                float trackLength = RoadInit.roadLength;

                var ents = EntityManager.Instantiate(carPrefab, nCars, Allocator.Temp);
                ents.Dispose();

                var nCarsInLane = 0;
                byte currentLane = 0;
                var nextTrackPos = 0.0f;

                var rand = new Random((uint) DateTime.Now.Millisecond);
                rand.NextFloat();

                var maxCarsInLane = (nCars % nLanes == 0) ? (nCars / nLanes) : (nCars / nLanes) + 1;

                Entities.ForEach((ref Speed speed, ref TrackPos trackPos, ref TargetSpeed targetSpeed, ref DesiredSpeed desiredSpeed,
                    ref Lane lane, ref Blocking blockedDist, ref Color color) =>
                {
                    if (nCarsInLane >= maxCarsInLane)
                    {
                        nCarsInLane = 0;
                        currentLane++;
                        nextTrackPos = 0.0f;
                        Assert.IsTrue(currentLane < nLanes);
                    }

                    desiredSpeed.Unblocked = math.lerp(minSpeed, maxSpeed, rand.NextFloat());
                    targetSpeed.Val = desiredSpeed.Unblocked;
                    speed.Val = 2.0f; // start off slow but not stopped (todo: does logic break if cars stop?)
                    blockedDist.Dist = math.lerp(minBlockedDist, maxBlockedDist, rand.NextFloat());
                    desiredSpeed.Overtake = targetSpeed.Val * math.lerp(minOvertakeModifier, maxOvertakeModifier, rand.NextFloat());
                    trackPos.Val = nextTrackPos;
                    lane.Val = currentLane;
                    color.Val = new float4(SetColorSys.cruiseColor, 1.0f);

                    nextTrackPos += RoadInit.carSpawnDist;
                    Assert.IsTrue(nextTrackPos <= trackLength - RoadInit.carSpawnDist, "Spawning more cars than will fit in lane.");
                    nCarsInLane++;
                }).Run();
            }
        }
    }
}