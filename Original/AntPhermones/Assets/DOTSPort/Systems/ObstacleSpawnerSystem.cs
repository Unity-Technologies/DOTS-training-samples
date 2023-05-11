using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct ObstacleSpawnerSystem : ISystem
{
    public DynamicBuffer<ObstacleArcPrimitive> ObstaclePrimtitveBuffer;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ObstacleSpawnerExecution>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        GenerateObstacles(ref state);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void GenerateObstacles(ref SystemState state)
    {
        var ecbSystemSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystemSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var ObstacleEntity = ecb.CreateEntity();
        
                
#if UNITY_EDITOR
        ecb.SetName(ObstacleEntity, $"__Buffer_ObstacleArcPrimitive");
#endif
        
        ObstaclePrimtitveBuffer = ecb.AddBuffer<ObstacleArcPrimitive>(ObstacleEntity);

        Vector2 MapCenter = new Vector2(0.5f, 0.5f);

        foreach (var spawner in SystemAPI.Query<RefRO<ObstacleSpawner>>())
        {
            int iObstacleRingCount = spawner.ValueRO.ObstacleRingCount;
            float fObstaclePercentPerRing = spawner.ValueRO.ObstaclePercentPerRing;
            float fObstacleRadius = spawner.ValueRO.ObstacleRadius;

            for (int i = 1; i <= iObstacleRingCount; i++)
            {
                int holeCount = UnityEngine.Random.Range(1, 3);

                // float ringRadius = (i / (iObstacleRingCount + 1f)) * (spawner.MapSize * .5f);
                float ringRadius = (i / (iObstacleRingCount + 1f)) * (0.5f);
                float ringAngleStart = UnityEngine.Random.Range(0.0f, Mathf.PI);
                float ringAngleEndOffset = (2.0f * Mathf.PI / holeCount);

                // float ringArcLength = 2.0f * Mathf.PI * ringRadius / (float)holeCount;            

                for (int r = 0; r < holeCount; r++)
                {
                    ObstacleArcPrimitive PrototypeObstaclePrim = new ObstacleArcPrimitive();
                    PrototypeObstaclePrim.Position = MapCenter;
                    PrototypeObstaclePrim.Radius = ringRadius;
                    // Create angular start/end extents
                    PrototypeObstaclePrim.AngleStart = ringAngleStart;
                    PrototypeObstaclePrim.AngleEnd = ringAngleStart + ringAngleEndOffset * fObstaclePercentPerRing;
                    if (PrototypeObstaclePrim.AngleEnd > 2.0f * Mathf.PI) PrototypeObstaclePrim.AngleEnd -= 2.0f * Mathf.PI;

                    PrototypeObstaclePrim.AngleRange = ((PrototypeObstaclePrim.AngleStart < PrototypeObstaclePrim.AngleEnd) ? (PrototypeObstaclePrim.AngleEnd - PrototypeObstaclePrim.AngleStart) : (PrototypeObstaclePrim.AngleEnd + 2.0f * Mathf.PI - PrototypeObstaclePrim.AngleStart));

                    // Actually spawn the collision objects
                    ObstaclePrimtitveBuffer.Add(PrototypeObstaclePrim);                    

                    ringAngleStart += ringAngleEndOffset;
                }
            }
        }
    }

    public static bool CalculateRayCollision(in DynamicBuffer<ObstacleArcPrimitive> ObstaclePrimtitveBuffer, in Vector2 point, in float2 direction, out float2 CollisionPoint, out float Param)
    {
        Vector2 outColl;
        float outParam;
        bool result = CalculateRayCollision(ObstaclePrimtitveBuffer, point, direction, out outColl, out outParam);

        CollisionPoint.x = outColl.x;
        CollisionPoint.y = outColl.y;
        Param = outParam;

        return result;
    }

    public static bool CalculateRayCollision(in DynamicBuffer<ObstacleArcPrimitive> ObstaclePrimtitveBuffer, in Vector2 point, in Vector2 direction, out Vector2 CollisionPoint, out float Param)
    {
        Param = 1000000000.0f;
    
        int PrimIndex = -1;
        for( int i = 0; i < ObstaclePrimtitveBuffer.Length; i++ )
        {
            ObstacleArcPrimitive prim = ObstaclePrimtitveBuffer[i];
    
            Vector2 VectorFromCenterToPoint = point - prim.Position;
            float a = Vector2.Dot(direction, direction);
            float b = 2.0f * Vector2.Dot(direction, VectorFromCenterToPoint);
            float c = Vector2.Dot(VectorFromCenterToPoint, VectorFromCenterToPoint) - prim.Radius * prim.Radius;
    
            // Solve the quadratic equation
            float discriminant = b * b - 4.0f * a * c;
            if (0 > discriminant)
            {
                CollisionPoint = point;
                continue;
            }
    
            discriminant = Mathf.Sqrt(discriminant);
    
            float t1 = (-b - discriminant) / (2.0f * a);
            float t2 = (-b + discriminant) / (2.0f * a);
    
            // Calculate the smallest positive T value
            float t = 0.0f;
            if ((0.0f > t1) && (0.0f > t2)) continue;
    
            if ((0.0f <= t1) && (0.0f <= t2))
            {
                t = Mathf.Min(t1, t2);
            }
            else if (0.0f <= t1)
            {
                t = t1;
            }
            else if (0.0f <= t2)
            {
                t = t2;
            }
    
            // See if this collision is the closest
            if (t < Param)
            {
                if ((0.0f <= t) && (1.0f >= t))
                {
                    Vector2 TestCollPoint = point + t * direction;
                    Vector2 CollisionDirection = TestCollPoint - prim.Position;
    
                    float Angle = Mathf.Atan2(CollisionDirection.y, CollisionDirection.x);
                    
    
                    float AngleStart = prim.AngleStart;
                    float AngleEnd = prim.AngleEnd;
    
                    if (prim.AngleEnd < prim.AngleStart)
                    {
                        AngleEnd += Mathf.PI * 2.0f;
                    }
                    if (prim.AngleStart > Angle)
                    {
                        Angle += Mathf.PI * 2.0f;
                    }
    
                    if ((Angle >= AngleStart) && (Angle <= AngleEnd))
                    {
                        Param = t;
                        PrimIndex = i;
                    }
                }
            }
        }
    
        if (-1 == PrimIndex)
        {
            CollisionPoint = point;
            return false;
        }
    
        CollisionPoint = point + Param * direction;        
        return true;
    }
}

