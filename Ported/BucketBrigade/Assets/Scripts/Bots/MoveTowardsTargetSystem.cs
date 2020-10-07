using Unity.Entities;
using Unity.Mathematics;

public class MoveTowardsTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities
            .WithName("MoveTowardsTarget")
            .ForEach((ref ExecutingCommand executingCommand, ref Pos pos, ref Target target,
                in Speed speed, in CurrentBotCommand currentCommand) =>
            {
                if (executingCommand.Value == false)
                    return;

                float speedThisFrame = speed.Value * deltaTime;
                float2 offset = target.Position - pos.Value;
                if (math.lengthsq(offset) < speedThisFrame * speedThisFrame)
                {
                    target.ReachedTarget = true;
                    pos.Value = target.Position;
                }
                else
                {
                    pos.Value += math.normalize(offset) * speedThisFrame;
                }
                
                switch (currentCommand.Command)
                {
                    case Command.Move:
                    case Command.EmptyBucket:
                    case Command.FindOrKeepBucket:
                        if (target.ReachedTarget)
                            executingCommand.Value = false;
                        break;
                    case Command.FillBucket:
                        // executingCommand set to true when the bucket is full
                        break;
                }
            }).ScheduleParallel();
    }
}