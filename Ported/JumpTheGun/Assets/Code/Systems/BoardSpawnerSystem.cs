using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BoardSpawnerSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
 
    protected override void OnCreate()
    {

        RequireSingletonForUpdate<BoardSpawnerTag>();

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    private void SpawnBoard()
    {
        //var ecb = m_ECBSystem.CreateCommandBuffer();
        var randomGenerator = new Random(0x1234567);

        var boardEntity = GetSingletonEntity<BoardSpawnerTag>();
        var board = GetSingleton<BoardSpawnerData>();

        
        EntityManager.SetComponentData(boardEntity, new BoardSize {Value = new int2(board.SizeX, board.SizeY)});

        var playerPosition = new int2(board.SizeX >> 1, board.SizeY >> 1);;

        EntityManager.SetComponentData(boardEntity, new NumberOfTanks {Count = board.NumberOfTanks});
        EntityManager.SetComponentData(boardEntity, new MinMaxHeight {Value = new float2(board.MinHeight, board.MaxHeight)});
        EntityManager.SetComponentData(boardEntity, new HitStrength {Value = board.HitStrength});

        EntityManager.SetComponentData(boardEntity, new ReloadTime {Value = board.ReloadTime});
        EntityManager.SetComponentData(boardEntity, new Radius {Value = board.Radius});
        EntityManager.SetComponentData(boardEntity, new BulletArcHeightFactor { Value = board.BulletArcHeightFactor });
            
        var totalSize = board.SizeX * board.SizeY;
            
        //var offsets = OffsetGenerator.CreateRandomOffsets(board.SizeX, board.SizeY, board.MinHeight, board.MaxHeight, randomGenerator, Allocator.Temp);
        //var tanks = PlatformGenerator.CreateTanks(board.SizeX, board.SizeY, playerPosition, board.NumberOfTanks, randomGenerator, Allocator.Temp);
        var boardPosition = new int2(0, 0);
            
        EntityManager.Instantiate(board.PlaformPrefab, totalSize, Allocator.Temp);

        var tanks = EntityManager.AddBuffer<TankMap>(boardEntity);
        var tanksCoord = PlatformGenerator.CreateTanks(board.SizeX, board.SizeY, playerPosition, board.NumberOfTanks, randomGenerator, ref tanks, Allocator.TempJob);

        int tankCount = board.NumberOfTanks;

        EntityManager.Instantiate(board.TankPrefab, tankCount, Allocator.Temp);
        EntityManager.Instantiate(board.TurretPrefab, tankCount, Allocator.Temp);

        var offsets = EntityManager.AddBuffer<OffsetList>(boardEntity);
        OffsetGenerator.CreateRandomOffsets(board.SizeX, board.SizeY, board.MinHeight, board.MaxHeight, randomGenerator, ref offsets);

        Entities
            .WithAll<Platform>()
            .WithReadOnly(offsets)
            .ForEach((int entityInQueryIndex, ref LocalToWorld matrix, ref URPMaterialPropertyBaseColor color, ref BoardPosition boardPos) =>
            {
                int2 indices = CoordUtils.ToCoords(entityInQueryIndex, board.SizeX, board.SizeY);

                matrix.Value = math.mul(
                    math.mul(
                        float4x4.Translate(new float3(indices.x, -0.5f, indices.y)),
                        float4x4.Scale(1f, offsets[indices.y * board.SizeX + indices.x].Value, 1f)),
                    float4x4.Translate(new float3(0f, 0.5f, 0f)));

                boardPos.Value = indices;

                color.Value = Colorize.Platform(offsets[indices.y * board.SizeX + indices.x].Value, board.MinHeight, board.MaxHeight);

            }).ScheduleParallel();
        
        Entities
            .WithAll<Tank>()
            .WithReadOnly(offsets)
            .ForEach((int entityInQueryIndex, ref Translation translation, ref TimeOffset offset) =>
            {
                int2 coords = tanksCoord[entityInQueryIndex];
                translation.Value = new float3(coords.x, offsets[coords.y * board.SizeX + coords.x].Value, coords.y);
                offset.Value = math.fmod(randomGenerator.NextFloat(), 1f) * 5f;

            }).ScheduleParallel();

        Entities
            .WithAll<Turret>()
            .WithReadOnly(offsets)
            .WithDisposeOnCompletion(tanksCoord)
            .ForEach((int entityInQueryIndex, ref Translation translation, ref TimeOffset color) =>
            {
                int2 coords = tanksCoord[entityInQueryIndex];
                translation.Value = new float3(coords.x, offsets[coords.y * board.SizeX + coords.x].Value, coords.y);
            }).ScheduleParallel();


        EntityManager.RemoveComponent<BoardSpawnerTag>(boardEntity);
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
