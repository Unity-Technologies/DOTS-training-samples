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
            // If a water well is close to the bucket, just start filling it
            var wells = waterWellQuery.ToEntityArray(Allocator.TempJob);
            Entities
                .WithDeallocateOnJobCompletion(wells)
                .ForEach((ref FillAmount fill, in LocalToWorld translation) =>
                {
                    for (int wellIndex = 0; wellIndex < wells.Length; wellIndex++)
                    {
                        var distance = math.distance(GetComponent<LocalToWorld>(wells[wellIndex]).Position,
                            translation.Position);
                        if (distance <= 0.1f)
                        {
                            fill.Value += 0.1f;
                        }
                    }
                    
                }).Schedule();
        }
    }
}