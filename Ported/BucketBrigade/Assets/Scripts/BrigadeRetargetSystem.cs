using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(BrigadeInitializationSystem))]
public class BrigadeRetargetSystem : SystemBase
{
    private float updateSpeed = 10.0f;
    private float nextUpdate = 0.1f;
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

        var fireConfigEntity = GetSingletonEntity<FireConfiguration>();
        var fireConfig = EntityManager.GetComponentData<FireConfiguration>(fireConfigEntity);

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
            .WithReadOnly(translations)
            .WithoutBurst()
            .WithReadOnly(translations)
            .ForEach((ref Brigade brigade) =>
            {
                
                float3 waterTarget = GetComponent<LocalToWorld>(brigade.waterEntity).Value.c3.xyz;
                Debug.Log(waterTarget);
                // Find closest fire
                float closestDistSq = 0;
                float3 closestFirePosition = float3.zero;
                Entity closestFire = Entity.Null;
                bool foundFire = false;
                for (int i = 0; i < temperatureArray.Length; ++i)
                {
                    Entity e = temperatureArray[i];
                    Temperature theirTemp = temperatures[e];
                    Translation translation = translations[e];

                    float3 diff = waterTarget - translation.Value;
                    float distSq = diff.x * diff.x + diff.z * diff.z;
                    if (theirTemp.Value > fireConfig.FlashPoint && (!foundFire || distSq < closestDistSq))
                    {
                        foundFire = true;
                        closestDistSq = distSq;
                        closestFirePosition = translation.Value;
                        closestFirePosition.y = 0;
                        closestFire = e;
                    }
                }

                if (foundFire)
                {
                    brigade.fireTarget = closestFirePosition;
                    brigade.fireEntity = closestFire;
                }
                else
                {
                    brigade.fireTarget = brigade.random.NextFloat3() * 10.0f;
                    brigade.fireEntity = Entity.Null;
                }

                brigade.fireTarget.y = 0;
            }).Schedule(Dependency);
        
        var handle2 = Entities
            // .WithReadOnly(translations)
            .WithAll<TargetPosition>()
            .ForEach((Entity e, ref TargetPosition target, in BrigadeGroup group, in FullPasserInfo passerInfo) =>
            {
                var brigadeData = BrigadeDataLookup[@group.Value];
                var waterPosition = GetComponent<LocalToWorld>(brigadeData.waterEntity).Value.c3.xyz;
                target.Value = UtilityFunctions.GetChainPosition(passerInfo.ChainPosition, passerInfo.ChainLength, waterPosition, brigadeData.fireTarget);
            }).Schedule(handle1);
        
        var lastHandle = Entities
            // .WithReadOnly(translations)
            .WithAll<TargetPosition>()
            .ForEach((Entity e, ref TargetPosition target, in BrigadeGroup group, in EmptyPasserInfo passerInfo) =>
            {
                var brigadeData = BrigadeDataLookup[@group.Value];
                var waterPosition = GetComponent<LocalToWorld>(brigadeData.waterEntity).Value.c3.xyz;
                target.Value = UtilityFunctions.GetChainPosition(passerInfo.ChainPosition, passerInfo.ChainLength, brigadeData.fireTarget, waterPosition);
            }).Schedule(handle2);

        temperatureArray.Dispose(lastHandle);
        BrigadeDataLookup.Dispose(lastHandle);
        Dependency = lastHandle;
    }
}