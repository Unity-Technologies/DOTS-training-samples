
using Unity.Entities;
using UnityEngine;

public class InitPlayerSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<InitPlayerState>();
    }

    protected override void OnUpdate()
    {
        Debug.Log("Creating players");

        Entity playerManagerEntity = EntityManager.CreateEntity();

        EntityManager.SetName(playerManagerEntity, "PlayerManager");
        EntityManager.AddComponentData(playerManagerEntity, new PlayerManager()
        {
            Player0 = CreatePlayer(EntityManager, 0),
            Player1 = CreatePlayer(EntityManager, 1),
            Player2 = CreatePlayer(EntityManager, 2),
            Player3 = CreatePlayer(EntityManager, 3),
        });

        // Remove the initializer component to prevent this from running again.
        EntityManager.RemoveComponent<InitPlayerState>(GetSingletonEntity<InitPlayerState>());
    }

    private Entity CreatePlayer(EntityManager entityManager, int playerIndex)
    {
        Entity entity = entityManager.CreateEntity();
        Player player = new Player()
        {
            targetEntity = entity,
            index = playerIndex,
            score = 0,
            nextArrowIndex = 0,
        };

        entityManager.SetName(entity, $"Player {playerIndex}");
        entityManager.AddComponentData(entity, player);

        // This entity array will act as a ring buffer of board arrows belonging to this player.
        var arrowBuffer = entityManager.AddBuffer<BoardArrowBufferElement>(entity);

        arrowBuffer.Add(new BoardArrowBufferElement() { Value = Entity.Null });
        arrowBuffer.Add(new BoardArrowBufferElement() { Value = Entity.Null });
        arrowBuffer.Add(new BoardArrowBufferElement() { Value = Entity.Null });

        return entity;
    }
}
