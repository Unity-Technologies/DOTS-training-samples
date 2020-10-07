using Unity.Entities;
using Unity.Mathematics;

public class UpdateChainSystem : SystemBase
{
    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        // TODO, currently just simulating of moving start/end
        Entities
            .WithName("UpdateChain")
            .ForEach(
                (Entity entity, int entityInQueryIndex,
                    ref ChainStart start, ref ChainEnd end, in ChainID chainID) =>
                {
                    start.Value += new float2(0.01f, 0.01f);
                    end.Value += new float2(0.01f, 0.0f);
                })
            .ScheduleParallel();
    }
}