using Unity.Entities;

public class AssignBotRolesSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        /* Commenting this for now, Move commands are added from Chain creation system
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        
        Entities
            .WithName("AssignBotCommands")
            .WithNone<CurrentBotCommand>()
            .ForEach((Entity e, int entityInQueryIndex, in BotRole role) =>
            {
                ecb.AddComponent(entityInQueryIndex, e, new CurrentBotCommand { Index = -1});
                
                DynamicBuffer<CommandBufferElement> buffer = ecb.AddBuffer<CommandBufferElement>(entityInQueryIndex, e);
                DynamicBuffer<Command> commandBuffer = buffer.Reinterpret<Command>();
                
                switch (role.Value)
                {
                    case Role.None:
                        commandBuffer.Add(Command.Move);
                        break;
                    case Role.BriansSpecialTestRole:
                        commandBuffer.Add(Command.FindOrKeepBucket);
                        commandBuffer.Add(Command.FillBucket);
                        commandBuffer.Add(Command.Move);
                        commandBuffer.Add(Command.EmptyBucket);
                        break;
                }
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);*/
    }
}