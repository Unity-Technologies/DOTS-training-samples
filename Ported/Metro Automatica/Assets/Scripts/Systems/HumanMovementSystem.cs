using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

[BurstCompile]
partial struct HumanMovementSystem : ISystem
{
    private EntityQuery humanQuery;
    private float startTime;
    public float speed;
    //public EntityCommandBuffer ec;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        humanQuery = SystemAPI.QueryBuilder().WithNone<HumanWaitForRouteTag, HumanInTrainTag>().WithAll<LocalTransform, Human>().Build();
        startTime = Time.time;
        speed = 0.01f;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var humanTransforms = humanQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        var humanData = humanQuery.ToComponentDataArray<Human>(Allocator.Temp);
        var humanEntities = humanQuery.ToEntityArray(Allocator.Temp);

        for (int i = 0; i < humanEntities.Length; i++)
        {
            float3 finalDestination = humanData[i].QueuePoint;
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float journeyLength = Vector3.Distance(humanTransforms[i].Position, finalDestination);
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            //var newTransform = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney); 

            var newTransform = LocalTransform.FromPosition(Vector3.Lerp(humanTransforms[i].Position, finalDestination, fractionOfJourney));
            ecb.SetComponent(humanEntities[i], newTransform);
            
        }
    }
}
