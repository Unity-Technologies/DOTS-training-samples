using Aspects;
using Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{

    [BurstCompile]
    public partial struct PlayerMovementSystem : ISystem
    {
        private EntityQuery BallQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
			state.RequireForUpdate<Config>();
            BallQuery = SystemAPI.QueryBuilder().WithAll<Ball>().Build();
            state.RequireForUpdate(BallQuery);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            var direction = new float3();
            direction.x = Input.GetAxis("Horizontal");
            direction.z = Input.GetAxis("Vertical");
            direction *= 5 * SystemAPI.Time.DeltaTime;

            float3 playerPostion = new float3();

            foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<Player>())
            {
                transform.LocalPosition += direction;
                playerPostion = transform.LocalPosition;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var ball in SystemAPI.Query<BallAspect>())
                {
                    if (math.distancesq(ball.Position, playerPostion) < config.PlayerImpactRadius)
                    {
                        ball.Speed = math.normalize(ball.Position - playerPostion) * 5.0f;
                    }
                }
            }
        }
    }
}