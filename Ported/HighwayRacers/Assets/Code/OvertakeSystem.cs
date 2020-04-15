using Unity.Entities;

[UpdateAfter(typeof(BlockSystem))]
public class OvertakeSystem : SystemBase
{
    public const int LEFT_LANE = 3;
    public const int RIGHT_LANE = 0;
    protected override void OnUpdate()
    {
        EntityCommandBufferSystem endSim = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer.Concurrent ecb = endSim.CreateCommandBuffer().ToConcurrent();
        Entities
            .WithName("Update_Overtake")
            .WithAll<BlockSpeed>()
            .ForEach(
                (Entity ent, int nativeThreadIndex, ref LaneAssignment lane, in PercentComplete percent) =>
                {
                    // if move lane, add overtake tag; if can't move, don't add tag
                    // try moving left
                    if (lane.Value < LEFT_LANE)
                    {
                        lane = new LaneAssignment()
                        {
                            Value = lane.Value + 1
                        };
                        ecb.AddComponent<OvertakeTag>(nativeThreadIndex, ent);
                        ecb.RemoveComponent<BlockSpeed>(nativeThreadIndex, ent);
                        return;
                    }
                    // try moving right
                    if (lane.Value > RIGHT_LANE)
                    {
                        lane = new LaneAssignment()
                        {
                            Value = lane.Value - 1
                        };
                        ecb.AddComponent<OvertakeTag>(nativeThreadIndex, ent);
                        ecb.RemoveComponent<BlockSpeed>(nativeThreadIndex, ent);
                    }
                }
            )
            .ScheduleParallel();
        endSim.AddJobHandleForProducer(Dependency);
    }
}