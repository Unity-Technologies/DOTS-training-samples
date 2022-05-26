using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct CarMovementJob : IJobEntity
{
    public float DeltaTime;
    public float LaneLenght;
    public NativeArray<float> Distances;
    public NativeArray<float3> Directions;
    public NativeArray<float3> Positions;
    
    public float3 HighwayOffset;

    void Execute([ChunkIndexInQuery] int chunkIndex, ref MovableCarAspect car)
    {
        //TODO: solve the desired vs actual speed issue
        var speed = car.Speed;

        car.TraveledDistance += speed * DeltaTime;
        
        if (car.TraveledDistance >= LaneLenght)
            car.TraveledDistance = 0;
        
        var newPos = CarMovementSystem.GetCarPosition(car.TraveledDistance, LaneLenght);
        newPos = CarMovementSystem.ScalePosition(newPos, car.Lane);
        
        var direction = newPos - car.Position;
        
        car.Position = newPos;
        
        var rotation = quaternion.RotateY(math.atan2(direction.x, direction.z));
        car.Rotation = rotation;
        
        

        Positions[car.Id] = car.Position;
        Directions[car.Id] = math.normalize(direction);
        Distances[car.Id] = car.TraveledDistance;
    }
}

partial struct CarMovementSystem : ISystem
{
    public const int k_RoadPieces = 8;

    public static float LaneLength;
    
    public static NativeArray<float> Distances;
    public static NativeArray<float3> Positions;
    public static NativeArray<float3> Directions;

    public static NativeArray<float> RoadPiecesLength;
    public static NativeArray<float3> RoadPiecesStartPos;
    public static NativeArray<float3> RoadPiecesEndPos;
    
    public static NativeArray<float> RoadPiecesMagnitude;

    private bool init;
    public static float3 HighwayCentroid;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CarConfigComponent>();
        state.RequireForUpdate<HighwayConfig>();
        state.RequireForUpdate<Road>();
        
        init = false;
        
        RoadPiecesLength = new NativeArray<float>(8, Allocator.Persistent);
        RoadPiecesStartPos = new NativeArray<float3>(8, Allocator.Persistent);
        RoadPiecesEndPos = new NativeArray<float3>(8, Allocator.Persistent);

