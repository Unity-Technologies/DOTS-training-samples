using src;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateAfter(typeof(TranslationFollowingSystem))]
public class CarriageVisualsSystem : JobComponentSystem
{
    EntityQuery m_CarriageQuery;

    protected override void OnCreate()
    {
        m_CarriageQuery = GetEntityQuery(
            ComponentType.ReadOnly(typeof(LinePosition)),
            ComponentType.ReadOnly(typeof(Translation)),
            ComponentType.ReadOnly(typeof(Rotation))
        );
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var getTranslation = GetComponentDataFromEntity<Translation>(true);
        var getRotation = GetComponentDataFromEntity<Rotation>(true);
        
        var carriagePositions = new NativeArray<Translation>(m_CarriageQuery.CalculateEntityCount(),Allocator.TempJob);
        var carriageRotation = new NativeArray<Rotation>(m_CarriageQuery.CalculateEntityCount(),Allocator.TempJob);
        
        var jobHandle = Entities
            .WithName("CarriageVisualsSystem_Fetch_Values")
            .WithReadOnly(getTranslation)
            .WithReadOnly(getRotation)
            .ForEach((int entityInQueryIndex, in CarriageVisuals carriageVisuals) =>
        {
            carriagePositions[entityInQueryIndex] = getTranslation[carriageVisuals.LogicalEntity];
            carriageRotation[entityInQueryIndex] = getRotation[carriageVisuals.LogicalEntity];
        }).Schedule(inputDeps);
        
        jobHandle = Entities
            .WithName("CarriageVisualsSystem_Set_Values")
            .ForEach((int entityInQueryIndex, ref Translation translation, ref Rotation rotation, in CarriageVisuals carriageVisuals) =>
        {
            translation = carriagePositions[entityInQueryIndex];
            rotation = carriageRotation[entityInQueryIndex];
        }).Schedule(jobHandle);

        carriagePositions.Dispose(jobHandle);
        carriageRotation.Dispose(jobHandle);
        return jobHandle;
    }
}