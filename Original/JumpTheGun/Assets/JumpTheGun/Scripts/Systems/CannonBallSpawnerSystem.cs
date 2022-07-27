using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.ComponentModel;


[BurstCompile]
partial class CannonBallSpawner : SystemBase
{
    ComponentDataFromEntity<LocalToWorld> m_LocalToWorldFromEntity;
    EndSimulationEntityCommandBufferSystem endSimulationBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        endSimulationBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_LocalToWorldFromEntity = GetComponentDataFromEntity<LocalToWorld>(true);
    }

    protected override void OnUpdate()
    {
        m_LocalToWorldFromEntity.Update(this);
        float currentTime = UnityEngine.Time.realtimeSinceStartup;
        var ecb = endSimulationBufferSystem.CreateCommandBuffer();

        Entities
            .WithoutBurst()
            .ForEach((ref Turret turret) =>
            {
                /*if (config.tankLaunchPeriod> currentTime)
                {
                    return;
                }*/

               // config.tankLaunchPeriod = currentTime + (config.tankLaunchPeriod);

                var newEntity = ecb.Instantiate(turret.cannonBall);
                var spawnPoint = m_LocalToWorldFromEntity[turret.cannonBallSpawn];



                ecb.SetComponent<Translation>(newEntity, new Translation
                {
                    Value = spawnPoint.Position
                }) ;
               
            }).Run();
    }
}