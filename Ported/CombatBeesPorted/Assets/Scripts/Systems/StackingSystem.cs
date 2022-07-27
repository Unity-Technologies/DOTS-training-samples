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
            Ecb.SetComponentEnabled<ResourceStateGrabbable>(entity, false);
        }
        // Iterate over resources and do some math logic. 
    }
}

[BurstCompile]
public partial struct StackingSystem : ISystem
{
    private EntityQuery _grabbableResourceQuery;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        
        _grabbableResourceQuery = state.GetEntityQuery(typeof(ResourceStateGrabbable));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var playVolumeFloor = -SystemAPI.GetSingleton<Config>().PlayVolume.y;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var grabbableResource = _grabbableResourceQuery.ToEntityArray(state.WorldUpdateAllocator);
    // TODO: Parallel schedule please. 
        new StackingJob
            {
                PlayVolumeFloor = playVolumeFloor, 
                Ecb = ecb,
                GrabbableResources = grabbableResource
            }.Schedule();
    }
} 