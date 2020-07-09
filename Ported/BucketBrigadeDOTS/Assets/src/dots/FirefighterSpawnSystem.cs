using System.ComponentModel.Design.Serialization;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(BucketSpawnSystem))]
public class FirefighterSpawnSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in FirefighterSpawner spawner, in LocalToWorld ltw) =>
            {
                int firefightersInLineCount = spawner.Count;
                int firefightersTotalCount = spawner.Count * 2;

                var previousInstance = Entity.Null;
                var first = Entity.Null;
                
                for (int i = 0; i < firefightersTotalCount; ++i)
                {
                    var posX = 2 * (i - (spawner.Count - 1) / 2);
                    var instance = EntityManager.Instantiate(spawner.Prefab);

                    if (first == Entity.Null)
                        first = instance;

                    SetComponent<Translation2D>(instance, new Translation2D { Value = ltw.Position.xz + new float2(posX, 0) });
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
