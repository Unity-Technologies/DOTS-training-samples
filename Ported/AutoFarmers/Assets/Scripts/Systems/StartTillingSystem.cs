using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AutoFarmers
{
    public class StartTillingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<PathFindingTargetReached_Tag>()
                .WithStructuralChanges()
                .ForEach((Entity entity, in TillRect tillRect, in Translation translation) =>
                {
                    int gridX = (int) (translation.Value.x);
                    int gridY = (int) (translation.Value.z);

                    int2 dims = GetSingleton<Grid>().Size;
                    var grid = GetSingletonEntity<Grid>();
                    var buffer = EntityManager.GetBuffer<CellEntityElement>(grid);
                    
                    EntityManager.AddComponent<Tilled>(buffer[gridX * dims.x + gridY].Value);
                    EntityManager.RemoveComponent<PathFindingTargetReached_Tag>(entity);
                }).Run();
        }
    }
}
