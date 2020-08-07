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
            var random = new Random(1);
            random.InitState();

            for(int i = 0; i < spawnerData.SpawnCount; i++)
            {
                var randomPointInUnitSphere = (random.NextFloat3Direction()) * math.pow(random.NextFloat(), 1f / 3f);
                EntityManager.SetComponentData(blankParticles[i], new Translation { Value = randomPointInUnitSphere * 50f });
            }

            blankParticles.Dispose();
            EntityManager.DestroyEntity(spawnerEntity);
        }).Run();
    }
}
