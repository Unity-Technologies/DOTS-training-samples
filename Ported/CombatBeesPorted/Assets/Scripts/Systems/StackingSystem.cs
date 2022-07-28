using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[WithAll(typeof(Gravity), typeof(Resource))]
[BurstCompile]
partial struct StackingJob : IJobEntity
{
    [ReadOnly] public NativeArray<Entity> GrabbableResources;
    public ComponentDataFromEntity<Translation> TranslationComponentData;
    public ComponentDataFromEntity<ResourceStateGrabbable> ResourceStateGrabbableComponentData;
    public ComponentDataFromEntity<ResourceBelow> ResourceBelowComponentData;
    public ComponentDataFromEntity<ResourceStateStacked> ResourceStateStackedComponentData;
    public ComponentDataFromEntity<Gravity> GravityComponentData;
    public ComponentDataFromEntity<Velocity> VelocityComponentData;

    public float PlayVolumeFloor;
    void Execute(Entity entity, ref Translation trans)
    {
        if (trans.Value.y <= PlayVolumeFloor + 1)
        {
            GravityComponentData.SetComponentEnabled(entity, false);
            VelocityComponentData[entity] = new Velocity{Value = new float3(0,0,0) };
            TranslationComponentData[entity] = new Translation{Value = 
                new float3(trans.Value.x, PlayVolumeFloor + 1, trans.Value.z)};
            ResourceStateGrabbableComponentData.SetComponentEnabled(entity, true);
        }
        else
        {
            foreach (var grabbableResource in GrabbableResources)
            {
                if (ResourceStateGrabbableComponentData.IsComponentEnabled(grabbableResource) 
                    && !ResourceStateGrabbableComponentData.IsComponentEnabled(entity))
                {
                    var translation = TranslationComponentData[grabbableResource];
                    // Debug.Log($"{GrabbableTranslation[grabbableResource].Value.y}");
                    if (Math.Round(trans.Value.x, 1) == Math.Round(translation.Value.x, 1))
                    {
                        if (Math.Round(trans.Value.z, 1) == Math.Round(translation.Value.z, 1))
                        {
                            if (trans.Value.y < (translation.Value.y + 1))
                            {
                                trans = new Translation
                                {
                                    Value =
                                        new float3(translation.Value.x,
                                            translation.Value.y + 2,
                                            translation.Value.z)
                                };
                                ResourceStateGrabbableComponentData.SetComponentEnabled(entity, true);
                                GravityComponentData.SetComponentEnabled(entity, false);
                                VelocityComponentData[entity] = new Velocity { Value = new float3(0, 0, 0) };

                                ResourceStateGrabbableComponentData.SetComponentEnabled(grabbableResource, false);
                                ResourceStateStackedComponentData.SetComponentEnabled(grabbableResource, true);

                                ResourceBelowComponentData[entity] = new ResourceBelow { Value = grabbableResource };
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Iterate over resources and do some math logic. 
    }
}

[BurstCompile]
public partial struct StackingSystem : ISystem
{
    private EntityQuery _grabbableResourceQuery;
    private ComponentDataFromEntity<Translation> _translationComponentData;
    private ComponentDataFromEntity<ResourceStateGrabbable> _resourceStateGrabbableComponentData;
    private ComponentDataFromEntity<ResourceBelow> _resourceBelowComponentData;
    private ComponentDataFromEntity<ResourceStateStacked> _resourceStateStackedComponentData;
    private ComponentDataFromEntity<Gravity> _gravityComponentData;
    private ComponentDataFromEntity<Velocity> _velocityComponentData;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        _grabbableResourceQuery = state.GetEntityQuery(typeof(ResourceStateGrabbable));
        
        _translationComponentData = state.GetComponentDataFromEntity<Translation>();
        _resourceStateGrabbableComponentData = state.GetComponentDataFromEntity<ResourceStateGrabbable>();
        _resourceBelowComponentData= state.GetComponentDataFromEntity<ResourceBelow>();
        _resourceStateStackedComponentData= state.GetComponentDataFromEntity<ResourceStateStacked>();
        _gravityComponentData= state.GetComponentDataFromEntity<Gravity>();
        _velocityComponentData= state.GetComponentDataFromEntity<Velocity>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playVolumeFloor = -SystemAPI.GetSingleton<Config>().PlayVolume.y;
        var grabbableResource = _grabbableResourceQuery.ToEntityArray(state.WorldUpdateAllocator);
        
        _translationComponentData.Update(ref state);
        _resourceStateGrabbableComponentData.Update(ref state);
        _resourceBelowComponentData.Update(ref state);
        _resourceStateStackedComponentData.Update(ref state);
        _gravityComponentData.Update(ref state);
        _velocityComponentData.Update(ref state);

        new StackingJob
            {
                PlayVolumeFloor = playVolumeFloor, 
                GrabbableResources = grabbableResource,
                ResourceStateGrabbableComponentData = _resourceStateGrabbableComponentData,
                ResourceStateStackedComponentData = _resourceStateStackedComponentData,
                TranslationComponentData = _translationComponentData,
                ResourceBelowComponentData = _resourceBelowComponentData,
                GravityComponentData = _gravityComponentData,
                VelocityComponentData = _velocityComponentData,
            }.Run();
    }
} 