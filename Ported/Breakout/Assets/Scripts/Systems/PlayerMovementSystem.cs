using Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

                // convert player in to a native array of entities
                var balls = BallQuery.ToEntityArray(Allocator.Temp);
                foreach (var ball in balls)
                {
                    var transform = SystemAPI.GetComponent<LocalToWorld>(ball);

                    if (math.distancesq(transform.Position, playerPostion) < 1.0f)
                    {
                        var speed = math.normalize(transform.Position - playerPostion) * 5.0f;
                        state.EntityManager.SetComponentData(ball, new Ball { Speed = speed });
                    }
                }

            }
        }
    }
}