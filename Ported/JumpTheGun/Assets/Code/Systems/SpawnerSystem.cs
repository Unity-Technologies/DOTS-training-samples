using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SpawnerSystem : SystemBase
{
    private Random m_Random;
   
    protected override void OnCreate()
    {
        m_Random = new Random(0x1234567);
    }
    
    private void SpawnBoard()
    {
  
        Entities
            .WithStructuralChanges()
            .WithAll<BoardSpawnerTag>()
            .ForEach((Entity boardEntity, in BoardSpawnerData board) =>
        {
            EntityManager.AddComponentData(boardEntity, new BoardSize {Value = new int2(board.SizeX, board.SizeY)});
            EntityManager.AddComponent<OffsetList>(boardEntity);

            var playerPosition = new int2(board.SizeX >> 1, board.SizeY >> 1);;

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
            //buffer.AddRange(offsets.Reinterpret<OffsetList>());
            buffer.ResizeUninitialized(totalSize);
            for (int i = 0; i < totalSize; ++i)
                buffer[i] = new OffsetList {Value = offsets[i]};
            
            var tankMap = EntityManager.AddBuffer<TankMap>(boardEntity);
            //tankMap.AddRange(platforms.Reinterpret<TankMap>());
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

                    EntityManager.SetComponentData(instance, new URPMaterialPropertyBaseColor { Value = color });
                    EntityManager.SetComponentData(instance, new BoardPosition { Value = boardPosition });

                    EntityManager.RemoveComponent<Rotation>(instance);
                    
                    EntityManager.SetComponentData(instance, new LocalToWorld {Value = localToWorld});
                }
            }

            var random = new System.Random();
            
            for (int i = 0; i < platforms.Length; ++i)
            {
                if (platforms[i] == PlatformGenerator.PlatformType.Tank)
                {
                    int2 coords = CoordUtils.ToCoords(i, board.SizeX, board.SizeY);
                    Entity tank = EntityManager.Instantiate(board.TankPrefab);
                    EntityManager.SetComponentData(tank, new Translation {Value = new float3(coords.x, offsets[coords.y * board.SizeX + coords.x], coords.y)});
                    EntityManager.SetComponentData(tank, new TimeOffset {Value = math.fmod(m_Random.NextFloat(), 1f) * 5f});
                    
                    Entity turret = EntityManager.Instantiate(board.TurretPrefab);
                    EntityManager.SetComponentData(turret, new Translation {Value = new float3(coords.x, offsets[coords.y * board.SizeX + coords.x], coords.y)});
                }
            }

            EntityManager.RemoveComponent<BoardSpawnerTag>(boardEntity);
        }).Run();

    }


    private void SpawnPlayer()
    {
        Entity boardEntity;
        if (!TryGetSingletonEntity<BoardSize>(out boardEntity))
            return;

        var random = new System.Random();

        float3 targetPosition = new float3(0.0f, 0.0f, 0.0f);

        var boardSize = GetComponent<BoardSize>(boardEntity);
        var offsets = GetBuffer<OffsetList>(boardEntity);

        //TODO Randomize at the beginning.
        //startX = random.Next(0, boardEntity.SizeX);
        //startY = random.Next(0, boardEntity.SizeY);

        Entities
            .WithStructuralChanges()
            .WithReadOnly(offsets)
            .WithAll<PlayerSpawnerTag>()
            .ForEach((Entity player, ref NonUniformScale scale, in Radius radius) =>
            {
                int2 boardPos = new int2(boardSize.Value.x >> 1, boardSize.Value.y >> 1);
                float3 size = new float3(radius.Value * 2F, radius.Value * 2F, radius.Value * 2F);

                Camera mainCamera = Camera.main;


                float3 targetPos = CoordUtils.BoardPosToWorldPos(boardPos, offsets[CoordUtils.ToIndex(boardPos, boardSize.Value.x, boardSize.Value.y)].Value);

                float3 cameraPosition = new float3(targetPos.x, mainCamera.transform.position.y, targetPos.z);

                EntityManager.SetComponentData(player, new Translation { Value = targetPos });
                EntityManager.SetComponentData(player, new BoardPosition { Value = boardPos });
                EntityManager.SetComponentData(player, new NonUniformScale { Value = size });

                EntityManager.RemoveComponent<PlayerSpawnerTag>(player);

                mainCamera.transform.position = cameraPosition;
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
        SpawnPlayer();
        SpawnDebugEntities();
    }
}
