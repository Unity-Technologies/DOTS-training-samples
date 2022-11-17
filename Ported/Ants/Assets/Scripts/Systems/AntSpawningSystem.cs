using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;


[BurstCompile]
[UpdateAfter(typeof(MapInitiationSystem))]
public partial struct AntSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var rand = new Random(123);

        var config = SystemAPI.GetSingleton<Config>();

        //Only use ECB if you don't pass it into a job!
        // var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        // var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var ants = CollectionHelper.CreateNativeArray<Entity>(config.Amount, state.WorldUpdateAllocator); // use this, so that the native array is disposed automatically after 2 ms ! Thats is what we use for jobs 
        // use the entity manager, if your going to use an ECB in a job.
        state.EntityManager.Instantiate(config.AntPrefab, ants); 
        var xPos = config.MapSize * 0.5f;
        var yPos = config.MapSize * 0.5f;

        new SpawningJob()
        {
            xPos = xPos,
            yPos = yPos,
            rand = rand
        }.ScheduleParallel();

        state.Enabled = false;
    }
}

[BurstCompile]
public partial struct SpawningJob : IJobEntity
{
    public float xPos;
    public float yPos;
    public Random rand;

    public void Execute(ref Ant ant, ref LocalToWorldTransform localToWorldTransform,
        ref PostTransformMatrix postTransformMatrix)
    {
        //faster to set component than to add component
        var antComponent = new Ant()
        {
            Speed = 5,
            Angle = rand.NextFloat(0f, 360f),
            HasFood = false
        };
        ant = antComponent;

        var local = new LocalToWorldTransform();
        local.Value.Position = new float3(xPos, yPos, 0f);
        local.Value.Scale = 1f;
        localToWorldTransform = local;

        var postTransform = new PostTransformMatrix()
        {
            Value = float4x4.Scale(1f, 0.5f, 0.5f)
        };
        postTransformMatrix = postTransform;
    }
}