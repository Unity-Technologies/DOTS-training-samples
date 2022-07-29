using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial class CannonBallMovementSystem : SystemBase
{
    public float speed = 5;
    public float arcHeight = 0.2f;
    private EntityCommandBufferSystem ecsSystem;
    protected override void OnCreate()
    {
        ecsSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var ecb = ecsSystem.CreateCommandBuffer();
        var ecbParallel = ecb.AsParallelWriter();
        float deltaTime = Time.DeltaTime;
        LocalToWorld playerTransform = GetComponent<LocalToWorld>(GetSingletonEntity<PlayerComponent>());
        Entities
            .WithoutBurst()
            .WithAll<CannonBall>()
            .ForEach((int entityInQueryIndex, Entity entity,ref TransformAspect transform) =>
            {
                float x0 = transform.Position.x;
                float x1 = playerTransform.Position.x;
                float dist = x1 - x0;
                float nextX = Mathf.MoveTowards(transform.Position.x, x1, speed * deltaTime);
                float baseY = Mathf.Lerp(transform.Position.y, playerTransform.Position.y, (nextX - x0) / dist);
                float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
                var nextPos = new Vector3(nextX, baseY + arc, transform.Position.z);
                transform.Position = nextPos;

                if (nextPos.x == playerTransform.Position.x
                  || nextPos.y == playerTransform.Position.y
                  || nextPos.z == playerTransform.Position.z)
                {
                    //UnityEngine.Debug.Log("after destroy");
                    ecbParallel.DestroyEntity(entityInQueryIndex, entity);
                }





            }).Run();
    }

   
}