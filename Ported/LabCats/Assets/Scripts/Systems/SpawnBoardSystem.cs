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

        Entities
            .WithNone<BoardInitializedTag>()
            .WithoutBurst()
            .ForEach((Entity entity, ref GameData gameData, in BoardDefinition boardDefinition) =>
            {
                var firstCellPosition = new FirstCellPosition
                {
                    // TODO: Also get the following from authoring:
                    Value = new float3(0, 0, 0)
                };
                ecb.AddComponent(entity, firstCellPosition);

                // TODO: Add time?

                // TODO: Set up walls
                // TODO: Set up spawners
                // TODO: Set up goals
                // TODO: Set up holes
                // ...
                // TODO: Set up Game Data

                // Setup camera
                var gameObjectRefs = this.GetSingleton<GameObjectRefs>();
                var camera = gameObjectRefs.Camera;
                camera.orthographic = true;
                var overheadFactor = 1.5f;

                var maxSize = Mathf.Max(boardDefinition.NumberRows, boardDefinition.NumberColumns);
                var maxCellSize = boardDefinition.CellSize;
                camera.orthographicSize = maxSize * maxCellSize * .65f;

                // scale based on board dimensions - james
                var posXZ = Vector2.Scale(new Vector2(boardDefinition.NumberRows, boardDefinition.NumberColumns) * 0.5f, new Vector2(boardDefinition.CellSize, boardDefinition.CellSize));

                // hold position value adjusted by dimensions of board
                float3 camPosition = new Vector3(0, maxSize * maxCellSize * overheadFactor, 0);
                camera.transform.position = camPosition;

                // set camera to look at board center
                camera.transform.LookAt(new Vector3(posXZ.x, 0f, posXZ.y));


                // Only run on first frame the BoardInitializedTag is not found. Add it so we don't run again
                ecb.AddComponent(entity, new BoardInitializedTag());
            }).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
