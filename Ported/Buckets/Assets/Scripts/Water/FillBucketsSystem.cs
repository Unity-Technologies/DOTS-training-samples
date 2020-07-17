using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Water
{
    public class FillBucketsSystem : SystemBase
    {
        private EntityQuery waterWellQuery;
        
        protected override void OnCreate()
        {
            waterWellQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(WellTag));
        }
        protected override void OnUpdate()
        {
            // fill buckets attached to a well
            // var wells = waterWellQuery.ToEntityArray(Allocator.TempJob);
            Entities
                // .WithDeallocateOnJobCompletion(wells)
                .ForEach((ref FillAmount fill, in Attached attachedEntity) =>
                {
                    if (HasComponent<WellTag>(attachedEntity.Value))
                    {
                        fill.Value += 0.1f;
                        fill.Value = math.max(1f, fill.Value);
                    }
                }).Schedule();
        }
    }
}