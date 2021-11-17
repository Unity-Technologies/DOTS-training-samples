using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial class BeeSpawner : SystemBase
{
    protected override void OnCreate()
    {
        this.RequireSingletonForUpdate<Spawner>();
    }
   
    // May run before scene is loaded
    protected override void OnUpdate()
    {
        var spawner = GetSingletonEntity<Spawner>();
        var spawnerComponent = GetComponent<Spawner>(spawner);

        var totalBees = spawnerComponent.BeeCount;

        var random = new Random(1234);

        Entities
            .WithAll<HiveTag>()
            .WithStructuralChanges()
            .ForEach((in AABB aabb, in TeamID teamID, in URPMaterialPropertyBaseColor urpColor) => 
        {
            for (int i = 0; i < totalBees; ++i)
            {
                var entity = EntityManager.Instantiate(spawnerComponent.BeePrefab);
                
                var vel = math.normalize(random.NextFloat3Direction());
                // Optimize by Setting the velocity instead of adding.
                EntityManager.AddComponentData(entity, new Velocity { Value = vel });
                EntityManager.AddComponentData(entity, new Bee());
                EntityManager.AddComponentData(entity, new BeeIdleMode());
                EntityManager.AddComponentData(entity, teamID);
                
                // Move bee to hive location
                EntityManager.SetComponentData<Translation>(entity, new Translation { Value = aabb.center });
                // Set bee color based off the hive
                URPMaterialPropertyBaseColor color = urpColor;
                color.Value.w = 1;
                EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(entity, color);

            }
        }).Run();

        this.Enabled = false;
    }
}
