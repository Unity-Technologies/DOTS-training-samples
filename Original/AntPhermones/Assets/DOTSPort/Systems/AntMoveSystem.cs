using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial struct AntMoveSystem : ISystem
{
    DynamicBuffer<ObstacleArcPrimitive> ObstacleArcPrimitiveBuffer;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AntData>();
        state.RequireForUpdate<FoodData>();
        state.RequireForUpdate<GlobalSettings>();
        state.RequireForUpdate<PheromoneBufferElement>();
        state.RequireForUpdate<ObstacleArcPrimitive>();
    }
    public void OnDestroy(ref SystemState state)
    {

    }

    int ParametricWallSteering(RefRW<AntData> ant, float distance, float mapSize, float WallThickness)
    {
        int output = 0;
    
        float2 Direction, OutCollision;
        for (int i = -1; i <= 1; i += 2)
        {
            float angle = ant.ValueRO.FacingAngle + i * math.PI * .25f;
            while (angle < 0.0f) angle += 2.0f * math.PI;
    
            float dx = math.cos(angle);
            float dy = math.sin(angle);
    
            Direction.x = dx * distance;
            Direction.y = dy * distance;
    
            float2 AntWorldSpace = ant.ValueRO.Position / mapSize;
            if (ParametricLineCast(AntWorldSpace, AntWorldSpace + Direction, out OutCollision))
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
        return output;
    }
    
    bool ParametricLineCast(float2 point1, float2 point2)
    {
        float2 collisionPoint;
        return ParametricLineCast(point1, point2, out collisionPoint);
    }
    bool ParametricLineCast(float2 point1, float2 point2, out float2 OutCollision)
    {
        if (0 != ObstacleArcPrimitiveBuffer.Length)
        {
            float OutParam;
            float2 RayVec = point2 - point1;
            if (ObstacleSpawnerSystem.CalculateRayCollision(ObstacleArcPrimitiveBuffer, point1, RayVec, out OutCollision, out OutParam))
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

    public void OnUpdate(ref SystemState state)
    {
        // make consts configurable
        float randomSteering = 0;
        float antSpeed = 0;
        float antAccel = 0;
        int mapSizeX = 1024;
        int mapSizeY = 1024;
        float goalSteerStrength = 0.04f;
        float wallSteerStrength = 0.15f;

        var settings = SystemAPI.GetSingleton<GlobalSettings>();
        randomSteering = settings.AntRandomSteering;
        antSpeed = settings.AntSpeed;
        antAccel = settings.AntAccel;
        goalSteerStrength = settings.AntGoalSteerStrength;
        mapSizeX = settings.MapSizeX;
        mapSizeY = settings.MapSizeY;
        float mapSize = math.min(mapSizeX, mapSizeY);
        float WallThickness = 0.01f;

        float antSightDistance = 5.0f;

        var food = SystemAPI.GetSingleton<FoodData>();
        var pheromoneBuffer = SystemAPI.GetSingletonBuffer<PheromoneBufferElement>();
        ObstacleArcPrimitiveBuffer = SystemAPI.GetSingletonBuffer<ObstacleArcPrimitive>();

        float dt = SystemAPI.Time.DeltaTime;

        foreach (var ant in SystemAPI.Query<RefRW<AntData>, RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>())
        {
            // random walk
            ant.Item1.ValueRW.FacingAngle += ant.Item1.ValueRW.Rand.NextFloat(-randomSteering, randomSteering) * dt * 4;

            // TODO: adjust for pheremone and walls
            float pheroSteering = PheromoneSteering(ant.Item2.ValueRO.Position, ant.Item1.ValueRO.FacingAngle, 3f, mapSizeX, mapSizeY, pheromoneBuffer);
            //int wallSteering = WallSteering(ant, 1.5f);
            ant.Item1.ValueRW.FacingAngle += pheroSteering * settings.PheromoneSteerStrength;
            //ant.facingAngle += wallSteering * wallSteerStrength;

            float targetSpeed = antSpeed;
            targetSpeed *= 1f /*- (Mathf.Abs(pheroSteering) + Mathf.Abs(wallSteering)) / 3f*/;

            ant.Item1.ValueRW.Speed += (targetSpeed - ant.Item1.ValueRW.Speed) * antAccel;

            // adjust for goal
            float2 targetPos;
            float targetRadius;
            if (ant.Item1.ValueRW.HoldingResource)
            {
                targetPos = ant.Item1.ValueRO.SpawnerCenter;
                targetRadius = 1.0f;
            } else
            {
                targetPos = food.Center;
                targetRadius = food.Radius;
            }
            // TODO: linecast to target to see if the ant sees it
            float2 collisionPoint = float2.zero;
            float param = 0;
            float2 targetVector = targetPos - ant.Item1.ValueRW.Position;
            if (!ObstacleSpawnerSystem.CalculateRayCollision(ObstacleArcPrimitiveBuffer, ant.Item1.ValueRW.Position / mapSize, targetVector / mapSize, out collisionPoint, out param))
            {
                float targetAngle = math.atan2(
                    targetPos.y - ant.Item1.ValueRW.Position.y, 
                    targetPos.x - ant.Item1.ValueRW.Position.x);
                if (targetAngle - ant.Item1.ValueRW.FacingAngle > math.PI)
                {
                    ant.Item1.ValueRW.FacingAngle += math.PI * 2f;
                }
                else if (targetAngle - ant.Item1.ValueRW.FacingAngle < -math.PI)
                {
                    ant.Item1.ValueRW.FacingAngle -= math.PI * 2f;
                }
                else
                {
                    if (math.abs(targetAngle - ant.Item1.ValueRW.FacingAngle) < math.PI * .5f)
                    {
                        ant.Item1.ValueRW.FacingAngle += (targetAngle - ant.Item1.ValueRW.FacingAngle) * goalSteerStrength;
                    }
                }
                //Debug.DrawLine(ant.position/mapSize,targetPos/mapSize,color);
            }

            float vx = math.cos(ant.Item1.ValueRW.FacingAngle) * ant.Item1.ValueRW.Speed;
            float vy = math.sin(ant.Item1.ValueRW.FacingAngle) * ant.Item1.ValueRW.Speed;
            float ovx = vx;
            float ovy = vy;

            float dx = vx * dt;
            float dy = vy * dt;

            // reverse on map edges
            if (ant.Item1.ValueRO.Position.x + dx < 0 || ant.Item1.ValueRO.Position.x + dx > mapSizeX)
            {
                //ant.Item1.ValueRW.FacingAngle += math.PI;
                vx = -vx;
                dx = vx * dt;
            }
            if (ant.Item1.ValueRO.Position.y + dy < 0 || ant.Item1.ValueRO.Position.y + dy > mapSizeY)
            {
                //ant.Item1.ValueRW.FacingAngle += math.PI;
                vy = -vy;
                dy = vy * dt;
            }
            ant.Item1.ValueRW.Position.x += dx;
            ant.Item1.ValueRW.Position.y += dy;

            // TODO: push ant away from walls
            int wallSteering = ParametricWallSteering(ant.Item1, antSightDistance / mapSize, mapSize, WallThickness);
            // ant.Item1.ValueRW.FacingAngle += pheroSteering * pheromoneSteerStrength;
            ant.Item1.ValueRW.FacingAngle += wallSteering * wallSteerStrength;

            // flip ant when it hits target
            if (math.distance(ant.Item1.ValueRW.Position, targetPos) < targetRadius)
            {
                ant.Item1.ValueRW.HoldingResource = !ant.Item1.ValueRO.HoldingResource;
                ant.Item3.ValueRW.Value = ant.Item1.ValueRO.HoldingResource ? settings.ExitedColor : settings.RegularColor;
                ant.Item1.ValueRW.FacingAngle += math.PI;
            }
            // clamp angle to +-180 degrees
            while (ant.Item1.ValueRW.FacingAngle < -math.PI)
            {
                ant.Item1.ValueRW.FacingAngle += math.PI * 2;
            }
            while (ant.Item1.ValueRW.FacingAngle > math.PI)
            {
                ant.Item1.ValueRW.FacingAngle -= math.PI * 2;
            }

            // TODO: need to scale simulation space to render space
            ant.Item2.ValueRW.Position.x = ant.Item1.ValueRW.Position.x;
            ant.Item2.ValueRW.Position.y = ant.Item1.ValueRW.Position.y;
            ant.Item2.ValueRW.Rotation = quaternion.AxisAngle(new float3(0, 0, 1f), ant.Item1.ValueRW.FacingAngle);
        }
        
     //   // TODO: uncomment for jobs
     //   AntsMoveJob moveJob = new AntsMoveJob()
     //   {
     //       Pheromones = pheromoneBuffer.AsNativeArray(),
     //      // Obstacles = ObstacleArcPrimitiveBuffer.AsNativeArray(),
     //       GlobalSettings = settings
     //   };
     //   
     //   state.Dependency = moveJob.ScheduleParallel(state.Dependency);
    }
    
    [BurstCompile]
    public partial struct AntsMoveJob : IJobEntity
    {
        [ReadOnly] public NativeArray<PheromoneBufferElement> Pheromones;
       // [ReadOnly] public NativeArray<ObstacleArcPrimitive> Obstacles;
        [ReadOnly] public GlobalSettings GlobalSettings;
        
        void Execute(ref AntData ant, ref LocalTransform localTransform, ref URPMaterialPropertyBaseColor baseColor)
        {
               // TODO: move all code here
        }
    }
    
    static float PheromoneSteering(float3 position, float facingAngle, float distance, int mapSizeX, int mapSizeY, DynamicBuffer<PheromoneBufferElement> pheromones)
    {
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

        return math.sign(output);
    }
}
