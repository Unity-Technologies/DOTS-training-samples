using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct ResourceBehaviourSystem : ISystem
{
    ComponentLookup<ResourceCarried> resourceCarriedLookup;
    ComponentLookup<ResourceDropped> resourceDroppedLookup;
    ComponentLookup<LocalTransform> resourceLookup;
    ComponentLookup<LocalTransform> beeTransformLookup;
    NativeArray<Entity> beeEntities;
    EntityQuery beeQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        resourceCarriedLookup = state.GetComponentLookup<ResourceCarried>();
        resourceDroppedLookup = state.GetComponentLookup<ResourceDropped>();
        resourceLookup = state.GetComponentLookup<LocalTransform>(false);
        beeTransformLookup = state.GetComponentLookup<LocalTransform>();
        beeQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<BeeState>().Build(ref state); 
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
        resourceCarriedLookup.Update(ref state);
        resourceDroppedLookup.Update(ref state);
        resourceLookup.Update(ref state);

        beeTransformLookup.Update(ref state);
        beeEntities = beeQuery.ToEntityArray(state.WorldUpdateAllocator);
        
        foreach (var (transform, resourceComponent, entity) in SystemAPI.Query<TransformAspect, RefRW<Resource>>().WithEntityAccess())
        {
            var topLeft = transform.WorldPosition - resourceComponent.ValueRO.boundsExtents;
            var bottomRight = transform.WorldPosition + resourceComponent.ValueRO.boundsExtents;
            
            for (int i = 0; i < beeEntities.Length; i++)
            {
                var beePosition = beeTransformLookup[beeEntities[i]].Position;

                // we also need a check if ResourceCarriedComponent is disabled here, but since nothing disables it yet
                // this check is left alone for now
                /*if (CheckBoundingBox(topLeft, bottomRight, beePosition))
                {
                    SystemAPI.SetComponentEnabled<ResourceCarried>(entity, true);
                    break;
                }
                if (SystemAPI.IsComponentEnabled<ResourceCarried>(entity))
                {
                    transform.WorldPosition = new float3(beePosition.x, beePosition.y - 0.5f, beePosition.z);
                    break;
                }*/
            }

            if (SystemAPI.IsComponentEnabled<ResourceDropped>(entity) && transform.WorldPosition.y > floorY)
            {
                resourceComponent.ValueRW.velocity += config.gravity * timeData.DeltaTime;
                transform.TranslateWorld(resourceComponent.ValueRW.velocity * timeData.DeltaTime);
            }
        }
    }
}