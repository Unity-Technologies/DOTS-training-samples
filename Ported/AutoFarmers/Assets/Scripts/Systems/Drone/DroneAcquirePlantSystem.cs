using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
public partial struct DroneAcquirePlantSystem : ISystem
{
    EntityQuery siloQuery;

    ComponentDataFromEntity<LocalToWorld> localToWorldFromEntity;

    public void OnCreate(ref SystemState state)
    {
        siloQuery = state.World.EntityManager.CreateEntityQuery(typeof(Silo));
        localToWorldFromEntity = state.GetComponentDataFromEntity<LocalToWorld>(true);
    }


    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var chunks = siloQuery.CreateArchetypeChunkArray(Allocator.Temp);
        localToWorldFromEntity.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var drone in SystemAPI.Query<DroneGettingPlantAspect>())
        {
            if (drone.AtDesiredLocation && drone.HasPlant)
            {
                ecb.AddComponent(drone.Plant, new Follow
                {
                    EntityToFollow = drone.Self,
                    Offset = new float3(0, 1, 0)
                });

                ecb.RemoveComponent<DroneAquirePlantIntent>(drone.Self);

                ecb.AddComponent(drone.Self, new DroneDepositPlantIntent
                {
                    Plant = drone.Plant
                });

                var dronePos = localToWorldFromEntity[drone.Self].Position;
                var closestSiloDistance = float.MaxValue;
                var closestSiloPos = new float3(0, 0, 0);
                for (int i = 0; i < chunks.Length; i++)
                {
                    var chunk = chunks[i];
                    var silos = chunk.GetNativeArray(state.GetEntityTypeHandle());
                    for (int j = 0; j < chunk.Count; j++)
                    {
                        var silo = silos[j];
                        var siloPos = localToWorldFromEntity[silo].Position;
                       
                        var dist = math.distancesq(siloPos, dronePos);
                        if (dist < closestSiloDistance)
                        {
                            closestSiloDistance = dist;
                            closestSiloPos = siloPos;
                        }
                    }
                    silos.Dispose();
                }

                drone.DesiredLocation = new int2(
                    UnityEngine.Mathf.RoundToInt(closestSiloPos.x),
                    UnityEngine.Mathf.RoundToInt(closestSiloPos.z)
                );
            }
        }

    }
}
