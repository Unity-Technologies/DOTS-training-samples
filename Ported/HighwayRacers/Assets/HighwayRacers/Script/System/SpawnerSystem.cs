using System;
using System.Collections;
using System.Collections.Generic;
using HighwayRacers;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using Random = Unity.Mathematics.Random;

public class SpawnerSystem : SystemBase
{
    private EntityQuery RequirePropagation;
    private TrackOccupancySystem m_TrackOccupancySystem;

    protected override void OnCreate()
    {
        m_TrackOccupancySystem = World.GetExistingSystem<TrackOccupancySystem>();
    }

 
    
    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);
        uint laneCount = m_TrackOccupancySystem.LaneCount;

        // todo - get this from outside
        int carCount = 256;
        int[] laneTotals = new int[laneCount];

        // roughly distribute the total care count between lanes
        int avgDistrib = Mathf.RoundToInt(carCount / laneCount);
        int lowDistrib = avgDistrib / 2;
        int highDistrib = 3 * avgDistrib / 2;
        int allocated = 0;
        
        for (int j = 0; j < laneCount - 1; j++)
        {
            int thischunk = random.NextInt(lowDistrib, highDistrib);
            allocated += thischunk;
            laneTotals[j] = thischunk;
        }
        laneTotals[laneCount - 1] = carCount - allocated;
        
        

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (int lane = 0; lane < laneCount; lane++)
                {
                    int carsMax = laneTotals[lane];
                    Entity[] carsInList = new Entity[carsMax];

                    // create the cars from head to tail in this lane
                    // ranging from 0.75 to 0.25 in offset
                    // will this need to be scaled to accommodate different track sizes
                    float head = 0.75f;
                    float stride = -1f / (carsMax * 2f);
                    
                    for (int ctr = 0; ctr < carsMax; ctr ++)
                    {
                        Entity currentEnt;
                        carsInList[ctr] = currentEnt = ecb.Instantiate(spawner.CarPrefab);
                        
                        var translation = new Translation {Value = new float3(0, 0, 0)};
                        ecb.SetComponent(currentEnt, translation);

                        ecb.SetComponent(currentEnt, new URPMaterialPropertyBaseColor
                        {
                            Value = random.NextFloat4()
                        });
                        
                        head += stride;
                        
                        ecb.SetComponent(currentEnt, new CarMovement
                        {
                            Offset =  head,
                            Lane = Convert.ToUInt32(lane),
                            Velocity = random.NextFloat(0.015f, 0.03f),
                        });
                    }
                    
                    // now set up the linked list
                    for (int eachEnt = 0; eachEnt < carsMax; eachEnt++)
                    {
                        int ahead = (eachEnt + carsMax - 1) % carsMax;
                        int behind = (eachEnt + carsMax + 1) % carsMax;
                        ecb.SetComponent(carsInList[eachEnt], new LinkedListLane
                        {
                            Behind = carsInList[behind],
                            Ahead = carsInList[ahead]
                        });
                    }

                }
              
            }).WithoutBurst().Run();

        ecb.Playback(EntityManager);
        
        // Propagate color from parent to child entities
        // We do this every frame because we change the color of the cars

        // A "ComponentDataFromEntity" allows random access to a component type from a job.
        // This much slower than accessing the components from the current entity via the
        // lambda parameters.
        var cdfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();

        Entities
            // Random access to components for writing can be a race condition.
            // Here, we know for sure that prefabs don't share their entities.
            // So explicitly request to disable the safety system on the CDFE.
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithStoreEntityQueryInField(ref RequirePropagation)
            .WithAll<PropagateColor>()
            .ForEach((in DynamicBuffer<LinkedEntityGroup> group
                , in URPMaterialPropertyBaseColor color) =>
            {
                for (int i = 1; i < group.Length; ++i)
                {
                    cdfe[group[i].Value] = color;
                }
            }).ScheduleParallel();
    }
}
