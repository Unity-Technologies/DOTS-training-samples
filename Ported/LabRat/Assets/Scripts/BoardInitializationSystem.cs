using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class BoardInitializationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in Board board) =>
            {
                var rand = new Unity.Mathematics.Random((uint) DateTime.Now.Millisecond);
                var boardQuarter = board.size / 4;
                
                var holeProbability = board.holeCount / (float) (board.size * board.size);
                var holeCount = board.holeCount;
                
                var wallProbability = board.wallCount / (float) (board.size * board.size);
                var wallCount = board.wallCount;

                for (int x = 0; x < board.size; ++x)
                for (int z = 0; z < board.size; ++z)
                {
                    uint localWall = 0;
                    
                    var posX = (x - (board.size - 1) / 2);
                    var posY = rand.NextFloat(0.0f, board.yNoise);
                    var posZ = (z - (board.size - 1) / 2);
                    
                    var instance = EntityManager.Instantiate(board.tilePrefab);
                    SetComponent(instance, new Translation { Value = new float3(posX, posY, posZ) });
                    SetComponent(instance,
                        (x + z) % 2 == 0
                            ? new URPMaterialPropertyBaseColor {Value = new float4(0.8f, 0.8f, 0.8f, 1.0f)}
                            : new URPMaterialPropertyBaseColor {Value = new float4(0.6f, 0.6f, 0.6f, 1.0f)});

                    // Place border walls
                    if (x == 0) // East
                    {
                        PlaceWall(posX, posZ, 2, board.wallPrefab, posY);
                        localWall |= 4;
                    }
                    else if (x == (board.size - 1)) // West
                    {
                        PlaceWall(posX, posZ, 3, board.wallPrefab, posY);
                        localWall |= 8;
                    }

                    if (z == 0) // South
                    {
                        PlaceWall(posX, posZ, 1, board.wallPrefab, posY);
                        localWall |= 2;
                    }
                    else if (z == (board.size - 1)) // North
                    {
                        PlaceWall(posX, posZ, 0, board.wallPrefab, posY);
                        localWall |= 1;
                    }

                    // Place home base
                    if (x == boardQuarter && z == boardQuarter)
                        PlaceHomeBase(posX, posZ, 0, board.homeBasePrefab, posY);
                    else if (x == boardQuarter && z == board.size - boardQuarter - 1)
                        PlaceHomeBase(posX, posZ, 1, board.homeBasePrefab, posY);
                    else if (x == board.size - boardQuarter - 1 && z == board.size - boardQuarter - 1)
                        PlaceHomeBase(posX, posZ, 2, board.homeBasePrefab, posY);
                    else if (x == board.size - boardQuarter - 1 && z == boardQuarter)
                        PlaceHomeBase(posX, posZ, 3, board.homeBasePrefab, posY);
                    else  // Not a base: can be a hole
                    {
                        if (holeCount > 0 && rand.NextFloat(1f) < holeProbability)
                        {
                            EntityManager.DestroyEntity(instance);
                            holeCount--;
                        }
                    }
                    
                    // Place other wall
                    if (wallCount > 0 && localWall < 15 && rand.NextFloat(1f) < wallProbability)
                    {
                        var coordinate = 2 ^ rand.NextUInt(0, 3);
                        if ((localWall & coordinate) == 0)
                        {
                            PlaceWall(posX, posZ, coordinate, board.wallPrefab, posY);
                            wallCount--;
                        }
                    }
                }
                
                EntityManager.DestroyEntity(entity);
        }).Run();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="coordinate">North (0), South (1), East (2) or West (3)</param>
    /// <param name="board"></param>
    /// <param name="yOffset"></param>
    void PlaceWall(float posX, float posZ, uint coordinate, Entity wallPrefab, float yOffset)
    {
        switch (coordinate)
        {
            case 0: // North
                posZ += 0.5f;
                break;
            case 1: // South
                posZ -= 0.5f;
                break;
            case 2: // East
                posX -= 0.5f;
                break;
            case 3: // West
                posX += 0.5f;
                break;
            default:
                break;
        }

        var instance = EntityManager.Instantiate(wallPrefab);
        SetComponent(instance, new Translation { Value = new float3(posX, 0.725f + yOffset, posZ) });

        if (coordinate <= 1)  // Turn the wall if it's placed on North or South
            SetComponent(instance, new Rotation { Value = quaternion.RotateY(math.radians(90f)) });
    }

    void PlaceHomeBase(int posX, int posZ, int playerIndex, Entity homeBasePrefab, float yOffset)
    {
        var instance = EntityManager.Instantiate(homeBasePrefab);
        SetComponent(instance, new Translation { Value = new float3(posX, yOffset, posZ) });

        URPMaterialPropertyBaseColor color;
        switch (playerIndex)
        {
            case 0: // Red
                color = new URPMaterialPropertyBaseColor {Value = new float4(1.0f, 0.0f, 0.0f, 1.0f)};
                break;
            case 1: // Green
                color = new URPMaterialPropertyBaseColor {Value = new float4(0.0f, 1.0f, 0.0f, 1.0f)};
                break;
            case 2: // Blue
                color = new URPMaterialPropertyBaseColor {Value = new float4(0.0f, 0.0f, 1.0f, 1.0f)};
                break;
            default: // Black
                color = new URPMaterialPropertyBaseColor {Value = new float4(1.0f, 1.0f, 1.0f, 1.0f)};
                break;
        }

        SetComponent(instance, color);
        // Add a base tag?
    }
}


