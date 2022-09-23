using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.VersionControl;

[BurstCompile]
partial struct BeeClampingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<FieldConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var fieldConfig = SystemAPI.GetSingleton<FieldConfig>();
        state.Dependency = new BeeClampingJob() {
            fieldBounds = fieldConfig.FieldScale,
            scaleLookup = state.GetComponentLookup<UniformScale>(),
            targetIDLookup = state.GetComponentLookup<TargetId>()
        }.ScheduleParallel(state.Dependency);
    }
}