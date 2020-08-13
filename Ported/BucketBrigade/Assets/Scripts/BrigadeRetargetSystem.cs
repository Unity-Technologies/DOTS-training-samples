using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(BrigadeInitializationSystem))]
public class BrigadeRetargetSystem : SystemBase
{
    private float updateSpeed = 10.0f;
    private float nextUpdate = 0;
    private Random random;

    private EntityQuery temperatureGroup;
    
    protected override void OnCreate()
    {
        random = new Random(1);
        temperatureGroup = GetEntityQuery(typeof(Temperature));
    }

    protected override void OnUpdate()
    {
        // do an occasional target update
        nextUpdate -= Time.DeltaTime;
        if (nextUpdate > 0)
        {
            return;
        }
        nextUpdate = updateSpeed;

        // build the brigade lookup
        // this should really be done once
        var BrigadeDataLookup = new NativeHashMap<Entity, Brigade>(16, Allocator.TempJob);
        Entities
            .WithAll<Brigade>()
            .ForEach((Entity e, in Brigade brigade) => { BrigadeDataLookup.Add(e, brigade); })
            .Schedule();
        
        var temperatures = GetComponentDataFromEntity<Temperature>(true);
        var translations = GetComponentDataFromEntity<Translation>(true);
        var temperatureArray = temperatureGroup.ToEntityArray(Allocator.TempJob);
            
        // just randomly updating the targets
        // will need to be replaced by the search
        var handle1 = Entities.WithAll<Brigade>()
            .WithReadOnly(temperatures)
            .WithoutBurst()
            .WithReadOnly(translations)
            .ForEach((ref Brigade brigade) =>
        {
            brigade.waterTarget = brigade.random.NextFloat3() * 10.0f;
            brigade.waterTarget.y = 0;
            
            // Find closest fire
            float closestDistSq = 0;
            float3 closestFirePosition = float3.zero;
            bool foundFire = false;
            for (int i = 0; i < temperatureArray.Length; ++i)
            {
                Entity e = temperatureArray[i];
                Temperature theirTemp = temperatures[e];
                Translation translation = translations[e];

                float3 diff = brigade.waterTarget - translation.Value;
                float distSq = diff.x * diff.x + diff.z * diff.z;
                if (!foundFire || distSq < closestDistSq)
                {
                    foundFire = true;
                    closestDistSq = distSq;
                    closestFirePosition = translation.Value;
                    closestFirePosition.y = 0;
                }
            }

            if (foundFire)
            {
                brigade.fireTarget = closestFirePosition;
            }   
            else
            {
                brigade.fireTarget = brigade.random.NextFloat3() * 10.0f;
            }

            brigade.fireTarget.y = 0;
        }).Schedule(Dependency);
        
        var handle2 = Entities
            .WithAll<TargetPosition>()
            .ForEach((Entity e, ref TargetPosition target, in BrigadeGroup group, in EmptyPasserInfo passerInfo) =>
            {
                var brigadeData = BrigadeDataLookup[@group.Value];
                target.Value = BrigadeInitializationSystem.GetChainPosition(passerInfo.ChainPosition, passerInfo.ChainLength, brigadeData.waterTarget, brigadeData.fireTarget);
            }).Schedule(handle1);
        
        var lastHandle = Entities
            .WithAll<TargetPosition>()
            .ForEach((Entity e, ref TargetPosition target, in BrigadeGroup group, in FullPasserInfo passerInfo) =>
            {
                var brigadeData = BrigadeDataLookup[@group.Value];
                target.Value = BrigadeInitializationSystem.GetChainPosition(passerInfo.ChainPosition, passerInfo.ChainLength, brigadeData.fireTarget, brigadeData.waterTarget);
            }).Schedule(handle2);

        temperatureArray.Dispose(lastHandle);
        BrigadeDataLookup.Dispose(lastHandle);

        Dependency = lastHandle;
    }
}