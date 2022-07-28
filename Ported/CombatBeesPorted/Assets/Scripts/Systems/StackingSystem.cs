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
    [ReadOnly] public ComponentDataFromEntity<Translation> GrabbableTranslation;
    [ReadOnly] public ComponentDataFromEntity<ResourceStateGrabbable> ResourceStateGrabbableComponentData;
    public float PlayVolumeFloor;
    public EntityCommandBuffer Ecb;
    void Execute(in Entity entity, in Translation trans)
    {
        if (trans.Value.y <= PlayVolumeFloor + 1)
        {
            Ecb.SetComponentEnabled<Gravity>(entity, false);
            Ecb.SetComponent(entity, new Velocity{Value = new Vector3(0,0,0) });
            Ecb.SetComponent(entity, new Translation{Value = 
                new float3(trans.Value.x, PlayVolumeFloor+1, trans.Value.z)});
            Ecb.SetComponentEnabled<ResourceStateGrabbable>(entity, true);
        }

        foreach (var grabbableResource in GrabbableResources)
        {
            if (ResourceStateGrabbableComponentData.IsComponentEnabled(grabbableResource))
            {
                // Debug.Log($"{GrabbableTranslation[grabbableResource].Value.y}");
                if (Math.Round(trans.Value.x, 1) == Math.Round(GrabbableTranslation[grabbableResource].Value.x, 1))
                {
                    if (Math.Round(trans.Value.z, 1) == Math.Round(GrabbableTranslation[grabbableResource].Value.z, 1))
                    {
                        if (trans.Value.y < GrabbableTranslation[grabbableResource].Value.y)
                        {
                            Ecb.SetComponent(entity, new Translation
                            {
                                Value =
                                    new float3(GrabbableTranslation[grabbableResource].Value.x,
                                        GrabbableTranslation[grabbableResource].Value.y + 2,
                                        GrabbableTranslation[grabbableResource].Value.z)
                            });
                            Ecb.SetComponentEnabled<ResourceStateGrabbable>(entity, true);
                            Ecb.SetComponentEnabled<Gravity>(entity, false);
                            Ecb.SetComponent(entity, new Velocity { Value = new Vector3(0, 0, 0) });
                            Ecb.SetComponentEnabled<ResourceStateGrabbable>(grabbableResource, false);
                            Ecb.SetComponentEnabled<ResourceStateStacked>(grabbableResource, true);
                            Ecb.SetComponent<ResourceBelow>(entity, new ResourceBelow { Value = grabbableResource });
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
    private ComponentDataFromEntity<Translation> _translationGrabbableResource;
    private ComponentDataFromEntity<ResourceStateGrabbable> _resourceStateGrabbableComponentData;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        _grabbableResourceQuery = state.GetEntityQuery(typeof(ResourceStateGrabbable));
        _translationGrabbableResource = state.GetComponentDataFromEntity<Translation>();
        _resourceStateGrabbableComponentData = state.GetComponentDataFromEntity<ResourceStateGrabbable>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playVolumeFloor = -SystemAPI.GetSingleton<Config>().PlayVolume.y;
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var grabbableResource = _grabbableResourceQuery.ToEntityArray(state.WorldUpdateAllocator);
        
        _resourceStateGrabbableComponentData.Update(ref state);
        _translationGrabbableResource.Update(ref state);

        new StackingJob
            {
                PlayVolumeFloor = playVolumeFloor, 
                Ecb = ecb,
                GrabbableResources = grabbableResource,
                GrabbableTranslation =_translationGrabbableResource,
                ResourceStateGrabbableComponentData = _resourceStateGrabbableComponentData,
            }.Schedule();
    }
} 