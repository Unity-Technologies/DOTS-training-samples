using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.VersionControl;

partial struct BeeClampingSystem : ISystem
{
    public void OnCreate(ref SystemState state) {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.TryGetSingleton<FieldConfig>(out var fieldConfig)) {
            return;
        }
        state.Dependency = new BeeClampingJob() {
            fieldBounds = fieldConfig.FieldScale,
            scaleLookup = state.GetComponentLookup<UniformScale>(),
            hasHolding = state.GetComponentLookup<IsHolding>()
        }.ScheduleParallel(state.Dependency);
    }
}