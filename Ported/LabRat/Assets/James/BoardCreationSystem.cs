using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BoardCreationSystem : SystemBase
{
    public struct CreationComplete : IComponentData
    {
    }

    protected override void OnUpdate()
    {
        Entities
        .WithNone<CreationComplete>()
        .ForEach((Entity e, in BoardCreationAuthor boardCreationAuthor) =>
        {
            Random rand = new Random(1);
            for (int x = 0; x < boardCreationAuthor.SizeX; x++)
            {
                for (int y = 0; y < boardCreationAuthor.SizeY; y++)
                {
                    Entity tile = EntityManager.Instantiate(boardCreationAuthor.TilePrefab);
                    Tile newTile = new Tile();
                    PositionXZ tilePos = new PositionXZ();
                    Translation translation = new Translation();
                    float2 wallPos = new float2(x,y);

                    // Create the outer walls & spawn points
                    if (y == 0)
                    {
                        newTile.Value = Tile.Attributes.WallUp;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallUp);
                    }
                    else if (y == boardCreationAuthor.SizeY - 1)
                    {
                        newTile.Value = Tile.Attributes.WallDown;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallDown);
                    }

                    if (x == 0)
                    {
                        newTile.Value = Tile.Attributes.WallLeft;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallLeft);
                        if (y == 0)
                            newTile.Value = Tile.Attributes.WallUp | Tile.Attributes.WallLeft | Tile.Attributes.Spawn;
                        else if (y == boardCreationAuthor.SizeY - 1)
                            newTile.Value = Tile.Attributes.WallDown | Tile.Attributes.WallLeft | Tile.Attributes.Spawn;
                    }
                    else if (x == boardCreationAuthor.SizeX - 1)
                    {
                        newTile.Value = Tile.Attributes.WallRight;
                        PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallRight);
                        if (y == 0)
                        {
                            newTile.Value = Tile.Attributes.WallUp | Tile.Attributes.WallRight | Tile.Attributes.Spawn;
                        }
                            
                        else if (x == boardCreationAuthor.SizeX - 1 && y == boardCreationAuthor.SizeY - 1)
                        {
                            newTile.Value = Tile.Attributes.WallDown | Tile.Attributes.WallRight | Tile.Attributes.Spawn;
                        }
                            
                    }

                    // Place Random Walls and Holes
                    if (rand.NextInt(0, 100) < 20)
                    {
                        var result = rand.NextInt(0, 4);
                        switch (result)
                        {
                            case 0:
                                newTile.Value |= Tile.Attributes.Hole;
                                break;
                            case 1:
                                newTile.Value |= Tile.Attributes.WallDown;
                                PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallDown);
                                break;
                            case 2:
                                newTile.Value |= Tile.Attributes.WallLeft;
                                PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallLeft);
                                break;
                            case 3:
                                newTile.Value |= Tile.Attributes.WallRight;
                                PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallRight);
                                break;
                            case 4:
                                newTile.Value |= Tile.Attributes.WallUp;
                                PlaceWall(boardCreationAuthor.WallPrefab, wallPos, Tile.Attributes.WallUp);
                                break;
                        }
                    }

                    // Place Goals
                    if (x == 2 || x == boardCreationAuthor.SizeX - 3)
                    {
                        if(y == 2 || y == boardCreationAuthor.SizeY - 3)
                        {
                            newTile.Value = Tile.Attributes.Goal;
                            translation.Value = new float3(x, 0.75f, y);
                            Entity goal = EntityManager.Instantiate(boardCreationAuthor.GoalPrefab);
                            EntityManager.AddComponentData(goal, translation);
                        }
                    }
                    // Set Tile values

                    tilePos.Value = new float2(x, y);
                    EntityManager.AddComponentData(tile, newTile);
                    EntityManager.AddComponentData(tile, tilePos);


                }
            }
            
            // Place rat and cat spawners in diagonally opposite corners (if any are present)
            if (boardCreationAuthor.RatSpawner != Entity.Null)
            {
                var ratSpawners = EntityManager.Instantiate(boardCreationAuthor.RatSpawner, 2, Allocator.Temp);
                EntityManager.AddComponent<PositionXZ>(ratSpawners[0]);
                EntityManager.AddComponentData(ratSpawners[1], new PositionXZ { Value = new float2(boardCreationAuthor.SizeX - 1f, boardCreationAuthor.SizeY - 1f)});
                ratSpawners.Dispose();
            }
            if (boardCreationAuthor.CatSpawner != Entity.Null)
            {
                var catSpawners = EntityManager.Instantiate(boardCreationAuthor.CatSpawner, 2, Allocator.Temp);
                EntityManager.AddComponentData(catSpawners[0], new PositionXZ { Value = new float2(0f, boardCreationAuthor.SizeY - 1f)});
                EntityManager.AddComponentData(catSpawners[1], new PositionXZ { Value = new float2(boardCreationAuthor.SizeX - 1f, 0f)});
                catSpawners.Dispose();
            }

            EntityManager.AddComponent<CreationComplete>(e);
        }).WithStructuralChanges().Run();
    }

    void PlaceWall(Entity prefab, float2 pos, Tile.Attributes attributes)
    {
        Translation translation = new Translation();
        Rotation rot = new Rotation();
        switch (attributes)
        {
            case Tile.Attributes.WallUp:
                rot.Value = quaternion.EulerXYZ(0, math.radians(90), 0);
                translation.Value = new float3(pos.x, 1f, pos.y -0.5f);
                break;

            case Tile.Attributes.WallDown:
                rot.Value = quaternion.EulerXYZ(0, math.radians(90), 0);
                translation.Value = new float3(pos.x, 1f, pos.y + 0.5f);
                break;

            case Tile.Attributes.WallLeft:
                translation.Value = new float3(pos.x -0.5f, 1f, pos.y);
                break;

            case Tile.Attributes.WallRight:
                translation.Value = new float3(pos.x + 0.5f, 1f, pos.y);
                break;
        }
        
        Entity wall = EntityManager.Instantiate(prefab);
        EntityManager.AddComponentData(wall, translation);
        EntityManager.AddComponentData(wall, rot);
    }
}
