using System;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct AntsManagementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Ant>();
        state.RequireForUpdate<Config>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var random = Unity.Mathematics.Random.CreateFromIndex((uint)DateTime.UtcNow.Ticks);//this should be centralised

        var config = SystemAPI.GetSingleton<Config>();
        
        foreach (var (ant, entity) in SystemAPI.Query<RefRW<Ant>>().WithEntityAccess())
        {
            //creating a var rwAnt = ant.ValueRW and updating this resulted in the changes not propagating up to the actual components
            
            var facingAngle = ant.ValueRO.facingAngle;
            
            facingAngle += random.NextFloat(-config.RandomSteering, config.RandomSteering) * SystemAPI.Time.DeltaTime;

            var speed = ant.ValueRO.speed * SystemAPI.Time.DeltaTime;
            
            float vx = math.cos(facingAngle) * speed;
            float vy = math.sin(facingAngle) * speed;
            
            var position = ant.ValueRO.position;
            if (position.x + vx < 0f || position.x + vx > config.MapSize) 
            {
                facingAngle += math.PI;//this should model reflection, not inversion (as it is here)
            }
            else
            {
                position.x += vx;
            }

            if (position.y + vy < 0f || position.y + vy > config.MapSize) // since the direction of vy is reversed, this logic needs revisited
            {
                facingAngle += math.PI;//this should model reflection, not inversion (as it is here)
            }
            else
            {
                position.y += vy; //this used to be += vy
            }

            //I'm not sure that this is the most efficient way, but it intuitively feels like it to me
            ant.ValueRW.facingAngle = facingAngle;
            ant.ValueRW.position = position;
            
            state.EntityManager.SetComponentData(entity,
                LocalTransform.FromPositionRotationScale(
                    new float3(position.x, 0f, position.y),
                    quaternion.AxisAngle(new float3(0,1,0), facingAngle),
                    config.ObstacleRadius * 2));
        }
    }
}
