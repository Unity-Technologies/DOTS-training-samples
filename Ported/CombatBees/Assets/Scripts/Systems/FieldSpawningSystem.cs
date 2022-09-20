using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct FieldSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FieldConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<FieldConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var postTransformMatrix = new PostTransformMatrix
        {
            Value = float4x4.TRS(float3.zero, quaternion.identity,
                new float3(config.FieldScale.x, config.FieldScale.y, config.FieldScale.z))
        };

        var field = ecb.Instantiate(config.FieldMesh);
        ecb.AddComponent(field, postTransformMatrix);
        //TODO: Override "_HiveLocation" using MaterialPropertyOverride?
        //TODO: Add Gravity Component??
        
        //Run only once
        state.Enabled = false;
    }
}