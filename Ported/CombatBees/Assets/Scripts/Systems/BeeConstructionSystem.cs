using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct BeeConstructionJob : IJobEntity
{
    public EntityCommandBuffer ECB;
    public Random random;

    public float3 absSpawnOffset;

    public float minSize;
    public float maxSize;
    public float maxSpawnSpeed;

    void Execute(Entity bee, in BeePrototype prototype, ref TransformAspect prs)
    {
        if (prototype.Hive == BeeTeam.Blue)
        {
            ECB.AddComponent<BlueTeam>(bee);
        }
        else
        {
            ECB.AddComponent<YellowTeam>(bee);
        }
        
        ECB.AddComponent(bee, new URPMaterialPropertyBaseColor{ Value = BeeSpawnHelper.GetTeamColor(prototype.Hive) });

        var localToWorld = prs.LocalToWorld;
        float3 spawnSide = prototype.Hive == BeeTeam.Blue ? localToWorld.Right() : -localToWorld.Right();
        if (prototype.GroundSpawnPosition.Equals(float2.zero))
        {
            localToWorld.Position = spawnSide * (-absSpawnOffset * .4f + absSpawnOffset * .8f);
        }
        else
        {
            localToWorld.Position.x = prototype.GroundSpawnPosition.x;
            localToWorld.Position.z = prototype.GroundSpawnPosition.y;   
        }

        float scale = minSize + (random.NextFloat() * (maxSize - minSize));
        localToWorld.Scale = scale;

        prs.LocalToWorld = localToWorld;

        ECB.AddComponent(bee,
            new Velocity() { Value = random.NextFloat3Direction() * random.NextFloat() * maxSpawnSpeed });
        ECB.AddComponent(bee, new SmoothDirection() { Value = float3.zero });
        ECB.AddComponent(bee, new SmoothPosition() { Value = localToWorld.Right() * .01f });

        // TODO remove UniformScale once BeeClampingJob is updated
        ECB.AddComponent(bee, new UniformScale() { Value = scale });
        ECB.AddComponent(bee, new IsAttacking() {Value = false} );
        ECB.AddComponent<IsHolding>(bee);
        ECB.AddComponent<TargetId>(bee);
        ECB.AddComponent<PostTransformMatrix>(bee);
        ECB.SetComponentEnabled<IsAttacking>(bee, false);
        ECB.SetComponentEnabled<IsHolding>(bee, false);
        ECB.SetComponentEnabled<TargetId>(bee, false);
        
        ECB.RemoveComponent<BeePrototype>(bee);
    }
}

[BurstCompile]
partial struct BeeConstructionSystem : ISystem
{
    private EntityQuery m_prototypeBeesQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        new BeeConstructionJob()
        {
            ECB = ecb,
            random = Random.CreateFromIndex((uint)state.Time.ElapsedTime * 1000),
            absSpawnOffset = Field.size.x,
            minSize = config.minBeeSize,
            maxSize = config.maxBeeSize,
            maxSpawnSpeed = config.maxSpawnSpeed
        }.Schedule();
    }
}