using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct BeeConstructionJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public uint randomBase;

    public float3 absSpawnOffset;

    public float minSize;
    public float maxSize;
    public float maxSpawnSpeed;

    void Execute(Entity bee, [EntityInQueryIndex] int idx, in BeePrototype prototype, ref TransformAspect prs)
    {
        var random = Random.CreateFromIndex((uint)idx * randomBase);
        
        if (prototype.Hive == BeeTeam.Blue)
        {
            ECB.AddComponent<BlueTeam>(idx, bee);
        }
        else
        {
            ECB.AddComponent<YellowTeam>(idx, bee);
        }


        float4 beeColor;
        BeeSpawnHelper.GetTeamColor(prototype.Hive, out beeColor);
        ECB.AddComponent(idx, bee, new URPMaterialPropertyBaseColor { Value =  beeColor});

        var localToWorld = prs.LocalToWorld;
        float3 spawnSide = prototype.Hive == BeeTeam.Blue ? -localToWorld.Right() : localToWorld.Right();
        localToWorld.Position = prototype.SpawnPosition;

        float scale = minSize + (random.NextFloat() * (maxSize - minSize));
        localToWorld.Scale = scale;

        prs.LocalToWorld = localToWorld;

        ECB.AddComponent(idx, bee,
            new Velocity() { Value = random.NextFloat3Direction() * random.NextFloat() * maxSpawnSpeed });
        ECB.AddComponent(idx, bee, new SmoothDirection() { Value = float3.zero });
        ECB.AddComponent(idx, bee, new SmoothPosition() { Value = localToWorld.Right() * .01f });

        // TODO remove UniformScale once BeeClampingJob is updated
        ECB.AddComponent(idx, bee, new UniformScale() { Value = scale });
        ECB.AddComponent(idx, bee, new IsAttacking() { Value = false });
        ECB.AddComponent<IsHolding>(idx, bee);
        ECB.AddComponent<TargetId>(idx, bee);
        ECB.AddComponent<PostTransformMatrix>(idx, bee);
        ECB.AddComponent<Falling>(idx, bee);
        ECB.SetComponentEnabled<Falling>(idx, bee, false);
        ECB.SetComponentEnabled<IsAttacking>(idx, bee, false);
        ECB.SetComponentEnabled<IsHolding>(idx, bee, false);
        ECB.SetComponentEnabled<TargetId>(idx, bee, false);

        ECB.RemoveComponent<BeePrototype>(idx, bee);
    }
}

[BurstCompile]
[UpdateAfter(typeof(InitialBeeSpawningSystem))]
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

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();
        var fieldConfig = SystemAPI.GetSingleton<FieldConfig>();
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        new BeeConstructionJob()
        {
            ECB = ecb.AsParallelWriter(),
            randomBase = (uint)state.Time.ElapsedTime * 1000,
            absSpawnOffset = fieldConfig.FieldScale.x,
            minSize = config.minBeeSize,
            maxSize = config.maxBeeSize,
            maxSpawnSpeed = config.maxSpawnSpeed
        }.ScheduleParallel();
    }
}