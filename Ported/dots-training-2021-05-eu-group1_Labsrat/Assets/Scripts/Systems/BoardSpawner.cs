using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.AI;
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
    public NativeArray<Cardinals> walls;

    protected override void OnCreate() { }

    protected override void OnUpdate()
    {
        var random = Random.CreateFromIndex((uint)System.DateTime.Now.Ticks);

        if (TryGetSingleton(out GameConfig gameConfig))
        {
            cells = new NativeArray<Entity>(gameConfig.BoardDimensions.x * gameConfig.BoardDimensions.y, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            walls = new NativeArray<Cardinals>(gameConfig.BoardDimensions.x * gameConfig.BoardDimensions.y, Allocator.Persistent, NativeArrayOptions.ClearMemory);

            // Spawn Tiles
            for (int x = 0; x < gameConfig.BoardDimensions.x; x++)
            {
                for (int y = 0; y < gameConfig.BoardDimensions.y; y++)
                {
                    Entity cell = EntityManager.Instantiate(gameConfig.CellPrefab);

                    EntityManager.AddComponent<ForcedDirection>(cell);

                    EntityManager.AddComponent<Cell>(cell);
                    EntityManager.SetComponentData(cell, new Translation() { Value = new float3(x, -.5f, y) });

                    float4 color = (x + y) % 2 == 0 ? gameConfig.TileColor1 : gameConfig.TileColor2;
                    EntityManager.AddComponentData(cell, new URPMaterialPropertyBaseColor() { Value = color });

                    int idx = Utils.CellCoordinatesToCellIndex(gameConfig, x, y);

                    cells[idx] = cell;

                    EntityManager.AddComponentData(cell, new Direction());
                }
            }

            // Spawn Walls
            var r = Random.CreateFromIndex(gameConfig.RandomSeed ? (uint)System.DateTime.Now.Ticks : gameConfig.Seed ^ 3495736491);

            for (int x = 0; x < gameConfig.BoardDimensions.x; x++)
            {
                for (int y = 0; y < gameConfig.BoardDimensions.y; y++)
                {
                    int idx = Utils.CellCoordinatesToCellIndex(gameConfig, x, y);
                    Cardinals direction = EntityManager.GetComponentData<Direction>(cells[idx]).Value;

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
                    EntityManager.SetComponentData<Direction>(cells[idx], new Direction(direction));
                   
                    walls[idx] = direction;
                    // Add flags in adjacent cells
                    if (direction.HasFlag(Cardinals.West))
                    {
                        if(x > 0)
                        {
                            int tmpIdx = Utils.CellCoordinatesToCellIndex(gameConfig, x - 1, y);
                            var tmpCellDirection = EntityManager.GetComponentData<Direction>(cells[tmpIdx]);
                            tmpCellDirection.Value |= Cardinals.East;
                            walls[tmpIdx] = tmpCellDirection.Value;
                            EntityManager.SetComponentData<Direction>(cells[tmpIdx], tmpCellDirection);
                        }

                        CreateWall(x, y, Cardinals.West);
                    }

                    if (direction.HasFlag(Cardinals.East))
                    {
                        if(x < gameConfig.BoardDimensions.x - 1)
                        {
                            int tmpIdx = Utils.CellCoordinatesToCellIndex(gameConfig, x + 1, y);
                            var tmpCellDirection = EntityManager.GetComponentData<Direction>(cells[tmpIdx]);
                            tmpCellDirection.Value |= Cardinals.West;
                            walls[tmpIdx] = tmpCellDirection.Value;
                            EntityManager.SetComponentData<Direction>(cells[tmpIdx], tmpCellDirection);
                        }

                        CreateWall(x, y, Cardinals.East);
                    }

                    if (direction.HasFlag(Cardinals.North))
                    {
                        if(y < gameConfig.BoardDimensions.y - 1)
                        {
                            int tmpIdx = Utils.CellCoordinatesToCellIndex(gameConfig, x, y + 1);
                            var tmpCellDirection = EntityManager.GetComponentData<Direction>(cells[tmpIdx]);
                            tmpCellDirection.Value |= Cardinals.South;
                            walls[tmpIdx] = tmpCellDirection.Value;
                            EntityManager.SetComponentData<Direction>(cells[tmpIdx], tmpCellDirection);
                        }

                        CreateWall(x, y, Cardinals.North);
                    }

                    if (direction.HasFlag(Cardinals.South))
                    {
                        if(y > 0)
                        {
                            int tmpIdx = Utils.CellCoordinatesToCellIndex(gameConfig, x, y - 1);
                            var tmpCellDirection = EntityManager.GetComponentData<Direction>(cells[tmpIdx]);
                            tmpCellDirection.Value |= Cardinals.North;
                            walls[tmpIdx] = tmpCellDirection.Value;
                            EntityManager.SetComponentData<Direction>(cells[tmpIdx], tmpCellDirection);
                        }

                        CreateWall(x, y, Cardinals.South);
                    }


                }
            }

            // Create players

            for (int i = 0; i < gameConfig.NumOfAIPlayers + 1; i++)
            {
                var player = EntityManager.Instantiate(gameConfig.CursorPrefab);
                float4 color = random.NextFloat4();
                color.w = 1f;

                if (i == 0)
                {
                    EntityManager.AddComponent<DisableRendering>(player);
                }
                else
                {
                    EntityManager.AddComponent<AIState>(player);
                }

                EntityManager.AddComponentData(player, new PlayerIndex() { Index = i });
                EntityManager.AddComponentData(player, new PlayerInput() { TileIndex = -1 });
                EntityManager.AddComponentData(player, new Score());
                EntityManager.SetComponentData(player, new Translation() { Value = new float3(random.NextInt(gameConfig.BoardDimensions.x), 0.1f, random.NextInt(gameConfig.BoardDimensions.y)) });
                EntityManager.AddComponentData(player, new URPMaterialPropertyBaseColor() { Value = color });
                EntityManager.AddComponentData(player, new PlayerColor() { Color = color });
                EntityManager.AddBuffer < CreatedArrowData >(player);
            }

            Enabled = false;
        }

        void CreateWall(int x, int y, Cardinals direction)
        {
            Entity renderedWall = EntityManager.Instantiate(gameConfig.WallPrefab);
            EntityManager.AddComponent<Wall>(renderedWall);
            EntityManager.SetComponentData(renderedWall, new Translation() { Value = new float3(x, 0, y) });
            EntityManager.SetComponentData(renderedWall, new Rotation() { Value = quaternion.RotateY(Direction.GetAngle(direction)) });
        }
    }

    protected override void OnDestroy()
    {
        cells.Dispose();
        walls.Dispose();
    }
}
