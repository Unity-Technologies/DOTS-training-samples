using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Random = Unity.Mathematics.Random;

public struct PlantedSeedTag : IComponentData {}

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
        }

        // Add plants from seeds
        Entities.WithStructuralChanges().WithAll<PlantedSeedTag>().ForEach((Entity entity, in Position2D position) =>
        {
            var newPlant = EntityManager.Instantiate(plantPrefab);

            EntityManager.SetComponentData<Translation>(newPlant, new Translation{ Value = new Unity.Mathematics.float3(position.position.x, 0, position.position.y)});
            EntityManager.SetComponentData<Size>(newPlant, new Size{ value = 1 });
            // TODO: set plant size, ect...
            var renderer = EntityManager.GetSharedComponentData<RenderMesh>(newPlant);

            renderer.mesh = bakedPlants[random.NextInt(0, bakedPlants.Length)].mesh;
            EntityManager.SetSharedComponentData(newPlant, renderer);

            EntityManager.RemoveComponent<PlantedSeedTag>(entity);
        }).Run();

        // TODO: system to increase plant size
    }
}
