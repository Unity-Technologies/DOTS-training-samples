using Unity.Entities;
using Unity.Mathematics;

public class StartNextCommandSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Random rand = new Random((uint)System.DateTime.UtcNow.Ticks);
        
        Entities
            .WithName("StartNextCommand")
            .ForEach((ref CurrentBotCommand currentCommand, ref Target target, ref DynamicBuffer<CommandBufferElement> commandQueue) =>
            {
                if (currentCommand.Command != Command.None)
                    return;
                if (commandQueue.Length == 0)
                    return;

                currentCommand.Command = commandQueue[0].Value;
                commandQueue.RemoveAt(0);

                switch (currentCommand.Command)
                {
                    case Command.Move:
                        target.Value = GetRandomPosition(ref rand);
                        break;
                }
            }).ScheduleParallel();
    }

    // This is just a placeholder, will eventually become GetNearestFire, GetNearestBucket, GetChainPosition etc.
    static float2 GetRandomPosition(ref Random rand)
    {
        float radius = 10;
        float2 pos = rand.NextFloat2() * radius - new float2(radius * 0.5f);
        return pos;
    }
}

public class MoveTowardsTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities
            .WithName("MoveTowardsTarget")
            .ForEach((ref CurrentBotCommand currentCommand, ref Pos pos,
                in Target target, in Speed speed) =>
            {
                switch (currentCommand.Command)
                {
                    case Command.Move:
                        float speedThisFrame = speed.Value * deltaTime;
                        float2 offset = target.Value - pos.Value;
                        if (math.lengthsq(offset) < speedThisFrame * speedThisFrame)
                        {
                            pos.Value = target.Value;
                            currentCommand.Command = Command.None;
                        }
                        else
                        {
                            pos.Value += math.normalize(offset) * speedThisFrame;
                        }
                        break;
                }
            }).ScheduleParallel();
    }
}