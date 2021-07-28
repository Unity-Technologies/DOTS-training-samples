using DOTSRATS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSRATS
{
    public class CatEatRatSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            EntityQuery cats = GetEntityQuery(ComponentType.ReadOnly<Cat>(), ComponentType.ReadOnly<Velocity>(), ComponentType.ReadOnly<Translation>());

            Entities
                .WithAll<Rat, Velocity, InPlay>()
                .ForEach((Entity entity, in Translation translation) =>
                {
                    foreach (Translation catTranslation in cats.ToComponentDataArray<Translation>(Allocator.Temp))
                    {
                        if (Vector2.Distance(new Vector2(catTranslation.Value.x, catTranslation.Value.z), new Vector2(translation.Value.x, translation.Value.z)) < 0.5)
                        {
                            ecb.DestroyEntity(entity);
                        }
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}
