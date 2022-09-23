using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.VersionControl;
using UnityEngine;

[BurstCompile]
partial struct BeeClampingSystem : ISystem
{
    private ComponentLookup<UniformScale> scaleLookup;
    private ComponentLookup<TargetId> targetIDLookup;
    
        
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<FieldConfig>();
        scaleLookup = state.GetComponentLookup<UniformScale>();
        targetIDLookup = state.GetComponentLookup<TargetId>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        scaleLookup.Update(ref state);
        targetIDLookup.Update(ref state);
        
        var fieldConfig = SystemAPI.GetSingleton<FieldConfig>();
        state.Dependency = new BeeClampingJob() {
            fieldBounds = fieldConfig.FieldScale,
            scaleLookup = scaleLookup,
            targetIDLookup = targetIDLookup
        }.ScheduleParallel(state.Dependency);
    }
}