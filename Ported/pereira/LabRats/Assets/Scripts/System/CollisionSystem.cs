using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;


[UpdateBefore(typeof(DestroySystem))]
public class CollisionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;

    EntityQuery m_BoardQuery;
    EntityQuery m_CatQuery;

    NativeQueue<Entity> m_Queue;

    protected override void OnCreate()
    {
        m_Barrier = World.Active.GetOrCreateSystem<LbSimulationBarrier>();

        m_CatQuery = GetEntityQuery(typeof(LbCat), typeof(Translation));
        m_BoardQuery = GetEntityQuery(typeof(LbBoard), typeof(LbCatMap));

        m_Queue = new NativeQueue<Entity>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        m_Queue.Dispose();
    }

    [BurstCompile]
    struct CatMapJob : IJob
    {
        public int Size;
        public NativeArray<LbCatMap> Buffer;

        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<Translation> Translations;

        public void Execute()
        {
            var bitsInWord = sizeof(int) * 8;

            for (int i = 0; i < Translations.Length; ++i)
            {
                var translation = Translations[i].Value;

                var bufferBitIndex = ((int)translation.z) * Size + (int)translation.x;
                var bufferWordIndex = bufferBitIndex / bitsInWord;
                var bitOffset = bufferBitIndex % bitsInWord;

                var currentWord = Buffer[bufferWordIndex].Value;

                var bit = 1 << bitOffset;
                currentWord |= bit;

                Buffer[bufferWordIndex] = new LbCatMap() { Value = currentWord };
            }
        }
    }

    [BurstCompile]
    struct CollisionJob : IJobForEachWithEntity<LbRat, Translation>
    {
        public const int kBitsInWord = sizeof(int) * 8;

        public int Size;
        
        [ReadOnly] public NativeArray<LbCatMap> CatLocationBuffer;
        public NativeQueue<Entity>.ParallelWriter Queue;

        public void Execute(Entity entity, int jobIndex, [ReadOnly] ref LbRat rat, [ReadOnly] ref Translation translation)
        {
            var position = translation.Value;

            var bufferBitIndex = ((int)position.z) * Size + (int)position.x;
            var bufferWordIndex = bufferBitIndex / kBitsInWord;
            var bitOffset = bufferBitIndex % kBitsInWord;

            var currentWord = CatLocationBuffer[bufferWordIndex].Value;
            var bit = 1 << bitOffset;

            if ((currentWord & bit) == bit)
            {
                Queue.Enqueue(entity);
            }
        }
    }

    struct CollisionCleanJob : IJob
    {
        public NativeQueue<Entity> Queue;
        public EntityCommandBuffer CommandBuffer;

        public void Execute()
        {
            for (int i = 0; i < Queue.Count; i++)
            {
                CommandBuffer.AddComponent(Queue.Dequeue(), new LbDestroy());
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (m_BoardQuery.CalculateEntityCount() <= 0)
            return inputDeps;

        var boardEntity = m_BoardQuery.GetSingletonEntity();
        var board = m_BoardQuery.GetSingleton<LbBoard>();
        var bufferLookup = GetBufferFromEntity<LbCatMap>();

        var buffer = bufferLookup[boardEntity];
        var bufferArray = buffer.AsNativeArray();

        var handle = new MemsetNativeArray<LbCatMap>()
        {
            Source = bufferArray,
            Value = new LbCatMap()
        }.Schedule(bufferArray.Length, 32, inputDeps);

        handle = new CatMapJob
        {
            Size = board.SizeY,
            Buffer = bufferArray,
            Translations = m_CatQuery.ToComponentDataArray<Translation>(Allocator.TempJob)
        }.Schedule(handle);

        handle = new CollisionJob
        {
            Size = board.SizeY,
            CatLocationBuffer = bufferArray,
            Queue = m_Queue.AsParallelWriter(),
        }.Schedule(this, handle);

        handle = new CollisionCleanJob
        {
            Queue = m_Queue,
            CommandBuffer = m_Barrier.CreateCommandBuffer(),
        }.Schedule(handle);

        m_Barrier.AddJobHandleForProducer(handle);

        return handle;
    }
}