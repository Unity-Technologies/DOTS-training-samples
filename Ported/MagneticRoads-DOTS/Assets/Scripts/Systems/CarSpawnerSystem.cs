using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

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
        var ecb = new EntityCommandBuffer(Allocator.Persistent);
        var random = new Random(1234);
        
        //for each road, spawn some cars on it at regular interval
        //the first move will position them
        Entities
            .WithAll<SplineDef>().ForEach((Entity entity, in SplineDef spline) =>
        {
            for (var i = 0; i < carPerRoad; i++)
            {
                var instance = ecb.Instantiate(prefab);
                ecb.AddComponent(instance, new SplineDefForCar(spline));
                ecb.AddComponent(instance, new SplinePosition { position = (float)i / carPerRoad});
                ecb.AddComponent(instance, new Speed { speed = carSpeed });
                ecb.AddComponent(instance, new CurrentRoad { currentRoad = entity });
                
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
