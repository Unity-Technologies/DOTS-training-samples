using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct BeeSpawningJob : IJobEntity
{
    public EntityCommandBuffer ECB;
    public Random random;

    public float3 absSpawnOffset;

    public float minSize;
    public float maxSize;
    public float maxSpawnSpeed;

    void Execute(Entity bee)
    {
        float3 right;
        right.x = 1;
        right.y = 0;
        right.z = 0;
        
        if (random.NextBool())
        {
            ECB.AddComponent<BlueTeam>(bee);
            ECB.AddComponent(bee, new Position() { Value = right * (-absSpawnOffset * .4f + absSpawnOffset * .8f) });
        }
        else
        {
            ECB.AddComponent<YellowTeam>(bee);
            ECB.AddComponent(bee, new Position() { Value = -right * (-absSpawnOffset * .4f + absSpawnOffset * .8f) });
        }
        
        ECB.AddComponent(bee, new Velocity() { Value = random.NextFloat3Direction() * random.NextFloat() * maxSpawnSpeed });
        ECB.AddComponent(bee, new SmoothDirection() { Value = float3.zero });
        ECB.AddComponent(bee, new SmoothPosition() { Value = float3.zero });

        ECB.AddComponent(bee, new UniformScale() { Value = minSize + (random.NextFloat() * (maxSize - minSize)) });
        ECB.AddComponent<IsAttacking>(bee);
        ECB.AddComponent<IsHolding>(bee);

        ECB.SetComponentEnabled<IsAttacking>(bee, false);
        ECB.SetComponentEnabled<IsHolding>(bee, false);
    }
}

[BurstCompile]
partial struct BeeSpawningSystem : ISystem
{
    private EntityQuery m_prototypeBeesQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();

        m_prototypeBeesQuery =
            state.GetEntityQuery(typeof(IsAttacking), typeof(IsHolding), ComponentType.Exclude<Position>());
        state.RequireForUpdate(m_prototypeBeesQuery);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        new BeeSpawningJob()
        {
            ECB = ecb,
            random = Random.CreateFromIndex((uint)state.Time.ElapsedTime*1000),
            absSpawnOffset = Field.size.x,
            minSize = config.minBeeSize,
            maxSize = config.maxBeeSize,
            maxSpawnSpeed = config.maxSpawnSpeed
        }.Schedule(m_prototypeBeesQuery);
    }
}