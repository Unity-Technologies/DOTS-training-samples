using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial class FarmerIntentSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(World.Unmanaged).AsParallelWriter();

        float dt = Time.DeltaTime;
        Entities.WithAll<Farmer>().ForEach((Entity entity, int entityInQueryIndex, ref FarmerIntent intent) =>
        {
            if (intent.value == FarmerIntentState.None)
            {
                intent.value = /*(FarmerIntentState)intent.random.NextInt(1, 3);*/FarmerIntentState.TillGround; // Temporary: Just till ground for now.
                //ecb.SetComponent<FarmerIntent>(entityInQueryIndex, entity, intent);
                ecb.AddComponent<ColorOverride>(entityInQueryIndex, entity, new ColorOverride { Value = new float4(1, 1, 1, 1) });
            }
        }).ScheduleParallel();
    }

    //static void PickNewIntent(int entityInQueryIndex, Entity entity, ref EntityCommandBuffer.ParallelWriter ecb, Random random)
    //{
    //    if(random.NextInt(0, 2) == 0)
    //    {
    //        // only smash.  Until we have other systems working
    //        FarmerIntent intent = new FarmerIntent
    //        {
    //            value = FarmerIntentState.TillGround,
    //            elapsed = 0,
    //            random = random
    //        };

    //        if(intent.value == FarmerIntentState.SmashRocks) { return; } // Smash rocks is done now - remove this when all states are ready
    //        if(intent.)

    //        intent.value = (FarmerIntentState)intent.random.NextInt(2, 4);
    //        ecb.SetComponent<FarmerIntent>(entityInQueryIndex, entity, intent);
    //    }
    //}

    //static void ColorFarmerByIntent(int entityInQueryIndex, Entity entity, in FarmerIntent intent, ref EntityCommandBuffer.ParallelWriter ecb)
    //{
    //    float4 overrideColor = new float4();
    //    switch (intent.value)
    //    {
    //        case FarmerIntentState.SmashRocks:
    //            overrideColor = new float4(1, 0, 0, 1); // red
    //            break;
    //        case FarmerIntentState.TillGround:
    //            overrideColor = new float4(1, 1, 1, 1); // white
    //            break;
    //        case FarmerIntentState.SellPlants:
    //            overrideColor = new float4(1, 0, 1, 1); // magenta
    //            break;
    //    }

    //    ecb.AddComponent<ColorOverride>(entityInQueryIndex, entity, new ColorOverride { Value = overrideColor });
    //}
}
