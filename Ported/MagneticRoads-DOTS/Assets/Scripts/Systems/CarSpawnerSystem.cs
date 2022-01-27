using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Random = Unity.Mathematics.Random;

public partial class CarSpawnerSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        Enabled = false;
    }

    protected override void OnUpdate()
    {
        var spawnerEntity = GetSingletonEntity<CarSpawner>();
        var carSpawnerComp = GetComponent<CarSpawner>(spawnerEntity);
        var prefab = carSpawnerComp.carPrefab;
        var carPerRoad = carSpawnerComp.carPerRoad;
        var carSpeed = carSpawnerComp.carSpeed;
        var carLength = carSpawnerComp.carLength;
        var ecb = new EntityCommandBuffer(Allocator.Persistent);
        var random = new Random(1234);
        
        var singleton = GetSingletonEntity<SplineDefArrayElement>();
        var splinesArray = GetBuffer<SplineDefArrayElement>(singleton);

        foreach (var spline in splinesArray)
        {
            var maxCarOnRoad = Math.Min(carPerRoad, spline.Value.measuredLength / carLength);
            for (var i = 0; i < maxCarOnRoad; i++)
            {
                var instance = ecb.Instantiate(prefab);
                ecb.AddComponent(instance, spline.Value);
                ecb.AddComponent(instance, new SplinePosition { position = i / maxCarOnRoad});
                ecb.AddComponent(instance, new Speed { speed = carSpeed });
                
                ecb.AddComponent(instance, new URPMaterialPropertyBaseColor
                {
                    Value = random.NextFloat4()
                });
            }
        }
        
        //for each road, spawn some cars on it at regular interval
        //the first move will position them
        Entities
            .WithAll<SplineDef>().ForEach((Entity entity, in SplineDef spline) =>
        {
            for (var i = 0; i < carPerRoad; i++)
            {
                var instance = ecb.Instantiate(prefab);
                ecb.AddComponent(instance, spline);
                ecb.AddComponent(instance, new SplinePosition { position = (float)i / carPerRoad});
                ecb.AddComponent(instance, new Speed { speed = carSpeed });
                
                ecb.AddComponent(instance, new URPMaterialPropertyBaseColor
                {
                    Value = random.NextFloat4()
                });
            }

        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();

        Enabled = false;
    }
}
