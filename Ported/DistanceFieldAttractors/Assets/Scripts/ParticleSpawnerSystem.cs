using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ParticleSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithStructuralChanges()
        .ForEach((Entity spawnerEntity, in ParticleSpawner spawnerData, in Translation translation) =>
        {
            var blankParticles = EntityManager.Instantiate(spawnerData.Prefab, spawnerData.SpawnCount, Allocator.Temp);
            var random = new Random((uint)spawnerEntity.Index);

            for(int i = 0; i < spawnerData.SpawnCount; i++)
            {
                EntityManager.SetComponentData(blankParticles[i], new Translation { Value = random.NextFloat3Direction() * 50f });
            }

            blankParticles.Dispose();
            EntityManager.DestroyEntity(spawnerEntity);
        }).Run();
    }
}
