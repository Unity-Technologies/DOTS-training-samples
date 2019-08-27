using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

public class PlayerCursorInitializerSystem : ComponentSystem
{
    private EntityQuery m_Cursors;
    private EntityQuery m_BoardQuery;

    protected override void OnCreate()
    {
        m_Cursors = GetEntityQuery(ComponentType.ReadOnly<LbCursorInit>(), typeof(LbMovementTarget), typeof(LbDistanceToTarget));
        m_BoardQuery = GetEntityQuery(ComponentType.ReadOnly<LbBoard>());
    }

    protected override void OnUpdate()
    {
        var cursorsToInit = m_Cursors.CalculateEntityCount();
        if (cursorsToInit <= 0)
            return;

        var boardCount = m_BoardQuery.CalculateEntityCount();
        if (boardCount <= 0)
            return;

        var board = m_BoardQuery.GetSingleton<LbBoard>();
        var random = new Unity.Mathematics.Random();
        random.InitState(1);

        Entities.ForEach((Entity entity, ref LbCursorInit ci, ref LbMovementTarget movement, ref LbDistanceToTarget distance) =>
        {
            var position = new float3(random.NextInt(0, board.SizeX), 1, random.NextInt(0, board.SizeY));
            movement.To = position;
            distance.Value = 0.0f;

            World.EntityManager.RemoveComponent<LbCursorInit>(entity);
        });
    }
}
