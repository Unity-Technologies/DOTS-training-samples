using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(SetupGameSystem))]
[UpdateAfter(typeof(PropagateFireSystem))]
public partial class ExtinguishFire : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var gameConstants = GetSingleton<GameConstants>();
        var field = GetBuffer<FireHeat>(GetSingletonEntity<FireField>());
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        Entities
            .WithAll<ThrownBucket>()
            .WithNone<EmptyBucket>()
            .ForEach((Entity e, ref Bucket bucket, in Translation translation) =>
            {
                for (int outerY = 0; outerY < gameConstants.FieldSize.y; outerY++)
                {
                    for (int outerX = 0; outerX < gameConstants.FieldSize.x; outerX++)
                    {
                        float3 cellPos = new float3(outerX, 0, outerY);
                        var distance = math.distance(translation.Value, cellPos);

                        if (distance < gameConstants.BucketExtinguishRadius && field[outerX + outerY * gameConstants.FieldSize.x] > 0.2f)
                        {
                            field[outerX + outerY * gameConstants.FieldSize.x] -= gameConstants.BucketCoolingRate;
                            bucket.Volume = 0;
                            ecb.AddComponent<EmptyBucket>(e);
                        }
                    }
                }
            }).Schedule();
        //HACK: fix readonly query with DynamicBuffer
        Entities.ForEach((ref DynamicBuffer<FireHeat> _) => { }).Schedule();
        
        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}