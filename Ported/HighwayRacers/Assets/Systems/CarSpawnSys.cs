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

        protected override void OnCreate()
        {
            base.OnCreate();
            carQuery = GetEntityQuery(typeof(RenderMesh));
        }

        private bool carsSpawned = false;

        protected override void OnUpdate()
        {
            if (!carsSpawned)
            {
                const int nCars = 50;
                const int nLanes = 4;
                const float trackLength = 400.0f;
                const int maxCarsInLane = 20; // todo calculate this from track length

                const float minSpeed = 10.0f;
                const float maxSpeed = 30.0f; // max cruising speed
                const float minOvertakeModifier = 1.2f;
                const float maxOvertakeModifier = 1.6f;
                const float spawnDist = 12.0f;

                Assert.IsTrue(nCars < maxCarsInLane * nLanes, "Spawning more cars than will fit on track.");

                carPrefab = carQuery.GetSingletonEntity();
                Debug.Log("got prefab");
                EntityManager.AddComponent<Speed>(carPrefab);
                EntityManager.AddComponent<TrackPos>(carPrefab);
                EntityManager.AddComponent<TrackSegment>(carPrefab);
                EntityManager.AddComponent<TargetSpeed>(carPrefab);
                EntityManager.AddComponent<UnblockedSpeed>(carPrefab);
                EntityManager.AddComponent<OvertakeSpeed>(carPrefab);
                EntityManager.AddComponent<Lane>(carPrefab);
                EntityManager.AddComponent<BlockedDist>(carPrefab);
                EntityManager.AddComponent<Translation>(carPrefab);
                EntityManager.AddComponent<Rotation>(carPrefab);
                EntityManager.AddComponent<Color>(carPrefab);

                var ents = EntityManager.Instantiate(carPrefab, nCars, Allocator.Temp);
                ents.Dispose();

                var nCarsInLane = 0;
                byte currentLane = 0;
                var nextTrackPos = 0.0f;

                var rand = new Random((uint) DateTime.Now.Millisecond);
                rand.NextFloat();

                carsSpawned = true;

                Entities.ForEach((ref Speed speed, ref TrackPos trackPos, ref TargetSpeed targetSpeed, ref UnblockedSpeed unblockedSpeed,
                    ref OvertakeSpeed overtakeSpeed, ref Lane lane) =>
                {
                    if (nCarsInLane >= maxCarsInLane)
                    {
                        nCarsInLane = 0;
                        currentLane++;
                        nextTrackPos = 0.0f;
                        Assert.IsTrue(currentLane < nLanes);
                    }

                    speed.Val = 2.0f; // start off slow but not stopped (todo: does logic break if cars stop?)
                    targetSpeed.Val = 1.0f;
                    unblockedSpeed.Val = math.lerp(minSpeed, maxSpeed, rand.NextFloat());
                    overtakeSpeed.Val = targetSpeed.Val * math.lerp(minOvertakeModifier, maxOvertakeModifier, rand.NextFloat());
                    trackPos.Val = nextTrackPos;
                    lane.Val = currentLane;

                    nextTrackPos += spawnDist;
                    Assert.IsTrue(nextTrackPos <= trackLength - spawnDist, "Spawning more cars than will fit in lane.");
                    nCarsInLane++;
                }).Run();

                Debug.Log("cars spawned");
            }
        }
    }
}