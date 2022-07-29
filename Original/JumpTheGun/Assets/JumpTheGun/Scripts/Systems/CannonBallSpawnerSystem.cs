using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial struct CannonBallSpawnerSystem : ISystem
{
    // A ComponentDataFromEntity provides random access to a component (looking up an entity).
    // We'll use it to extract the world space position and orientation of the spawn point (cannon nozzle).
    ComponentDataFromEntity<LocalToWorld> m_LocalToWorldFromEntity;
    float tankLaunchDelay;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        // ComponentDataFromEntity structures have to be initialized once.
        // The parameter specifies if the lookups will be read only or if they should allow writes.
        m_LocalToWorldFromEntity = state.GetComponentDataFromEntity<LocalToWorld>(true);
        tankLaunchDelay = 3.0f;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // ComponentDataFromEntity structures have to be updated every frame.
        m_LocalToWorldFromEntity.Update(ref state);

        // Creating an EntityCommandBuffer to defer the structural changes required by instantiation.
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
       

        var CannonBallSpawningJob = new CannonBallSpawning
        {
            LocalToWorldFromEntity = m_LocalToWorldFromEntity,
            ECB = ecb
        };

         float currentTime = UnityEngine.Time.realtimeSinceStartup;

        if (currentTime > tankLaunchDelay)
        {
            //UnityEngine.Debug.Log("inside if");
            tankLaunchDelay = currentTime + 0.5f;

            // Schedule execution in a single thread, and do not block main thread.
            CannonBallSpawningJob.Schedule();
            
        }
        else {
            //UnityEngine.Debug.Log("outside if");
            return;

        }

    }

    [BurstCompile]
    partial struct CannonBallSpawning : IJobEntity
    {

        [ReadOnly] public ComponentDataFromEntity<LocalToWorld> LocalToWorldFromEntity;
        public EntityCommandBuffer ECB;


        // Note that the TurretAspects parameter is "in", which declares it as read only.
        // Making it "ref" (read-write) would not make a difference in this case, but you
        // will encounter situations where potential race conditions trigger the safety system.
        // So in general, using "in" everywhere possible is a good principle.
        void Execute(in TurretAspect turret)
        {
            
            var instance = ECB.Instantiate(turret.CannonBallPrefab);
            var spawnLocalToWorld = LocalToWorldFromEntity[turret.CannonBallSpawn];
            ECB.SetComponent<Translation>(instance, new Translation
            {
                Value = spawnLocalToWorld.Position
            });

           
        }
    }

}