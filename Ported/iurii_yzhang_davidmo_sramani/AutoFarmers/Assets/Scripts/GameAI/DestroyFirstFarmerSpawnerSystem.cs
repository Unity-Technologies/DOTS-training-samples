using GameAI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

namespace GameAI
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class DestroyFirstFarmerSpawnerSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var entity = GetEntityQuery(typeof(InitialSpawnerTagComponent)).GetSingletonEntity();
            EntityManager.DestroyEntity(entity);
        }
    }
}
