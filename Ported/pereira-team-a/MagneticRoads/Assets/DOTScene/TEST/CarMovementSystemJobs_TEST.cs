
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static TrackSplineSpawner_TEST;

public class CarMovementSystemJobs_TEST : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }


    //[BurstCompile]
    struct TranslationJob : IJobForEachWithEntity<Translation, Rotation, MovementSpeedComponent_TEST>
    {
        [ReadOnly]
        public float DeltaTime;

        [ReadOnly]
        public NativeArray<TrackSplineDOTS_TEST> someDynamicBuffer;

        [ReadOnly]
        public Unity.Mathematics.Random random;

        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index, ref Translation position, ref Rotation rotation, ref MovementSpeedComponent_TEST movSpeedComponent)
        {
            TrackSplineDOTS_TEST currentTrack = someDynamicBuffer[movSpeedComponent.currentTrackSpline];

            if (!movSpeedComponent.init)
            {
                position.Value = currentTrack.startPoint;
                movSpeedComponent.init = true;
            }

            movSpeedComponent.forward = (currentTrack.endPoint - currentTrack.startPoint);
            position.Value = position.Value + movSpeedComponent.forward * movSpeedComponent.speed * movSpeedComponent.dir * DeltaTime;
            if (movSpeedComponent.dir == 1)
            {
                if (math.distance(position.Value, currentTrack.endPoint) < 0.1f)
                {
                    CommandBuffer.AddComponent<AssignNewTarget_TEST>(index, entity);
                }
            }
            else
            {
                if (math.distance(position.Value, currentTrack.startPoint) < 0.1f)
                {
                    CommandBuffer.AddComponent<AssignNewTarget_TEST>(index, entity);
                }
            }
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000));
        var job = new TranslationJob
        {
            random = random,
            DeltaTime = Time.deltaTime,
            someDynamicBuffer = TrackSplineSpawner_TEST.myNativeArray,
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
        };

        return job.Schedule(this, inputDependencies);

    }
}
