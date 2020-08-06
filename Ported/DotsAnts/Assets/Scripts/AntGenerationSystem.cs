using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class AntGenerationSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        AntDefaults antDefaults = (AntDefaults)Camera.main.GetComponent(typeof(AntDefaults));
        int mapSize = antDefaults.mapSize;
        int antCount = antDefaults.antCount;
        float2 colonyLocation = new float2();
        bool successfullySpawned = false;
       
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        Entities.ForEach((Entity spawnerEntity, in AntGeneration spawner, in Position spawnerPosition) =>
        {
            colonyLocation = spawnerPosition.value + new float2(mapSize, mapSize) * 0.5f;

            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(89729364);
            
            for (int i = 0; i < antCount; i++)
            {   
                var instance = ecb.Instantiate(spawner.AntPrefab);
                ecb.SetComponent(instance, new Position { value = colonyLocation });
                ecb.SetComponent(instance, new DirectionAngle { value = 2.0f * math.PI * rng.NextFloat() });
                ecb.SetComponent(instance, new CarryingFood { value = false });
            }
            ecb.DestroyEntity(spawnerEntity);
            successfullySpawned = true;

        }).Run();

        if(successfullySpawned)
        {
            //EntityManager.CreateEntity(ComponentType.ReadOnly<ColonyLocation>());
            SetSingleton(new ColonyLocation { value = colonyLocation });
        }
    }
}