using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AssignNewTargetSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    //[BurstCompile]
    struct TranslationJob : IJobForEachWithEntity<ReadyToAssignNewTargetComponent, TrackSplineComponent>
    {
        [ReadOnly]
        public float DeltaTime;

        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index,[ReadOnly] ref ReadyToAssignNewTargetComponent newAssign, ref TrackSplineComponent trackSpline)
        { 
           // trackSpline = ;
            CommandBuffer.RemoveComponent<ReadyToAssignNewTargetComponent>(index, entity);
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new TranslationJob
        {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            DeltaTime = Time.deltaTime,
        };
        //m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
        return job.Schedule(this, inputDependencies);
    }
}
