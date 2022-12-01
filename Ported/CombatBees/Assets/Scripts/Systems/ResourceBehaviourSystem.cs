using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct ResourceBehaviourSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var timeData = state.WorldUnmanaged.Time;
        var halfFieldSize = config.fieldSize * .5f;
        var floorY = -1f * halfFieldSize.y;
        var up = new float3(0f, 1f, 0f);
        
        foreach (var (transform, resource, entity) in SystemAPI.Query<TransformAspect, RefRW<Resource>>().WithEntityAccess())
        {
            if (SystemAPI.IsComponentEnabled<ResourceCarried>(entity) && resource.ValueRO.ownerBee != Entity.Null)
            {
                var ownerTransform = SystemAPI.GetAspectRO<TransformAspect>(resource.ValueRO.ownerBee);
                var ownerBeeState = SystemAPI.GetComponent<BeeState>(resource.ValueRO.ownerBee);
                var targetPos = ownerTransform.WorldPosition - (up * 5f);
                transform.WorldPosition = math.lerp(ownerTransform.WorldPosition, targetPos, config.carryStiffness * timeData.DeltaTime);
                resource.ValueRW.velocity = ownerBeeState.velocity;
            }

            if (!SystemAPI.IsComponentEnabled<ResourceCarried>(entity) && transform.WorldPosition.y > floorY)
            {
                resource.ValueRW.velocity += config.gravity * timeData.DeltaTime;
                transform.TranslateWorld(resource.ValueRW.velocity * timeData.DeltaTime);
            }

            var velocity = resource.ValueRW.velocity;
            var worldPosition = transform.WorldPosition;
            if (math.abs(worldPosition.x) > config.fieldSize.x * .5f) {
                worldPosition.x = (config.fieldSize.x * .5f) * math.sign(transform.WorldPosition.x);
                velocity.x *= -.5f;
                velocity.y *= .8f;
                velocity.z *= .8f;
            }
            if (math.abs(worldPosition.z) > config.fieldSize.z * .5f) {
                worldPosition.z = (config.fieldSize.z * .5f) * math.sign(worldPosition.z);
                velocity.z *= -.5f;
                velocity.x *= .8f;
                velocity.y *= .8f;
            }
            if (math.abs(worldPosition.y) > config.fieldSize.y * .5f) {
                worldPosition.y = (config.fieldSize.y * .5f) * math.sign(worldPosition.y);
                //velocity.y *= -.5f;
                velocity.z *= .8f;
                velocity.x *= .8f;
                SystemAPI.SetComponentEnabled<ResourceDropped>(entity, false);
            }
            
            transform.WorldPosition = worldPosition;
            transform.WorldPosition += velocity * timeData.DeltaTime;
            resource.ValueRW.velocity = velocity;
        }
    }
}