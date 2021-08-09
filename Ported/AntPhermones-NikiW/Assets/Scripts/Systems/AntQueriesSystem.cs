using System;
using Unity.Entities;
using Unity.NetCode;

/// <summary>
///     Client & Server system that stores common queries.
/// </summary>
[UpdateInWorld(UpdateInWorld.TargetWorld.ClientAndServer)]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class AntQueriesSystem : SystemBase
{
    public EntityQuery antsHoldingFoodQuery;
    public EntityQuery antsQuery;
    public EntityQuery antsSearchingQuery;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        antsHoldingFoodQuery = GetEntityQuery(ComponentType.ReadOnly<AntSimulationTransform2D>(), ComponentType.ReadOnly<AntBehaviourFlag>(), ComponentType.ReadOnly<IsHoldingFoodFlag>());
        antsSearchingQuery = GetEntityQuery(ComponentType.ReadOnly<AntSimulationTransform2D>(), ComponentType.ReadOnly<AntBehaviourFlag>(), ComponentType.Exclude<IsHoldingFoodFlag>());
        antsQuery = GetEntityQuery(ComponentType.ReadOnly<AntBehaviourFlag>(), ComponentType.ReadOnly<AntSimulationTransform2D>(), ComponentType.ReadOnly<LifeTicks>());
    }

    protected override void OnUpdate()
    {
        Enabled = false;
    }
}
