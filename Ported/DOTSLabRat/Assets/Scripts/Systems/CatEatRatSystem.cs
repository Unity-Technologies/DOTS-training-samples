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
        EntityCommandBufferSystem CommandBufferSystem;

        protected override void OnCreate()
        {
            CommandBufferSystem
                = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            ComponentDataFromEntity<Translation> translationComponents = GetComponentDataFromEntity<Translation>();

            NativeArray<Entity> cats = GetEntityQuery(ComponentType.ReadWrite<Scaling>(), ComponentType.ReadOnly<Cat>()).ToEntityArray(Allocator.Temp);
            NativeArray<Vector2> catTranslation = new NativeArray<Vector2>(cats.Length, Allocator.Temp);

            for (int i = 0; i < cats.Length; i++)
            {
                if(translationComponents[cats[i]].Value.y == -0.5f)
                {
                    catTranslation[i] = new Vector2(translationComponents[cats[i]].Value.x, translationComponents[cats[i]].Value.z);
                }
            }

            Entities
                .WithAll<Rat, Velocity, InPlay>()
                .WithNativeDisableContainerSafetyRestriction(catTranslation)
                .WithNativeDisableContainerSafetyRestriction(cats)
                .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
                {
                    for (int i = 0; i < catTranslation.Length; i++)
                    {
                        if (Vector2.Distance(catTranslation[i], new Vector2(translation.Value.x, translation.Value.z)) < 0.5)
                        {
                            ecb.DestroyEntity(entityInQueryIndex, entity);
                            ecb.SetComponent<Scale>(entityInQueryIndex, cats[i], new Scale { Value = 2 });
                        }
                    }
                }).Schedule();

            cats.Dispose();
            catTranslation.Dispose();
        }
    }
}
