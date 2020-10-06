using Unity.Entities;
using Unity.Mathematics;
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
                    {
                        SetComponent(instance, new Color
                        {
                            Value = new float4(1.0f, 0.0f, 0.0f, 0.7f)
                        });
                    }

                    float wallPosX = (x - (board.size - 1) / 2);
                    float wallPosZ = (z - (board.size - 1) / 2);

                    if (z == 0 || z == (board.size - 1))
                    {
                        var wallPosZ2 = wallPosZ;
                        if (z == 0)
                            wallPosZ2 -= 0.5f;
                        else if (z == (board.size - 1))
                            wallPosZ2 += 0.5f;

                        var wallInstance = EntityManager.Instantiate(board.wallPrefab);
                        SetComponent(wallInstance, new Translation
                        {
                            Value = ltw.Position + new float3(wallPosX, 0.725f + posY, wallPosZ2)
                        });
                        SetComponent(wallInstance, new Rotation
                        {
                            Value = quaternion.RotateY(math.radians(90f))
                        });
                    }
                    
                    if (x == 0 || x == (board.size - 1))
                    {
                        if (x == 0)
                            wallPosX -= 0.5f;
                        else if (x == (board.size - 1))
                            wallPosX += 0.5f;

                        var wallInstance = EntityManager.Instantiate(board.wallPrefab);
                        SetComponent(wallInstance, new Translation
                        {
                            Value = ltw.Position + new float3(wallPosX, 0.725f + posY, wallPosZ)
                        });
                    }
                }
                
                EntityManager.DestroyEntity(entity);
        }).Run();
    }
}


