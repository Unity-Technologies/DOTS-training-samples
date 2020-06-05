using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

class SpawnerSystem : SystemBase
{
    private EntityCommandBufferSystem commandBufferSystem;
    Entity randomEntity;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        randomEntity = EntityManager.CreateEntity(typeof(RandomComponent));
        EntityManager.SetComponentData(randomEntity, new RandomComponent { Value = new Random((uint)System.DateTime.UtcNow.Ticks) });
    }
    protected override void OnUpdate()
    {
        var gameModeComponent = GetSingleton<GameModeComponent>();
        if (gameModeComponent.Value != GameMode.GamePlay)
        {
            return;
        }

        var ecb = commandBufferSystem.CreateCommandBuffer().ToConcurrent();

        RandomComponent random = GetSingleton<RandomComponent>();
        Entity tmpRandomEntity = randomEntity;

        float dt = (Time.DeltaTime * ConstantData.Instance.SpawnerFrequencyMultiplier);

        Entities.ForEach((int entityInQueryIndex, Entity entity, ref SpawnerInstance instance, in SpawnerInfo info, in Position2D position, in Direction2D direction) =>
        {
            instance.Time += dt;
            instance.AlternateSpawnTime += dt;
            Entity toSpawn = Entity.Null;

            if (info.AlternateSpawnMinFrequency != 0 & info.AlternateSpawnMaxFrequency != 0 && instance.CurrentAlternateSpawnFrenquency == 0)
            {
                instance.CurrentAlternateSpawnFrenquency = random.Value.NextFloat(info.AlternateSpawnMinFrequency, info.AlternateSpawnMaxFrequency) + instance.Time;
                instance.CurrentAlternateSpawnFrenquency += info.Frequency - instance.CurrentAlternateSpawnFrenquency % info.Frequency;
            }

            float walkSpeed = 0f;
            float rotationSpeed = 0f;
            float scale = 0f;
            if (instance.CurrentAlternateSpawnFrenquency != 0 && instance.AlternateSpawnTime >= instance.CurrentAlternateSpawnFrenquency)
            {
                instance.AlternateSpawnTime -= instance.CurrentAlternateSpawnFrenquency;
                instance.CurrentAlternateSpawnFrenquency = 0;
                instance.Time = 0; // Reset the default spawner time so it doesn't spawn randomly another entity straight away
                toSpawn = ecb.Instantiate(entityInQueryIndex, info.AlternatePrefab);
                walkSpeed = random.Value.NextFloat(info.AlternateWalkSpeed.x, info.AlternateWalkSpeed.y);
                scale = random.Value.NextFloat(info.AlternateScale.x, info.AlternateScale.y);
                rotationSpeed = info.AlternateRotationSpeed;
            }
            else if (info.Frequency != 0 && instance.Time >= info.Frequency)
            {
                instance.Time -= info.Frequency;
                toSpawn = ecb.Instantiate(entityInQueryIndex, info.Prefab);
                walkSpeed = random.Value.NextFloat(info.WalkSpeed.x, info.WalkSpeed.y);
                scale = random.Value.NextFloat(info.Scale.x, info.Scale.y);
                rotationSpeed = info.RotationSpeed;
                instance.TotalSpawned++;
            }

            if (toSpawn != Entity.Null)
            {
                ecb.SetComponent(entityInQueryIndex, toSpawn, new Position2D { Value = position.Value });
                ecb.SetComponent(entityInQueryIndex, toSpawn, new Direction2D { Value = direction.Value });
                ecb.SetComponent(entityInQueryIndex, toSpawn, new Rotation2D { Value = Utility.DirectionToAngle(direction.Value) });
                ecb.SetComponent(entityInQueryIndex, toSpawn, new WalkSpeed { Value = walkSpeed, RotationSpeed = rotationSpeed });
                ecb.AddComponent(entityInQueryIndex, toSpawn, new NonUniformScale { Value = new float3(scale) });

            }
            float t = random.Value.NextFloat();
            ecb.SetComponent(entityInQueryIndex, tmpRandomEntity, random);

            if (info.MaxSpawns > 0 && instance.TotalSpawned >= info.MaxSpawns)
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            }
        })
        .WithName("UpdateSpawners")
        .Schedule();

        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}