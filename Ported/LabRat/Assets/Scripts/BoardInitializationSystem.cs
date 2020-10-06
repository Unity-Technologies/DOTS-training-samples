using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class BoardInitializationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in Board board, in LocalToWorld ltw) =>
            {
                var rand = new Unity.Mathematics.Random(10);

                for (int x = 0; x < board.size; ++x)
                for (int z = 0; z < board.size; ++z)
                {
                    var posX = (x - (board.size - 1) / 2);
                    var posY = rand.NextFloat(0.0f, board.yNoise);
                    var posZ = (z - (board.size - 1) / 2);
                    var instance = EntityManager.Instantiate(board.tilePrefab);
                    SetComponent(instance, new Translation
                    {
                        Value = ltw.Position + new float3(posX, posY, posZ)
                    });

                    if ((x + z) % 2 == 0)
                        SetComponent(instance, new URPMaterialPropertyBaseColor { Value = new float4(0.8f, 0.8f, 0.8f, 1.0f) });
                    else
                        SetComponent(instance, new URPMaterialPropertyBaseColor { Value = new float4(0.6f, 0.6f, 0.6f, 1.0f) });

                    if (x == 0)
                        PlaceWall(x, z, 2, board, posY);
                    else if (x == (board.size - 1))
                        PlaceWall(x, z, 3, board, posY);

                    if (z == 0)
                        PlaceWall(x, z, 1, board, posY);
                    else if (z == (board.size - 1))
                        PlaceWall(x, z, 0, board, posY);
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
    void PlaceWall(int x, int z, int coordinate, Board board, float yOffset)
    {
        float posX = (x - (board.size - 1) / 2);
        float posZ = (z - (board.size - 1) / 2);

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

        var instance = EntityManager.Instantiate(board.wallPrefab);
        SetComponent(instance, new Translation { Value = new float3(posX, 0.725f + yOffset, posZ) });

        if (coordinate <= 1)  // Turn the wall if it's placed on North or South
            SetComponent(instance, new Rotation { Value = quaternion.RotateY(math.radians(90f)) });
    }
}


