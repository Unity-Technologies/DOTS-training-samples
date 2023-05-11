using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial struct AntMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AntData>();
        state.RequireForUpdate<FoodData>();
        state.RequireForUpdate<GlobalSettings>();
        state.RequireForUpdate<PheromoneBufferElement>();
    }
    public void OnDestroy(ref SystemState state)
    {

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

        var settings = SystemAPI.GetSingleton<GlobalSettings>();
        randomSteering = settings.AntRandomSteering;
        antSpeed = settings.AntSpeed;
        antAccel = settings.AntAccel;
        goalSteerStrength = settings.AntGoalSteerStrength;
        mapSizeX = settings.MapSizeX;
        mapSizeY = settings.MapSizeY;

        var food = SystemAPI.GetSingleton<FoodData>();
        var pheromoneBuffer = SystemAPI.GetSingletonBuffer<PheromoneBufferElement>();

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
            if (true)
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
                        //ant.Item1.ValueRW.FacingAngle = targetAngle;
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
    }
    
    [BurstCompile]
    public partial struct AntsMoveJob : IJobEntity
    {
        [ReadOnly] public NativeArray<PheromoneBufferElement> Pheromones;
        [ReadOnly] public GlobalSettings GlobalSettings;
        void Execute(ref AntData ant, ref LocalTransform localTransform, ref URPMaterialPropertyBaseColor baseColor)
        {
               
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
