using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


public class BrigadeFindSourceSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    EntityQuery resourceQuery;
    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        resourceQuery = GetEntityQuery(new EntityQueryDesc() {
            All = new[] 
            {
                ComponentType.ReadOnly<Resource>(),
                ComponentType.ReadOnly<Translation>()
            },
            None = new[]
            {
                ComponentType.ReadOnly<ResourceClaimed>(),
            }
        });
    }

    protected override void OnUpdate()
    {
        var resourcePositions = resourceQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var resourcePosQueryHandle);
        var resourceEntities = resourceQuery.ToEntityArrayAsync(Allocator.TempJob, out var resourceEntityJobHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, resourcePosQueryHandle, resourceEntityJobHandle);

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        Entities
            .WithNone<ResourceSourcePosition>()
            .WithDeallocateOnJobCompletion(resourcePositions)
            .WithDeallocateOnJobCompletion(resourceEntities)
            .ForEach((int entityInQueryIndex, Entity e, in BrigadeLine line) =>
            {
                var bestDist = float.MaxValue;
                var bestPos = float2.zero;
                var bestIndex = -1;
                for (int i = 0; i < resourcePositions.Length; i++)
                {
                    var rp = resourcePositions[i].Value;
                    var d2 = math.distancesq(new float2(rp.x, rp.z), line.Center);
                    if (d2 < bestDist)
                    {
                        bestIndex = i;
                        bestDist = d2;
                        bestPos = new float2(rp.x, rp.z);
                    }
                }
                if (bestIndex >= 0)
                {
                    //   UnityEngine.Debug.Log("Found source!!!");
                    ecb.RemoveComponent<BrigadeLineEstablished>(entityInQueryIndex, e);
                    ecb.AddComponent(entityInQueryIndex, e, new ResourceSourcePosition() { Value = bestPos, Id = resourceEntities[bestIndex]});
                    ecb.AddComponent<ResourceClaimed>(entityInQueryIndex, resourceEntities[bestIndex]);
                }
            }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}


