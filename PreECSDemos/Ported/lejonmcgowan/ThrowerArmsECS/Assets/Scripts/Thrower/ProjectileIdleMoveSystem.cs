using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class ProjectileIdleMoveSystem: JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithStructuralChanges().WithNone<GrabbedTag>().ForEach((Entity entity,ProjectileComponentData data) =>
        {
            Translation pos = EntityManager.GetComponentData<Translation>(entity);
            pos.Value.x += data.velocityX * Time.DeltaTime;
            EntityManager.SetComponentData(entity,pos);
            
            if(pos.Value.x > data.rangeXMax || pos.Value.x < data.rangeXMin)
            {
                if (EntityManager.HasComponent<ReserveComponentData>(entity))
                {
                    Entity arm = EntityManager.GetComponentData<ReserveComponentData>(entity).reserver;
                    var armBaseComponentData = EntityManager.GetComponentData<ArmBaseComponentData>(arm);
                    armBaseComponentData.grabT = 0.0f;
                    EntityManager.SetComponentData(arm,armBaseComponentData);
                    
                    EntityManager.RemoveComponent<ReserveComponentData>(arm);
                }
                
                EntityManager.DestroyEntity(entity);
            }

        }).Run();

        return inputDeps;
    }
}
