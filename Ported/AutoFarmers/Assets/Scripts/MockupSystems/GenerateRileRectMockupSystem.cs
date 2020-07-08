using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    public class GenerateRileRectMockupSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithNone<TillRect>()
                .WithStructuralChanges()
                .WithAll<Farmer_Tag>()
                .ForEach((Entity entity) =>
                {
                    var grid = GetSingleton<Grid>();
                    
                    int2 randomPos, randomDims;
                    randomPos.x = UnityEngine.Random.Range(0, grid.Size.x);
                    randomPos.y = UnityEngine.Random.Range(0, grid.Size.y);
                    
                    randomDims.x = UnityEngine.Random.Range(1, 4);
                    randomDims.y = UnityEngine.Random.Range(1, 4);

                    randomDims.x = math.min(randomDims.x, grid.Size.x - randomPos.x);
                    randomDims.y = math.min(randomDims.y, grid.Size.y - randomPos.y);

                    UnityEngine.Debug.Log("HELOO: " + randomPos.ToString() + "  " + randomDims.ToString());

                    EntityManager.AddComponent<TillRect>(entity);
                    SetComponent(entity, new TillRect
                    {
                        X = randomPos.x,
                        Y = randomPos.y,
                        Width = randomDims.x,
                        Height = randomDims.y
                    });
                    
                    EntityManager.RemoveComponent<PathFindingTargetReached_Tag>(entity);
                }).Run();
        }
    }
}
