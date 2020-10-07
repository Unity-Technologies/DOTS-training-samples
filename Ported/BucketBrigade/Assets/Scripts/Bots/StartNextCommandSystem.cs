using Unity.Entities;
using Unity.Mathematics;

public class StartNextCommandSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Random rand = new Random((uint)System.DateTime.UtcNow.Ticks);
        
        Entities
            .WithName("StartNextCommand")
            .ForEach((ref ExecutingCommand executingCommand, ref CurrentBotCommand currentCommand, ref Target target, in DynamicBuffer<CommandBufferElement> commandQueue) =>
            {
                if (executingCommand.Value)
                    return;

                executingCommand.Value = true;
                currentCommand.Index++;
                if (currentCommand.Index >= commandQueue.Length)
                    currentCommand.Index = 0;
                currentCommand.Command = commandQueue[currentCommand.Index].Value; 

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
            .ForEach((ref ExecutingCommand executingCommand, ref Pos pos,
                in Target target, in Speed speed, in CurrentBotCommand currentCommand) =>
            {
                if (executingCommand.Value == false)
                    return;

                switch (currentCommand.Command)
                {
                    case Command.Move:
                        float speedThisFrame = speed.Value * deltaTime;
                        float2 offset = target.Value - pos.Value;
                        if (math.lengthsq(offset) < speedThisFrame * speedThisFrame)
                        {
                            pos.Value = target.Value;
                            executingCommand.Value = false;
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