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
    
    protected override void OnCreate()
    {
        playerInitSystem = World.GetExistingSystem<PlayerInitializationSystem>();

#if UNITY_EDITOR
        new GameObject("BoardDebugger", typeof(BoardCreationDebug));
#endif
    }
    
    protected override void OnUpdate()
    {
        Entities.WithName("BoardCreation_Initialize")
        .WithAll<GameStateInitialize>()
        .ForEach((Entity e, in BoardCreationAuthor boardCreationAuthor) =>
        {
            Entity[,] board = new Entity[boardCreationAuthor.SizeX, boardCreationAuthor.SizeY];
            int spawnedGoals = 0;
            System.Random random = new System.Random();
            Random rand = new Random((uint)random.Next());
            for (int x = 0; x < boardCreationAuthor.SizeX; x++)
            {
                for (int y = 0; y < boardCreationAuthor.SizeY; y++)
                {
                    Entity tile = EntityManager.Instantiate(boardCreationAuthor.TilePrefab);
                    EntityManager.AddComponent<Static>(tile);
                    Tile newTile = new Tile();
                    PositionXZ tilePos = new PositionXZ();
                    float2 wallPos = new float2(x, y);

                    // Create the outer walls & spawn points
                    if (y == 0)
                    {
                        newTile.Value |= Tile.Attributes.WallDown;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallDown);
                    }
                    else if (y == boardCreationAuthor.SizeY - 1)
                    {
                        newTile.Value |= Tile.Attributes.WallUp;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallUp);
                    }

                    if (x == 0)
                    {
                        newTile.Value |= Tile.Attributes.WallLeft;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallLeft);
                        if (y == 0)
                            newTile.Value |= (Tile.Attributes.WallDown | Tile.Attributes.WallLeft);
                        else if (y == boardCreationAuthor.SizeY - 1)
                            newTile.Value |= (Tile.Attributes.WallUp | Tile.Attributes.WallLeft);
                    }
                    else if (x == boardCreationAuthor.SizeX - 1)
                    {
                        newTile.Value = Tile.Attributes.WallRight;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallRight);
                        if (y == 0)
                        {
                            newTile.Value |= (Tile.Attributes.WallDown | Tile.Attributes.WallRight);
                        }
                        else if (x == boardCreationAuthor.SizeX - 1 && y == boardCreationAuthor.SizeY - 1)
                        {
                            newTile.Value |= (Tile.Attributes.WallUp | Tile.Attributes.WallRight);
                        }
                    }

                    // Place Random Walls and Holes
                    if (rand.NextInt(0, 100) < boardCreationAuthor.randomAmount)
                    {
                        var result = rand.NextInt(0, 4);
                        switch (result)
                        {
                            case 0:
                                if (x != 0 && x != boardCreationAuthor.SizeX - 1 && y != 0 && y != boardCreationAuthor.SizeY - 1 && rand.NextInt(0,4) >= 3)
                                    newTile.Value = Tile.Attributes.Hole;
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
                            EntityManager.AddComponent<Static>(goal);
                            EntityManager.SetComponentData(goal, new PositionXZ(){Value = new float2(x, y)});

                            var player = playerInitSystem.Players[spawnedGoals++];
                            EntityManager.AddComponentData(tile, new TileOwner {Value = player});
                            
                            var cc = EntityManager.GetComponentData<LabRat_Color>(player);
                            EntityManager.AddComponentData(goal, cc);
                        }
                    }

                    var even = ((boardCreationAuthor.SizeY * y + x) % 2 == 0);
                    LabRat_Color color;
                    if (even)
                    {
                        color = new LabRat_Color()
                        {
                            Value = new  float4(0.95f, 0.95f, 0.95f, 1.0f)
                        };
                    }
                    else
                    {
                        color = new LabRat_Color
                        {
                            Value = new float4(0.68f, 0.68f, 0.68f, 1.0f)
                        };
                    }

                    EntityManager.AddComponentData(tile, color);
                    // Set Tile values

                    tilePos.Value = new float2(x, y);
                    EntityManager.AddComponentData(tile, newTile);
                    EntityManager.AddComponentData(tile, tilePos);
                    if (newTile.Value == Tile.Attributes.Hole)
                        EntityManager.AddComponent<DisableRendering>(tile);

                    board[x, y] = tile;
                }
            }

            // Place neighbor walls
            for (int x = 0; x < boardCreationAuthor.SizeX; x++)
            {
                for (int y = 0; y < boardCreationAuthor.SizeY; y++)
                {

                    Tile t;

                    t = EntityManager.GetComponentData<Tile>(board[x, y]);

                    if (t.Value.HasFlag(Tile.Attributes.WallUp) && y < boardCreationAuthor.SizeY -1)
                    {
                        Tile r = EntityManager.GetComponentData<Tile>(board[x, y + 1]);
                        r.Value |= Tile.Attributes.WallDown;
                        EntityManager.SetComponentData(board[x, y + 1], r);
                    }
                    
                    t = EntityManager.GetComponentData<Tile>(board[x, y]);

                    if (t.Value.HasFlag(Tile.Attributes.WallDown) && y > 0)
                    {
                        Tile r = EntityManager.GetComponentData<Tile>(board[x, y - 1]);
                        r.Value |= Tile.Attributes.WallUp;
                        EntityManager.SetComponentData(board[x, y - 1], r);
                    }

                    t = EntityManager.GetComponentData<Tile>(board[x, y]);

                    if (t.Value.HasFlag(Tile.Attributes.WallLeft) && x > 0)
                    {
                        Tile r = EntityManager.GetComponentData<Tile>(board[x - 1, y]);
                        r.Value |= Tile.Attributes.WallRight;
                        EntityManager.SetComponentData(board[x - 1, y], r);
                    }

                    t = EntityManager.GetComponentData<Tile>(board[x, y]);

                    if (t.Value.HasFlag(Tile.Attributes.WallRight) && x < boardCreationAuthor.SizeX - 1)
                    {
                        Tile r = EntityManager.GetComponentData<Tile>(board[x + 1, y]);
                        r.Value |= Tile.Attributes.WallLeft;
                        EntityManager.SetComponentData(board[x + 1, y], r);
                    }
                }
            }

            // HACK - needs removing
          //  UnityEngine.Camera.main.transform.position = new UnityEngine.Vector3(boardCreationAuthor.SizeX /2, 4, boardCreationAuthor.SizeY /2);
          //  UnityEngine.Camera.main.orthographicSize = 0.55f*math.max(boardCreationAuthor.SizeX, boardCreationAuthor.SizeY);
        }).WithStructuralChanges().Run();

        Entities
            .WithName("BoardCreation_spawners")
            .WithAll<GameStateStart>()
            .ForEach((in BoardCreationAuthor boardCreationAuthor) =>
        {
            // Place rat and cat spawners in diagonally opposite corners
            var ratSpawners = EntityManager.Instantiate(boardCreationAuthor.RatSpawner, 2, Allocator.Temp);
            EntityManager.AddComponent<PositionXZ>(ratSpawners[0]);
            EntityManager.AddComponentData(ratSpawners[1], new PositionXZ { Value = new float2(boardCreationAuthor.SizeX - 1f, boardCreationAuthor.SizeY - 1f) });
            EntityManager.AddComponent<Static>(ratSpawners[0]);
            EntityManager.AddComponent<Static>(ratSpawners[1]);
            ratSpawners.Dispose();
            
            var catSpawners = EntityManager.Instantiate(boardCreationAuthor.CatSpawner, 2, Allocator.Temp);
            EntityManager.AddComponent<Static>(catSpawners[0]);
            EntityManager.AddComponent<Static>(catSpawners[1]);
            EntityManager.AddComponentData(catSpawners[0], new PositionXZ { Value = new float2(0f, boardCreationAuthor.SizeY - 1f) });
            EntityManager.AddComponentData(catSpawners[1], new PositionXZ { Value = new float2(boardCreationAuthor.SizeX - 1f, 0f) });
            catSpawners.Dispose();
        }).WithStructuralChanges().Run();
    }

    private void PlaceWall(Entity prefab, float2 pos, Tile.Attributes attributes)
    {
        Translation translation = new Translation();
        Rotation rot = new Rotation();
        switch (attributes)
        {
            case Tile.Attributes.WallUp:
                rot.Value = quaternion.EulerXYZ(0, math.radians(90), 0);
                translation.Value = new float3(pos.x, 0.75f, pos.y + 0.5f);
                break;

            case Tile.Attributes.WallDown:
                rot.Value = quaternion.EulerXYZ(0, math.radians(90), 0);
                translation.Value = new float3(pos.x, 0.75f, pos.y - 0.5f);
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
        EntityManager.AddComponent<Static>(wall);
    }

#if UNITY_EDITOR
    [System.Flags]
    public enum DebugAttributes : ushort
    {
        None,

        Left = Tile.Attributes.WallLeft,
        Up = Tile.Attributes.WallUp,
        Right = Tile.Attributes.WallRight,
        Down = Tile.Attributes.WallDown,

        Hole = Tile.Attributes.Hole,
        Goal = Tile.Attributes.Goal,

        ArrowLeft = Tile.Attributes.ArrowLeft,
        ArrowUp = Tile.Attributes.ArrowUp,
        ArrowRight = Tile.Attributes.ArrowRight,
        ArrowDown = Tile.Attributes.ArrowDown,
    }

    public void DrawDebugHandles()
    {
        if (!HasSingleton<BoardCreationAuthor>() || !GetSingleton<BoardCreationAuthor>().ShowDebugData)
            return;
        
        Entities.ForEach((in PositionXZ pos, in Tile tile) =>
        {
            UnityEditor.Handles.Label(new float3(pos.Value.x, .1f, pos.Value.y), $"({pos.Value.x},{pos.Value.y})\n{(DebugAttributes)tile.Value}");
        }).WithoutBurst().Run();
    }
#endif
}