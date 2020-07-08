using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    public class FindTillableFieldSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<TillField_Intent>()
                .WithNone<TillRect>()
                .WithStructuralChanges()
                .ForEach((Entity entity) =>
                {
                    int2 GridSize = GetSingleton<Grid>().Size;
                    int2 fieldPos, fieldDimensions;

                    // Find a suitable area to till on the farm
                    {
                        // For now, just any random rectangle that fits on the farm
                        // Without considering rocks, shops, or anything that's already tilled

                        fieldPos.x = UnityEngine.Random.Range(0, GridSize.x);
                        fieldPos.y = UnityEngine.Random.Range(0, GridSize.y);

                        fieldDimensions.x = UnityEngine.Random.Range(2, 5);
                        fieldDimensions.y = UnityEngine.Random.Range(2, 5);

                        fieldDimensions.x = math.min(fieldDimensions.x, GridSize.x - fieldPos.x);
                        fieldDimensions.y = math.min(fieldDimensions.y, GridSize.y - fieldPos.y);
                    }

                    EntityManager.RemoveComponent<PathFindingTargetReached_Tag>(entity);
                    EntityManager.AddComponent<TillRect>(entity);
                    SetComponent(entity, new TillRect
                    {
                        X = fieldPos.x,
                        Y = fieldPos.y,
                        Width = fieldDimensions.x,
                        Height = fieldDimensions.y
                    });
                    
                }).Run();
        }
    }
}
