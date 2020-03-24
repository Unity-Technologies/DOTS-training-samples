using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class TargetIdleMoveSystem: JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithStructuralChanges().ForEach((Entity entity,TargetComponentData data) =>
        {
            Translation pos = EntityManager.GetComponentData<Translation>(entity);
            pos.Value.x += data.velocityX * Time.DeltaTime;
            EntityManager.SetComponentData(entity,pos);
            
            if(pos.Value.x > data.rangeXMax || pos.Value.x < data.rangeXMin)
                EntityManager.DestroyEntity(entity);

        }).Run();

        return inputDeps;
    }
}
