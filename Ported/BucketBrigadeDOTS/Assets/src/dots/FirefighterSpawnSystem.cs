using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(BucketSpawnSystem))]
public class FirefighterSpawnSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        Bounds gridBounds = EntityManager.GetComponentData<Bounds>(fireGridEntity);
        float2 gridBottomLeft = gridBounds.BoundsCenter - gridBounds.BoundsExtent * 0.5f;
        float2 gridTopRight = gridBounds.BoundsCenter + gridBounds.BoundsExtent * 0.5f;

        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in FirefighterSpawner spawner, in LocalToWorld ltw) =>
            {
                int firefightersInLineCount = spawner.Count;
                int firefightersTotalCount = spawner.Count * 2;

                var previousInstance = Entity.Null;
                var first = Entity.Null;
                
                var rand = new Unity.Mathematics.Random(876);

                for (int i = 0; i < firefightersTotalCount; ++i)
                {
                    float2 pos = rand.NextFloat2(gridBottomLeft, gridTopRight);
                    var instance = EntityManager.Instantiate(spawner.Prefab);

                    if (first == Entity.Null)
                        first = instance;
                    
                    EntityManager.SetComponentData(instance, new Translation2D { Value = pos });
                    EntityManager.AddComponent<Firefighter>(instance);
                    
                    if (i < spawner.Count)
                        EntityManager.AddComponent<FirefighterFullTag>(instance);
                    else
                        EntityManager.AddComponent<FirefighterEmptyTag>(instance);
                    
                    if (i == spawner.Count - 1)
                        EntityManager.AddComponent<FirefighterFullLastTag>(instance);
                    
                    if (i == spawner.Count * 2 - 1)
                        EntityManager.AddComponent<FirefighterEmptyLastTag>(instance);

                    float positionInLine = firefightersInLineCount > 1 ? (float)(i % firefightersInLineCount) / (firefightersInLineCount - 1) : 0;
                    EntityManager.AddComponentData<FirefighterPositionInLine>(instance, new FirefighterPositionInLine { Value = positionInLine });

                    EntityManager.AddComponentData<FirefighterNext>(instance, new FirefighterNext { Value = previousInstance });

                    previousInstance = instance;
                }

                EntityManager.SetComponentData(first, new FirefighterNext { Value = previousInstance });

                EntityManager.DestroyEntity(entity);
            }).Run();
    }
}
