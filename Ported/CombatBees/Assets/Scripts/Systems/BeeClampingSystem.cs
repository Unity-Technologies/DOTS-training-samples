using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.VersionControl;

partial struct BeeClampingSystem : ISystem
{
    private ComponentLookup<UniformScale> scaleLookup;
    private ComponentLookup<IsHolding> hasHolding;
    
    public void OnCreate(ref SystemState state) {
        scaleLookup = state.GetComponentLookup<UniformScale>();
        hasHolding = state.GetComponentLookup<IsHolding>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.TryGetSingleton<FieldConfig>(out var fieldConfig)) {
            return;
        }
        new BeeClampingJob() {
            fieldBounds = fieldConfig.FieldScale,
            scaleLookup = scaleLookup,
            hasHolding = hasHolding
        }.ScheduleParallel();
    }
}