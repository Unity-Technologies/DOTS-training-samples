using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Random = Unity.Mathematics.Random;

class AIInputSystem : SystemBase
{
    private EntityCommandBufferSystem commandBufferSystem;
    private EntityArchetype arrowRequestArchetype;
    private Random m_Random = new Random(1234);

    protected override void OnCreate()
    {
        arrowRequestArchetype = EntityManager.CreateArchetype(typeof(ArrowRequest));
    }

    protected override void OnUpdate()
    {
        if (m_Random.NextFloat(100f) > 99.5f)
        {
            GridDirection dir = (GridDirection) (1 << m_Random.NextInt(0, 3));
            int playerId = m_Random.NextInt(1, ConstantData.Instance.NumPlayers);
            int2 position = new int2(m_Random.NextInt(0, ConstantData.Instance.BoardDimensions.x),
                m_Random.NextInt(0, ConstantData.Instance.BoardDimensions.y));
            Entity e = EntityManager.CreateEntity(arrowRequestArchetype);
            EntityManager.SetComponentData(e, new ArrowRequest
            {
                Direction = dir,
                OwnerID = playerId,
                Position = position
            });
        }
    }
}