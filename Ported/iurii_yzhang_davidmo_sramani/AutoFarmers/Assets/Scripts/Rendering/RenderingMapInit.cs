using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct TilePositionRequest : IComponentData
{
    public int2 position;
}

public struct StonePositionRequest : IComponentData
{
    public int2 position;
    public int2 size;
}

public struct PlantPositionRequest : IComponentData
{
    public int2 position;
}

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class RenderingMapInit : JobComponentSystem
{
    private Entity m_executeOnceEntity;
    struct ExecuteOnceTag : IComponentData {}
    
    public Entity groundEntityPrefab;
    public Entity stoneEntityPrefab;
    public Entity plantEntityPrefab;

    protected override void OnCreate()
    {
        var ru = RenderingUnity.instance;

        if (ru != null)
        {
            groundEntityPrefab = ru.CreateGround(EntityManager);
            stoneEntityPrefab = ru.CreateStone(EntityManager);
            plantEntityPrefab = ru.CreatePlant(EntityManager);
            
            m_executeOnceEntity = EntityManager.CreateEntity();
            EntityManager.AddComponent<ExecuteOnceTag>(m_executeOnceEntity);

            var query = GetEntityQuery(ComponentType.ReadOnly<ExecuteOnceTag>());
            RequireForUpdate(query);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityManager.DestroyEntity(m_executeOnceEntity);
        
        var ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        
        var ground = groundEntityPrefab;
        var stone = stoneEntityPrefab;
        var plant = plantEntityPrefab;
        
        var rendering = RenderingUnity.instance;
        
        var ecb1 = ecbSystem.CreateCommandBuffer().ToConcurrent();
        var handle1 = Entities.ForEach((int nativeThreadIndex, ref TilePositionRequest req) =>
        {
            var wpos = RenderingUnity.Tile2WorldPosition(req.position);
            var e = ecb1.Instantiate(nativeThreadIndex, ground);
            ecb1.SetComponent(nativeThreadIndex, e, new Translation {Value = wpos}); //TODO: rewrites y pos. fix
            
        }).Schedule(inputDeps);
        
        var ecb2 = ecbSystem.CreateCommandBuffer().ToConcurrent();
        var handle2 = Entities.ForEach((int nativeThreadIndex, ref StonePositionRequest req) =>
        {
            var wpos = RenderingUnity.Tile2WorldPosition(req.position); // min pos
            var wsize = RenderingUnity.Tile2WorldSize(req.size);
            var wposCenter = wpos + wsize * 0.5f;
            
            var e = ecb2.Instantiate(nativeThreadIndex, stone);
            ecb2.SetComponent(nativeThreadIndex, e, new Translation {Value = wposCenter});
            ecb2.SetComponent(nativeThreadIndex, e, new NonUniformScale {Value = wsize});
            
        }).Schedule(inputDeps);

        var ecb3 = ecbSystem.CreateCommandBuffer().ToConcurrent();
        var handle3 = Entities.ForEach((int nativeThreadIndex, ref PlantPositionRequest req) =>
        {
            var wpos = RenderingUnity.Tile2WorldPosition(req.position);
            var e = ecb3.Instantiate(nativeThreadIndex, plant);
            ecb3.SetComponent(nativeThreadIndex, e, new Translation {Value = wpos});
            
        }).Schedule(inputDeps);

        var handle = JobHandle.CombineDependencies(handle1, handle2, handle3);
        
        ecbSystem.AddJobHandleForProducer(handle1);
        ecbSystem.AddJobHandleForProducer(handle2);
        ecbSystem.AddJobHandleForProducer(handle3);

        Debug.Log("Map Init");
        return handle;
    }
}