// public partial class ObstacleSpawnerSystem : SystemBase
// public class ObstacleSpawnerSystemOld : MonoBehaviour
// {
//     public int ObstacleRingCount;
//     public float ObstaclePercentPerRing;
//     public float ObstacleRadius;
// 
//     // TODO bind this to the actual map size
//     public int MapSize;
//     // TODO deprecate this
//     public GameObject Prefab;
//     // TODO remove this
//     List<Obstacle> ObstacleList;
// 
//     Matrix4x4[][] ObstacleMatrices;
//     Obstacle[,][] ObstacleBuckets;
//     public int bucketResolution;
// 
//     public ObstacleArcPrimitive [] ArcPrimitives;
// 
//     const int InstancesPerBatch = 1023;
// 
//     public bool bIsCreated;
// 
//     // Start is called before the first frame update
//     void Start()
//     {
//         bIsCreated = false;
//     }
// 
//     // Update is called once per frame
//     void Update()
//     {
//         GenerateObstacles();
//     }
// 
//     // public void OnCreate()
//     // {
//     //     bIsCreated = false;
//     // }
//     // 
//     // void OnUpdate()
//     // {
//     //     GenerateObstacles();
//     // }
//     // 
//     // void OnDestroy()
//     // {
//     // }
// 
//     public void DrawObstacles()
//     {
//         int PrimIndex = -1;
//         for (int i = 0; i < ArcPrimitives.Length; i++)
//         {
//             ObstacleArcPrimitive prim = ArcPrimitives[i];
// 
//             float Angle = prim.AngleStart;
//             float AngleEnd = prim.AngleEnd + ((prim.AngleStart < prim.AngleEnd) ? 0.0f : 2.0f * Mathf.PI);
//             Vector2 PrevPoint = prim.Position;
//             PrevPoint.x += Mathf.Cos(Angle) * prim.Radius;
//             PrevPoint.y += Mathf.Sin(Angle) * prim.Radius;            
// 
//             while (Angle < AngleEnd)
//             {
//                 Angle += 0.1f;
//                 Vector2 NextPoint = prim.Position;
//                 NextPoint.x += Mathf.Cos(Angle) * prim.Radius;
//                 NextPoint.y += Mathf.Sin(Angle) * prim.Radius;
//                 
//                 Debug.DrawRay(PrevPoint, (NextPoint - PrevPoint), Color.gray, 0.05f);
//                 PrevPoint = NextPoint;
//             }
//         }        
//     }
// 
//     public bool CalculateRayCollision(in Vector2 point, in Vector2 direction, out Vector2 CollisionPoint, out float Param)
//     {
//         Param = 1000000000.0f;
// 
//         int PrimIndex = -1;
//         for( int i = 0; i < ArcPrimitives.Length; i++ )
//         {
//             ObstacleArcPrimitive prim = ArcPrimitives[i];
// 
//             Vector2 VectorFromCenterToPoint = point - prim.Position;
//             float a = Vector2.Dot(direction, direction);
//             float b = 2.0f * Vector2.Dot(direction, VectorFromCenterToPoint);
//             float c = Vector2.Dot(VectorFromCenterToPoint, VectorFromCenterToPoint) - prim.Radius * prim.Radius;
// 
//             // Solve the quadratic equation
//             float discriminant = b * b - 4.0f * a * c;
//             if (0 > discriminant)
//             {
//                 CollisionPoint = point;
//                 continue;
//             }
// 
//             discriminant = Mathf.Sqrt(discriminant);
// 
//             float t1 = (-b - discriminant) / (2.0f * a);
//             float t2 = (-b + discriminant) / (2.0f * a);
// 
//             // Calculate the smallest positive T value
//             float t = 0.0f;
//             if ((0.0f > t1) && (0.0f > t2)) continue;
// 
//             if ((0.0f <= t1) && (0.0f <= t2))
//             {
//                 t = Mathf.Min(t1, t2);
//             }
//             else if (0.0f <= t1)
//             {
//                 t = t1;
//             }
//             else if (0.0f <= t2)
//             {
//                 t = t2;
//             }
// 
//             // See if this collision is the closest
//             if (t < Param)
//             {
//                 if ((0.0f <= t) && (1.0f >= t))
//                 {
//                     Vector2 TestCollPoint = point + t * direction;
//                     Vector2 CollisionDirection = TestCollPoint - prim.Position;
// 
//                     float Angle = Mathf.Atan2(CollisionDirection.y, CollisionDirection.x);
//                     
// 
//                     float AngleStart = prim.AngleStart;
//                     float AngleEnd = prim.AngleEnd;
// 
//                     if (prim.AngleEnd < prim.AngleStart)
//                     {
//                         AngleEnd += Mathf.PI * 2.0f;
//                     }
//                     if (prim.AngleStart > Angle)
//                     {
//                         Angle += Mathf.PI * 2.0f;
//                     }
// 
//                     if ((Angle >= AngleStart) && (Angle <= AngleEnd))
//                     {
//                         Param = t;
//                         PrimIndex = i;
//                     }
//                 }
//             }
//         }
// 
//         if (-1 == PrimIndex)
//         {
//             CollisionPoint = point;
//             return false;
//         }
// 
//         CollisionPoint = point + Param * direction;        
//         return true;
//     }
// 
//     public bool CalculateRayCollision( in Vector2 point, in Vector2 direction, out Vector2 CollisionPoint )
//     {
//         float BestT = 1000000000.0f;
//         return CalculateRayCollision(point, direction, out CollisionPoint, out BestT);
//     }
// 
//     public void GenerateObstacles()
//     {
//         if (bIsCreated) return;
//         bIsCreated = true;
// 
//         bool bStillUseSphereObstacles = true;
//         int iTotalNumSphereObstacles = 0;
//         Vector2 MapCenter = new Vector2(0.5f, 0.5f);
// 
//         ObstacleSpawnerOld spawner = new ObstacleSpawnerOld();
// 
//         spawner.ObstacleRingCount = this.ObstacleRingCount;
//         spawner.ObstaclePercentPerRing = this.ObstaclePercentPerRing;
//         spawner.ObstacleRadius = this.ObstacleRadius;
//         spawner.MapSize = this.MapSize;
//         spawner.Prefab = this.Prefab;
// 
//         {
//             List<ObstacleArcPrimitive> ObstaclePrimList = new List<ObstacleArcPrimitive>();
//             int iObstacleRingCount = spawner.ObstacleRingCount;
//             for (int i = 1; i <= iObstacleRingCount; i++)
//             {
//                 int holeCount = UnityEngine.Random.Range(1, 3);
// 
//                 // float ringRadius = (i / (iObstacleRingCount + 1f)) * (spawner.MapSize * .5f);
//                 float ringRadius = (i / (iObstacleRingCount + 1f)) * (0.5f);
//                 float ringAngleStart = UnityEngine.Random.Range(0.0f, Mathf.PI);
//                 float ringAngleEndOffset = (2.0f * Mathf.PI / holeCount);
// 
//                 // float ringArcLength = 2.0f * Mathf.PI * ringRadius / (float)holeCount;            
// 
//                 for (int r = 0; r < holeCount; r++)
//                 {
//                     ObstacleArcPrimitive obstaclePrim = new ObstacleArcPrimitive();
//                     obstaclePrim.Position = MapCenter;
//                     obstaclePrim.Radius = ringRadius;
//                     // Create angular start/end extents
//                     obstaclePrim.AngleStart = ringAngleStart;
//                     obstaclePrim.AngleEnd = ringAngleStart + ringAngleEndOffset * spawner.ObstaclePercentPerRing;
//                     if (obstaclePrim.AngleEnd > 2.0f * Mathf.PI) obstaclePrim.AngleEnd -= 2.0f * Mathf.PI;
// 
//                     obstaclePrim.AngleRange = ((obstaclePrim.AngleStart < obstaclePrim.AngleEnd) ? (obstaclePrim.AngleEnd - obstaclePrim.AngleStart) : (obstaclePrim.AngleEnd + 2.0f * Mathf.PI - obstaclePrim.AngleStart));
// 
//                     if (bStillUseSphereObstacles)
//                     {
//                         // Ring circumference / obstacleRadius
//                         float circumference = (2.0f * Mathf.PI * ringRadius);
//                         float numObjectsPerCircumference = circumference / spawner.ObstacleRadius;
//                         obstaclePrim.iAnglePerObstacle = (2.0f * Mathf.PI) / numObjectsPerCircumference;
//                         obstaclePrim.iNumObstacles = Mathf.CeilToInt(obstaclePrim.AngleRange / obstaclePrim.iAnglePerObstacle);
// 
//                         iTotalNumSphereObstacles += obstaclePrim.iNumObstacles;
//                     }
// 
//                     ObstaclePrimList.Add(obstaclePrim);
// 
//                     ringAngleStart += ringAngleEndOffset;
//                 }
//             }
// 
//             // Save off the arc primitives
//             ArcPrimitives = ObstaclePrimList.ToArray();
// 
//             if (bStillUseSphereObstacles)
//             {
//                 ObstacleList = new List<Obstacle>();
// 
//                 List<Obstacle>[,] tempObstacleBuckets = new List<Obstacle>[bucketResolution, bucketResolution];
//                 for (int x = 0; x < bucketResolution; x++)
//                 {
//                     for (int y = 0; y < bucketResolution; y++)
//                     {
//                         tempObstacleBuckets[x, y] = new List<Obstacle>();
//                     }
//                 }
// 
//                 ObstacleMatrices = new Matrix4x4[Mathf.CeilToInt((float)iTotalNumSphereObstacles / InstancesPerBatch)][];
//                 for (int i = 0; i < ObstacleMatrices.Length; i++)
//                 {
//                     ObstacleMatrices[i] = new Matrix4x4[Mathf.Min(InstancesPerBatch, iTotalNumSphereObstacles - i * InstancesPerBatch)];
//                 }
// 
//                 int obstacleInd = 0;
//                 ObstacleArcPrimitive[] obstaclePrimitives = ObstaclePrimList.ToArray();
//                 foreach (ObstacleArcPrimitive Prim in ObstaclePrimList)
//                 {
//                     float FullRingCircumference = Prim.Radius * 2.0f * Mathf.PI;
//                     float PrimitiveArcLength = FullRingCircumference / Prim.AngleRange;
// 
//                     for (int primObstacle = 0; primObstacle < Prim.iNumObstacles; primObstacle++)
//                     {
//                         int bucketInd = obstacleInd / InstancesPerBatch;
//                         int matrixInd = obstacleInd - InstancesPerBatch * bucketInd;
//                         {
//                             float ObstacleAnglePosition = Prim.AngleStart + Prim.iAnglePerObstacle * primObstacle;
//                             Vector2 ObstaclePosition = new Vector2(spawner.MapSize * .5f + Mathf.Cos(ObstacleAnglePosition) * Prim.Radius, spawner.MapSize * .5f + Mathf.Sin(ObstacleAnglePosition) * Prim.Radius);
//                             Matrix4x4 ObstacleMatrix = Matrix4x4.TRS(ObstaclePosition / spawner.MapSize, Quaternion.identity, new Vector3(spawner.ObstacleRadius * 2f, spawner.ObstacleRadius * 2f, 1f) / spawner.MapSize);
//                             ObstacleMatrices[bucketInd][matrixInd] = ObstacleMatrix;
// 
//                             // Initialize the obstacle spheres
//                             float radius = spawner.ObstacleRadius;
//                             for (int x = Mathf.FloorToInt((ObstaclePosition.x - radius) / spawner.MapSize * bucketResolution); x <= Mathf.FloorToInt((ObstaclePosition.x + radius) / spawner.MapSize * bucketResolution); x++)
//                             {
//                                 if (x < 0 || x >= bucketResolution)
//                                 {
//                                     continue;
//                                 }
//                                 for (int y = Mathf.FloorToInt((ObstaclePosition.y - radius) / spawner.MapSize * bucketResolution); y <= Mathf.FloorToInt((ObstaclePosition.y + radius) / spawner.MapSize * bucketResolution); y++)
//                                 {
//                                     if (y < 0 || y >= bucketResolution)
//                                     {
//                                         continue;
//                                     }
// 
//                                     Obstacle NewObstacle = new Obstacle();
//                                     NewObstacle.position = ObstaclePosition;
//                                     NewObstacle.radius = radius;
// 
//                                     tempObstacleBuckets[x, y].Add(NewObstacle);
//                                    
//                                     // var entity = ecb.Instantiate(spawner.ValueRO.Prefab);
//                                     // ecb.SetComponent(entity, new LocalTransform() { Position = new Vector3(NewObstacle.position.x, NewObstacle.position.y, 0.0f), Scale = NewObstacle.radius });
//                                 }
//                             }
//                         }
// 
//                         obstacleInd++;
//                     }
//                 }
// 
//                 ObstacleBuckets = new Obstacle[bucketResolution, bucketResolution][];
//                 for (int x = 0; x < bucketResolution; x++)
//                 {
//                     for (int y = 0; y < bucketResolution; y++)
//                     {
//                         ObstacleBuckets[x, y] = tempObstacleBuckets[x, y].ToArray();
// 
//                         int iNumObstacles = ObstacleBuckets[x, y].Length;
//                         for( int i = 0; i < iNumObstacles; i++ )
//                         {
//                             Obstacle o = ObstacleBuckets[x, y][i];
// 
//                             Obstacle NewObstacle = new Obstacle();
//                             NewObstacle.position = o.position;
//                             NewObstacle.radius = o.radius;
//                             ObstacleList.Add(NewObstacle);
//                         }
//                     }
//                 }
//             }
//         }
// 
//         // return state;
//     }
// 
//     public void GenerateObstacles(AntManager manager)
//     {
//         GenerateObstacles();
// 
//         manager.obstacleRingCount = ObstacleRingCount;
//         manager.ObstacleMatrices = ObstacleMatrices;
//         manager.ObstacleBuckets = ObstacleBuckets;
// 
//         manager.Obstacles = ObstacleList.ToArray();
//     }
// }

public struct ObstacleSpawner : IComponentData
{
    public int ObstacleRingCount;
    public Entity Prefab;

    public float ObstaclePercentPerRing;
    public float ObstacleRadius;
}

public struct ObstacleArcPrimitive : IBufferElementData
{
    public Vector2 Position;
    public float Radius;
    public float AngleStart;
    public float AngleEnd;
    public float AngleRange;

    public float iAnglePerObstacle;
    public int iNumObstacles;
}

public struct ObstacleSpawnerOld
{
    public int ObstacleRingCount;
    public float ObstaclePercentPerRing;
    public float ObstacleRadius;

    // TODO bind this to the actual map size
    public int MapSize;
    // TODO deprecate this
    public GameObject Prefab;
}
