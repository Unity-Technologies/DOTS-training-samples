using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

public class SpawnBoardSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var random = new Unity.Mathematics.Random(1234);

        var gameObjectRefs = this.GetSingleton<GameObjectRefs>();

        // set reference to MainCamera gameObject - james
        var camera = this.GetSingleton<GameObjectRefs>().Camera;

        // orthographic default state for camera - james
        camera.orthographic = true;

        // adjustment to orthograhpic angle - james
        var overheadFactor = 1.5f;

        var maxSize = Mathf.Max(NumberRows, NumberColumns);
        var maxCellSize = Mathf.Max(CellSize.x, CellSize.y);

        // set the orthographic size based on board and cell dimensions - james
        camera.orthographicSize = maxSize * maxCellSize * .65f;

        // scale based on board dimensions - james
        var posXZ = Vector2.Scale(new Vector2(NumberRows, NumberColumns) * 0.5f, CellSize);

        // hold position value adjusted by dimensions of board - james
        float3 camPosition = new Vector3(0, maxSize * maxCellSize * overheadFactor, 0);

        // set camera position to modified value - james
        camera.transform.position = camPosition;

        // set camera to look at board center - james
        camera.transform.LookAt(new Vector3(posXZ.x, 0f, posXZ.y));

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
