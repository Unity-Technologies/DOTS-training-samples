using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(BoardSpawnerSystem))]
public class PlayerSpawnerSystem : SystemBase
{
    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(OffsetList));
        RequireForUpdate(query);
    }
    
    protected override void OnUpdate()
    {
        Entity boardEntity;
        if (!TryGetSingletonEntity<BoardSize>(out boardEntity))
            return;

        var random = new System.Random();

        float3 targetPosition = new float3(0.0f, 0.0f, 0.0f);

        var boardSize = GetComponent<BoardSize>(boardEntity);
        var offsets = GetBuffer<OffsetList>(boardEntity);

        //TODO Randomize at the beginning.
        int startX = random.Next(0, boardSize.Value.x);
        int startY = random.Next(0, boardSize.Value.y);

        Entities
            .WithStructuralChanges()
            .WithReadOnly(offsets)
            .WithAll<PlayerSpawnerTag>()
            .ForEach((Entity player, ref NonUniformScale scale, in Radius radius) =>
            {
                int2 boardPos = new int2(startX, startY);
                float3 size = new float3(radius.Value * 2F, radius.Value * 2F, radius.Value * 2F);

                Camera mainCamera = Camera.main;


                float3 targetPos = CoordUtils.BoardPosToWorldPos(boardPos, offsets[CoordUtils.ToIndex(boardPos, boardSize.Value.x, boardSize.Value.y)].Value);

                float3 cameraPosition = new float3(targetPos.x, mainCamera.transform.position.y, targetPos.z);

                EntityManager.SetComponentData(player, new Translation { Value = targetPos });
                EntityManager.SetComponentData(player, new BoardPosition { Value = boardPos });
                EntityManager.SetComponentData(player, new NonUniformScale { Value = size });

                EntityManager.RemoveComponent<PlayerSpawnerTag>(player);

                mainCamera.transform.position = cameraPosition;
            }).Run();
    }
}