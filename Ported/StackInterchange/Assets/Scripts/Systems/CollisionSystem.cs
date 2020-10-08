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

    protected override void OnUpdate()
    {
        /*var origin = new float2(-5000, -5000);
        var maxSize = new float2(5000, 5000);
        var cellSize = 50f;
        int numCols = (int)(maxSize.x / cellSize);
        int numRows = (int)(maxSize.y / cellSize);

        Entities
            .WithStructuralChanges()
            .ForEach((Entity entity, in RoadId roadId, in Translation translation) =>
        {
            int col = (int)((translation.Value.x - origin.x) / cellSize);
            int row = (int) ((translation.Value.y - origin.y) / cellSize);
            var cell = (row * numCols) + col;
            
            if (roadId.Value != cell)
            {
                EntityManager.SetSharedComponentData<RoadId>(entity, new RoadId {Value = cell});
            }
        }).Run();*/
        
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

                var hitDetectionPoint = localToWorld.Position + (carMovement.travelVec * 1);
                
                for (var i = 0; i < entityArray.Length; ++i)
                {
                    if (entityArray[i] == entity)
                        continue;

                    var dist = math.distancesq(hitDetectionPoint, translationArray[i].Value);
                    if (dist < maxCollisionDist)
                    {
                        if (math.dot(carMovementArray[i].travelVec, carMovement.travelVec) > 0)
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