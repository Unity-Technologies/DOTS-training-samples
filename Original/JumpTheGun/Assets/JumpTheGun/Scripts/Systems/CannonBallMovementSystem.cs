using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial class CannonBallMovementSystem : SystemBase
{
    public float speed = 10;
    public float arcHeight = 1;

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        LocalToWorld playerTransform = GetComponent<LocalToWorld>(GetSingletonEntity<PlayerComponent>());
        Entities
            .WithoutBurst()
            .WithAll<CannonBall>()
            .ForEach((ref TransformAspect transform) =>
            {
                float x0 = transform.Position.x;
                float x1 = playerTransform.Position.x;
                float dist = x1 - x0;
                float nextX = Mathf.MoveTowards(transform.Position.x, x1, speed * deltaTime);
                float baseY = Mathf.Lerp(transform.Position.y, playerTransform.Position.y, (nextX - x0) / dist);
                float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
                var nextPos = new Vector3(nextX, baseY + arc, transform.Position.z);
                transform.Position = nextPos;

                //ParabolaCluster.Create(transform.Position.y, 2.0f, playerTransform.Position.y,out entity.para.paraA, out entity.para.paraB, out entity.para.paraC);
                //math.slerp(transform.Rotation, playerTransform.Rotation, 2.0f);
                //translation.Value = translation.Value + playerTransform.Up * deltaTime;

                transform.Position = nextPos;

            }).Run();
    }

   
}