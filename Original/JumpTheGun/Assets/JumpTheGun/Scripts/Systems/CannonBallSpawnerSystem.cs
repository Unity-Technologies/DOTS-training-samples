using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.ComponentModel;

[BurstCompile]
partial class CannonBallSpawner : SystemBase
{
    public ComponentDataFromEntity<LocalToWorld> LocalToWorldFromEntity;
    public EntityCommandBuffer ECB;
    protected override void OnUpdate()
    {
        float deltaTime = this.Time.DeltaTime;

        // Check to make sure the target Entity still exists and has
        // the needed component
        if (!HasComponent<LocalToWorld>(GetSingletonEntity<PlayerComponent>()))
            return;

        // Look up the entity data
        LocalToWorld targetTransform = GetComponent<LocalToWorld>(GetSingletonEntity<PlayerComponent>());
       // Para para = GetComponent<Para>(CannonBall).paraA;

        Entities
            .WithAll<Turret>()
            .ForEach((TransformAspect transform, in Turret turret) =>
            {
               /* float3 targetPosition = targetTransform.Position;

                // Calculate the rotation
                float3 displacement = targetPosition - transform.Position;
                float3 upReference = new float3(0, 1, 0);
                displacement.y = transform.Position.y;
                quaternion lookRotation = quaternion.LookRotationSafe(displacement, upReference);

                transform.Rotation = math.slerp(transform.Rotation, lookRotation, deltaTime);

                var instance = ECB.Instantiate(turret.cannonBall);
                var spawnLocalToWorld = LocalToWorldFromEntity[turret.cannonBallSpawn];
                ECB.SetComponent(instance, new Translation { Value = spawnLocalToWorld.Position });*/
                
             //   ParabolaCluster.Create(transform.Position.y,2,targetPosition.y,);

            }).ScheduleParallel();

    }
}