        RoadPiecesMagnitude = new NativeArray<float>(8, Allocator.Persistent);
    }

    public void OnDestroy(ref SystemState state)
    {
        Distances.Dispose();
        Positions.Dispose();
        Directions.Dispose();
        
        RoadPiecesLength.Dispose();
        RoadPiecesStartPos.Dispose();
        RoadPiecesEndPos.Dispose();

        RoadPiecesMagnitude.Dispose();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!init)
            InitArrays();

        //TODO: Remove this from the update or make it only once
        SetRoadPiecesLenght();
        
        var highwayConfig = SystemAPI.GetSingleton<HighwayConfig>();
        var carMovementJob = new CarMovementJob
        {
            DeltaTime = state.Time.DeltaTime,
            LaneLenght = highwayConfig.InsideLaneLength,
            HighwayOffset = 0,
            Distances = Distances,
            Positions = Positions,
            Directions = Directions
        };
        carMovementJob.Schedule(state.Dependency);
    }

    private void InitArrays()
    {
        init = true;
        var carConfig = SystemAPI.GetSingleton<CarConfigComponent>();
        CarMovementSystem.Distances = new NativeArray<float>(carConfig.CarCount, Allocator.Persistent);
        CarMovementSystem.Positions = new NativeArray<float3>(carConfig.CarCount, Allocator.Persistent);
        CarMovementSystem.Directions = new NativeArray<float3>(carConfig.CarCount, Allocator.Persistent);
        
        
        var highwayConfig = SystemAPI.GetSingleton<HighwayConfig>();
        LaneLength = highwayConfig.InsideLaneLength;
    }
    
    public static float3 GetCarPosition2(float distance, float laneLenght)
    {
        float3 newPosition = 0;
        float r = laneLenght / (2 * math.PI);
        var alpha = distance / laneLenght;
        newPosition.x = r * math.cos(math.radians(360 * alpha));
        newPosition.z = r * math.sin(math.radians(360 * alpha));
        
        return newPosition;
    }
    
    public static float3 GetCarPosition(float distance, float laneLenght)
    {
        GetRoadPiece(distance, out var roadPiece, out var alpha);

        float3 newPosition = 0;
        if (roadPiece % 2 == 0)
            newPosition = GetCarPositionInStraightRoadPiece(roadPiece, alpha);
        else
            newPosition = GetCarPositionInCurvedRoadPiece(roadPiece, alpha);
        

        return newPosition;
    }

    private static void GetRoadPiece(float distance, out int roadPiece, out float alpha)
    {
        roadPiece = -1;
        alpha = -1f;
        
        var nextPieceDistance = 0.0f;
        for (int i = 0; i < RoadPiecesLength.Length; i++)
        {
            var prevPieceDistance = nextPieceDistance;
            nextPieceDistance += RoadPiecesLength[i];

            if (distance < nextPieceDistance)
            {
                roadPiece = i;
                alpha = GetAlpha(prevPieceDistance, nextPieceDistance, distance);
                break;
            }
        }
    }

    private static float GetAlpha(float prevPieceDistance, float nextPieceDistance, float distance)
    {
        var total = nextPieceDistance - prevPieceDistance;
        var current = distance - prevPieceDistance;
        return current / total;
    }

    private static float3 GetCarPositionInStraightRoadPiece(int roadPiece, float alpha)
    {
        var startPos = RoadPiecesStartPos[roadPiece];
        var endPos = RoadPiecesEndPos[roadPiece];

        var newPos = ((endPos - startPos) * alpha) + startPos;
        
        return newPos;
    }

    private static float3 GetCarPositionInCurvedRoadPiece(int roadPiece, float alpha)
    {
        var startPos = RoadPiecesStartPos[roadPiece];
        var endPos = RoadPiecesEndPos[roadPiece];

        var origin = GetOrigin(startPos, endPos, roadPiece);
        
        Vector3 v0 = startPos - origin;
        Vector3 v1 = endPos - origin;
        
        float3 newPos = (float3)Vector3.Slerp(v0, v1, alpha) + origin;
        
        return newPos;
    }

    private static float3 GetOrigin(float3 startPos, float3 endPos, int roadPiece)
    {
        var trackOrigin = CarMovementSystem.HighwayCentroid;
        
        var inner0 = startPos - trackOrigin;
        var inner1 = endPos - trackOrigin;

        var min = (float3)0;
        
        min.x = math.abs(inner0.x) < math.abs(inner1.x) ? startPos.x : endPos.x;
        min.z = math.abs(inner0.z) < math.abs(inner1.z) ? startPos.z : endPos.z;
        
        return min;
    }

    public static void SetRoadPiecesLenght()
    {
        var totalRoadMagnitude = GetRoadMagnitude();
        
        for (int i = 0; i < k_RoadPieces; i++)
        {
            RoadPiecesLength[i] = (RoadPiecesMagnitude[i] / totalRoadMagnitude) * LaneLength;
        }
    }

    private static float GetRoadMagnitude()
    {
        var totalMagnitude = 0f;
        for (int i = 0; i < k_RoadPieces; i++)
        {
            var currentMagnitude = math.length(RoadPiecesEndPos[i] - RoadPiecesStartPos[i]);
            RoadPiecesMagnitude[i] = currentMagnitude;
            totalMagnitude += currentMagnitude;
        }

        return totalMagnitude;
    }


    public static float3 ScalePosition(float3 carPosition, int carLane)
    {
        var direction = carPosition - HighwayCentroid;
        var scaleFactor = 0.02f;
        var startScale = 0.95f;
        direction *= startScale + (scaleFactor * carLane);
        var scaledPosition = direction + HighwayCentroid;
        return scaledPosition;
    }
}
