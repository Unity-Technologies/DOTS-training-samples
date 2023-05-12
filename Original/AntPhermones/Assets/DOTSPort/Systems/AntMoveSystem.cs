using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Profiling;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct AntMoveSystem : ISystem
{
    static readonly ProfilerMarker k_AntsMoveJob_Execute = new ProfilerMarker("AntsMoveJob: Execute");
    static readonly ProfilerMarker k_AntsMoveJob_PheromoneSteering = new ProfilerMarker("AntsMoveJob: PheromoneSteering");
    static readonly ProfilerMarker k_AntsMoveJob_WallSteering = new ProfilerMarker("AntsMoveJob: ParametricWallSteering");

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AntData>();
        state.RequireForUpdate<FoodData>();
        state.RequireForUpdate<GlobalSettings>();
        state.RequireForUpdate<PheromoneBufferElement>();
        state.RequireForUpdate<ObstacleArcPrimitive>();
        state.RequireForUpdate<CollisionHashSet>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        
        AntsMoveJob moveJob = new AntsMoveJob()
        {
            Pheromones = SystemAPI.GetSingletonBuffer<PheromoneBufferElement>().AsNativeArray(),
            ObstacleArcPrimitiveBuffer = SystemAPI.GetSingletonBuffer<ObstacleArcPrimitive>().AsNativeArray(),
            CollisionHashSet = SystemAPI.GetSingleton<CollisionHashSet>().CollisionSet,
            GlobalSettings = globalSettings,
            Food = SystemAPI.GetSingleton<FoodData>(),
            WallSteeringDistance = globalSettings.AntSightDistance / globalSettings.MapSizeX
        };
        
        state.Dependency = moveJob.ScheduleParallel(state.Dependency);
    }

    public void OnDestroy(ref SystemState state)
    {

    }
    
    [BurstCompile]
    public partial struct AntsMoveJob : IJobEntity
    {
        static readonly float3 rotationAxis = new float3(0, 0, 1f);
        [ReadOnly] public NativeArray<PheromoneBufferElement> Pheromones;
        [ReadOnly] public NativeArray<ObstacleArcPrimitive> ObstacleArcPrimitiveBuffer;
        [ReadOnly] public NativeHashSet<int2> CollisionHashSet;
        [ReadOnly] public GlobalSettings GlobalSettings;
        [ReadOnly] public FoodData Food;
        [ReadOnly] public float WallSteeringDistance;

        void Execute(ref AntData ant, ref LocalTransform localTransform, ref URPMaterialPropertyBaseColor baseColor)
        {
           // k_AntsMoveJob_Execute.Begin();

            var randomSteering = GlobalSettings.AntRandomSteering;
            var antSpeed = GlobalSettings.AntSpeed;
            var antAccel = GlobalSettings.AntAccel;
            var goalSteerStrength = GlobalSettings.AntGoalSteerStrength;
            var mapSizeX = GlobalSettings.MapSizeX;
            var mapSizeY = GlobalSettings.MapSizeY;
            float mapSize = math.min(mapSizeX, mapSizeY);

            float2 antPosition;
            antPosition.x = localTransform.Position.x;
            antPosition.y = localTransform.Position.y;
           
           // random walk
           
           // Steering ------------------------------------------------------------------------------------------------
           
            ant.FacingAngle += ant.Rand.NextFloat(-randomSteering, randomSteering);
            float pheroSteering = PheromoneSteering(localTransform.Position, ant.FacingAngle, 3f, mapSizeX, mapSizeY, Pheromones);
            int wallSteering = ParametricWallSteering(ObstacleArcPrimitiveBuffer, CollisionHashSet, ant.FacingAngle, antPosition, WallSteeringDistance, mapSize, GlobalSettings.WallThickness);

            ant.FacingAngle += pheroSteering * GlobalSettings.PheromoneSteerStrength;
            ant.FacingAngle += wallSteering * GlobalSettings.WallSteerStrength;

            // Speed ------------------------------------------------------------------------------------------------
            float targetSpeed = antSpeed;
            // targetSpeed *= 1f - (math.abs(pheroSteering) + math.abs(wallSteering)) / 3f;
            ant.Speed += (targetSpeed - ant.Speed) * antAccel;
            
            // Line cast -------------------------------------------------------------------------------------------------
            float2 collisionPoint = float2.zero;
            float param = 0;
            float2 targetVector = ant.TargetPosition - antPosition;
            if (!ObstacleSpawnerSystem.CalculateRayCollision(ObstacleArcPrimitiveBuffer, antPosition / mapSize, targetVector / mapSize, out collisionPoint, out param))
            {
                float targetAngle = math.atan2(
                    ant.TargetPosition.y - antPosition.y, 
                    ant.TargetPosition.x - antPosition.x);
                if (targetAngle - ant.FacingAngle > math.PI)
                {
                    ant.FacingAngle += math.PI * 2f;
                }
                else if (targetAngle - ant.FacingAngle < -math.PI)
                {
                    ant.FacingAngle -= math.PI * 2f;
                }
                else
                {
                    if (math.abs(targetAngle - ant.FacingAngle) < math.PI * .5f)
                    {
                        ant.FacingAngle += (targetAngle - ant.FacingAngle) * goalSteerStrength;
                    }
                }
            }
            
            float dx = math.cos(ant.FacingAngle) * ant.Speed;
            float dy = math.sin(ant.FacingAngle) * ant.Speed;
            
            // reverse on map edges
            bool flip = false;
            
            if (antPosition.x + dx < 0 || antPosition.x + dx > mapSizeX)
            {
                flip = true;
                dx = -dx;
            }
            
            if (antPosition.y + dy < 0 || antPosition.y + dy > mapSizeY)
            {
                flip = true;
                dy = -dy;
            }
            
            if (flip)
            {
                ant.FacingAngle = math.atan2(dy, dx);
            }
            
            antPosition.x += dx;
            antPosition.y += dy;
            
            // flip ant when it hits target
            if (math.distancesq(antPosition, ant.TargetPosition) < GlobalSettings.FoodGrabDistanceSq)
            {
                ant.HoldingResource = !ant.HoldingResource;
                baseColor.Value = ant.HoldingResource ? GlobalSettings.ExitedColor : GlobalSettings.RegularColor;
                ant.FacingAngle += math.PI;
                ant.TargetPosition = ant.HoldingResource ? ant.SpawnerCenter : Food.Center;
            }
            
            // clamp angle to +-180 degrees
            while (ant.FacingAngle < -math.PI)
            {
                ant.FacingAngle += math.PI * 2;
            }
            while (ant.FacingAngle > math.PI)
            {
                ant.FacingAngle -= math.PI * 2;
            }

            // TODO: need to scale simulation space to render space
            localTransform.Position.x = antPosition.x;
            localTransform.Position.y = antPosition.y;
            localTransform.Rotation = quaternion.AxisAngle(rotationAxis, ant.FacingAngle);

            //k_AntsMoveJob_Execute.End();
        }
    }
    
    static int ParametricWallSteering(in NativeArray<ObstacleArcPrimitive> ObstaclePrimtitveBuffer, in NativeHashSet<int2> hashSet, float facingAngle, float2 antPosition, float distance, float mapSize, float WallThickness)
    {
        //k_AntsMoveJob_WallSteering.Begin();

        int output = 0;
    
        float2 Direction, OutCollision;
        for (int i = -1; i <= 1; i += 2)
        {
            float angle = facingAngle + i * math.PI * .25f;
            
            while (angle < 0.0f) 
                angle += 2.0f * math.PI;
            
            Direction.x = math.cos(angle) * distance;
            Direction.y = math.sin(angle) * distance;

            int2 worldPos = (int2)(antPosition + Direction);

            // // if out of map bounds
            // if (worldPos.x <= 0 || worldPos.y <= 0 || worldPos.x >= mapSize || worldPos.y >= mapSize)
            // {
            //     return output = -(int)ant.FacingAngle;
            // }
            
            if (!hashSet.Contains(worldPos))
                continue;
            
            float2 AntWorldSpace = antPosition / mapSize;
            
            if (ParametricLineCast(ObstaclePrimtitveBuffer, AntWorldSpace, AntWorldSpace + Direction, out OutCollision))
            {
                float2 DirectionVec = OutCollision - AntWorldSpace;
    
                if (math.dot(DirectionVec, DirectionVec) > (distance * distance))
                {
                    continue;
                }

                float DirectionDist = math.length(DirectionVec) - WallThickness;
                float value = 4.0f * (1.0f - DirectionDist / distance);
                output -= i * (int)math.floor(value);
            }
        }

        //k_AntsMoveJob_WallSteering.End();

        return output;
    }

    static float PheromoneSteering(float3 position, float facingAngle, float distance, int mapSizeX, int mapSizeY, in NativeArray<PheromoneBufferElement> pheromones)
    {
        //k_AntsMoveJob_PheromoneSteering.Begin();

        float output = 0;

        for (int i = -1; i <= 1; i += 2)
        {
            float angle = facingAngle + i * math.PI * .25f;
                
            float testX = position.x + math.cos(angle) * distance;
            float testY = position.y + math.sin(angle) * distance;

            if (testX < 0 || testY < 0 || testX >= mapSizeX || testY >= mapSizeY)
            {
                // Should be empty
            }
            else
            {
                int index = PheromonesSystem.PheromoneIndex((int)testX, (int)testY, mapSizeX);
                float value = pheromones[index];
                output += value * i;
            }
        }

        //k_AntsMoveJob_PheromoneSteering.End();

        return math.sign(output);
    }
    
    //--------------------------------------------------------------------------------------------------------------

    #region Other
    
    
    
    static bool ParametricLineCast(in NativeArray<ObstacleArcPrimitive> ObstaclePrimtitveBuffer, float2 point1, float2 point2)
    {
        float2 collisionPoint;
        return ParametricLineCast(ObstaclePrimtitveBuffer, point1, point2, out collisionPoint);
    }
    static bool ParametricLineCast(in NativeArray<ObstacleArcPrimitive> ObstaclePrimtitveBuffer, float2 point1, float2 point2, out float2 OutCollision)
    {
        if (0 != ObstaclePrimtitveBuffer.Length)
        {
            float OutParam;
            float2 RayVec = point2 - point1;
            if (ObstacleSpawnerSystem.CalculateRayCollision(ObstaclePrimtitveBuffer, point1, RayVec, out OutCollision, out OutParam))
            {
                float2 DeltaVec = OutCollision - point1;
                if (math.dot(DeltaVec, DeltaVec) < math.dot(RayVec, RayVec))
                {
                    // Debug.DrawRay(point1, (point2 - point1), Color.red, 0.05f);
                    return true;
                }
            }
        }
    
        // Debug.DrawRay(point1, (point2 - point1), Color.green, 0.05f);
        OutCollision = point2;
        return false;
    }
    
    static int ParametricWallSteering(in NativeHashSet<int2> hashSet, float facingAngle, float2 antPosition, float distance, float mapSize, float WallThickness)
    {
        k_AntsMoveJob_WallSteering.Begin();

        int output = 0;
    
        float2 OutCollision;
        float2 positionAtMap;
        for (int i = -1; i <= 1; i += 2)
        {
            float angle = facingAngle + i * math.PI * .25f;
            while (angle < 0.0f) angle += 2.0f * math.PI;
    
            float dx = math.cos(angle);
            float dy = math.sin(angle);
    
            positionAtMap.x = dx * distance;
            positionAtMap.y = dy * distance;

            float2 AntWorldSpace = antPosition / mapSize;
            OutCollision = positionAtMap / mapSize;
            
            if (hashSet.Contains((int2)(positionAtMap + antPosition)))
            {
                float2 DirectionVec = OutCollision - AntWorldSpace;
                
                // if (math.dot(DirectionVec, DirectionVec) > (distance * distance))
                // {
                //     continue;
                // }


                float DirectionDist = math.length(DirectionVec) - WallThickness;
                float value = 4.0f * (1.0f - DirectionDist / distance);
                output -= i * (int)math.floor(value);

                output = math.clamp(output, -1, 1);

                //Debug.LogError($"OutCollision: {OutCollision.x}; {OutCollision.y}  - AntWorldSpace {AntWorldSpace.x}; {AntWorldSpace.y}: output:{output}");
            }
        }

        k_AntsMoveJob_WallSteering.End();

        return output;
    }
    #endregion

    
}
