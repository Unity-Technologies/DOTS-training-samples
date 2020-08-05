using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class FakeSegmentSpawnerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        var segmentEntity = EntityManager.CreateEntity();
        EntityManager.AddComponent<Segment>(segmentEntity);
        EntityManager.SetComponentData(segmentEntity,new Segment{Start=new float3(0,0,0), End=new float3(1,0,1)});
    }

    protected override void OnUpdate()
    {
    }
}