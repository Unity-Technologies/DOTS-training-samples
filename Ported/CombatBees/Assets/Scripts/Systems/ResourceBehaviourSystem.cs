using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct ResourceBehaviourSystem : ISystem
{
    ComponentLookup<ResourceCarriedComponent> resourceCarriedLookup;
    ComponentLookup<ResourceDroppedComponent> resourceDroppedLookup;
    ComponentLookup<LocalTransform> resourceLookup;
    ComponentLookup<LocalTransform> beeTransformLookup;
    NativeArray<Entity> beeEntities;
    EntityQuery beeQuery;

    private const float floorY = -5f;
    private const float gravity = 0.01f;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        resourceCarriedLookup = state.GetComponentLookup<ResourceCarriedComponent>();
        resourceDroppedLookup = state.GetComponentLookup<ResourceDroppedComponent>();
        resourceLookup = state.GetComponentLookup<LocalTransform>(false);
        beeTransformLookup = state.GetComponentLookup<LocalTransform>();
        beeQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<BeeTempComponent>().Build(ref state); 
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        resourceCarriedLookup.Update(ref state);
        resourceDroppedLookup.Update(ref state);
        resourceLookup.Update(ref state);

        beeTransformLookup.Update(ref state);
        beeEntities = beeQuery.ToEntityArray(state.WorldUpdateAllocator);
        
        foreach (var (transform, resourceComponent, entity) in SystemAPI.Query<TransformAspect, RefRW<ResourceComponent>>().WithEntityAccess())
        {
            float3 topLeft = transform.Position - 0.2f;
            float3 bottomRight = transform.Position + 0.2f;
            
            for (int i = 0; i < beeEntities.Length; i++)
            {
                var beePosition = beeTransformLookup[beeEntities[i]].Position;

                // we also need a check if ResourceCarriedComponent is disabled here, but since nothing disables it yet
                // this check is left alone for now
                if (CheckBoundingBox(topLeft, bottomRight, beePosition))
                {
                    SystemAPI.SetComponentEnabled<ResourceCarriedComponent>(entity, true);
                }
                else if (SystemAPI.IsComponentEnabled<ResourceCarriedComponent>(entity))
                {
                    transform.Position = new float3(beePosition.x, beePosition.y - 0.5f, beePosition.z);
                }
                else
                {
                    transform.Position = GetFallingPos(transform.Position, floorY, gravity);
                }
            }

            if (beeEntities.Length == 0)
            {
                transform.Position = GetFallingPos(transform.Position, floorY, gravity);
            }
        }
    }
    
    bool CheckBoundingBox(float3 topLeft, float3 bottomRight, float3 beePosition)
    {
        return (topLeft.x <= beePosition.x && beePosition.x <= bottomRight.x
                                           && topLeft.z <= beePosition.x && beePosition.x <= bottomRight.z);
    }

    float3 GetFallingPos(float3 position, float floor, float gravity)
    {
        if (position.y > floor)
        {
            position = new float3(position.x, position.y - gravity /*fake gravity for now*/, position.z);
        }

        return position;
    }
}