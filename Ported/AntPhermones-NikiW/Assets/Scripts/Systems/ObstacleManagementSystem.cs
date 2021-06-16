using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ObstacleManagementSystem : SystemBase
{
    // NWALKER - THIS CAN BE IMPROVED! NO NEED TO DO SO MANY STEPS!
    [NoAlias]
    public NativeBitArray obstacleCollisionLookup;
    EntityQuery m_CircleObstacleGeneratorMarkerQuery;
    public EntityQuery obstaclesQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<AntSimulationParams>();
        m_CircleObstacleGeneratorMarkerQuery = GetEntityQuery(ComponentType.ReadOnly<CircleObstacleGeneratorMarker>(), ComponentType.ReadOnly<AntSimulationTransform2D>());
        obstaclesQuery = GetEntityQuery(ComponentType.ReadOnly<StaticObstacleFlag>(), ComponentType.ReadOnly<AntSimulationTransform2D>());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        obstacleCollisionLookup.Dispose();
    }

    protected override void OnUpdate()
    {
        var simParams = GetSingleton<AntSimulationParams>();

        if (!TryGetSingleton(out AntSimulationRuntimeData simRuntimeData))
        {
            obstacleCollisionLookup = new NativeBitArray(simParams.mapSize * simParams.mapSize, Allocator.Persistent);

            simRuntimeData.colonyPos = new float2(simParams.mapSize * .5f);
            var resourceAngle = Random.value * 2f * math.PI;
            simRuntimeData.foodPosition = simRuntimeData.colonyPos + (new float2(math.cos(resourceAngle), math.sin(resourceAngle)) * simParams.mapSize * .475f);
            EntityManager.CreateEntity(ComponentType.ReadOnly<AntSimulationRuntimeData>());
        }

        simRuntimeData.perFrameRandomSeed = (uint)(Random.value * uint.MaxValue);
        SetSingleton(simRuntimeData);
        
        if (!m_CircleObstacleGeneratorMarkerQuery.IsEmpty)
        {
            var transforms = m_CircleObstacleGeneratorMarkerQuery.ToComponentDataArray<AntSimulationTransform2D>(Allocator.Temp);
            var circleObstacleMarkers = m_CircleObstacleGeneratorMarkerQuery.ToComponentDataArray<CircleObstacleGeneratorMarker>(Allocator.Temp);
            EntityManager.DestroyEntity(m_CircleObstacleGeneratorMarkerQuery);

            for (int i = 0; i < circleObstacleMarkers.Length; i++)
                GenerateObstacles(transforms[i].position, circleObstacleMarkers[i], in simParams);

            //     var obstaclePositions = new NativeList<float2>(1024, Allocator.Temp);
            //     for (int circle = 0; circle < transforms.Length; circle++)
            //     {
            //         // Generate obstacles in a circle:
            //         var marker = circleObstacleMarkers[circle];
            //         var markerPos = transforms[circle];
            //         
            //         for (var i = 1; i <= marker.ringCount; i++)
            //         {
            //             var ringRadius = i / (marker.ringCount + 1f) * (marker.radius);
            //             var circumference = ringRadius * 2f * math.PI;
            //             var maxCount = Mathf.CeilToInt(circumference / (2f * marker.radius) * 2f);
            //             var offset = Random.Range(0, maxCount);
            //             var holeCount = Random.Range(1, 3);
            //             for (var j = 0; j < maxCount; j++)
            //             {
            //                 var t = (float)j / maxCount;
            //                 if (t * holeCount % 1f < marker.obstaclesPerRing)
            //                 {
            //                     var angle = (j + offset) / (float)maxCount * (2f * math.PI);
            //                 
            //                     var obstaclePosition = markerPos.position + (new float2(math.cos(angle), math.sin(angle)) * ringRadius);
            //                     obstaclePositions.Add(obstaclePosition);
            //
            //                     //Debug.DrawRay(obstacle.position / mapSize,-Vector3.forward * .05f,Color.green,10000f);
            //                 }
            //             }
            //         }
            //     }
            //     
            //     // Batch create the obstacle entities:
            //     if (obstaclePositions.Length > 0)
            //     {
            //         Debug.Assert(HasComponent<AntSimulationTransform2D>(simParams.obstaclePrefab), "ObstaclePrefab MUST have a Transform2D!");
            //         var entities = EntityManager.Instantiate(simParams.obstaclePrefab, obstaclePositions.Length, Allocator.Temp);
            //         for (var i = 0; i < entities.Length; i++)
            //         {
            //             // NWALKER: I don't like the fact that we have to set each component individually, BUT it doesn't seem possible to grab ALL of these at once.
            //             EntityManager.AddComponentData(entities[i], new AntSimulationTransform2D
            //             {
            //                 position = obstaclePositions[i],
            //             });
            //         }
            //
            //         entities.Dispose();
            //     }
            //     obstaclePositions.Dispose();
            //     transforms.Dispose();
            //     circleObstacleMarkers.Dispose();
            // }
            //
            // Entities.WithChangeFilter<StaticObstacleFlag>().ForEach((ref AntSimulationTransform2D trans) =>
            // {
            //     // NWALKER: THIS DOES NOT ACCOUNT FOR OBSTACLES BEING MOVED!
            //     var pos = trans.position;
            //     for (var x = Mathf.FloorToInt(pos.x - simParams.obstacleRadius); x <= Mathf.FloorToInt(pos.x + simParams.obstacleRadius); x++)
            //     {
            //         if (x < 0 || x >= simParams.mapSize)
            //             continue;
            //
            //         for (var y = Mathf.FloorToInt(pos.y - simParams.obstacleRadius); y <= Mathf.FloorToInt(pos.y + simParams.obstacleRadius); y++)
            //         {
            //             if (y < 0 || y >= simParams.mapSize)
            //                 continue;
            //
            //             var isInBounds = AntSimulationUtilities.CalculateIsInBounds(in x, in y, in simParams.mapSize, out var obstacleBucketIndex);
            //             if (math.all(isInBounds))
            //             {
            //                 obstacleCollisionLookupLocal.Set(obstacleBucketIndex, true);
            //             }
            //         }
            //     }
            // }).Run();
        }

        void GenerateObstacles(float2 position, CircleObstacleGeneratorMarker marker, in AntSimulationParams simParams)
        {
            // Generate obstacles in a circle:
            var obstaclePositions = new NativeList<float2>(1024, Allocator.Temp);
            for (var i = 1; i <= marker.ringCount; i++)
            {
                var ringRadius = i / (marker.ringCount + 1f) * (marker.radius * .5f);
                var circumference = ringRadius * 2f * math.PI;
                var maxCount = Mathf.CeilToInt(circumference / (2f * simParams.obstacleRadius) * 2f);
                var offset = Random.Range(0, maxCount);
                var holeCount = Random.Range(1, 3);
                for (var j = 0; j < maxCount; j++)
                {
                    var t = (float)j / maxCount;
                    if (t * holeCount % 1f < marker.obstaclesPerRingNormalized)
                    {
                        var angle = (j + offset) / (float)maxCount * (2f * math.PI);
                        var obstaclePosition = new float2(simParams.mapSize * .5f + math.cos(angle) * ringRadius, simParams.mapSize * .5f + math.sin(angle) * ringRadius);
                        obstaclePositions.Add(obstaclePosition);

                        //Debug.DrawRay(obstacle.position / mapSize,-Vector3.forward * .05f,Color.green,10000f);
                    }
                }
            }

            // var obstacleMatrices = new Matrix4x4[Mathf.CeilToInt((float)obstaclePositions.Length / AntSimulationRenderSystem.k_InstancesPerBatch)][];
            // for (var i = 0; i < obstacleMatrices.Length; i++)
            // {
            //     obstacleMatrices[i] = new Matrix4x4[math.min(AntSimulationRenderSystem.k_InstancesPerBatch, obstaclePositions.Length - i * AntSimulationRenderSystem.k_InstancesPerBatch)];
            //     for (var j = 0; j < obstacleMatrices[i].Length; j++) obstacleMatrices[i][j] = Matrix4x4.TRS((Vector2)obstaclePositions[i * AntSimulationRenderSystem.k_InstancesPerBatch + j] / simParams.mapSize, Quaternion.identity, new Vector3(simParams.obstacleRadius * 2f, simParams.obstacleRadius * 2f, 1f) / simParams.mapSize);
            // }

            for (var i = 0; i < obstaclePositions.Length; i++)
            {
                var pos = obstaclePositions[i];

                for (var x = Mathf.FloorToInt((pos.x - simParams.obstacleRadius) ); x <= Mathf.FloorToInt((pos.x + simParams.obstacleRadius) ); x++)
                {
                    if (x < 0 || x >= simParams.mapSize)
                        continue;

                    for (var y = Mathf.FloorToInt((pos.y - simParams.obstacleRadius) ); y <= Mathf.FloorToInt((pos.y + simParams.obstacleRadius) ); y++)
                    {
                        if (y < 0 || y >= simParams.mapSize)
                            continue;

                        var isInBounds = AntSimulationUtilities.CalculateIsInBounds(in x, in y, in simParams.mapSize, out var obstacleBucketIndex);
                        if (math.all(isInBounds))
                            obstacleCollisionLookup.Set(obstacleBucketIndex, true);
                    }
                }
            }

            // Assert collision map:
            var log = $"OBSTACLES: {obstaclePositions.Length}\nCOLLISION BUCKETS: [{obstacleCollisionLookup.Length}";
            {
                for (var x = 0; x < simParams.mapSize; x++)
                {
                    log += $"\n{x:0000}|";
                    for (var y = 0; y < simParams.mapSize; y++)
                    {
                        var isInBounds = AntSimulationUtilities.CalculateIsInBounds(x, y, simParams.mapSize, out var obstacleIndex);
                        if (math.all(isInBounds))
                            log += obstacleCollisionLookup.IsSet(obstacleIndex) ? "/" : ".";
                        else throw new InvalidOperationException();
                    }
                }
            }
            Debug.Log(log);

            obstaclePositions.Dispose();
        }
    }
}
