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
            ComponentDataFromEntity<Translation> translationComponents = GetComponentDataFromEntity<Translation>();

            EntityQuery cats = GetEntityQuery(ComponentType.ReadWrite<Scaling>(), ComponentType.ReadOnly<Cat>(), ComponentType.ReadOnly<Velocity>(), ComponentType.ReadOnly<Translation>());

            Entities
                .WithAll<Rat, Velocity, InPlay>()
                .ForEach((Entity entity, in Translation translation) =>
                {
                    foreach (Entity cat in cats.ToEntityArray(Allocator.Temp))
                    {
                        Translation catTranslation = translationComponents[cat];
                        if (Vector2.Distance(new Vector2(catTranslation.Value.x, catTranslation.Value.z), new Vector2(translation.Value.x, translation.Value.z)) < 0.5 && catTranslation.Value.y >= -0.5f)
                        {
                            ecb.DestroyEntity(entity);
                            ecb.SetComponent<Scale>(cat, new Scale { Value = 2 });
                        }
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}
