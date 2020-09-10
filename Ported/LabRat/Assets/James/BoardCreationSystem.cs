using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(PlayerInitializationSystem))]
public class BoardCreationSystem : SystemBase
{
    private PlayerInitializationSystem playerInitSystem;
    struct BoardVisualElement : ISystemStateComponentData {}

    EntityQuery m_AnyTileOrWallQuery;
    
    protected override void OnCreate()
    {
        m_AnyTileOrWallQuery = GetEntityQuery(new EntityQueryDesc {Any = new[] {ComponentType.ReadOnly<Tile>(), ComponentType.ReadOnly<BoardVisualElement>()}});
            
        playerInitSystem = World.GetExistingSystem<PlayerInitializationSystem>();
    }

    protected override void OnUpdate()
    {
        Entities.WithName("BoardCreation_Initialize")
        .WithAll<GameStateInitialize>()
        .ForEach((Entity e, in BoardCreationAuthor boardCreationAuthor) =>
        {
            int spawnedGoals = 0;
            System.Random random = new System.Random();
            Random rand = new Random((uint)random.Next());
            for (int x = 0; x < boardCreationAuthor.SizeX; x++)
            {
                for (int y = 0; y < boardCreationAuthor.SizeY; y++)
                {
                    Entity tile = EntityManager.Instantiate(boardCreationAuthor.TilePrefab);
                    Tile newTile = new Tile();
                    PositionXZ tilePos = new PositionXZ();
                    Translation translation = new Translation();
                    float2 wallPos = new float2(x, y);

                    // Create the outer walls & spawn points
                    if (y == 0)
                    {
                        newTile.Value = Tile.Attributes.WallDown;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallUp);
                    }
                    else if (y == boardCreationAuthor.SizeY - 1)
                    {
                        newTile.Value = Tile.Attributes.WallUp;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallDown);
                    }

                    if (x == 0)
                    {
                        newTile.Value = Tile.Attributes.WallLeft;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallLeft);
                        if (y == 0)
                            newTile.Value = Tile.Attributes.WallDown | Tile.Attributes.WallLeft;
                        else if (y == boardCreationAuthor.SizeY - 1)
                            newTile.Value = Tile.Attributes.WallUp | Tile.Attributes.WallLeft;
                    }
                    else if (x == boardCreationAuthor.SizeX - 1)
                    {
                        newTile.Value = Tile.Attributes.WallRight;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallRight);
                        if (y == 0)
                        {
                            newTile.Value = Tile.Attributes.WallDown | Tile.Attributes.WallRight;
                        }
                        else if (x == boardCreationAuthor.SizeX - 1 && y == boardCreationAuthor.SizeY - 1)
                        {
                            newTile.Value = Tile.Attributes.WallUp | Tile.Attributes.WallRight;
                        }
                    }

                    // Place Random Walls and Holes
                    if (rand.NextInt(0, 100) < boardCreationAuthor.randomAmount)
                    {
                        var result = rand.NextInt(0, 4);
                        switch (result)
                        {
                            case 0:
                                if (x != 0 && x != boardCreationAuthor.SizeX - 1 && y != 0 && y != boardCreationAuthor.SizeY - 1)
                                    newTile.Value |= Tile.Attributes.Hole;
                                break;

                            case 1:
                                if (y != boardCreationAuthor.SizeY - 1)
                                {
                                    newTile.Value |= Tile.Attributes.WallUp;
                                    PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallUp);
                                }
                                break;

                            case 2:
                                if (x != 0)
                                {
                                    newTile.Value |= Tile.Attributes.WallLeft;
                                    PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallLeft);
                                }
                                break;

                            case 3:
                                if (x != boardCreationAuthor.SizeX - 1)
                                {
                                    newTile.Value |= Tile.Attributes.WallRight;
                                    PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallRight);
                                }
                                break;

