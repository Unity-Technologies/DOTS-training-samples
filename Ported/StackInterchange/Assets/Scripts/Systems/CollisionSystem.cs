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

        var deltaTime = Time.DeltaTime;
        
        foreach (var road in roadList)
        {
            mEntityQuery.SetSharedComponentFilter(road);
            var entityArray = mEntityQuery.ToEntityArray(Allocator.TempJob);
            var translationArray = mEntityQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            if (entityArray.Length != translationArray.Length)
            {
                throw new System.InvalidOperationException("CollisionSystem expected entityArray.Length and translationArray.Length to be equal");
            }

            const float maxCollisionDist = 6;
            const float minCollisionDist = 2;
            
            #if COLLISION_DEBUG_DRAW
            var blue = new float4(0, 0, 1, 1);
            var yellow = new float4(1, 1, 0, 1);
            var red = new float4(1, 0, 0, 1);
            #endif

            Entities.WithSharedComponentFilter(road)
                .ForEach((Entity entity, ref Color color, ref CarMovement carMovement, in LocalToWorld localToWorld, in Translation translation, in Car car) =>
            {
                float hitDist = 1000000;

                var hitDetectionPoint = localToWorld.Position + (carMovement.travelVec * 1);
                
                for (var i = 0; i < entityArray.Length; ++i)
                {
                    if (entityArray[i] == entity)
                        continue;

                    var dist = math.distancesq(hitDetectionPoint, translationArray[i].Value);
                    if (dist < maxCollisionDist)
                    {
                        hitDist = dist;
                        break;
                    }
                }

                if (hitDist < minCollisionDist)
                {
                    carMovement.Velocity = 0;
                }
                else if(hitDist < maxCollisionDist)

                {
                    carMovement.Velocity -= carMovement.Deceleration * deltaTime;
                    carMovement.Velocity = math.max(carMovement.Velocity, 0);
                }
                else
                {
                    carMovement.Velocity += carMovement.Acceleration * deltaTime;
                    carMovement.Velocity = math.min(carMovement.Velocity, carMovement.MaxSpeed);
                }
                
                #if COLLISION_DEBUG_DRAW
                if (hitDist < maxCollisionDist)
                {
                    color.Value = math.lerp(red, yellow, (hitDist - minCollisionDist) / (maxCollisionDist - minCollisionDist));
                }
                else
                {
                    color.Value = blue;
                }
                #endif
            }).ScheduleParallel();
        }
    }
}