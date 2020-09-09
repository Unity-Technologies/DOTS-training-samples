using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in Spawner spawner, in LocalToWorld ltw) =>
            {
                for (int x = 0; x < spawner.CountX; ++x)
                for (int z = 0; z < spawner.CountZ; ++z)
                {
                    var posX = 2 * (x - (spawner.CountX - 1) / 2);
                    var posZ = 2 * (z - (spawner.CountZ - 1) / 2);
                    var instance = EntityManager.Instantiate(spawner.Prefab);
                    SetComponent(instance, new Translation
                    {
                        Value = ltw.Position + new float3(posX, 0, posZ)
                    });
                }

                EntityManager.DestroyEntity(entity);
            }).Run();
    }
}