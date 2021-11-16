using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BeeSpawner : SystemBase
{
    public int totalBees = 100;

    protected override void OnUpdate()
    {
        var spawner = GetSingletonEntity<Spawner>();
        var spawnerComponent = GetComponent<Spawner>(spawner);

        var random = new Random(1234);

        Entities
            .WithAll<HiveTag>()
            .WithStructuralChanges()
            .ForEach((in AABB aabb, in TeamID teamID) => 
        {
            for (int i = 0; i < totalBees; ++i)
            {
                var entity = EntityManager.Instantiate(spawnerComponent.BeePrefab);

                var vel = math.normalize(random.NextFloat3Direction());
                EntityManager.AddComponentData<Velocity>(entity, new Velocity { Value = vel });
            }
        }).Run();
    }
}
