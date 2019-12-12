using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using Random = Unity.Mathematics.Random;

struct Pair<T1, T2>
{
    public Pair(T1 _t1, T2 _t2) { t1 = _t1; t2 = _t2; }
    public T1 t1;
    public T2 t2;
}

public class CollisionSystem : JobComponentSystem
{
    private EntityQuery m_TinCanQuery;
    protected override void OnCreate()
    {
        base.OnCreate();
        m_TinCanQuery = GetEntityQuery(ComponentType.ReadOnly<TinCanComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var hashMap = new NativeMultiHashMap<int, Pair<Entity, float3> >(m_TinCanQuery.CalculateEntityCount()*2, Allocator.TempJob);


        var parallelHashMap = hashMap.AsParallelWriter();
        var hashPositionsJobHandle = Entities
            .WithName("TinCanPosition")
            .WithAll<TinCanComponent>()
            .ForEach((Entity entity, in Translation pos) =>
            {
                var hash = (int)math.hash(new int3(math.floor(pos.Value / 2f)));
                parallelHashMap.Add(hash, new Pair<Entity, float3>(entity, pos.Value));
            })
            .Schedule(inputDeps);

        EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        EntityCommandBuffer.Concurrent concurrentBuffer = entityCommandBuffer.ToConcurrent();
        Random random = new Random((uint)(Time.ElapsedTime*1000));

        var lastJobHandle = Entities
            .WithName("CollisionJob")
            .WithReadOnly(hashMap)
            .WithAll<RockComponent>()
            .ForEach((ref Velocity velocity, in Translation pos) =>
            {
                var hash = (int)math.hash(new int3(math.floor(pos.Value / 2f)));
                var tincans = hashMap.GetValuesForKey(hash);
                foreach(var tincan in tincans)
                {
                    if(distancesq(tincan.t2, pos.Value) < 0.5f*0.5f)
                    {
                        concurrentBuffer.AddComponent(0, tincan.t1, new FlyingState());
                        concurrentBuffer.AddComponent(0, tincan.t1, new Gravity());
                        concurrentBuffer.AddComponent(0, tincan.t1, new MarkedForDeath());
                        concurrentBuffer.SetComponent(0, tincan.t1, velocity);
                        concurrentBuffer.AddComponent(0, tincan.t1, new AngularVelocity() { Value = random.NextFloat3Direction() * length(velocity.Value) * 40f });
                        velocity.Value = random.NextFloat3Direction() * 3f;

                    }
                }

            }).Schedule(hashPositionsJobHandle);
        hashMap.Dispose(lastJobHandle);

        return lastJobHandle;
    }
}
