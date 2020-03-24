using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


public class ThrowerSeekSystem: JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityQuery projectileQuery = GetEntityQuery(typeof(ProjectileComponentData));
        var projEntitiies = projectileQuery.ToEntityArray(Allocator.TempJob);
        
        Entities.WithNone<ReserveComponentData>().WithStructuralChanges().ForEach((Entity entity, ArmBaseComponentData armBase) =>
        {
            
            
            Entity? closestEntity = null;
            float minDist = Single.PositiveInfinity;
            
            for (int i = 0; i < projEntitiies.Length; i++)
            {
                Entity proj = projEntitiies[i];
                
                if(!EntityManager.HasComponent<ReserveComponentData>(proj))
                {
                    float distance = math.distancesq(armBase.anchorPosition.Value,
                        EntityManager.GetComponentData<Translation>(proj).Value);

                    if (distance < minDist && distance < armBase.reach)
                    {
                        closestEntity = proj;
                        minDist = distance;
                    }
                }
            }

            if (closestEntity != null)
            {
                ReserveComponentData reserveProJ = closestEntity.Value;
                ReserveComponentData reserveArm = entity;
                
                EntityManager.AddComponentData(closestEntity.Value,reserveArm);
                armBase.target = EntityManager.GetComponentData<Translation>(closestEntity.Value);
                EntityManager.AddComponentData(entity,reserveProJ);
            }

        }).Run();

        projEntitiies.Dispose();
        
        return inputDeps;
    }
}
