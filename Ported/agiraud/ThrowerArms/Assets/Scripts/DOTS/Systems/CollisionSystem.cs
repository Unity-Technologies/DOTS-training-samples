using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
[UpdateAfter(typeof(PhysicsSystem))]
public class CollisionSystem : JobComponentSystem
{
    EntityQuery m_group;
    private EntityQuery m_RocksQuery;

    [BurstCompile]
    struct CollisionSystemJob : IJobForEachWithEntity<Physics, Translation>
    {
        [ReadOnly]
        public NativeArray<float3> rocks;

        public void Execute(Entity entity, int index, ref Physics physics, [ReadOnly] ref Translation translation)
        {
            if (physics.flying)
                return;

            float canDistance;
            float3 rockPos;
            NearestPosition(rocks, translation.Value, out canDistance, out rockPos);

            // TODO: expose radius
            if (canDistance < 1.0)
            {
                physics.flying = true;

                physics.velocity += math.normalize(translation.Value - rockPos) * 10;
                physics.angularVelocity += math.normalize(physics.velocity) * 2;
            }
        }

        void NearestPosition(NativeArray<float3> rocks, float3 position, out float nearestDistance, out float3 nearestPos)
        {
            nearestDistance = math.lengthsq(position - rocks[0]);
            nearestPos = rocks[0];

            for (int i = 1; i < rocks.Length; i++)
            {
                var targetPosition = rocks[i];
                var distance = math.lengthsq(position - targetPosition);
                var nearest = distance < nearestDistance;

                nearestDistance = math.select(nearestDistance, distance, nearest);
                nearestPos = math.select(nearestPos, targetPosition, nearest);
            }
            nearestDistance = math.sqrt(nearestDistance);
        }
    }

    [BurstCompile]
    struct CopyRocks : IJobForEachWithEntity<Translation, Physics>
    {
        public float3 boundsMin;
        public float3 boundsMax;
        public NativeHashMap<int, float3>.ParallelWriter hashMap;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref Physics physics)
        {
            bool inRange = physics.flying;
            inRange = inRange && (translation.Value.z > boundsMin.z);
            inRange = inRange && (translation.Value.z < boundsMax.z);

            if (inRange)
                hashMap.TryAdd(index, translation.Value);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var rockCount = m_RocksQuery.CalculateEntityCount();
        if (rockCount == 0)
            return inputDeps;

        var rocks = new NativeHashMap<int, float3>(rockCount, Allocator.TempJob);

        var copyJob = new CopyRocks
        {
            hashMap = rocks.AsParallelWriter(),

            // TODO: remove hardcoded values
            boundsMin = TinCanManagerAuthoring.SpawnBoxMin - new Vector3(1, 1, 1),
            boundsMax = TinCanManagerAuthoring.SpawnBoxMax + new Vector3(1, 1, 1)
        };

        var jobHandle = copyJob.Schedule(m_RocksQuery, inputDeps);
        jobHandle.Complete();

        var rocksNativeArray = rocks.GetValueArray(Allocator.TempJob);
        if (rocksNativeArray.Length > 0)
        {
            var job = new CollisionSystemJob()
            {
                rocks = rocksNativeArray
            };

            jobHandle = job.Schedule(m_group, jobHandle);
            jobHandle.Complete();
        }

        rocks.Dispose();
        rocksNativeArray.Dispose();

        return jobHandle;
    }

    protected override void OnCreate()
    {
        m_group = GetEntityQuery(ComponentType.ReadOnly<TinCanTag>(), ComponentType.ReadWrite<Physics>(), ComponentType.ReadOnly<Translation>());

        m_RocksQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<RockTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Physics>() },
        });
    }
}
