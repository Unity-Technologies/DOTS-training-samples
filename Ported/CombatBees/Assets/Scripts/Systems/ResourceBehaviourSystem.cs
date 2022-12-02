using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct ResourceBehaviourSystem : ISystem
{
    private ComponentLookup<ResourceCarried> resourceCarriedLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        resourceCarriedLookup = state.GetComponentLookup<ResourceCarried>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (resource, entity) in SystemAPI.Query<RefRW<Resource>>().WithEntityAccess())
        {
            resource.ValueRW.carriedEnabled = SystemAPI.IsComponentEnabled<ResourceCarried>(entity);
            resource.ValueRW.droppedEnabled = SystemAPI.IsComponentEnabled<ResourceDropped>(entity);

            if (resource.ValueRO.ownerBee != Entity.Null && SystemAPI.Exists(resource.ValueRO.ownerBee))
            {
                resource.ValueRW.ownerPosition = SystemAPI.GetAspectRO<TransformAspect>(resource.ValueRO.ownerBee).WorldPosition;
                resource.ValueRW.ownerVelocity = SystemAPI.GetComponent<BeeState>(resource.ValueRO.ownerBee).velocity;
            }
        }
        
        var config = SystemAPI.GetSingleton<Config>();
        var timeData = state.WorldUnmanaged.Time;
        var halfFieldSize = config.fieldSize * .5f;
        var floorY = -1f * halfFieldSize.y;
        var up = new float3(0f, 1f, 0f);

        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var resourcePositionUpdateJob = new ResourcePositionUpdateJob
        {
            floorY = floorY,
            up = up,
            deltaTime = timeData.DeltaTime,
            fieldSize = config.fieldSize,
            carryStiffness = config.carryStiffness,
            gravity = config.gravity,
            ECB = ecb.AsParallelWriter()
        };

        resourcePositionUpdateJob.Schedule();
    }
}

[BurstCompile]
partial struct ResourcePositionUpdateJob : IJobEntity
{
    public float floorY;
    public float3 up;
    public float deltaTime;

    public float3 fieldSize;      // config.fieldSize
    public float carryStiffness;  // config.carryStiffness
    public float3 gravity;        // config.gravity
    
    public EntityCommandBuffer.ParallelWriter ECB;
    
    public void Execute(ref ResourceAspect resource)
    {
        if (resource.carriedEnabled && resource.carriedBee != Entity.Null)
        {
            var ownerTransform = resource.ownerPosition;//SystemAPI.GetAspectRO<TransformAspect>(resource.carriedBee);
            var ownerBeeVelocity = resource.ownerVelocity;//SystemAPI.GetComponent<BeeState>(resource.carriedBee);
            var targetPos = ownerTransform - (up * 5f);//ownerTransform.WorldPosition - (up * 5f);
            resource.Position = math.lerp(ownerTransform, targetPos, carryStiffness * deltaTime);
            resource.velocity = ownerBeeVelocity;
        }
        if (!resource.carriedEnabled && resource.Position.y > floorY)
        {
            resource.velocity += gravity * deltaTime;
            resource.Transform.TranslateWorld(resource.velocity * deltaTime);
        }
        
        var velocity = resource.velocity;
        var worldPosition = resource.Position;
        if (math.abs(worldPosition.x) > fieldSize.x * .5f) {
            worldPosition.x = (fieldSize.x * .5f) * math.sign(resource.Position.x);
            velocity.x *= -.5f;
            velocity.y *= .8f;
            velocity.z *= .8f;
        }
        if (math.abs(worldPosition.z) > fieldSize.z * .5f) {
            worldPosition.z = (fieldSize.z * .5f) * math.sign(resource.Position.z);
            velocity.z *= -.5f;
            velocity.x *= .8f;
            velocity.y *= .8f;
        }
        if (math.abs(worldPosition.y) > fieldSize.y * .5f) {
            worldPosition.y = (fieldSize.y * .5f) * math.sign(resource.Position.y);
            //velocity.y *= -.5f;
            velocity.z *= .8f;
            velocity.x *= .8f;
            resource.droppedEnabled = false;
            ECB.SetComponentEnabled<ResourceDropped>(0, resource.Self, false);
        }
        
        resource.Position = worldPosition;
        resource.Position += velocity * deltaTime;
        resource.velocity = velocity;
    }
}
