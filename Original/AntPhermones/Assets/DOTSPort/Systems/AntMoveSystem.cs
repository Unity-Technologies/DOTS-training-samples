using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial struct AntMoveSystem : ISystem
{
    //DynamicBuffer<ObstacleArcPrimitive> ObstacleArcPrimitiveBuffer;

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

    static int ParametricWallSteering(in NativeArray<ObstacleArcPrimitive> ObstaclePrimtitveBuffer, AntData ant, float distance, float mapSize, float WallThickness)
    {
        int output = 0;
    
        float2 Direction, OutCollision;
        for (int i = -1; i <= 1; i += 2)
        {
            float angle = ant.FacingAngle + i * math.PI * .25f;
            while (angle < 0.0f) angle += 2.0f * math.PI;
    
            float dx = math.cos(angle);
            float dy = math.sin(angle);
    
            Direction.x = dx * distance;
            Direction.y = dy * distance;
    
            float2 AntWorldSpace = ant.Position / mapSize;
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
        return output;
    }
    
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

    public void OnUpdate(ref SystemState state)
    {
        AntsMoveJob moveJob = new AntsMoveJob()
        {
            Pheromones = SystemAPI.GetSingletonBuffer<PheromoneBufferElement>().AsNativeArray(),
            ObstacleArcPrimitiveBuffer = SystemAPI.GetSingletonBuffer<ObstacleArcPrimitive>().AsNativeArray(),
            DeltaTime = SystemAPI.Time.DeltaTime,
            GlobalSettings = SystemAPI.GetSingleton<GlobalSettings>(),
            Food = SystemAPI.GetSingleton<FoodData>()
        };
        
        state.Dependency = moveJob.ScheduleParallel(state.Dependency);
    }
    
    [BurstCompile]
    public partial struct AntsMoveJob : IJobEntity
    {
        [ReadOnly] public NativeArray<PheromoneBufferElement> Pheromones;
        [ReadOnly] public NativeArray<ObstacleArcPrimitive> ObstacleArcPrimitiveBuffer;
        [ReadOnly] public GlobalSettings GlobalSettings;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public FoodData Food;
        
        void Execute(ref AntData ant, ref LocalTransform localTransform, ref URPMaterialPropertyBaseColor baseColor)
        {
            var randomSteering = GlobalSettings.AntRandomSteering;
            var antSpeed = GlobalSettings.AntSpeed;
            var antAccel = GlobalSettings.AntAccel;
            var goalSteerStrength = GlobalSettings.AntGoalSteerStrength;
            var mapSizeX = GlobalSettings.MapSizeX;
            var mapSizeY = GlobalSettings.MapSizeY;
            float mapSize = math.min(mapSizeX, mapSizeY);

            // foreach (var ant in
           // SystemAPI.Query<
           
           // RefRW<AntData>,
           // RefRW<LocalTransform>,
           // RefRW<URPMaterialPropertyBaseColor>>())
           
           // random walk
           
            ant.FacingAngle += ant.Rand.NextFloat(-randomSteering, randomSteering) * DeltaTime * 4;

            // TODO: adjust for pheremone and walls
            float pheroSteering = PheromoneSteering(localTransform.Position, ant.FacingAngle, 3f, mapSizeX, mapSizeY, Pheromones);
            //int wallSteering = WallSteering(ant, 1.5f);
            ant.FacingAngle += pheroSteering * GlobalSettings.PheromoneSteerStrength;
            //ant.facingAngle += wallSteering * wallSteerStrength;

            float targetSpeed = antSpeed;
            targetSpeed *= 1f /*- (Mathf.Abs(pheroSteering) + Mathf.Abs(wallSteering)) / 3f*/;

            ant.Speed += (targetSpeed - ant.Speed) * antAccel;

            // adjust for goal
            float2 targetPos;
            float targetRadius;
            if (ant.HoldingResource)
            {
                targetPos = ant.SpawnerCenter;
                targetRadius = 1.0f;
            } else
            {
                targetPos = Food.Center;
                targetRadius = Food.Radius;
            }
            // TODO: linecast to target to see if the ant sees it
            float2 collisionPoint = float2.zero;
            float param = 0;
            float2 targetVector = targetPos - ant.Position;
            if (!ObstacleSpawnerSystem.CalculateRayCollision(ObstacleArcPrimitiveBuffer, ant.Position / mapSize, targetVector / mapSize, out collisionPoint, out param))
            {
                float targetAngle = math.atan2(
                    targetPos.y - ant.Position.y, 
                    targetPos.x - ant.Position.x);
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
                //Debug.DrawLine(ant.position/mapSize,targetPos/mapSize,color);
            }

            float vx = math.cos(ant.FacingAngle) * ant.Speed;
            float vy = math.sin(ant.FacingAngle) * ant.Speed;
            float ovx = vx;
            float ovy = vy;

            float dx = vx * DeltaTime;
            float dy = vy * DeltaTime;

            // reverse on map edges
            if (ant.Position.x + dx < 0 || ant.Position.x + dx > mapSizeX)
            {
                //ant.FacingAngle += math.PI;
                vx = -vx;
                dx = vx * DeltaTime;
            }
            if (ant.Position.y + dy < 0 || ant.Position.y + dy > mapSizeY)
            {
                //ant.FacingAngle += math.PI;
                vy = -vy;
                dy = vy * DeltaTime;
            }
            ant.Position.x += dx;
            ant.Position.y += dy;

            // TODO: push ant away from walls
            int wallSteering = ParametricWallSteering(ObstacleArcPrimitiveBuffer, ant, GlobalSettings.AntSightDistance / mapSize, mapSize, GlobalSettings.WallThickness);
            // ant.FacingAngle += pheroSteering * pheromoneSteerStrength;
            ant.FacingAngle += wallSteering * GlobalSettings.WallSteerStrength;

            // flip ant when it hits target
            if (math.distance(ant.Position, targetPos) < targetRadius)
            {
                ant.HoldingResource = !ant.HoldingResource;
                baseColor.Value = ant.HoldingResource ? GlobalSettings.ExitedColor : GlobalSettings.RegularColor;
                ant.FacingAngle += math.PI;
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
            localTransform.Position.x = ant.Position.x;
            localTransform.Position.y = ant.Position.y;
            localTransform.Rotation = quaternion.AxisAngle(new float3(0, 0, 1f), ant.FacingAngle);
        }
    }
    
    static float PheromoneSteering(float3 position, float facingAngle, float distance, int mapSizeX, int mapSizeY, NativeArray<PheromoneBufferElement> pheromones)
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
