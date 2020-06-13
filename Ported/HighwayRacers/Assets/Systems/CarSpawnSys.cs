using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Assert = UnityEngine.Assertions.Assert;
using Random = Unity.Mathematics.Random;

namespace HighwayRacer
{
    public class CarSpawnSys : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }
        
        private bool carsSpawned = false;
        
        protected override void OnUpdate()
        {
            if (!carsSpawned)
            {
                const int nCars = 50;
                const int nLanes = 4;
                const float trackLength = 400.0f;
                const int maxCarsInLane = 80;   // todo calculate this from track length

                Assert.IsTrue(nCars < maxCarsInLane * nLanes, "Spawning more cars than will fit on track.");
                
                var arch = EntityManager.CreateArchetype(typeof(Speed), typeof(TrackPos), typeof(TrackSegment), typeof(TargetSpeed), 
                        typeof(UnblockedSpeed), typeof(OvertakeSpeed), typeof(Lane), typeof(BlockedDist), typeof(Translation), typeof(Rotation));
                
                var ents = EntityManager.CreateEntity(arch, nCars, Allocator.Temp);
                ents.Dispose();

                const float minSpeed = 10.0f;
                const float maxSpeed = 30.0f;   // max cruising speed
                const float minOvertakeModifier = 1.2f;
                const float maxOvertakeModifier = 1.6f;
                const float spawnDist = 12.0f;
                
                var nCarsInLane = 0;
                var lane = 0;
                var nextTrackPos = 0.0f;

                var rand = new Random((uint)DateTime.Now.Millisecond);
                rand.NextFloat();
                
                Entities.ForEach((ref Speed speed, ref TrackPos trackPos, ref TargetSpeed targetSpeed, ref UnblockedSpeed unblockedSpeed, 
                        ref OvertakeSpeed overtakeSpeed) =>
                {
                    if (nCarsInLane >= maxCarsInLane)
                    {
                        nCarsInLane = 0;
                        lane++;
                        nextTrackPos = 0.0f;
                        Assert.IsTrue(lane < nLanes);
                    }

                    speed.Val = 1.0f;          // start off slow but not stopped (todo: does logic break if cars stop?)
                    targetSpeed.Val = 1.0f;    
                    unblockedSpeed.Val = math.lerp(minSpeed, maxSpeed, rand.NextFloat()); 
                    overtakeSpeed.Val = targetSpeed.Val * math.lerp(minOvertakeModifier, maxOvertakeModifier, rand.NextFloat());
                    trackPos.Val = nextTrackPos;

                    nextTrackPos += spawnDist;
                    Assert.IsTrue(nextTrackPos <= trackLength - spawnDist, "Spawning more cars than will fit in lane.");
                    nCarsInLane++;
                }).Run();

                carsSpawned = true;
            }
        }
    }
}