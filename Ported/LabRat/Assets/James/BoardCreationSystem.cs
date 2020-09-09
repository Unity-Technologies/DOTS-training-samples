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
        .ForEach((Entity e, ref BoardCreationAuthor boardCreationAuthor) =>
        {
            for (int x = 0; x < boardCreationAuthor.SizeX; x++)
            {
                for (int y = 0; y < boardCreationAuthor.SizeY; y++)
                {
                    Entity tile = EntityManager.Instantiate(boardCreationAuthor.TilePrefab);
                    Tile t = new Tile();
                    PositionXZ p = new PositionXZ();
                    Random rand = new Random();

                    // Create the outer walls & spawn points
                    if (y == 0)
                    {
                        t.Value = Tile.Attributes.WallUp;
                    }
                    else if (y == boardCreationAuthor.SizeY - 1)
                    {
                        t.Value = Tile.Attributes.WallDown;
                    }

                    if (x == 0)
                    {
                        t.Value = Tile.Attributes.WallLeft;
                        if (y == 0)
                            t.Value = Tile.Attributes.WallUp | Tile.Attributes.WallLeft | Tile.Attributes.Spawn;
                        else if (y == boardCreationAuthor.SizeY - 1)
                            t.Value = Tile.Attributes.WallDown | Tile.Attributes.WallLeft | Tile.Attributes.Spawn;
                    }
                    else if (x == boardCreationAuthor.SizeX - 1)
                    {
                        t.Value = Tile.Attributes.WallRight;
                        if (y == 0)
                            t.Value = Tile.Attributes.WallUp | Tile.Attributes.WallRight | Tile.Attributes.Spawn;
                        else if (x == boardCreationAuthor.SizeX - 1 && y == boardCreationAuthor.SizeY - 1)
                            t.Value = Tile.Attributes.WallDown | Tile.Attributes.WallRight | Tile.Attributes.Spawn;
                    }

                    // Place Random Walls and Holes
                    if (y != 0 || x != 0 || x != boardCreationAuthor.SizeX - 1 || y != boardCreationAuthor.SizeY - 1)
                        if (rand.NextInt(0, 100) < 30)
                        {
                            var result = rand.NextInt(0, 4);
                        }

                    // Place Goals

                    if (x == 2)
                    {
                        if(y == 2 || y == boardCreationAuthor.SizeY - 3)
                            t.Value = Tile.Attributes.Goal;
                    }
                    if (y == 2)
                    {
                        if (x == 2 || x == boardCreationAuthor.SizeX - 3)
                            t.Value = Tile.Attributes.Goal;
                    }


                    // Set Tile values

                    p.Value = new float2(x, y);
                    EntityManager.AddComponentData(tile, t);
                    EntityManager.AddComponentData(tile, p);


                }
            }
            EntityManager.AddComponent<CreationComplete>(e);
        }).WithStructuralChanges().Run();
    }
}
