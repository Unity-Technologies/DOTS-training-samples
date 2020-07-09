using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AutoFarmers
{
    public class SowingTestSpawnerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity, in SowingTestSpawner spawner) =>
                {
                    for (int f = 0; f < spawner.NumFarmers; ++f)
                    {
                        int2 gridDims = GetSingleton<Grid>().Size;
                        int randomX = UnityEngine.Random.Range(0, gridDims.x);
                        int randomY = UnityEngine.Random.Range(0, gridDims.y);
                        // Instantiate a spawner
                        var farmer = EntityManager.Instantiate(spawner.Farmer);
                        SetComponent(farmer, new Translation
                        {
                            Value = new float3(randomX, 0.5f, randomY)
                        });
                        EntityManager.AddComponent<Farmer_Tag>(farmer);
                        EntityManager.AddComponent<PlantSeeds_Intent>(farmer);
                    }


                    EntityManager.DestroyEntity(entity);
                }).Run();
        }
    }
}
