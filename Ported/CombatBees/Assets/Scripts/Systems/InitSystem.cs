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
        this.RequireSingletonForUpdate<HiveTeam0>();
        this.RequireSingletonForUpdate<HiveTeam1>();
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
        var hive0 = GetSingletonEntity<HiveTeam0>();
        var hive0Data = GetComponent<HiveTeam0>(hive0);
        var hive0Bounds = GetComponent<Bounds>(hive0);
        var hive0Spawner = EntityManager.CreateEntity();
        EntityManager.AddComponentData(hive0Spawner, new Spawner { Prefab = hive0Data.BeePrefab, SpawnPosition = hive0Bounds.Value.Center, Count = 10 });
        EntityManager.AddComponentData(hive0Spawner, new TeamID { Value = 0 });

        var hive1 = GetSingletonEntity<HiveTeam1>();
        var hive1Data = GetComponent<HiveTeam1>(hive1);
        var hive1Bounds = GetComponent<Bounds>(hive1);
        var hive1Spawner = EntityManager.CreateEntity();
        EntityManager.AddComponentData(hive1Spawner, new Spawner { Prefab = hive1Data.BeePrefab, SpawnPosition = hive1Bounds.Value.Center, Count = 10 });
        EntityManager.AddComponentData(hive1Spawner, new TeamID { Value = 1 });

        this.Enabled = false;
    }
}
