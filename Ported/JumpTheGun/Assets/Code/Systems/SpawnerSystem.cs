using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SpawnerSystem : SystemBase
{
    private void SpawnBoard()
    {
        //var player = GetSingletonEntity<Player>();
        var startX = 0;
        var startY = 0;

        Entities
            .WithStructuralChanges()
            .WithAll<BoardSpawnerTag>()
            .ForEach((Entity boardEntity, in BoardSpawnerData board) =>
        {
            EntityManager.AddComponentData(boardEntity, new BoardSize {Value = new int2(board.SizeX, board.SizeY)});
            EntityManager.AddComponent<OffsetList>(boardEntity);

            var playerPosition = new int2(board.SizeX >> 1, board.SizeY >> 1);

            startX = board.SizeX;
            startY = board.SizeY;

            EntityManager.AddComponentData(boardEntity, new NumberOfTanks {Count = board.NumberOfTanks});
            EntityManager.AddComponentData(boardEntity, new MinMaxHeight {Value = new float2(board.MinHeight, board.MaxHeight)});
            EntityManager.AddComponentData(boardEntity, new HitStrength {Value = board.HitStrength});

            EntityManager.AddComponentData(boardEntity, new ReloadTime {Value = board.ReloadTime});
            EntityManager.AddComponentData(boardEntity, new Radius {Value = board.Radius});
            
            var totalSize = board.SizeX * board.SizeY;
            
            var offsets = OffsetGenerator.CreateRandomOffsets(board.SizeX, board.SizeY, board.MinHeight, board.MaxHeight, Allocator.Temp);
            var platforms = PlatformGenerator.CreatePlatforms(board.SizeX, board.SizeY, playerPosition, board.NumberOfTanks, Allocator.Temp);
            var boardPosition = new int2(0, 0);
            
            // TODO find a better way to do that.
            var buffer = EntityManager.AddBuffer<OffsetList>(boardEntity);
            buffer.ResizeUninitialized(totalSize);
            for (int i = 0; i < totalSize; ++i)
                buffer[i] = new OffsetList {Value = offsets[i]};

            var tankMap = EntityManager.AddBuffer<TankMap>(boardEntity);
            tankMap.ResizeUninitialized(totalSize);
            for (int i = 0; i < totalSize; ++i)
                tankMap[i] = new TankMap {Value = (platforms[i] == PlatformGenerator.PlatformType.Tank)}; 

            for (int y = 0; y < board.SizeY; ++y)
            {
                for (int x = 0; x < board.SizeX; ++x)
                {
                    var instance = EntityManager.Instantiate(board.PlaformPrefab);

                    var localToWorld = math.mul(
                        math.mul(
                            float4x4.Translate(new float3(x, -0.5f, y)),
                            float4x4.Scale(1f, offsets[y * board.SizeX + x], 1f)),
                        float4x4.Translate(new float3(0f, 0.5f, 0f)));

                    EntityManager.RemoveComponent<Translation>(instance);

                    

                    float4 color = Colorize.Platform(offsets[y * board.SizeX + x], board.MinHeight, board.MaxHeight);

                    boardPosition.x = x;
                    boardPosition.y = y;

                    float3 myPos = new float3(x, offsets[y * board.SizeX + x], y); 

                    EntityManager.AddComponentData(instance, new TargetPosition { Value = myPos });
                    EntityManager.SetComponentData(instance, new URPMaterialPropertyBaseColor { Value = color });
                    EntityManager.SetComponentData(instance, new BoardPosition { Value = boardPosition });

                    EntityManager.RemoveComponent<Rotation>(instance);
                    
                    EntityManager.SetComponentData(instance, new LocalToWorld {Value = localToWorld});
                }
            }

            for (int i = 0; i < platforms.Length; ++i)
            {
                if (platforms[i] == PlatformGenerator.PlatformType.Tank)
                {
                    int2 coords = CoordUtils.ToCoords(i, board.SizeX, board.SizeY);
                    Entity tank = EntityManager.Instantiate(board.TankPrefab);
                    EntityManager.SetComponentData(tank, new Translation {Value = new float3(coords.x, offsets[coords.y * board.SizeX + coords.x], coords.y)});

                    Entity turret = EntityManager.Instantiate(board.TurretPrefab);
                    EntityManager.SetComponentData(turret, new Translation {Value = new float3(coords.x, offsets[coords.y * board.SizeX + coords.x], coords.y)});
                }
            }

            EntityManager.RemoveComponent<BoardSpawnerTag>(boardEntity);
        }).Run();

        var player = GetSingletonEntity<Player>();
        var random = new System.Random();
        startX = random.Next(0, 8);
        startY = random.Next(0, 8);

        float3 targetPos = new float3(0.0f, 0.0f, 0.0f);


        Entities
        .WithAll<Platform>()
        .ForEach((ref BoardPosition boardPosition, ref TargetPosition translation) =>
        {

            if (boardPosition.Value.x == startX && boardPosition.Value.y == startY)
            {
                targetPos += translation.Value;
            }
        }).Run();

        SetComponent(player, new Translation { Value = targetPos });
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
