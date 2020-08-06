using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Random = Unity.Mathematics.Random;

public struct PlantedSeedTag : IComponentData {}
public struct PlantGrowingTag : IComponentData {}
public struct ReadyForHarvest : IComponentData {}

[UpdateBefore(typeof(TransformSystemGroup))]
public class PlantSystem : SystemBase
{
    Plant[] bakedPlants = new Plant[10];
    Entity plantPrefab;
    Random  random;

    protected override void OnCreate()
    {
        // Generate a bunch of plants
        random = new Random(42);
        for (int i = 0; i < bakedPlants.Length; i++)
        {
            bakedPlants[i] = new Plant();
            bakedPlants[i].Init(0, 0, random.NextInt(-100, 100));
        }
    }

    protected override void OnUpdate()
    {
        if (plantPrefab == Entity.Null)
        {
            plantPrefab = GetEntityQuery(typeof(PlantAuthoring)).GetSingleton<PlantAuthoring>().plantPrefab;
            EntityManager.RemoveComponent<Scale>(plantPrefab);
            EntityManager.RemoveComponent<NonUniformScale>(plantPrefab);
            EntityManager.AddComponent<Size>(plantPrefab);
            EntityManager.AddComponent<PlantGrowingTag>(plantPrefab);
        }

        // Add plants from seeds
        Entities.WithStructuralChanges().WithAll<PlantedSeedTag>().ForEach((Entity entity, in Position2D position) =>
        {
            var newPlant = EntityManager.Instantiate(plantPrefab);

            EntityManager.SetComponentData<Translation>(newPlant, new Translation{ Value = new Unity.Mathematics.float3(position.position.x, 0, position.position.y)});
            EntityManager.SetComponentData<Size>(newPlant, new Size{ value = 0.1f });
            // TODO: set plant size, ect...
            var renderer = EntityManager.GetSharedComponentData<RenderMesh>(newPlant);

            renderer.mesh = bakedPlants[random.NextInt(0, bakedPlants.Length)].mesh;
            EntityManager.SetSharedComponentData(newPlant, renderer);

            EntityManager.RemoveComponent<PlantedSeedTag>(entity);
        }).Run();

        // Growing
        float t = Time.DeltaTime;
        Entities.WithAll<PlantGrowingTag>().ForEach((Entity entity, ref Size size) =>
        {
            size.value += t / 8.0f;
        }).ScheduleParallel();

        // Mark ready for harvest
        Entities.WithStructuralChanges().WithAll<PlantGrowingTag>().ForEach((Entity entity, in Size size) =>
        {
            if (size.value >= 1.0f)
            {
                EntityManager.AddComponent<ReadyForHarvest>(entity);
                EntityManager.RemoveComponent<PlantGrowingTag>(entity);
            }
        }).Run();
    }
}
