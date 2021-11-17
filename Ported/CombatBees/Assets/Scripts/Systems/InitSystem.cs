using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Rendering;
using UnityEditorInternal;

public partial class InitSystem : SystemBase
{
    protected override void OnCreate()
    {
        this.RequireSingletonForUpdate<Globals>();
    }

    protected override void OnUpdate()
    {
        var globals = GetSingletonEntity<Globals>();
        var globalsComponent = GetComponent<Globals>(globals);

        var random = new Random(1234);
        int foodCount = 100;

        // Spawn some food
        float3 min = new float3(-5, 5, -5);
        float3 max = new float3(5, 5, 5);
        for (int i = 0; i < foodCount; ++i)
        {
            var entity = EntityManager.Instantiate(globalsComponent.FoodPrefab);
            EntityManager.AddComponentData(entity, new Gravity());
            EntityManager.AddComponentData(entity, new Velocity { Value = new float3(0, 0, 0) });

            EntityManager.SetComponentData(entity, new Translation
            {
                Value = random.NextFloat3(min, max)
            });
        }

        // Create initial spawners
        Entities
            .WithAll<HiveTag>()
            .WithStructuralChanges()
            .ForEach((in Bounds aabb, in TeamID teamID) =>
        {
            var spawnEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(spawnEntity, new Spawner { SpawnPosition = aabb.Value.Center, Count = 10 });
            EntityManager.AddComponentData(spawnEntity, new TeamID { Value = teamID.Value });
        }).Run();

        this.Enabled = false;
    }
}
