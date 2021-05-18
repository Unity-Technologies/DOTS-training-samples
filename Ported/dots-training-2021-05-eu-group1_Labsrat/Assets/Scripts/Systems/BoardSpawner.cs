using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class BoardSpawner : SystemBase
{
    public NativeArray<Entity> cells;

    protected override void OnCreate()
    {
    }

    public static int CoordintateToIndex(GameConfig c, int x, int y)
    {
        if (x >= c.BoardDimensions.x || y >= c.BoardDimensions.y || x < 0 || y < 0)
            return -1;
        
        return (y * c.BoardDimensions.x) + x;
    }
    
    protected override void OnUpdate()
    {
        if (TryGetSingleton(out GameConfig gameConfig))
        {
            cells = new NativeArray<Entity>(gameConfig.BoardDimensions.x * gameConfig.BoardDimensions.y, Allocator.Persistent, NativeArrayOptions.ClearMemory);

            // Spawn Tiles
            for (int x = 0; x < gameConfig.BoardDimensions.x; x++)
            {
                for (int y = 0; y < gameConfig.BoardDimensions.y; y++)
                {
                    Entity renderedCell = EntityManager.Instantiate(gameConfig.CellPrefab);
                    EntityManager.SetComponentData(renderedCell, new Translation() { Value = new float3(x, -.5f, y) });

                    float4 color = (x + y) % 2 == 0 ? gameConfig.TileColor1 : gameConfig.TileColor2;
                    EntityManager.AddComponentData(renderedCell, new URPMaterialPropertyBaseColor() { Value = color });

                    int idx = CoordintateToIndex(gameConfig, x, y);

                    cells[idx] = renderedCell;

                    EntityManager.AddComponentData(renderedCell, new Direction());
                }
            }

            // Spawn Walls
            var r = Random.CreateFromIndex(0);

            for (int x = 0; x < gameConfig.BoardDimensions.x; x++)
            {
                for (int y = 0; y < gameConfig.BoardDimensions.y; y++)
                {
                    if (r.NextFloat() > gameConfig.WallProbability)
                        continue;

                    Cardinals direction = (Cardinals)r.NextInt(15);

                    if (x == 0 && direction.HasFlag(Cardinals.West))
                        direction &= ~Cardinals.West;

                    if (x == gameConfig.BoardDimensions.x - 1 && direction.HasFlag(Cardinals.East))
                        direction &= ~Cardinals.East;

                    if (y == 0 && direction.HasFlag(Cardinals.South))
                        direction &= ~Cardinals.South;

                    if (y == gameConfig.BoardDimensions.y - 1 && direction.HasFlag(Cardinals.North))
                        direction &= ~Cardinals.North;

                    int idx = CoordintateToIndex(gameConfig, x, y);

                    // Set flags in current cell
                    EntityManager.SetComponentData<Direction>(cells[idx], new Direction(direction));


                    // Add flags in adjacent cells
                    if (direction.HasFlag(Cardinals.West))
                    {
                        int tmpIdx = CoordintateToIndex(gameConfig, x - 1, y);
                        var tmpCellDirection = EntityManager.GetComponentData<Direction>(cells[tmpIdx]);
                        tmpCellDirection.Value |= Cardinals.East;
                        EntityManager.SetComponentData<Direction>(cells[tmpIdx], tmpCellDirection);

                        CreateWall(x, y, Cardinals.West);
                    }

                    if (direction.HasFlag(Cardinals.East))
                    {
                        int tmpIdx = CoordintateToIndex(gameConfig, x + 1, y);
                        var tmpCellDirection = EntityManager.GetComponentData<Direction>(cells[tmpIdx]);
                        tmpCellDirection.Value |= Cardinals.West;
                        EntityManager.SetComponentData<Direction>(cells[tmpIdx], tmpCellDirection);

                        CreateWall(x, y, Cardinals.East);
                    }

                    if (direction.HasFlag(Cardinals.North))
                    {
                        int tmpIdx = CoordintateToIndex(gameConfig, x, y + 1);
                        var tmpCellDirection = EntityManager.GetComponentData<Direction>(cells[tmpIdx]);
                        tmpCellDirection.Value |= Cardinals.South;
                        EntityManager.SetComponentData<Direction>(cells[tmpIdx], tmpCellDirection);

                        CreateWall(x, y, Cardinals.North);
                    }

                    if (direction.HasFlag(Cardinals.South))
                    {
                        int tmpIdx = CoordintateToIndex(gameConfig, x, y - 1);
                        var tmpCellDirection = EntityManager.GetComponentData<Direction>(cells[tmpIdx]);
                        tmpCellDirection.Value |= Cardinals.North;
                        EntityManager.SetComponentData<Direction>(cells[tmpIdx], tmpCellDirection);

                        CreateWall(x, y, Cardinals.South);
                    }
                }
            }
            
            // Create players

            for (int i = 0; i < gameConfig.NumOfAIPlayers+1; i++)
            {
                var player = EntityManager.CreateEntity();

                EntityManager.AddComponentData(player, new PlayerIndex() { Index = i});
                EntityManager.AddComponentData(player, new PlayerInput() { TileIndex = -1});
                EntityManager.AddComponentData(player, new Score());
                EntityManager.AddComponentData(player, new PlayerColor());
            }
           

            Enabled = false;
        }

        void CreateWall(int x, int y, Cardinals direction)
        {
            Entity renderedWall = EntityManager.Instantiate(gameConfig.WallPrefab);
            EntityManager.SetComponentData(renderedWall, new Translation() { Value = new float3(x, .5f, y) });
            EntityManager.SetComponentData(renderedWall, new Rotation() { Value = quaternion.RotateY(GetAngle(direction)) });
        }

        static float GetAngle(Cardinals direction)
        {
            switch (direction)
            {
                default:
                case Cardinals.None:
                case Cardinals.North:
                    return 0;
                case Cardinals.West:
                    return math.radians(90);
                case Cardinals.South:
                    return math.radians(180);
                case Cardinals.East:
                    return math.radians(270);

            }
        }
    }

    protected override void OnDestroy()
    {
        cells.Dispose();
    }
}
