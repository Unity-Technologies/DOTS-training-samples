using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Profiling;
using Unity.Burst.Intrinsics;

public partial struct ObstacleSpawnerSystem : ISystem
{
    public DynamicBuffer<ObstacleArcPrimitive> ObstaclePrimtitveBuffer;

    static readonly ProfilerMarker k_ProfileMarker_GenerateObstacles = new ProfilerMarker("Obstacle: GenerateObstacles");
    static readonly ProfilerMarker k_ProfileMarker_CalculateRayCollision = new ProfilerMarker("Obstacle: CalculateRayCollision");

    const float MAXFLOAT = 10000000f; 

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
        k_ProfileMarker_GenerateObstacles.Begin();

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

                    PrototypeObstaclePrim.VectorToStart = new float2(math.cos(PrototypeObstaclePrim.AngleStart), math.sin(PrototypeObstaclePrim.AngleStart));
                    PrototypeObstaclePrim.VectorToEnd = new float2(math.cos(PrototypeObstaclePrim.AngleEnd), math.sin(PrototypeObstaclePrim.AngleEnd));

                    // Actually spawn the collision objects
                    ObstaclePrimtitveBuffer.Add(PrototypeObstaclePrim);                    

                    ringAngleStart += ringAngleEndOffset;
                }
            }
        }

        k_ProfileMarker_GenerateObstacles.End();
    }

    public static bool IsGridOccupied(in NativeArray<ObstacleArcPrimitive> ObstaclePrimtitveBuffer, float2 Coordinate, float WallThickness)
    {
        for (int i = 0; i < ObstaclePrimtitveBuffer.Length; i++)
        {
            ObstacleArcPrimitive prim = ObstaclePrimtitveBuffer[i];
            float2 Delta = Coordinate - prim.Position;
            if( math.abs( math.length( Delta ) - prim.Radius) <= WallThickness)
            {
                float CoordinateAngle = math.atan2(Delta.y, Delta.x);
                float StartAngle = prim.AngleStart;
                float EndAngle = prim.AngleEnd;
                while (StartAngle > EndAngle)
                {
                    EndAngle += 2.0f * math.PI;
                }
                if (StartAngle > CoordinateAngle)
                {
                    CoordinateAngle += 2.0f * math.PI;
                }
                if( (CoordinateAngle >= StartAngle) && (CoordinateAngle <= EndAngle) )
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool CalculateRayCollision(in NativeArray<ObstacleArcPrimitive> ObstaclePrimtitveBuffer, in float2 point, in float2 direction, out float2 CollisionPoint, out float Param)
    {
       // k_ProfileMarker_CalculateRayCollision.Begin();

        bool ReturnValue = false;
        // if (X86.Sse.IsSseSupported)
        // {
        //     return CalculateRayCollisionSSE(ObstaclePrimtitveBuffer, point, direction, out CollisionPoint, out Param);
        // }
        // else
        {
            Param = MAXFLOAT;

            int PrimIndex = -1;
            for (int i = 0; i < ObstaclePrimtitveBuffer.Length; i++)
            {
                ObstacleArcPrimitive prim = ObstaclePrimtitveBuffer[i];
                float t1, t2;
                if (CalculateRayCollisionWithPrimitive(prim, point, direction, out t1, out t2))
                {
                    if (t1 < Param)
                    {
                        if ((0.0f <= t1) && (1.0f >= t1))
                        {
                            float2 TestCollPoint = point + t1 * direction;
                            if (CalculateCollisionOnPrimitive(prim, TestCollPoint))
                            {
                                Param = t1;
                                PrimIndex = i;
                            }
                        }
                    }
                    if (t2 < Param)
                    {
                        if ((0.0f <= t2) && (1.0f >= t2))
                        {
                            float2 TestCollPoint = point + t2 * direction;
                            if (CalculateCollisionOnPrimitive(prim, TestCollPoint))
                            {
                                Param = t2;
                                PrimIndex = i;
                            }
                        }
                    }
                }
            }

            
            if (-1 == PrimIndex)
            {
                CollisionPoint = point;
                ReturnValue = false;
            }
            else
            {
                CollisionPoint = point + Param * direction;
                ReturnValue = true;
            }
        }

       // k_ProfileMarker_CalculateRayCollision.End();
        return ReturnValue;
    }

    // ublic static bool CalculateRayCollisionSSE(in NativeArray<ObstacleArcPrimitive> ObstaclePrimtitveBuffer, in float2 point, in float2 direction, out float2 CollisionPoint, out float Param)
    // 
    //    k_ProfileMarker_CalculateRayCollision.Begin();
    // 
    //    Param = 1000000000.0f;
    // 
    //    // static float CopyValues;
    // 
    //    int PrimIndex = -1;
    //    for (int i = 0; i < ObstaclePrimtitveBuffer.Length; i+=4)
    //    {
    //        v128 PrimPosX, PrimPosY, PrimRadius, AntPosX, AntPosY, AntDirectionX, AntDirectionY;
    //        for (int j = 0; j < 4; j++)
    //        {
    //            ObstacleArcPrimitive prim = ObstaclePrimtitveBuffer[math.min(i + j, ObstaclePrimtitveBuffer.Length - 1)];
    //            switch( j )
    //            {
    //                case 0:
    //                    PrimPosX.Float0 = prim.Position.x;
    //                    PrimPosY.Float0 = prim.Position.y;
    //                    PrimRadius.Float0 = prim.Radius;
    //                    AntPosX.Float0 = point.x;
    //                    AntPosY.Float0 = point.y;
    //                    AntDirectionX.Float0 = direction.x;
    //                    AntDirectionY.Float0 = direction.y;
    //                    break;
    //                case 1:
    //                    PrimPosX.Float1 = prim.Position.x;
    //                    PrimPosY.Float1 = prim.Position.y;
    //                    PrimRadius.Float0 = prim.Radius;
    //                    AntPosX.Float1 = point.x;
    //                    AntPosY.Float1 = point.y;
    //                    AntDirectionX.Float1 = direction.x;
    //                    AntDirectionY.Float1 = direction.y;
    //                    break;
    //                case 2:
    //                    PrimPosX.Float2 = prim.Position.x;
    //                    PrimPosY.Float2 = prim.Position.y;
    //                    PrimRadius.Float0 = prim.Radius;
    //                    AntPosX.Float2 = point.x;
    //                    AntPosY.Float2 = point.y;
    //                    AntDirectionX.Float2 = direction.x;
    //                    AntDirectionY.Float2 = direction.y;
    //                    break;
    //                case 3:
    //                    PrimPosX.Float3 = prim.Position.x;
    //                    PrimPosY.Float3 = prim.Position.y;
    //                    PrimRadius.Float0 = prim.Radius;
    //                    AntPosX.Float3 = point.x;
    //                    AntPosY.Float3 = point.x;
    //                    AntDirectionX.Float3 = direction.x;
    //                    AntDirectionY.Float3 = direction.y;
    //                    break;
    //            }
    //        }
    // 
    //        float t;
    //        if (CalculateRayCollisionWithPrimitiveSSE(PrimPosX, PrimPosY, PrimRadius, AntPosX, AntPosY, AntDirectionX, AntDirectionY, out t))
    //        {
    //            if (t < Param)
    //            {
    //                if ((0.0f <= t) && (1.0f >= t))
    //                {
    //                    float2 TestCollPoint = point + t * direction;
    //                    if (CalculateCollisionOnPrimitive(prim, TestCollPoint))
    //                    {
    //                        Param = t;
    //                        PrimIndex = i;
    //                    }
    //                }
    //            }
    //        }
    //    }
    // 
    //    bool ReturnValue = false;
    //    if (-1 == PrimIndex)
    //    {
    //        CollisionPoint = point;
    //        ReturnValue = false;
    //    }
    //    else
    //    {
    //        CollisionPoint = point + Param * direction;
    //        ReturnValue = true;
    //    }
    // 
    //    k_ProfileMarker_CalculateRayCollision.End();
    //    return ReturnValue;
    // 

    public static bool CalculateCollisionOnPrimitive(in ObstacleArcPrimitive prim, in float2 CollisionPoint)
    {
        float2 CollisionDirection = CollisionPoint - prim.Position;
        bool bOldTest = false;
        if (bOldTest)
        {
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
                return true;
            }
        }
        else
        {
            if (math.PI < prim.AngleRange)
            {
                float DetStart = CollisionDirection.x * prim.VectorToEnd.y - CollisionDirection.y * prim.VectorToEnd.x;
                float DetEnd = CollisionDirection.x * prim.VectorToStart.y - CollisionDirection.y * prim.VectorToStart.x;

                return !((DetStart <= 0.0f) && (DetEnd >= 0.0f));
            }
            else
            {
                float DetStart = CollisionDirection.x * prim.VectorToStart.y - CollisionDirection.y * prim.VectorToStart.x;
                float DetEnd = CollisionDirection.x * prim.VectorToEnd.y - CollisionDirection.y * prim.VectorToEnd.x;

                return ((DetStart <= 0.0f) && (DetEnd >= 0.0f));
            }
        }

        return false;
    }

    public static bool CalculateRayCollisionWithPrimitive(in ObstacleArcPrimitive prim, in float2 point, in float2 direction, out float Param1, out float Param2)
    {
        Param1 = MAXFLOAT;
        Param2 = MAXFLOAT;

        float2 VectorFromCenterToPoint = point - prim.Position;
        float a = math.dot(direction, direction);
        float b = 2.0f * math.dot(direction, VectorFromCenterToPoint);
        float c = math.dot(VectorFromCenterToPoint, VectorFromCenterToPoint) - prim.Radius * prim.Radius;
        // Solve the quadratic equation
        float discriminant = b * b - 4.0f * a * c;
        if (0 > discriminant)
        {
            return false;
        }
        discriminant = Mathf.Sqrt(discriminant);
        float t1 = (-b - discriminant) / (2.0f * a);
        float t2 = (-b + discriminant) / (2.0f * a);
        // Calculate the smallest positive T value
        float t = 0.0f;
        if ((0.0f > t1) && (0.0f > t2))
        {
            return false;
        }

        if ((0.0f <= t1) && (0.0f <= t2))
        {
            Param1 = math.min(t1, t2);
            Param2 = math.max(t1, t2);
        }
        else if (0.0f <= t1)
        {
            Param1 = t1;
        }
        else if (0.0f <= t2)
        {
            Param1 = t2;
        }
        // See if this collision is the closest        

        return true;
    }

    static float4 KrazSSE_MUL( in float4 A, in float4 B )
    {
        return new float4(A.x * B.x, A.y * B.y, A.z * B.z, A.w * B.w);
    }
    static float4 KrazSSE_MULC(in float4 A, in float B)
    {
        return new float4(A.x * B, A.y * B, A.z * B, A.w * B);
    }
    static float4 KrazSSE_ADD(in float4 A, in float4 B)
    {
        return new float4(A.x + B.x, A.y + B.y, A.z + B.z, A.w + B.w);
    }

    static float4 KrazSSE_SUB(in float4 A, in float4 B)
    {
        return new float4(A.x - B.x, A.y - B.y, A.z - B.z, A.w - B.w);
    }

    static float4 KrazSSE_MADD(in float4 A, in float4 B)
    {
        return new float4(A.x - B.x, A.y - B.y, A.z - B.z, A.w - B.w);
    }

    static float4 KrazSSE_DOTV2(in float4 AX, in float4 AY, in float4 BX, in float4 BY)
    {
        float4 DotX = KrazSSE_MUL(AX, BX);
        float4 DotY = KrazSSE_MUL(AY, BY);
        return KrazSSE_ADD(DotX, DotY);
    }

    // public static bool CalculateRayCollisionWithPrimitiveSSE(in float4 PrimPosX, in float4 PrimPosY, in float4 PrimRadius, in float4 AntPosX, in float4 AntPosY, in float4 AntDirX, in float4 AntDirY, out float Param)
    // {
    //     Param = 1000000000f;
    // 
    //     float4 Zero = new float4(0.0f, 0.0f, 0.0f, 0.0f);
    // 
    //     // float2 VectorFromCenterToPoint = point - prim.Position;
    //     float4 VectorFromCenterToPointX = AntPosX - PrimPosX;
    //     float4 VectorFromCenterToPointY = AntPosY - PrimPosY;
    // 
    //     // float a = math.dot(direction, direction);
    //     float4 a = KrazSSE_DOTV2(AntDirX, AntDirY, AntDirX, AntDirY);
    // 
    //     // float b = 2.0f * math.dot(direction, VectorFromCenterToPoint);
    //     float4 b = KrazSSE_DOTV2(AntDirX, AntDirY, VectorFromCenterToPointX, VectorFromCenterToPointY);
    // 
    //     // float c = math.dot(VectorFromCenterToPoint, VectorFromCenterToPoint) - prim.Radius * prim.Radius;
    //     float4 vdv = KrazSSE_DOTV2(VectorFromCenterToPointX, VectorFromCenterToPointY, VectorFromCenterToPointX, VectorFromCenterToPointY);
    //     float4 c = KrazSSE_SUB(vdv, PrimRadius);
    // 
    //     // Solve the quadratic equation
    //     // float discriminant = b * b - 4.0f * a * c;
    //     float4 bsq = KrazSSE_MUL(b, b);
    //     float4 ac = KrazSSE_MUL(a, a);
    //     float4 ac4 = KrazSSE_MULC(ac, 4.0f);
    //     float4 discriminant = KrazSSE_SUB(bsq, ac4);
    //     bool4 Valid = KrazSSE_CMPGE(discriminant, Zero);
    //     if (0 > discriminant)
    //     {
    //         return false;
    //     }
    //     discriminant = Mathf.Sqrt(discriminant);
    //     float t1 = (-b - discriminant) / (2.0f * a);
    //     float t2 = (-b + discriminant) / (2.0f * a);
    //     // Calculate the smallest positive T value
    //     float t = 0.0f;
    //     if ((0.0f > t1) && (0.0f > t2))
    //     {
    //         return false;
    //     }
    // 
    //     if ((0.0f <= t1) && (0.0f <= t2))
    //     {
    //         Param = Mathf.Min(t1, t2);
    //     }
    //     else if (0.0f <= t1)
    //     {
    //         Param = t1;
    //     }
    //     else if (0.0f <= t2)
    //     {
    //         Param = t2;
    //     }
    //     // See if this collision is the closest        
    // 
    //     return true;
    // }

}

public struct ObstacleSpawner : IComponentData
{
    public int ObstacleRingCount;
    public Entity Prefab;

    public float ObstaclePercentPerRing;
    public float ObstacleRadius;
}

public struct ObstacleArcPrimitive : IBufferElementData
{
    public float2 Position;
    public float Radius;
    public float AngleStart;
    public float AngleEnd;
    public float AngleRange;

    public float2 VectorToStart;
    public float2 VectorToEnd;
}

