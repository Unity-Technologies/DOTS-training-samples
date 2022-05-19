using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(TrainMoverSystem))]
partial struct CarriageMover : ISystem
{
    private ComponentDataFromEntity<DistanceAlongBezier> _componentFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        _componentFromEntity = state.GetComponentDataFromEntity<DistanceAlongBezier>(false);
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Config config = SystemAPI.GetSingleton<Config>();
        _componentFromEntity.Update(ref state);
        foreach (var (carriage, position) in SystemAPI.Query<RefRO<Carriage>, RefRW<DistanceAlongBezier>>())
        {
            //var train = SystemAPI.GetComponent<DistanceAlongBezier>(carriage.ValueRO.Train);
            var train = _componentFromEntity[carriage.ValueRO.Train];
            float carriageDistance = train.Distance - config.TrainOffset - (config.CarriageLength * carriage.ValueRO.CarriageIndex);
            position.ValueRW.Distance = carriageDistance;

        }
    }
}

