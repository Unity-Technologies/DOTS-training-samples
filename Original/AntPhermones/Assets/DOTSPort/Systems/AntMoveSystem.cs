//using System.Collections;
//using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
//using UnityEngine;

public partial struct AntMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AntData>();
    }
    public void OnDestroy(ref SystemState state)
    {

    }
    public void OnUpdate(ref SystemState state)
    {
        // make consts configurable
        const float randomSteering = math.PI / 4f;
        const float antSpeed = 0.2f;
        const float antAccel = 0.07f;
        const float mapSize = 1024f;

        float dt = SystemAPI.Time.DeltaTime;

        foreach (var ant in SystemAPI.Query<RefRW<AntData>, RefRW<LocalTransform>>())
        {
            // random walk
            ant.Item1.ValueRW.FacingAngle += ant.Item1.ValueRW.Rand.NextFloat(-randomSteering, randomSteering) * dt * 4;

            // TODO: adjust for pheremone and walls
            //float pheroSteering = PheromoneSteering(ant, 3f);
            //int wallSteering = WallSteering(ant, 1.5f);
            //ant.facingAngle += pheroSteering * pheromoneSteerStrength;
            //ant.facingAngle += wallSteering * wallSteerStrength;

            float targetSpeed = antSpeed;
            targetSpeed *= 1f /*- (Mathf.Abs(pheroSteering) + Mathf.Abs(wallSteering)) / 3f*/;

            ant.Item1.ValueRW.Speed += (targetSpeed - ant.Item1.ValueRO.Speed) * antAccel;

            // TODO: adjust for goal

            float vx = math.cos(ant.Item1.ValueRW.FacingAngle) * ant.Item1.ValueRW.Speed;
            float vy = math.sin(ant.Item1.ValueRW.FacingAngle) * ant.Item1.ValueRW.Speed;
            float ovx = vx;
            float ovy = vy;

            float dx = vx * dt;
            float dy = vy * dt;

            // reverse on map edges
            if (ant.Item1.ValueRO.Position.x + dx < 0 || ant.Item1.ValueRO.Position.x + dx > mapSize)
            {
                vx = -vx;
                dx = vx * dt;
            }
            if (ant.Item1.ValueRO.Position.y + dy < 0 || ant.Item1.ValueRO.Position.y + dy > mapSize)
            {
                vy = -vy;
                dy = vy * dt;
            }
            ant.Item1.ValueRW.Position.x += dx;
            ant.Item1.ValueRW.Position.y += dy;

            // TODO: push ant away from walls

            // TODO: need to scale simulation space to render space
            ant.Item2.ValueRW.Position.x = ant.Item1.ValueRO.SpawnerCenter.x + ant.Item1.ValueRW.Position.x;
            ant.Item2.ValueRW.Position.y = ant.Item1.ValueRO.SpawnerCenter.y + ant.Item1.ValueRW.Position.y;
            ant.Item2.ValueRW.Rotation = quaternion.AxisAngle(new float3(0, 0, 1f), ant.Item1.ValueRW.FacingAngle);
        }
    }
}
