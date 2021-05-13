using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SpawnerSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
 
    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    private void SpawnBoard()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer();
        var randomGenerator = new Random(0x1234567);
 
        Entities
            .WithAll<BoardSpawnerTag>()
            .ForEach((
                Entity boardEntity,
                in BoardSpawnerData board) =>
        {
            ecb.SetComponent(boardEntity, new BoardSize {Value = new int2(board.SizeX, board.SizeY)});

            var playerPosition = new int2(board.SizeX >> 1, board.SizeY >> 1);;

            ecb.SetComponent(boardEntity, new NumberOfTanks {Count = board.NumberOfTanks});
            ecb.SetComponent(boardEntity, new MinMaxHeight {Value = new float2(board.MinHeight, board.MaxHeight)});
            ecb.SetComponent(boardEntity, new HitStrength {Value = board.HitStrength});

            ecb.SetComponent(boardEntity, new ReloadTime {Value = board.ReloadTime});
            ecb.SetComponent(boardEntity, new Radius {Value = board.Radius});
            ecb.SetComponent(boardEntity, new BulletArcHeightFactor { Value = board.BulletArcHeightFactor });
            
            var totalSize = board.SizeX * board.SizeY;
            
            var offsets = OffsetGenerator.CreateRandomOffsets(board.SizeX, board.SizeY, board.MinHeight, board.MaxHeight, randomGenerator, Allocator.Temp);
            var platforms = PlatformGenerator.CreatePlatforms(board.SizeX, board.SizeY, playerPosition, board.NumberOfTanks, randomGenerator, Allocator.Temp);
            var boardPosition = new int2(0, 0);
            
            // TODO find a better way to do that.

            var buffer = ecb.AddBuffer<OffsetList>(boardEntity);
            buffer.AddRange(offsets.Reinterpret<OffsetList>());
            //buffer.ResizeUninitialized(totalSize);
            //for (int i = 0; i < totalSize; ++i)
            //    buffer[i] = new OffsetList {Value = offsets[i]};
            
            var tankMap = ecb.AddBuffer<TankMap>(boardEntity);
            //tankMap.AddRange(platforms.Reinterpret<TankMap>());
            tankMap.ResizeUninitialized(totalSize);
            for (int i = 0; i < totalSize; ++i)
                tankMap[i] = new TankMap {Value = (platforms[i] == PlatformGenerator.PlatformType.Tank)}; 

            for (int y = 0; y < board.SizeY; ++y)
            {
                for (int x = 0; x < board.SizeX; ++x)
                {
                    var instance = ecb.Instantiate(board.PlaformPrefab);

                    var localToWorld = math.mul(
                        math.mul(
                            float4x4.Translate(new float3(x, -0.5f, y)),
                            float4x4.Scale(1f, offsets[y * board.SizeX + x], 1f)),
                        float4x4.Translate(new float3(0f, 0.5f, 0f)));

                    ecb.RemoveComponent<Translation>(instance);
                    ecb.RemoveComponent<Rotation>(instance);
                    
                    ecb.SetComponent(instance, new LocalToWorld {Value = localToWorld});
                    
                    boardPosition.x = x;
                    boardPosition.y = y;
                    
                    //float4 color = new float4((float)boardPosition.x / board.SizeX, (float)boardPosition.y / board.SizeY, 0f, 1f); 
                    float4 color = Colorize.Platform(offsets[y * board.SizeX + x], board.MinHeight, board.MaxHeight);
                    
                    ecb.SetComponent(instance, new URPMaterialPropertyBaseColor { Value = color });
                    ecb.SetComponent(instance, new BoardPosition { Value = boardPosition });
                }
            }

            for (int i = 0; i < platforms.Length; ++i)
            {
                if (platforms[i] == PlatformGenerator.PlatformType.Tank)
                {
                    int2 coords = CoordUtils.ToCoords(i, board.SizeX, board.SizeY);
                    Entity tank = ecb.Instantiate(board.TankPrefab);
                    ecb.SetComponent(tank, new Translation {Value = new float3(coords.x, offsets[coords.y * board.SizeX + coords.x], coords.y)});
                    ecb.SetComponent(tank, new TimeOffset {Value = math.fmod(randomGenerator.NextFloat(), 1f) * 5f});
                    
                    Entity turret = ecb.Instantiate(board.TurretPrefab);
                    ecb.SetComponent(turret, new Translation {Value = new float3(coords.x, offsets[coords.y * board.SizeX + coords.x], coords.y)});
                }
            }

            ecb.RemoveComponent<BoardSpawnerTag>(boardEntity);
        }).Run();
    }

    private void SpawnDebugEntities()
    {
        Entities
            .WithStructuralChanges()
            .WithAll<DebugParabolaSpawnerTag>()
            .ForEach((Entity debugEntity, in DebugParabolaData debugData) =>
        {
            for (int i = 0; i < debugData.SampleCount; ++i)
            {
                var sampleEntity = EntityManager.Instantiate(debugData.SamplePrefab);
                EntityManager.AddComponentData(sampleEntity, new DebugParabolaSampleTag { id = i } );
                BulletAuthoring.CreateBulletArchetype(sampleEntity, EntityManager);
            }

            EntityManager.RemoveComponent<DebugParabolaSpawnerTag>(debugEntity);
        }).Run();
    }

    protected override void OnUpdate()
    {
        SpawnBoard();
        SpawnDebugEntities();
    }
}
