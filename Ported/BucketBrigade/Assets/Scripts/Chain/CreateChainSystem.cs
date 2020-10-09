using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class CreateChainSystem : SystemBase
{
    protected override void OnCreate()
    {
        var chainSystemQueue = EntityManager.CreateEntity();
        EntityManager.AddBuffer<CreateChainBufferElement>(chainSystemQueue);
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
                        in ChainStart chainStart, in ChainEnd chainEnd, in ChainLength chainLength, in ChainID chainID) =>
                    {
                        ecb.RemoveComponent<ChainCreateTag>(entity);
                        // full chain length = 2 * length; "0" bot - fills bucket with water, "length" bot - throws water to fire
                        for (int i = 0; i < chainLength.Value * 2; ++i)
                        {
                            chainQueueBuffer.Add(new CreateChainBufferElement()
                            {
                                chainID = chainID.Value,
                                position = i,
                                start = chainStart.Value,
                                end = chainEnd.Value,
                                length = chainLength.Value,
                            });
                        }
                    })
                .Run();

            var bufferLength = chainQueueBuffer.Length;
            if (bufferLength > 0)
            {
                var bufferPos = 0;
                Entities
                    .WithName("AssignBotsToChain")
                    .WithoutBurst()
                    .WithAll<BotTag>()
                    .WithNone<SharedChainComponent>()
                    .ForEach(
                        (Entity entity, int entityInQueryIndex) =>
                        {
                            if (bufferPos < bufferLength)
                            {
                                ecb.AddComponent(entity,
                                    new ChainPosition()
                                    {
                                        Value = chainQueueBuffer[bufferPos].position
                                    });

                                ecb.AddComponent(entity,
                                    new ChainObjectType()
                                    {
                                        Value = ObjectType.Bot
                                    });

                                ecb.AddSharedComponent(entity,
                                    new SharedChainComponent()
                                    {
                                        chainID = chainQueueBuffer[bufferPos].chainID,
                                        start = chainQueueBuffer[bufferPos].start,
                                        end = chainQueueBuffer[bufferPos].end,
                                        length = chainQueueBuffer[bufferPos].length
                                    });
                                ++bufferPos;
                            }
                        })
                    .Run();

                if (bufferPos > 0)
                {
                    chainQueueBuffer.RemoveRange(0, bufferPos);
                }

                // TODO need update buffer in entity?
                ecb.Playback(EntityManager);
            }
        }
    }
}