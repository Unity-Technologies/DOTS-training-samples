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

                    // Create the outer walls & spawn points
                    if (y == 0)
                    {
                        newTile.Value = Tile.Attributes.WallUp;
                    }
                    else if (y == boardCreationAuthor.SizeY - 1)
                    {
                        newTile.Value = Tile.Attributes.WallDown;
                    }

                    if (x == 0)
                    {
                        newTile.Value = Tile.Attributes.WallLeft;
                        if (y == 0)
                            newTile.Value = Tile.Attributes.WallUp | Tile.Attributes.WallLeft | Tile.Attributes.Spawn;
                        else if (y == boardCreationAuthor.SizeY - 1)
                            newTile.Value = Tile.Attributes.WallDown | Tile.Attributes.WallLeft | Tile.Attributes.Spawn;
                    }
                    else if (x == boardCreationAuthor.SizeX - 1)
                    {
                        newTile.Value = Tile.Attributes.WallRight;
                        if (y == 0)
                            newTile.Value = Tile.Attributes.WallUp | Tile.Attributes.WallRight | Tile.Attributes.Spawn;
                        else if (x == boardCreationAuthor.SizeX - 1 && y == boardCreationAuthor.SizeY - 1)
                            newTile.Value = Tile.Attributes.WallDown | Tile.Attributes.WallRight | Tile.Attributes.Spawn;
                    }

                    // Place Random Walls and Holes
                    if (y != 0 || x != 0 || x != boardCreationAuthor.SizeX - 1 || y != boardCreationAuthor.SizeY - 1)
                        if (rand.NextInt(0, 100) < 20)
                        {
                            var result = rand.NextInt(0, 4);
                            switch(result)
                            {
                                case 0:
                                    newTile.Value = Tile.Attributes.Hole;
                                    break;
                                case 1:
                                    newTile.Value = Tile.Attributes.WallDown;
                                    break;
                                case 2:
                                    newTile.Value = Tile.Attributes.WallLeft;
                                    break;
                                case 3:
                                    newTile.Value = Tile.Attributes.WallRight;
                                    break;
                                case 4:
                                    newTile.Value = Tile.Attributes.WallUp;
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
            
            // Place rat and cat spawners in diagonally opposite corners
            var ratSpawners = EntityManager.Instantiate(boardCreationAuthor.RatSpawner, 2, Allocator.Temp);
            var catSpawners = EntityManager.Instantiate(boardCreationAuthor.CatSpawner, 2, Allocator.Temp);
            EntityManager.AddComponent<PositionXZ>(ratSpawners[0]);
            EntityManager.AddComponentData(ratSpawners[1], new PositionXZ { Value = new float2(boardCreationAuthor.SizeX - 1f, boardCreationAuthor.SizeY - 1f)});
            EntityManager.AddComponentData(catSpawners[0], new PositionXZ { Value = new float2(0f, boardCreationAuthor.SizeY - 1f)});
            EntityManager.AddComponentData(catSpawners[1], new PositionXZ { Value = new float2(boardCreationAuthor.SizeX - 1f, 0f)});
            ratSpawners.Dispose();
            catSpawners.Dispose();

            EntityManager.AddComponent<CreationComplete>(e);
        }).WithStructuralChanges().Run();
    }
}
