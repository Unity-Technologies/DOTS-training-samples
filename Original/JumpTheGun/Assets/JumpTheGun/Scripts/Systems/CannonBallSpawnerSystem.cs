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
        float currentTime = UnityEngine.Time.realtimeSinceStartup;

        m_LocalToWorldFromEntity.Update(this);
        var ecb = endSimulationBufferSystem.CreateCommandBuffer();
        var config = SystemAPI.GetSingleton<Config>();


        Entities
            .WithoutBurst()
            .ForEach((ref Turret turret) =>
            {

                if (config.tankLaunchPeriod > currentTime)
                {
                    return;
                }

                config.tankLaunchPeriod = currentTime + 2;
                //UnityEngine.Debug.Log("config.tankLaunchPeriod " + config.tankLaunchPeriod);
                //UnityEngine.Debug.Log("current time "+ currentTime);

                var newEntity = ecb.Instantiate(turret.cannonBall);
                var spawnPoint = m_LocalToWorldFromEntity[turret.cannonBallSpawn];

                ecb.SetComponent<Translation>(newEntity, new Translation
                {
                    Value = spawnPoint.Position
                });

            }).Run();

    }
}