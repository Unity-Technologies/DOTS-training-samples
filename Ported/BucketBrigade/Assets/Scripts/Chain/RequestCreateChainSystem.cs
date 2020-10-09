using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Random = Unity.Mathematics.Random;

public class RequestCreateChainSystem : SystemBase
{
    private EntityQuery waterQuery;

    protected override void OnCreate()
    {
        waterQuery = GetEntityQuery(ComponentType.ReadOnly<WaterTag>(), ComponentType.ReadOnly<Pos>());
        Random rand = new Random((uint)System.DateTime.UtcNow.Millisecond);
        var createChainEntity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(createChainEntity, new FindChainPosition() { Value = rand.NextFloat2() * 5 });
    }

    protected override void OnUpdate()
    {
        using (var ecb = new EntityCommandBuffer(Allocator.TempJob))
        {
            NativeArray<Pos> waterPositions = waterQuery.ToComponentDataArrayAsync<Pos>(Allocator.TempJob, out JobHandle j);
            Dependency = JobHandle.CombineDependencies(Dependency, j);

            Entities
                //.WithDisposeOnCompletion(waterPositions)
                .ForEach(
                (Entity entity, int entityInQueryIndex,
                            in ClosestFireRequest fire) =>
                {
                    if (fire.requestResultIndex > 0)
                    {
                        ecb.RemoveComponent<ClosestFireRequest>(entity);
                        Utility.GetNearestPos(fire.closestFirePosition, waterPositions, out int nearestIndex);
                        var chain = ecb.CreateEntity();
                        ecb.AddComponent<ChainCreateTag>(chain);
                        ecb.AddComponent(chain, new ChainStart() { Value = waterPositions[nearestIndex].Value });
                        ecb.AddComponent(chain, new ChainEnd() { Value = fire.closestFirePosition });
                        ecb.AddComponent(chain, new ChainLength() { Value = 4 });
                        ecb.AddComponent(chain, new ChainID() { Value = 0 }); //TODO fix this, need someway to increment chainID
                    }
                })
                .Run();

            waterPositions.Dispose();

            Entities.ForEach(
                (Entity entity, int entityInQueryIndex,
                            in FindChainPosition position) =>
                {
                    ecb.RemoveComponent<FindChainPosition>(entity);
                    var request = ecb.CreateEntity();
                    ecb.AddComponent(request, new ClosestFireRequest(position.Value));
                })
                .Run();
            ecb.Playback(EntityManager);
        }
    }
}
