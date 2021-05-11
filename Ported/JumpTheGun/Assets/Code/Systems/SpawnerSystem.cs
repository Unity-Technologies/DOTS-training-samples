using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges().ForEach((Entity boardEntity, in Board board) =>
        {
            EntityManager.AddComponentData(boardEntity, new BoardSize {Value = new int2(board.SizeX, board.SizeY)});
            EntityManager.AddComponent<OffsetList>(boardEntity);

            var offsets = OffsetGenerator.CreateRandomOffsets(board.SizeX, board.SizeY, board.MinHeight, board.MaxHeight, Allocator.Temp);
            
            // TODO find a better way to do that.
            var totalSize = board.SizeX * board.SizeY;
            var buffer = EntityManager.AddBuffer<OffsetList>(boardEntity);
            buffer.ResizeUninitialized(totalSize);

            for (int i = 0; i < totalSize; ++i)
            {
                buffer[i] = new OffsetList {Value = offsets[i]};
            }
            
            for (int y = 0; y < board.SizeY; ++y)
            {
                for (int x = 0; x < board.SizeX; ++x)
                {
                    var instance = EntityManager.Instantiate(board.PlaformPrefab);

                    var localToWorld = math.mul(
                        math.mul(
                            float4x4.Translate(new float3(x, -0.5f, y)),
                            float4x4.Scale(1f, offsets[y * board.SizeX + x], 1f)),
                        float4x4.Translate(new float3(0f, 0.5f, 0f)));
                    
                    EntityManager.RemoveComponent<Translation>(instance);

                    float4 color = Colorize.Platform(localToWorld.c3.y, board.MinHeight, board.MaxHeight);
                    EntityManager.SetComponentData(instance, new URPMaterialPropertyBaseColor { Value = color });

                    EntityManager.RemoveComponent<Rotation>(instance);
                    
                    EntityManager.SetComponentData(instance, new LocalToWorld {Value = localToWorld});
                }
            }

            // TODO Remove that.
            EntityManager.DestroyEntity(boardEntity);
        }).Run();
    }
}