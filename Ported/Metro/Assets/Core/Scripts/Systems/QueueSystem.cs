using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct QueueSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Move commuters forward in queue
        foreach (var queues in SystemAPI.Query<DynamicBuffer<PlatformQueue>>().WithAll<PlatformTag>())
        {
            foreach(var item in queues)
            {
                var queue = item.queue;
                var length = queue.Length;
                int newLength = 0;
                for(int i = 0; i < length; i++)
                {
                    var entity = queue[i].commuter;
                    if(SystemAPI.HasComponent<QueueingTag>(entity))
                        newLength++;
                }

                if(length == newLength)
                    continue;

                queue.RemoveRange(0, length - newLength);
            }
        }
    }
}
