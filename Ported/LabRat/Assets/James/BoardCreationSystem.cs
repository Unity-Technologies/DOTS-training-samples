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
                        t.Value = Tile.Attributes.Up;
                    }
                    else if (y == boardCreationAuthor.SizeY - 1)
                    {
                        t.Value = Tile.Attributes.Down;
                    }

                    if (x == 0)
                    {
                        t.Value = Tile.Attributes.Left;
                        if (y == 0)
                            t.Value = Tile.Attributes.Up | Tile.Attributes.Left | Tile.Attributes.Spawn;
                        else if (y == boardCreationAuthor.SizeY - 1)
                            t.Value = Tile.Attributes.Down | Tile.Attributes.Left | Tile.Attributes.Spawn;
                    }
                    else if (x == boardCreationAuthor.SizeX - 1)
                    {
                        t.Value = Tile.Attributes.Right;
                        if (y == 0)
                            t.Value = Tile.Attributes.Up | Tile.Attributes.Right | Tile.Attributes.Spawn;
                        else if (x == boardCreationAuthor.SizeX - 1 && y == boardCreationAuthor.SizeY - 1)
                            t.Value = Tile.Attributes.Down | Tile.Attributes.Right | Tile.Attributes.Spawn;
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