                            case 4:
                                if (y != 0)
                                {
                                    newTile.Value |= Tile.Attributes.WallDown;
                                    PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallDown);
                                }
                                break;
                        }
                    }

                    // Place Goals
                    if (x == 2 || x == boardCreationAuthor.SizeX - 3)
                    {
                        if (y == 2 || y == boardCreationAuthor.SizeY - 3)
                        {
                            newTile.Value = Tile.Attributes.Goal;
                            var goal = EntityManager.Instantiate(boardCreationAuthor.GoalPrefab);
                            EntityManager.SetComponentData(goal, new PositionXZ(){Value = new float2(x, y)});

                            var player = playerInitSystem.Players[spawnedGoals++];
                            newTile.Owner = player;
                            
                            var cc = EntityManager.GetComponentData<ColorAuthoring>(player);
                            var linkedEntities = EntityManager.GetBuffer<LinkedEntityGroup>(goal);
                            for (var l = 0; l < linkedEntities.Length; ++l)
                            {
                                var linkedEntity = linkedEntities[l].Value;
                                if (EntityManager.HasComponent<ColorAuthoring>(linkedEntity))
                                {
                                    EntityManager.SetComponentData(linkedEntity, cc);
                                }
                            }
                        }
                    }

                    var even = ((boardCreationAuthor.SizeY * y + x) % 2 == 0);
                    ColorAuthoring color;
                    if (even)
                    {
                        color = new ColorAuthoring()
                        {
                            Color = new  UnityEngine.Color(0.95f, 0.95f, 0.95f, 1.0f)
                        };
                    }
                    else
                    {
                        color = new ColorAuthoring
                        {
                            Color = new UnityEngine.Color(0.68f, 0.68f, 0.68f, 1.0f)
                        };
                    }

                    EntityManager.AddComponentData(tile, color);
                    // Set Tile values

                    tilePos.Value = new float2(x, y);
                    EntityManager.AddComponentData(tile, newTile);
                    EntityManager.AddComponentData(tile, tilePos);
                    if (newTile.Value == Tile.Attributes.Hole)
                        EntityManager.AddComponent<DisableRendering>(tile);
                }
            }

            // HACK - needs removing
            UnityEngine.Camera.main.transform.position = new UnityEngine.Vector3(boardCreationAuthor.SizeX /2, 4, boardCreationAuthor.SizeY /2);
        }).WithStructuralChanges().Run();

        Entities
            .WithAll<GameStateStart>()
            .ForEach((Entity e, BoardCreationAuthor boardCreationAuthor) =>
        {
            // Place rat and cat spawners in diagonally opposite corners
            var ratSpawners = EntityManager.Instantiate(boardCreationAuthor.RatSpawner, 2, Allocator.Temp);
            EntityManager.AddComponent<PositionXZ>(ratSpawners[0]);
            EntityManager.AddComponentData(ratSpawners[1], new PositionXZ { Value = new float2(boardCreationAuthor.SizeX - 1f, boardCreationAuthor.SizeY - 1f) });
            ratSpawners.Dispose();
            var catSpawners = EntityManager.Instantiate(boardCreationAuthor.CatSpawner, 2, Allocator.Temp);
            EntityManager.AddComponentData(catSpawners[0], new PositionXZ { Value = new float2(0f, boardCreationAuthor.SizeY - 1f) });
            EntityManager.AddComponentData(catSpawners[1], new PositionXZ { Value = new float2(boardCreationAuthor.SizeX - 1f, 0f) });
            catSpawners.Dispose();
        }).WithStructuralChanges().Run();
     
        // Quick'n'dirty cleanup stuff
        if (EntityManager.HasComponent<GameStateEnd>(GetSingletonEntity<BoardCreationAuthor>()))
        {
            Entities.WithAll<Spawner>().ForEach((Entity entity) => EntityManager.DestroyEntity(entity)).WithStructuralChanges().WithoutBurst().Run();
            Entities.WithAll<Direction>().ForEach((Entity entity) => EntityManager.RemoveComponent<Direction>(entity)).WithStructuralChanges().WithoutBurst().Run();
            Entities.WithAll<Falling>().ForEach((Entity entity) => EntityManager.RemoveComponent<Falling>(entity)).WithStructuralChanges().WithoutBurst().Run();
        }
        if (EntityManager.HasComponent<GameStateCleanup>(GetSingletonEntity<BoardCreationAuthor>()))
        {
            Entities.WithAll<Tile>().ForEach((Entity entity) => EntityManager.DestroyEntity(entity)).WithStructuralChanges().WithoutBurst().Run();
            Entities.WithAll<BoardVisualElement>().ForEach((Entity entity) => EntityManager.DestroyEntity(entity)).WithStructuralChanges().WithoutBurst().Run();
            Entities.WithAll<CatTag>().ForEach((Entity entity) => EntityManager.DestroyEntity(entity)).WithStructuralChanges().WithoutBurst().Run();
            Entities.WithAll<RatTag>().ForEach((Entity entity) => EntityManager.DestroyEntity(entity)).WithStructuralChanges().WithoutBurst().Run();
        }
    }

    private void PlaceWall(Entity prefab, float2 pos, Tile.Attributes attributes)
    {
        Translation translation = new Translation();
        Rotation rot = new Rotation();
        switch (attributes)
        {
            case Tile.Attributes.WallUp:
                rot.Value = quaternion.EulerXYZ(0, math.radians(90), 0);
                translation.Value = new float3(pos.x, 0.75f, pos.y - 0.5f);
                break;

            case Tile.Attributes.WallDown:
                rot.Value = quaternion.EulerXYZ(0, math.radians(90), 0);
                translation.Value = new float3(pos.x, 0.75f, pos.y + 0.5f);
                break;

            case Tile.Attributes.WallLeft:
                translation.Value = new float3(pos.x - 0.5f, 0.75f, pos.y);
                break;

            case Tile.Attributes.WallRight:
                translation.Value = new float3(pos.x + 0.5f, 0.75f, pos.y);
                break;
        }

        Entity wall = EntityManager.Instantiate(prefab);
        EntityManager.AddComponentData(wall, translation);
        EntityManager.AddComponentData(wall, rot);
        EntityManager.AddComponent<BoardVisualElement>(wall);
    }
}