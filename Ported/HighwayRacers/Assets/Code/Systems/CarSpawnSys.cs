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
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(RoadSys))]
    public class CarSpawnSys : SystemBase
    {
        private EntityQuery carQuery;
        private Entity carPrefab;

        public const float minSpeed = 7.0f;
        public const float maxSpeed = 20.0f; // max cruising speed

        public const float minBlockedDist = 8.0f;
        public const float maxBlockedDist = 15.0f;

        public const float minOvertakeModifier = 1.2f;
        public const float maxOvertakeModifier = 1.6f;

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
                        typeof(Prefab), typeof(Translation), typeof(Rotation), typeof(URPMaterialPropertyBaseColor)
                    };
                    EntityManager.AddComponents(carPrefab, new ComponentTypes(types));
                }

                // destroy all cars except for prefab
                EntityManager.DestroyEntity(carQuery);

                int nCars = RoadSys.numCars;
                float trackLength = RoadSys.roadLength;

                var ents = EntityManager.Instantiate(carPrefab, nCars, Allocator.Temp);
                ents.Dispose();

                var seed = (uint) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var rand = new Random(seed);

                var carBuckets = RoadSys.CarBuckets;

                var bucketIdx = 0;
                var roadSegments = RoadSys.roadSegments;
                
                // add cars to the buckets
                for (int i = 0; i < nCars; i++)
                {
                    Car car = new Car();

                    car.DesiredSpeedUnblocked = math.lerp(minSpeed, maxSpeed, rand.NextFloat());
                    car.DesiredSpeedOvertake = car.DesiredSpeedUnblocked * math.lerp(minOvertakeModifier, maxOvertakeModifier, rand.NextFloat());
                    
                    car.Speed = 2.0f; // start off slow but not stopped (todo: does logic break if cars stop?)
                    car.BlockingDist = math.lerp(minBlockedDist, maxBlockedDist, rand.NextFloat());

                    while (carBuckets.BucketFull(bucketIdx, roadSegments))
                    {
                        bucketIdx++;
                    }

                    if (!carBuckets.IsBucket(bucketIdx))
                    {
                        Debug.LogError("ran out of buckets when adding cars. On car " + i);  // shouldn't reach here
                        return;
                    }

                    carBuckets.AddCar(ref car, bucketIdx);
                }
            }
        }
    }
}