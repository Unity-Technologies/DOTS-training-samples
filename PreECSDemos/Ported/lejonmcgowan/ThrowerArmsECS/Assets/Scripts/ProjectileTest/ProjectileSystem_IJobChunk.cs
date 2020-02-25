using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ProjectileSystem: JobComponentSystem
{
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float dt = Time.DeltaTime;
        
        Entities.
            WithStructuralChanges().
            ForEach(
                (Entity entity, ref Translation translation,ref ProjectileComponentData projectile) =>
                {
                    float3 vel = projectile.Velocity;

                    translation = new Translation
                    {
                        Value = translation.Value + vel * dt
                    };
                
                    projectile.TimeLeft -= dt;

                    if (projectile.TimeLeft <= 0.0f)
                    {
                        EntityManager.DestroyEntity(entity);
                    }
                
                }).Run();

        return inputDeps;
    }
}