using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTSRATS
{
    [UpdateAfter(typeof(SetupSystem))]
    public class AnimalSpawnerSystem : SystemBase
    {
        EntityCommandBufferSystem CommandBufferSystem;

        protected override void OnCreate()
        {
            CommandBufferSystem
                = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
            RequireSingletonForUpdate<GameState>();
            RequireSingletonForUpdate<CellStruct>();
        }

        protected override void OnUpdate()
        {
            var gameStateEntity = GetSingletonEntity<GameState>();
            var cellStructs = GetBuffer<CellStruct>(gameStateEntity);
            var gameState = EntityManager.GetComponentData<GameState>(gameStateEntity);

            var deltaTime = Time.DeltaTime;

            //var ecb = new EntityCommandBuffer(Allocator.Temp);
            var ecb = CommandBufferSystem.CreateCommandBuffer();

            int numCats = GetEntityQuery(ComponentType.ReadOnly<Cat>(), ComponentType.ReadOnly<InPlay>(), ComponentType.ReadOnly<Velocity>()).CalculateEntityCount();
            int numRats = GetEntityQuery(ComponentType.ReadOnly<Rat>(), ComponentType.ReadOnly<InPlay>(), ComponentType.ReadOnly<Velocity>()).CalculateEntityCount();

            Entities
                .WithAll<InPlay, AnimalSpawner>()
                .WithReadOnly(cellStructs)
                .ForEach((Entity entity, ref AnimalSpawner animalSpawner, ref DirectionData direction, in Translation translation) =>
                {
                    var isRat = HasComponent<Rat>(entity);
                    var numAnimals = isRat ? numRats : numCats;
                    if (numAnimals < animalSpawner.maxAnimals && (animalSpawner.timeToNextSpawn -= deltaTime) < 0f)
                    {
                        animalSpawner.timeToNextSpawn = animalSpawner.spawnRate;
                        var instance = ecb.Instantiate(animalSpawner.animalPrefab);
                        ecb.SetComponent(instance, translation);

                        var cell = cellStructs[(int)translation.Value.z * gameState.boardSize + (int)translation.Value.x];
                        if (cell.wallLayout == (Direction.North | Direction.South | Direction.East | Direction.West))
                        {
                            direction.Value = Direction.None;
                        }
                        else if ((direction.Value & cell.wallLayout) != Direction.None)
                        {
                            var proposedDirection = Utils.RotateClockWise(direction.Value);
                            if ((proposedDirection & cell.wallLayout) != Direction.None)
                            {
                                proposedDirection = Utils.RotateCounterClockWise(direction.Value);
                                if ((proposedDirection & cell.wallLayout) != Direction.None)
                                    proposedDirection = Utils.RotateCounterClockWise(proposedDirection);
                            }
                            direction.Value = proposedDirection;
                        }

                        ecb.SetComponent(instance, new Rotation { Value = quaternion.LookRotationSafe(DirectionExt.ToFloat3(direction.Value), new float3(0f, 1f, 0f)) });

                        bool spawnSmall = SpawnBigOrSmall(ref animalSpawner.random);
                        ecb.AddComponent(instance, new Velocity { Direction = direction.Value, Speed = animalSpawner.initialSpeed[spawnSmall ? 1 : 0] });
                        ecb.AddComponent(instance, new Scale { Value = spawnSmall ? 1f : 1.5f });
                        ecb.SetComponent(instance, new Scaling { targetScale = spawnSmall ? 1f : 1.5f });
                    }
                }).Schedule();

            CommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        static bool SpawnBigOrSmall(ref Random random)
            => random.NextInt(10) >= 2;
    }
}
