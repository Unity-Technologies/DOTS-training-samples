using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
partial struct HiveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (hiveBees, hiveTeam) in SystemAPI.Query<DynamicBuffer<TargetBee>, Team>())
        {
            hiveBees.Clear();

            foreach (var (beeState, worldTransform, beeEntity) in SystemAPI.Query<RefRO<BeeState>, RefRO<WorldTransform>>().WithEntityAccess().WithSharedComponentFilter(hiveTeam))
            {
                if (beeState.ValueRO.beeState != BeeStateEnumerator.Dying)
                {
                    hiveBees.Add(new TargetBee
                    {
                        enemy = beeEntity,
                        position = worldTransform.ValueRO.Position
                    });
                }
            }
        }

        var resourceBuffer = SystemAPI.GetSingletonBuffer<AvailableResources>();
        // For resources, pretty much same as above, but without shared components
        foreach (var (worldTransform, entity) in SystemAPI.Query<RefRO<WorldTransform>>().WithAny<Resource>().WithNone<ResourceCarried, ResourceDropped>().WithEntityAccess())
        {
            resourceBuffer.Add(new AvailableResources()
            {
                resource = entity,
                position = worldTransform.ValueRO.Position
            });
        }
    }
}