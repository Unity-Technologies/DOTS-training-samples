using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
[UpdateAfter(typeof(TrainMoverSystem))]
[UpdateBefore(typeof(DistanceAlongBezierSystem))]
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
        var job = new CarriageMoverJob { ComponentFromEntity = _componentFromEntity, Config = config };
        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct CarriageMoverJob : IJobEntity
{
    [ReadOnly]
    public ComponentDataFromEntity<DistanceAlongBezier> ComponentFromEntity;
    public Config Config;

    [BurstCompile]
    public void Execute(in Carriage carriage, ref DistanceAlongBezierBuffer position)
    {
        var train = ComponentFromEntity[carriage.Train];
        float carriageDistance = train.Distance - Config.TrainOffset - (Config.CarriageLength * carriage.CarriageIndex);
        position.Value = carriageDistance;
    }
}

