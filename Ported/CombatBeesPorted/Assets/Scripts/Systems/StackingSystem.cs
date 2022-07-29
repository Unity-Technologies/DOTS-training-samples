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

    public float PlayVolumeFloor;
    void Execute(Entity entity, ref Velocity velocity)
    {
        var resourceLocation = TranslationComponentData[entity].Value;
        if (resourceLocation.y <= PlayVolumeFloor + 1)
        {
            GravityComponentData.SetComponentEnabled(entity, false);
            velocity = new Velocity{ Value = new float3(0,0,0) };
            TranslationComponentData[entity] = new Translation{ Value = new float3(resourceLocation.x, PlayVolumeFloor + 1, resourceLocation.z)};
            ResourceStateGrabbableComponentData.SetComponentEnabled(entity, true);
        }
        else
        {
            foreach (var grabbableResource in GrabbableResources)
            {
                if (ResourceStateGrabbableComponentData.IsComponentEnabled(grabbableResource) 
                    && !ResourceStateGrabbableComponentData.IsComponentEnabled(entity))
                {
                    var translation = TranslationComponentData[grabbableResource].Value;
                    // Debug.Log($"{GrabbableTranslation[grabbableResource].Value.y}");
                    if (Math.Round(resourceLocation.x, 1) == Math.Round(translation.x, 1))
                    {
                        if (Math.Round(resourceLocation.z, 1) == Math.Round(translation.z, 1))
                        {
                            if (resourceLocation.y < (translation.y + 1))
                            {
                                TranslationComponentData[entity] = new Translation
                                {
                                    Value =
                                        new float3(translation.x,
                                            translation.y + 2,
                                            translation.z)
                                };
                                ResourceStateGrabbableComponentData.SetComponentEnabled(entity, true);
                                GravityComponentData.SetComponentEnabled(entity, false);
                                velocity = new Velocity { Value = new float3(0, 0, 0) };

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
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        _grabbableResourceQuery = state.GetEntityQuery(typeof(ResourceStateGrabbable));
        
        _translationComponentData = state.GetComponentDataFromEntity<Translation>();
        _resourceStateGrabbableComponentData = state.GetComponentDataFromEntity<ResourceStateGrabbable>();
        _resourceBelowComponentData= state.GetComponentDataFromEntity<ResourceBelow>();
        _resourceStateStackedComponentData= state.GetComponentDataFromEntity<ResourceStateStacked>();
        _gravityComponentData= state.GetComponentDataFromEntity<Gravity>();
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

        new StackingJob
            {
                PlayVolumeFloor = playVolumeFloor, 
                GrabbableResources = grabbableResource,
                ResourceStateGrabbableComponentData = _resourceStateGrabbableComponentData,
                ResourceStateStackedComponentData = _resourceStateStackedComponentData,
                TranslationComponentData = _translationComponentData,
                ResourceBelowComponentData = _resourceBelowComponentData,
                GravityComponentData = _gravityComponentData,
            }.Schedule();
    }
} 