using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(FireSimulationSystem))]
public class ExtinguishSystem : SystemBase
{
    public int ExtinguishDistance;
    public int ExtinguishAmountAtMaxDistance;

    public JobHandle Deps;

    private EntityCommandBufferSystem m_EcbSystem;

    protected override void OnCreate()
    {
        m_EcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var grid = GridData.Instance;
        var ecb = m_EcbSystem.CreateCommandBuffer().ToConcurrent();
        var radius = ExtinguishDistance;
        var amountAtMaxDistance = ExtinguishAmountAtMaxDistance;
        Entities
            .WithName("FireExtinguish")
            .ForEach((int entityInQueryIndex, Entity entity, in ExtinguishData data) =>
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);

                var address = new Vector2Int(data.X, data.Y);
                if (!grid.InBounds(address))
                    return;

                int minX = math.max(address.x - radius, 0);
                int maxX = math.min(address.x + radius, grid.Width - 1);
                int minY = math.max(address.y - radius, 0);
                int maxY = math.min(address.y + radius, grid.Height - 1);
                var otherAddress = new Vector2Int();
                for (int x = minX; x < maxX; x++)
                {
                    otherAddress.x = x;
                    for (int y = minY; y < maxY; y++)
                    {
                        otherAddress.y = y;
                        var distance = math.abs(address.x - otherAddress.x) + math.abs(address.y - otherAddress.y);
                        if (distance > radius)
                            continue;

                        int index = grid.GetIndex(otherAddress);
                        var t = distance / (float)radius;
                        var value = (byte)math.round(byte.MaxValue * (1 - t) + amountAtMaxDistance * t);
                        grid.Heat[index] = (byte)math.max(0, grid.Heat[index] - value);
                    }
                }

            }).Schedule();
        m_EcbSystem.AddJobHandleForProducer(Dependency);
        Deps = Dependency;
    }
}