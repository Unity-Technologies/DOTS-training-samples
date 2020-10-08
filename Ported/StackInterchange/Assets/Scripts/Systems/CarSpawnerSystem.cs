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
        var deltaTime = Time.DeltaTime;
        bool spawned = false;
        
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, ref SpawnerFrequency frequency, in CarSpawner spawner, in RoadNode node, in LocalToWorld ltw, in Rotation rotation) =>
            {
                frequency.counter += deltaTime;
                if (frequency.counter < frequency.Value)
                {
                    return;
                }
                else
                {
                    frequency.counter -= frequency.Value;
                    frequency.Value = random.NextFloat(frequency.minWait, frequency.maxWait);
                }

                var instance = EntityManager.Instantiate(spawner.CarPrefab);
                EntityManager.SetComponentData(instance, new Translation
                {
                    Value = ltw.Position
                });

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
                    default:
                        scale = spawner.carScaleV1;
                        break;
                }

                EntityManager.SetComponentData(instance, new NonUniformScale
                {
                    Value = scale
                });

                EntityManager.SetComponentData(instance, new Rotation
                {
                    Value = rotation.Value
                });

                // Amanda temp
                float4 col;
                randInt = random.NextInt(0, 4);
                switch (randInt) {
                    case 0: // red
                        col = new float4(1,0,0,1);
                        break;
                    case 1: // green
                        col = new float4(0,1,0,1);
                        break;
                    case 2: // blue
                        col = new float4(0,0,1,1);
                        break;
                    case 3: // white
                        col = new float4(1,1,1,1);
                        break;
                    default:
                        col = new float4(0,0,0,1);
                        break;
                }

                EntityManager.SetComponentData(instance, new Color
                {
                    Value = col
                });

                // Add CarMovement component to spawned entity
                float maxSpeed = 0.07f + random.NextFloat(0.0f, 0.04f);
                EntityManager.AddComponentData(instance, new CarMovement
                {
                    NextNode = node.nextNode,
                    Velocity = maxSpeed,
                    Acceleration = 0.05f,
                    Deceleration = 0.05f,
                    MaxSpeed = maxSpeed 
                });
                
                 EntityManager.AddSharedComponentData(instance, new RoadId
                {
                    Value = spawner.spawnerId
                });

                spawned = true;

                // EntityManager.DestroyEntity(entity);

            }).Run();

        Entities
            .ForEach((Entity entity, ref SpawnCount spawnCount) =>
            {
                if (spawned) {
                    spawnCount.TotalCount += 1;
                }
                
            }).Run();
    }
}
