using System;
using src.Components;
using Unity.Entities;

namespace src.Systems
{
    /// <inheritdoc cref="EcsTick"/>>
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(BeginInitializationEntityCommandBufferSystem))]
    public class TickSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!TryGetSingleton(out EcsTick tick))
            {
                var entity = EntityManager.CreateEntity(ComponentType.ReadWrite<EcsTick>());
                EntityManager.SetName(entity, nameof(EcsTick));
            }
            tick.CurrentTick++;
            SetSingleton(tick);
        }
    }
}
