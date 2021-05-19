using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class BeeSpawnerSystem : SystemBase
{
    private EntityCommandBufferSystem EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer();
        var random = Utils.GetRandom();

        var yellowBase = GetSingletonEntity<YellowBase>();
        var yellowBaseAABB = EntityManager.GetComponentData<Bounds>(yellowBase).Value;

        var blueBase = GetSingletonEntity<BlueBase>();
        var blueBaseAABB = EntityManager.GetComponentData<Bounds>(blueBase).Value;

        var arena = GetSingletonEntity<IsArena>();
        var arenaAABB = EntityManager.GetComponentData<Bounds>(arena).Value;

        var spawnerEntity = GetSingletonEntity<BeeSpawner>();
        var beeSpawner = GetComponent<BeeSpawner>(spawnerEntity);
        var numberOfBees = beeSpawner.BeeCountFromResource;

        Entities
            .WithAll<IsResource, OnCollision>()
            .WithNone<LifeSpan>()
            .ForEach((Entity entity, in Translation translation) =>
            {
                Entity? baseEntity = null;
                AABB bounds = blueBaseAABB;
                if (blueBaseAABB.Contains(translation.Value))
                {
                    baseEntity = blueBase;
                }
                else if (yellowBaseAABB.Contains(translation.Value))
                {
                    baseEntity = yellowBase;
                    bounds = yellowBaseAABB;
                }

                if (baseEntity.HasValue)
                {
                    for (int i = 0; i < numberOfBees; ++i)
                    {
                        var instance = ecb.Instantiate(beeSpawner.BeePrefab);
                        ecb.SetComponent<Team>(instance, GetComponent<Team>(baseEntity.Value));

                        var speed = random.NextFloat(0, beeSpawner.MaxSpeed);

                        float3 randomPointOnBase = Utils.BoundedRandomPosition(arenaAABB, ref random);

                        ecb.SetComponent(instance, new Velocity { Value = math.normalize(randomPointOnBase - translation.Value) * speed });
                        ecb.SetComponent(instance, new Speed { Value = speed });
                        ecb.SetComponent(instance, new Translation { Value = translation.Value });

                        ecb.SetComponent(instance, new URPMaterialPropertyBaseColor
                        {
                            Value = GetComponent<URPMaterialPropertyBaseColor>(baseEntity.Value).Value
                        });
                    }

                    //ecb.AddComponent<LifeSpan>(entity, new LifeSpan() { Value = 1f });
                }
            }).Schedule();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
