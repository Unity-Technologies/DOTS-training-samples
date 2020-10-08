//#define COLLISION_DEBUG_DRAW
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CollisionSystem : SystemBase
{
    EntityQuery mEntityQuery;

    protected override void OnCreate()
    {
        mEntityQuery = GetEntityQuery(typeof(Car), typeof(Translation), typeof(RoadId));
    }

    protected override void OnUpdate()
    {
        List<RoadId> roadList = new List<RoadId>();
        EntityManager.GetAllUniqueSharedComponentData<RoadId>(roadList);
        
        foreach (var road in roadList)
        {
            mEntityQuery.SetSharedComponentFilter(road);
            var entityArray = mEntityQuery.ToEntityArray(Allocator.TempJob);
            var translationArray = mEntityQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            
            Entities.WithSharedComponentFilter(road)
                .ForEach((Entity entity, ref Color color, in Translation translation, in Car car, in LocalToWorld localToWorld) =>
            {
                if (entityArray.Length != translationArray.Length)
                    return;

                float hitDist = 0;
                
                for (var i = 0; i < entityArray.Length; ++i)
                {
                    if (entityArray[i] == entity)
                        continue;

                    var dist = math.distancesq(translation.Value, translationArray[i].Value);
                    if (dist < .1)
                    {
                        hitDist = dist;
                        break;
                    }
                }

                #if COLLISION_DEBUG_DRAW
                var blue = new float4(0,0,1,1);
                var yellow = new float4(1,1,0,1);
                color.Value = hitDist > .001f ? yellow : blue;
                #endif
            }).ScheduleParallel();
        }
    }
}