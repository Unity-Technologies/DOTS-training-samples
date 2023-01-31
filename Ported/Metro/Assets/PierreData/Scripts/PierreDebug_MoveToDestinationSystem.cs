using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct PierreDebug_MoveToDestinationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var destinationAspect in SystemAPI.Query<DestinationAspect>().WithAll<Commuter>())
        {
            if (!destinationAspect.IsAtDestination())
            {
                float3 direction = math.normalize(destinationAspect.target.ValueRO.TargetPosition - destinationAspect.transform.WorldPosition);
                float3 move = 2 * direction * SystemAPI.Time.DeltaTime;

                float distToGoSq = math.distancesq(destinationAspect.target.ValueRO.TargetPosition, destinationAspect.transform.WorldPosition);
                if (distToGoSq < math.lengthsq(move))
                {
                    move = distToGoSq * direction;
                }
        
                destinationAspect.transform.WorldPosition += move;
            }
        }
    }
}
