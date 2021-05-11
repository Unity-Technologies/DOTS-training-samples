using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class SpawnBoardSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var random = new Random(1234);

        var gameObjectRefs = this.GetSingleton<GameObjectRefs>();

        Entities
            .WithNone<BoardInitializedTag>()
            .ForEach((Entity entity, ref GameData gameData) =>
            {
                var boardEntity = ecb.CreateEntity(); // ???

                var boardDefinition = new BoardDefinition
                {
                    // TODO: Get the following from authoring:
                    CellSize = 1,
                    NumberColumns = 13,
                    NumberRows = 13,
                };
                ecb.AddComponent(boardEntity, boardDefinition);

                var firstCellPosition = new FirstCellPosition
                {
                    // TODO: Also get the following from authoring:
                    Value = new float3(0, 0, 0)
                };
                ecb.AddComponent(boardEntity, firstCellPosition);

                // TODO: Add time?

                // TODO: Set up walls
                // TODO: Set up spawners
                // TODO: Set up goals
                // TODO: Set up holes
                // ...
                // TODO: Set up Game Data

                // Only run on first frame the BoardInitializedTag is not found. Add it so we don't run again
                ecb.AddComponent(entity, new BoardInitializedTag());
            }).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
