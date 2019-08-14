using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AssignNewTargetSystem_TEST : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    //[BurstCompile]
    struct AssignNewTargetJob : IJobForEachWithEntity<AssignNewTarget_TEST, MovementSpeedComponent_TEST>
    {
        [ReadOnly]
        public float DeltaTime;

        [ReadOnly]
        public NativeArray<TrackSplineDOTS_TEST> someDynamicBuffer;

        [ReadOnly]
        public Unity.Mathematics.Random random;

        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref AssignNewTarget_TEST newAssign, ref MovementSpeedComponent_TEST movSpeedComponent)
        {
            TrackSplineDOTS_TEST currentTrack = someDynamicBuffer[movSpeedComponent.currentTrackSpline];

            int next = 0;

            if (movSpeedComponent.dir == 1)
            {
                next = currentTrack.GetNextTrack(random.NextInt(0, 2));
                movSpeedComponent.next = next;
                if (next != -1)
                {
                    for (int i = 0; i < someDynamicBuffer.Length; i++)
                    {
                        if (next == someDynamicBuffer[i].id)
                        {
                            movSpeedComponent.lastTrackSpline = movSpeedComponent.currentTrackSpline;
                            movSpeedComponent.currentTrackSpline = i;

                        }
                    }
                }
                else
                {
                    movSpeedComponent.dir = -1;
                }
            }
            else
            {
                next = someDynamicBuffer[movSpeedComponent.lastTrackSpline].GetNextTrack(random.NextInt(0, 2));
                movSpeedComponent.next = next;
                if (next != -1)
                {
                    for (int i = 0; i < someDynamicBuffer.Length; i++)
                    {
                        if (next == someDynamicBuffer[i].id)
                        {
                            movSpeedComponent.currentTrackSpline = i;
                            movSpeedComponent.dir = 1;
                        }
                    }
                }
                else
                {
                    movSpeedComponent.dir = 1;
                }
            }
            movSpeedComponent.next = next;

            if (next != -1)
            {
                for (int i = 0; i < someDynamicBuffer.Length; i++)
                {
                    if (next == someDynamicBuffer[i].id)
                    {
                        movSpeedComponent.lastTrackSpline = movSpeedComponent.currentTrackSpline;
                        movSpeedComponent.currentTrackSpline = i;

                    }
                }
            }
            else
            {
                movSpeedComponent.dir = -1;
            }
            CommandBuffer.RemoveComponent<AssignNewTarget_TEST>(index, entity);
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000));

        var job = new AssignNewTargetJob
        {
            random = random,
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            DeltaTime = Time.deltaTime,
            someDynamicBuffer = TrackSplineSpawner_TEST.myNativeArray,
        };
        //m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
        return job.Schedule(this, inputDependencies);
    }
}
