using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

[BurstCompile]
partial class CannonBallMovementSystem : SystemBase
{

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

       LocalToWorld playerTransform = GetComponent<LocalToWorld>(GetSingletonEntity<PlayerComponent>());

        Entities
            .WithoutBurst()
            .WithAll<CannonBall>()
            .ForEach((ref CannonBall entity, ref TransformAspect transform) =>
            {
              ParabolaCluster.Create(transform.Position.y, 2.0f, playerTransform.Position.y,out entity.para.paraA, out entity.para.paraB, out entity.para.paraC);

            }).Run();
    }

   
}