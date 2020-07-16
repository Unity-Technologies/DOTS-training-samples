using Fire;
using FireBrigade.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Water;
using Random = Unity.Mathematics.Random;

namespace FireBrigade.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class SpawnFirefightersSystem : SystemBase
    {
        private EntityCommandBufferSystem m_ECBSystem;
        private Random m_random;

        private EntityQuery waterWellQuery;
        
        protected override void OnCreate()
        {
            m_ECBSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            m_random = new Random((uint)UnityEngine.Random.Range(100,10000));
            waterWellQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(WellTag));
        }

        protected override void OnUpdate()
        {
            var ecb = m_ECBSystem.CreateCommandBuffer();
            var random = m_random;
            
            // Get the water wells
            var wells = waterWellQuery.ToEntityArray(Allocator.TempJob);
            var fireBufferEntity = GetSingletonEntity<FireBufferElement>();
            var fireLookup = GetBufferFromEntity<FireBufferElement>(true);
            var fireBuffer = fireLookup[fireBufferEntity];
            
            Entities.WithDeallocateOnJobCompletion(wells)
                .ForEach((Entity entity, in FirefighterSpawner spawner, in LocalToWorld position) =>
            {
                random.InitState((uint)math.abs(spawner.seed));
                var numGroups = random.NextInt(spawner.minGroups, spawner.maxGroups);
                for (int i = 0; i < numGroups; i++)
                {
                    // Pick a random starting position for the group
                    var randomGroupPosition = position.Position + random.NextFloat3(spawner.spawnRange);
                    randomGroupPosition.y = 0f;
                    // Pick closest water to group position
                    var closestDistance = float.MaxValue;
                    var closestWellIndex = -1;
                    for (int wellIndex = 0; wellIndex < wells.Length; wellIndex++)
                    {
                        var distance = math.distancesq(GetComponent<LocalToWorld>(wells[wellIndex]).Position,
                            randomGroupPosition);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestWellIndex = wellIndex;
                        }
                    }
                    var waterPosition = GetComponent<LocalToWorld>(wells[closestWellIndex]).Position;
                    waterPosition.y = 0f;
                    // pick a fire cell closest to our chosen water position that is above a threshold in temp
                    closestDistance = float.MaxValue;
                    var closestFireIndex = -1;
                    for (int fireIndex = 0; fireIndex < fireBuffer.Length; fireIndex++)
                    {
                        var temperature = GetComponent<TemperatureComponent>(fireBuffer[fireIndex].FireEntity);
                        if (temperature.Value < 0.2) continue;
                        
                        var distance = math.distancesq(GetComponent<LocalToWorld>(fireBuffer[fireIndex].FireEntity).Position,
                            randomGroupPosition);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestFireIndex = fireIndex;
                        }
                    }
                    var firePosition = GetComponent<LocalToWorld>(fireBuffer[closestFireIndex].FireEntity).Position;
                    firePosition.y = 0f;
                    
                    // Spawn firefighters of each role
                    
                    // scooper
                    var firefighterEntity = ecb.Instantiate(spawner.firefighterPrefab);
                    var randomFirefighterPosition = random.NextFloat(0.5f, 1f) * randomGroupPosition;
                    randomFirefighterPosition.y = 0f;
                    ecb.SetComponent(firefighterEntity, new Translation {Value = randomFirefighterPosition});
                    ecb.SetComponent(firefighterEntity, new GroupIdentifier {Value = i});
                    ecb.AddComponent(firefighterEntity, new GroupCount {Value = spawner.numPerGroup});
                    ecb.AddComponent(firefighterEntity, new WaterPosition {Value = waterPosition});
                    ecb.AddComponent(firefighterEntity, new FirePosition {Value = firePosition});
                    ecb.SetComponent(firefighterEntity, new RoleIndex {Value = 0});
                    ecb.SetComponent(firefighterEntity, new Color(){Value = new float4(0f,0f,0f,1f)});
                    ecb.AddComponent(firefighterEntity, new GroupRole {Value = FirefighterRole.scooper});
                    
                    // thrower
                    firefighterEntity = ecb.Instantiate(spawner.firefighterPrefab);
                    randomFirefighterPosition = random.NextFloat(0.5f, 1f) * randomGroupPosition;
                    randomFirefighterPosition.y = 0f;
                    ecb.SetComponent(firefighterEntity, new Translation {Value = randomFirefighterPosition});
                    ecb.SetComponent(firefighterEntity, new GroupIdentifier {Value = i});
                    ecb.AddComponent(firefighterEntity, new GroupCount {Value = spawner.numPerGroup});
                    ecb.AddComponent(firefighterEntity, new WaterPosition {Value = waterPosition});
                    ecb.AddComponent(firefighterEntity, new FirePosition {Value = firePosition});
                    ecb.SetComponent(firefighterEntity, new RoleIndex {Value = 0});
                    ecb.SetComponent(firefighterEntity, new Color(){Value = new float4(1f,1f,1f,1f)});
                    ecb.AddComponent(firefighterEntity, new GroupRole {Value = FirefighterRole.thrower});

                    // Of the group count, 1 is scooper, 1 is thrower, then half the remainder are empty and the other half full
                    int chainCount = (spawner.numPerGroup - 2) / 2;
                    // empty
                    for (int emptyCount = 0; emptyCount < chainCount; emptyCount++)
                    {
                        firefighterEntity = ecb.Instantiate(spawner.firefighterPrefab);
                        randomFirefighterPosition = random.NextFloat(0.5f, 1f) * randomGroupPosition;
                        randomFirefighterPosition.y = 0f;
                        ecb.SetComponent(firefighterEntity, new Translation {Value = randomFirefighterPosition});
                        ecb.SetComponent(firefighterEntity, new GroupIdentifier {Value = i});
                        ecb.AddComponent(firefighterEntity, new GroupCount {Value = chainCount});
                        ecb.AddComponent(firefighterEntity, new WaterPosition {Value = waterPosition});
                        ecb.AddComponent(firefighterEntity, new FirePosition {Value = firePosition});
                        ecb.SetComponent(firefighterEntity, new RoleIndex {Value = emptyCount});
                        ecb.SetComponent(firefighterEntity, new Color(){Value = new float4(1f,1f,1f,1f)});
                        ecb.AddComponent(firefighterEntity, new GroupRole {Value = FirefighterRole.empty});
                    }
                    // full
                    for (int fullCount = 0; fullCount < chainCount; fullCount++)
                    {
                        firefighterEntity = ecb.Instantiate(spawner.firefighterPrefab);
                        randomFirefighterPosition = random.NextFloat(0.5f, 1f) * randomGroupPosition;
                        randomFirefighterPosition.y = 0f;
                        ecb.SetComponent(firefighterEntity, new Translation {Value = randomFirefighterPosition});
                        ecb.SetComponent(firefighterEntity, new GroupIdentifier {Value = i});
                        ecb.AddComponent(firefighterEntity, new GroupCount {Value = chainCount});
                        ecb.AddComponent(firefighterEntity, new WaterPosition {Value = waterPosition});
                        ecb.AddComponent(firefighterEntity, new FirePosition {Value = firePosition});
                        ecb.SetComponent(firefighterEntity, new RoleIndex {Value = fullCount});
                        ecb.SetComponent(firefighterEntity, new Color(){Value = new float4(0f,1f,0f,1f)});
                        ecb.AddComponent(firefighterEntity, new GroupRole {Value = FirefighterRole.full});
                    }
                    // collector
                    // The collector is an ADDITION to the set group size
                    firefighterEntity = ecb.Instantiate(spawner.firefighterPrefab);
                    randomFirefighterPosition = random.NextFloat(0.5f, 1f) * randomGroupPosition;
                    randomFirefighterPosition.y = 0f;
                    ecb.SetComponent(firefighterEntity, new Translation {Value = randomFirefighterPosition});
                    ecb.SetComponent(firefighterEntity, new GroupIdentifier {Value = i});
                    ecb.AddComponent(firefighterEntity, new GroupCount {Value = spawner.numPerGroup});
                    ecb.AddComponent(firefighterEntity, new WaterPosition {Value = waterPosition});
                    ecb.SetComponent(firefighterEntity, new Color(){Value = new float4(0f,1f,1f,1f)});
                    ecb.AddComponent(firefighterEntity, new BucketCollector());
                    

                    // for (int j = 0; j < spawner.numPerGroup; j++)
                    // {
                    //     var firefighterEntity = ecb.Instantiate(spawner.firefighterPrefab);
                    //     var randomFirefighterPosition = random.NextFloat(0.5f, 1f) * randomGroupPosition;
                    //     randomFirefighterPosition.y = 0f;
                    //     ecb.SetComponent(firefighterEntity, new Translation {Value = randomFirefighterPosition});
                    //     ecb.SetComponent(firefighterEntity, new GroupIdentifier {Value = i});
                    //     ecb.SetComponent(firefighterEntity, new RoleIndex {Value = j});
                    //     ecb.AddComponent(firefighterEntity, new GroupCount {Value = spawner.numPerGroup});
                    //     ecb.AddComponent(firefighterEntity, new WaterPosition {Value = waterPosition});
                    //     ecb.AddComponent(firefighterEntity, new FirePosition {Value = firePosition});
                    //     if (j == 0)
                    //     {
                    //         ecb.AddComponent(firefighterEntity, new GroupRole {Value = FirefighterRole.thrower});
                    //         ecb.SetComponent(firefighterEntity, new Color(){Value = new float4(1f,1f,1f,1f)});
                    //     }
                    //     else if (j == spawner.numPerGroup - 1)
                    //     {
                    //         ecb.AddComponent(firefighterEntity, new GroupRole {Value = FirefighterRole.scooper});
                    //         ecb.SetComponent(firefighterEntity, new Color(){Value = new float4(0f,0f,0f,1f)});
                    //     }
                    //     else
                    //     {
                    //         if (j % 2 == 0)
                    //         {
                    //             ecb.AddComponent(firefighterEntity, new GroupRole {Value = FirefighterRole.full});
                    //             ecb.SetComponent(firefighterEntity, new Color(){Value = new float4(0f,1f,0f,1f)});
                    //         }
                    //         else
                    //         {
                    //             ecb.AddComponent(firefighterEntity, new GroupRole {Value = FirefighterRole.empty});
                    //             ecb.SetComponent(firefighterEntity, new Color(){Value = new float4(1f,1f,0f,1f)});
                    //         }
                    //     }
                    // }
                }
                ecb.DestroyEntity(entity);
            }).Schedule();
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}