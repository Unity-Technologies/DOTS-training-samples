using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

//[UpdateBefore(typeof(BallBrickCollisionSystem))]
public class CreateChainSystem : SystemBase
{
    protected override void OnCreate()
    {
        var chainSystemQueue = EntityManager.CreateEntity();
        EntityManager.AddBuffer<CreateChainBufferElement>(chainSystemQueue);

        // TODO move this code to where we actually decide to create chain
        var chain = EntityManager.CreateEntity();
        var start = new ChainStart() { Value = new float2(0.0f, 0.0f) };
        var end = new ChainEnd() { Value = new float2(8.0f, 8.0f) };
        var length = new ChainLength() { Value = 10 };
        var chainID = new ChainID() { Value = 5 }; // TODO
        EntityManager.AddComponent<ChainCreateTag>(chain);
        EntityManager.AddComponentData(chain, start);
        EntityManager.AddComponentData(chain, end);
        EntityManager.AddComponentData(chain, length);
        EntityManager.AddComponentData(chain, chainID);
    }

    protected override void OnUpdate()
    {
        using (var ecb = new EntityCommandBuffer(Allocator.TempJob))
        {
            var chainSystemQueue = GetSingletonEntity<CreateChainBufferElement>();
            var chainQueueBuffer = EntityManager.GetBuffer<CreateChainBufferElement>(chainSystemQueue);
            Entities
                .WithName("CreateChain")
                .WithAll<ChainCreateTag>()
                .ForEach(
                    (Entity entity, int entityInQueryIndex,
                        in ChainStart start, in ChainEnd end, in ChainLength length, in ChainID chainID) =>
                    {
                        ecb.RemoveComponent<ChainCreateTag>(entity);
                        for (int i = 0; i < length.Value; ++i)
                        {
                            chainQueueBuffer.Add(new CreateChainBufferElement()
                                {chainID = chainID.Value, position = i});
                        }
                    })
                .Run();

            var bufferLength = chainQueueBuffer.Length;
            if (bufferLength > 0)
            {
                var bufferPos = 0;
                Entities
                    .WithName("AssignBotsToChain")
                    .WithAll<BotTag>()
                    .WithNone<SharedChainComponent>()
                    .ForEach(
                        (Entity entity, int entityInQueryIndex, ref DynamicBuffer<CommandBufferElement> commandQueue) =>
                        {
                            if (bufferPos < bufferLength)
                            {
                                ecb.AddComponent(entity, new CurrentBotCommand {Command = Command.None});
                                DynamicBuffer<Command> commandBuffer = commandQueue.Reinterpret<Command>();
                                commandBuffer.Add(Command.Move);
                                ecb.AddComponent(entity,
                                    new ChainPosition() {Value = chainQueueBuffer[bufferPos].position});
                                ecb.AddSharedComponent(entity,
                                    new SharedChainComponent() {chainID = chainQueueBuffer[bufferPos].chainID});
                                ++bufferPos;
                            }
                        })
                    .Run();

                if (bufferPos > 0)
                {
                    chainQueueBuffer.RemoveRange(0, bufferPos);
                }
            }

            // TODO need update buffer in entity?
            ecb.Playback(EntityManager);
        }
    }
}