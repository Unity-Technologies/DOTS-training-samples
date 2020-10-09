#define COLLISION_DEBUG_DRAW
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
        mEntityQuery = GetEntityQuery(typeof(Car), typeof(Translation), typeof(RoadId), typeof(CarMovement));
    }
    
    static float DistanceFromLineSegmentToSphere(float3 linePointStart, float3 linePointEnd, float3 sphereCenter, float sphereRadius)
    {
        float3 lineDiffVect = linePointEnd - linePointStart;
        float lineSegSqrLength = math.lengthsq(lineDiffVect);

        float3 lineToPointVect = sphereCenter - linePointStart;
        float dotProduct = math.dot(lineDiffVect, lineToPointVect);

        float percAlongLine = dotProduct / lineSegSqrLength;

        if ( percAlongLine < 0.0f )
        {
            percAlongLine = 0.0f;
        }
        else if ( percAlongLine > 1.0f )
        {
            percAlongLine = 1.0f;
        }

        float3 intersectionPt = linePointStart + (  percAlongLine  * ( linePointEnd - linePointStart ));

        float3 spherePtToIntersect = intersectionPt - sphereCenter;
        float sqrLengSphereToLine = math.lengthsq(spherePtToIntersect);

        return sqrLengSphereToLine;
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
            var carMovementArray = mEntityQuery.ToComponentDataArray<CarMovement>(Allocator.TempJob);

            if (entityArray.Length != translationArray.Length)
            {
                throw new System.InvalidOperationException("CollisionSystem expected entityArray.Length and translationArray.Length to be equal");
            }

            const float maxCollisionDist = 6;
            const float minCollisionDist = 2;
            const float maxCollisionDistRoot = 2.44f;
            
            #if COLLISION_DEBUG_DRAW
            var blue = new float4(0, 0, 1, 1);
            var yellow = new float4(1, 1, 0, 1);
            var red = new float4(1, 0, 0, 1);
            #endif

            Entities
                .WithSharedComponentFilter(road)
                .WithReadOnly(entityArray)
                .WithReadOnly(translationArray)
                .WithReadOnly(carMovementArray)
                .WithDeallocateOnJobCompletion(entityArray)
                .WithDeallocateOnJobCompletion(translationArray)
                .ForEach((Entity entity, ref Color color, ref CarMovement carMovement, in LocalToWorld localToWorld, in Translation translation, in Car car) =>
            {
                float hitDist = 1000000;

                var hitDetectionPoint = localToWorld.Position + (carMovement.travelVec * maxCollisionDistRoot);
                
                for (var i = 0; i < entityArray.Length; ++i)
                {
                    //Ignore self
                    if (entityArray[i] == entity)
                        continue;

                    //Ignore cars not travelling in the same general direction
                    if (math.dot(carMovementArray[i].travelVec, carMovement.travelVec) < 0)
                        continue;
                    
                    var dist = DistanceFromLineSegmentToSphere(translation.Value, hitDetectionPoint,
                        translationArray[i].Value, 1.0f);
                    
                    if (dist < maxCollisionDist)
                    {
                        //Detect cars in front of me
                        float3 toTarget = translationArray[i].Value - translation.Value;
                        toTarget = math.normalize(toTarget);
                        if (math.dot(carMovement.travelVec, toTarget) > .7)
                        {
                            hitDist = dist;
                            break;
                        }
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