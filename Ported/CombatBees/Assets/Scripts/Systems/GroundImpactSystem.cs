using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[WithAll(typeof(Decay))]
[WithNone(typeof(BlueTeam),typeof(YellowTeam))]
partial struct ResourceImpactJob : IJobEntity
{
    public EntityCommandBuffer ECB;
    
    public float3 FieldExtents;
    
    void Execute(Entity resource, in TransformAspect prs)
    {}
}

[WithAll(typeof(Decay))]
[WithAny(typeof(YellowTeam), typeof(BlueTeam))]
partial struct BeeImpactJob : IJobEntity
{
    public float3 FieldExtents;
    
    void Execute(Entity bee)
    {}
}

partial struct GroundImpactSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceConfig>();
        state.RequireForUpdate<FieldConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        var resourceConfig = SystemAPI.GetSingleton<ResourceConfig>();
        var fieldConfig = SystemAPI.GetSingleton<FieldConfig>();
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        new BeeImpactJob { FieldExtents = fieldConfig.FieldScale }.Schedule();
        new ResourceImpactJob { FieldExtents = fieldConfig.FieldScale }.Schedule();
    }
}