
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct PlayerManager : IComponentData
{
    public static Entity AddBoardArrow(
        EntityManager entityManager,
        Entity playerEntity,
        EntityDirection direction,
        int2 gridPosition
    )
    {
        const float halfPi = math.PI * 0.5f;

        var player = entityManager.GetComponentData<Player>(playerEntity);
        var arrowBuffer = entityManager
            .GetBuffer<BoardArrowBufferElement>(playerEntity)
            .Reinterpret<Entity>();

        var currentArrowEntity = arrowBuffer[player.nextArrowIndex];
        if (currentArrowEntity != Entity.Null)
        {
            entityManager.DestroyEntity(currentArrowEntity);
        }

        Entity newArrowEntity = entityManager.Instantiate(player.arrowPrefab);

        entityManager.SetName(newArrowEntity, $"BoardArrow {player.nextArrowIndex} (Player {player.index})");
        entityManager.AddComponentData(newArrowEntity, new BoardArrow()
        {
            targetEntity = newArrowEntity,
            direction = direction,
            gridPosition = gridPosition,
            playerIndex = player.index,
        });
        entityManager.SetComponentData(newArrowEntity, new Translation()
        {
            Value = new float3(gridPosition.x, 0.6f, gridPosition.y),
        });
        entityManager.SetComponentData(newArrowEntity, new Rotation()
        {
            Value = quaternion.RotateY(((float) direction * halfPi) + math.PI),
        });

        // Structural changes occurred, so the buffer has been invalidated.  Get it again to work around that.
        arrowBuffer = entityManager
            .GetBuffer<BoardArrowBufferElement>(playerEntity)
            .Reinterpret<Entity>();

        arrowBuffer.ElementAt(player.nextArrowIndex) = newArrowEntity;
        player.nextArrowIndex = (player.nextArrowIndex + 1) % 3;

        // Set the player component back to the player entity to persist changes.
        entityManager.SetComponentData(playerEntity, player);

        return newArrowEntity;
    }

    public static void RemoveBoardArrow(EntityManager entityManager, Entity playerEntity, Entity boardArrowEntity)
    {
        var player = entityManager.GetComponentData<Player>(playerEntity);
        var arrowBuffer = entityManager
            .GetBuffer<BoardArrowBufferElement>(playerEntity)
            .Reinterpret<Entity>();

        // Find where the arrow to be removed is located within the player's arrow ring buffer.
        int bufferIndex;
        for (bufferIndex = 0; bufferIndex < arrowBuffer.Length; ++bufferIndex)
        {
            if (arrowBuffer[bufferIndex] == boardArrowEntity)
            {
                break;
            }
        }

        // Check if the board arrow entity was found in the player's arrow buffer.
        if (bufferIndex >= arrowBuffer.Length)
        {
            Debug.LogWarning($"Attempting to remove board arrow entity that does not belong to player {player.index}");
            return;
        }

        entityManager.DestroyEntity(boardArrowEntity);

        // Get the buffer again to account for structural changes in the entity manager.
        arrowBuffer = entityManager
            .GetBuffer<BoardArrowBufferElement>(playerEntity)
            .Reinterpret<Entity>();

        // Compact the arrow buffer so all entities within it are contiguous;
        for (; bufferIndex < arrowBuffer.Length - 1; ++bufferIndex)
        {
            arrowBuffer.ElementAt(bufferIndex) = arrowBuffer[bufferIndex + 1];
        }

        // Clear the final entity in the arrow buffer and make it the location where the next arrow will be added.
        arrowBuffer.ElementAt(arrowBuffer.Length - 1) = Entity.Null;

        // Search for the last free slot from the end to determine the next arrow index.
        for (bufferIndex = 0; bufferIndex < arrowBuffer.Length; ++bufferIndex)
        {
            if (arrowBuffer[bufferIndex] == Entity.Null)
            {
                break;
            }
        }

        Debug.Assert(bufferIndex < arrowBuffer.Length, $"Invalid board arrow buffer index: {bufferIndex}");

        player.nextArrowIndex = bufferIndex;

        // Set the player component back to the player entity to persist changes.
        entityManager.SetComponentData(playerEntity, player);
    }

    public Entity Player0;
    public Entity Player1;
    public Entity Player2;
    public Entity Player3;
}
