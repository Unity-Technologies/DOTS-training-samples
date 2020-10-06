using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public class CarSpawnerSystem : SystemBase
{
    private Random random;

    protected override void OnCreate()
    {
        random = new Random(0x1234567);
    }

    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in CarSpawner spawner, in Translation trans, in Rotation rotation, in SpawnerFrequency frequency) =>
            {
                float r = random.NextFloat(0, 1);
                
                /*
                if (r > frequency.Value)
                {
                    return; 
                }
                */
                
                var instance = EntityManager.Instantiate(spawner.CarPrefab);
                EntityManager.SetComponentData(instance, new Translation
                {
                    Value = trans.Value
                });

                /*
                float3 scale;
                int randInt = random.NextInt(0,3);
                switch (randInt){
                    case 0:
                        scale = spawner.carScaleV1;
                        break;
                    case 1:
                        scale = spawner.carScaleV2;
                        break;
                    case 2:
                        scale = spawner.carScaleV3;
                        break;
                    default;
                        break;
                }

                */
                EntityManager.SetComponentData(instance, new NonUniformScale
                {
                    Value = spawner.carScaleV1
                });

                EntityManager.SetComponentData(instance, new Rotation
                {
                    Value = rotation.Value
                });

                /*
                float4 col;
                randInt = random.NextInt(0, ???);
                col = 
                */

                EntityManager.SetComponentData(instance, new Color
                {
                    Value = new float4(1,0,0,1)
                    // Value = col;
                });
                // EntityManager.SetComponentData(instance, new 
                // {
                //     Value = 
                // });



                EntityManager.DestroyEntity(entity); // temp
            }).Run();
    }
}
