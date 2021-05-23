using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[ImmediateStrucutalChanges]
public struct BoardSpawner : IJobEntity, IMain
{
    [ReadSingleton] public GameConfig gameConfig;
    [Singleton] public GameState gameState;
        
    [Singleton("cells")] public NativeArray<Entity> cells;
    [Singleton("walls")] public NativeArray<Cardinals> walls;

    public Lookup<Direction> directionLookup;

    // maybe singleton init / teardown should be done elsewhere? With methods on the singleton itself?
    public void Init()
    {
        cells = new NativeArray<Entity>(gameConfig.BoardDimensions.x * gameConfig.BoardDimensions.y, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        walls = new NativeArray<Cardinals>(gameConfig.BoardDimensions.x * gameConfig.BoardDimensions.y, Allocator.Persistent, NativeArrayOptions.ClearMemory);
    }

    public void Teardown()
    {
        cells.Dispose();
        walls.Dispose();
    }
    
    //todo: this system should declare what archetypes it might be accessing / creating. Otherwise it represents a hard sync point?
    public void Execute()
    {
        if (gameState.spawned)
        {
            return;
        }
        gameState.spawned = true;

        // Spawn Tiles
        for (int x = 0; x < gameConfig.BoardDimensions.x; x++)
        {
            for (int y = 0; y < gameConfig.BoardDimensions.y; y++)
            {
                Entity cell = Instantiate(gameConfig.CellPrefab);
                Add<ForcedDirection>(cell);
                Add<Cell>(cell);
                Set(cell, new Translation() { Value = new float3(x, -.5f, y) });

                float4 color = (x + y) % 2 == 0 ? gameConfig.TileColor1 : gameConfig.TileColor2;
                Add(cell, new URPMaterialPropertyBaseColor() { Value = color });

                int idx = Utils.CellCoordinatesToCellIndex(gameConfig, x, y);
                cells[idx] = cell;
                Add(cell, new Direction());
            }
        }

        // Spawn Walls
        var r = Random.CreateFromIndex(gameConfig.RandomSeed ? (uint)System.DateTime.Now.Ticks : gameConfig.Seed ^ 3495736491);

        for (int x = 0; x < gameConfig.BoardDimensions.x; x++)
        {
            for (int y = 0; y < gameConfig.BoardDimensions.y; y++)
            {
                int idx = Utils.CellCoordinatesToCellIndex(gameConfig, x, y);
                Cardinals direction = directionLookup[cells[idx]].Value;

                // We add the outer walls
                if (x == 0)
                    direction |= Cardinals.West;
                if (x == gameConfig.BoardDimensions.x - 1)
                    direction |= Cardinals.East;
                if (y == 0)
                    direction |= Cardinals.South;
                if (y == gameConfig.BoardDimensions.y - 1)
                    direction |= Cardinals.North;

                // Then we add random walls
                if (r.NextFloat() <= gameConfig.WallProbability)
                {
                    direction |= (Cardinals)r.NextInt(15);
                }

                // Set flags in current cell
                Set(cells[idx], new Direction(direction));
               
                walls[idx] = direction;
                // Add flags in adjacent cells
                if (direction.HasFlag(Cardinals.West))
                {
                    if(x > 0)
                    {
                        int tmpIdx = Utils.CellCoordinatesToCellIndex(gameConfig, x - 1, y);
                        var tmpCellDirection = directionLookup[cells[tmpIdx]];
                        tmpCellDirection.Value |= Cardinals.East;
                        walls[tmpIdx] = tmpCellDirection.Value;
                        Set(cells[tmpIdx], tmpCellDirection);
                    }

                    CreateWall(x, y, Cardinals.West);
                }

                if (direction.HasFlag(Cardinals.East))
                {
                    if(x < gameConfig.BoardDimensions.x - 1)
                    {
                        int tmpIdx = Utils.CellCoordinatesToCellIndex(gameConfig, x + 1, y);
                        var tmpCellDirection = directionLookup[cells[tmpIdx]];
                        tmpCellDirection.Value |= Cardinals.West;
                        walls[tmpIdx] = tmpCellDirection.Value;
                        Set(cells[tmpIdx], tmpCellDirection);
                    }

                    CreateWall(x, y, Cardinals.East);
                }

                if (direction.HasFlag(Cardinals.North))
                {
                    if(y < gameConfig.BoardDimensions.y - 1)
                    {
                        int tmpIdx = Utils.CellCoordinatesToCellIndex(gameConfig, x, y + 1);
                        var tmpCellDirection = directionLookup[cells[tmpIdx]];
                        tmpCellDirection.Value |= Cardinals.South;
                        walls[tmpIdx] = tmpCellDirection.Value;
                        Set(cells[tmpIdx], tmpCellDirection);
                    }

                    CreateWall(x, y, Cardinals.North);
                }

                if (direction.HasFlag(Cardinals.South))
                {
                    if(y > 0)
                    {
                        int tmpIdx = Utils.CellCoordinatesToCellIndex(gameConfig, x, y - 1);
                        var tmpCellDirection = directionLookup[cells[tmpIdx]];
                        tmpCellDirection.Value |= Cardinals.North;
                        walls[tmpIdx] = tmpCellDirection.Value;
                        Set(cells[tmpIdx], tmpCellDirection);
                    }

                    CreateWall(x, y, Cardinals.South);
                }
            }
        }

        var random = Random.CreateFromIndex(12345);
        // Create players
        for (int i = 0; i < gameConfig.NumOfAIPlayers + 1; i++)
        {
            var player = Instantiate(gameConfig.CursorPrefab);
            
            float4 color = random.NextFloat4();
            color.w = 1f;

            if (i == 0)
            {
                Add<DisableRendering>(player);
            }
            else
            {
                Add<AIState>(player);
            }

            Add(player, new PlayerIndex() { Index = i });
            Add(player, new PlayerInput() { TileIndex = -1 });
            Add(player, new Score());
            Set(player, new Translation() { Value = new float3(random.NextInt(gameConfig.BoardDimensions.x), 0.1f, random.NextInt(gameConfig.BoardDimensions.y)) });
            Add(player, new URPMaterialPropertyBaseColor() { Value = color });
            Add(player, new PlayerColor() { Color = color });
            AddBuffer<CreatedArrowData>(player);
            
            // Create homebase
            var homebase = Instantiate(gameConfig.HomebasePrefab);
            Add(homebase, new URPMaterialPropertyBaseColor() { Value = color });
            Add(homebase, new Homebase() { PlayerEntity = player });
            Set(homebase, new Translation() { Value = new float3(random.NextInt(1, gameConfig.BoardDimensions.x-1), 0.1f, random.NextInt(1, gameConfig.BoardDimensions.y-1)) });
        }
    }
    
    void CreateWall(int x, int y, Cardinals direction)
    {
        Entity renderedWall = Instantiate(gameConfig.WallPrefab);
        Add<Wall>(renderedWall);
        Set(renderedWall, new Translation() { Value = new float3(x, 0, y) });
        Set(renderedWall, new Rotation() { Value = quaternion.RotateY(Direction.GetAngle(direction)) });
    }
}
