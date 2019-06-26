using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace Unity.Collections.LowLevel.Unsafe
{
    /// <summary>
    ///  A deterministic data streaming supporting parallel reading and parallel writing. Allows you to write different types or arrays into a single stream.
    /// </summary>
    [NativeContainer]
    public unsafe struct UnsafeBlockStream : IDisposable
    {
        [NativeDisableUnsafePtrRestriction] BlockStreamData* m_Block;
        private Allocator m_AllocatorLabel;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_Safety;
        [NativeSetClassTypeToNullOnSchedule]
        private DisposeSentinel m_DisposeSentinel;
#endif

        public UnsafeBlockStream(int foreachCount, Allocator allocator = Allocator.TempJob)
        {
            AllocateBlock(out this, allocator);
            AllocateForEach(foreachCount);
        }
        
        public static JobHandle ScheduleConstruct<T>(out UnsafeBlockStream unsafeBlockStream, NativeList<T> forEachCountFromList, JobHandle dependency, Allocator allocator = Allocator.TempJob)
            where T : struct
        {
            AllocateBlock(out unsafeBlockStream, allocator);
            var jobData = new ConstructJobList<T> { List = forEachCountFromList, UnsafeBlockStream = unsafeBlockStream };
            return jobData.Schedule(dependency);
        }
        
        public static JobHandle ScheduleConstruct(out UnsafeBlockStream unsafeBlockStream, NativeArray<int> lengthFromIndex0, JobHandle dependency, Allocator allocator = Allocator.TempJob)
        {
            AllocateBlock(out unsafeBlockStream, allocator);
            var jobData = new ConstructJob { Length = lengthFromIndex0, UnsafeBlockStream = unsafeBlockStream };
            return jobData.Schedule(dependency);
        }

        static void AllocateBlock(out UnsafeBlockStream stream, Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (allocator <= Allocator.None)
                throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", "allocator");
#endif
            
            int blockCount = JobsUtility.MaxJobThreadCount;

            int allocationSize = sizeof(BlockStreamData) + sizeof(Block*) * blockCount;
            byte* buffer = (byte*)UnsafeUtility.Malloc(allocationSize, 16, allocator);
            UnsafeUtility.MemClear(buffer, allocationSize);

            var block = (BlockStreamData*) buffer; 
            
            stream.m_Block = block;
            stream.m_AllocatorLabel = allocator;

            block->Allocator = allocator;
            block->BlockCount = blockCount;
            block->Blocks = (Block**)(buffer + sizeof(BlockStreamData));

            block->Ranges = null;
            block->RangeCount = 0;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out stream.m_Safety, out stream.m_DisposeSentinel, 0, allocator);
#endif
        }

        void AllocateForEach(int forEachCount)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (forEachCount <= 0)
                throw new ArgumentException("foreachCount must be > 0", "foreachCount");

            Assert.IsTrue(m_Block->Ranges == null);
            Assert.AreEqual(0, m_Block->RangeCount);
            Assert.AreNotEqual(0, m_Block->BlockCount);
#endif        
                        
            long allocationSize = sizeof(Range) * forEachCount;
            m_Block->Ranges = (Range*)UnsafeUtility.Malloc(allocationSize, 16, m_AllocatorLabel);
            m_Block->RangeCount = forEachCount;
            UnsafeUtility.MemClear(m_Block->Ranges, allocationSize);
        }



        public bool IsCreated => m_Block != null;

        public int ForEachCount
        {
            get
            {
                CheckReadAccess();
                return m_Block->RangeCount;
            }
        }

        public Reader AsReader()
        {
            return new Reader(ref this);
        }

        public Writer AsWriter()
        {
            return new Writer(ref this);
        }
        
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckReadAccess()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckWriteAccess()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
        }

        public int ComputeItemCount()
        {
            CheckReadAccess();
            
            int itemCount = 0;

            for (int i = 0; i != m_Block->RangeCount; i++)
            {
                itemCount += m_Block->Ranges[i].ElementCount;
            }

            return itemCount;
        }

        public NativeArray<T> ToNativeArray<T>(Allocator allocator = Allocator.Temp) where T : struct
        {
            CheckReadAccess();
            
            var array = new NativeArray<T>(ComputeItemCount(), allocator, NativeArrayOptions.UninitializedMemory);
            var reader = AsReader();

            int offset = 0;
            for (int i = 0; i != reader.ForEachCount; i++)
            {
                reader.BeginForEachIndex(i);
                int rangeItemCount = reader.RemainingItemCount;
                for (int j = 0; j < rangeItemCount; ++j)
                {
                    array[offset] = reader.Read<T>();
                    offset++;
                }
                reader.EndForEachIndex();
            }

            return array;
        }

        void Deallocate()
        {
            if (m_Block == null)
                return;

            for (int i = 0; i != m_Block->BlockCount; i++)
            {
                Block* block = m_Block->Blocks[i];
                while (block != null)
                {
                    Block* next = block->Next;
                    UnsafeUtility.Free(block, m_AllocatorLabel);
                    block = next;
                }
            }

            UnsafeUtility.Free(m_Block->Ranges, m_AllocatorLabel);
            UnsafeUtility.Free(m_Block, m_AllocatorLabel);
            m_Block = null;
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif
            Deallocate();
        }

        public JobHandle Dispose(JobHandle inputDeps)
        {
            // [DeallocateOnJobCompletion] is not supported, but we want the deallocation to happen in a thread.
            // DisposeSentinel needs to be cleared on main thread.
            // AtomicSafetyHandle can be destroyed after the job was scheduled (Job scheduling will check that no jobs are writing to the container)
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Clear(ref m_DisposeSentinel);
#endif
            var jobHandle = new DisposeJob { UnsafeBlockStream = this }.Schedule(inputDeps);
            m_Block = null;
            
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.Release(m_Safety);            
#endif

            return jobHandle;
        }

        struct Range
        {
            public Block* Block;
            public int OffsetInFirstBlock;
            public int ElementCount;

            /// One byte past the end of the last byte written
            public int LastOffset;
            public int NumberOfBlocks;
        }

        struct BlockStreamData
        {
            public const int AllocationSize = 4 * 1024;
            public Allocator Allocator;

            public Block** Blocks;
            public int BlockCount;
            
            public Range* Ranges;
            public int RangeCount;

            public Block* Allocate(Block* oldBlock, int threadIndex)
            {
                Assert.IsTrue(threadIndex < BlockCount && threadIndex >= 0);

                Block* block = (Block*)UnsafeUtility.Malloc(AllocationSize, 16, Allocator);
                block->Next = null;

                if (oldBlock == null)
                {
                    if (Blocks[threadIndex] == null)
                        Blocks[threadIndex] = block;
                    else
                    {
                        // Walk the linked list and append our new block to the end.
                        // Otherwise, we leak memory.
                        Block* head = Blocks[threadIndex];
                        while (head->Next != null)
                        {
                            head = head->Next;
                        }
                        head->Next = block;
                    }
                }
                else
                {
                    oldBlock->Next = block;
                }

                return block;
            }
        }

        struct Block
        {
            public Block* Next;

            public fixed byte Data[1];
        }

        [NativeContainer]
        [NativeContainerSupportsMinMaxWriteRestriction]
        public struct Writer
        {
            [NativeDisableUnsafePtrRestriction]
            BlockStreamData* m_BlockStream;
            [NativeDisableUnsafePtrRestriction]
            Block* m_CurrentBlock;
            [NativeDisableUnsafePtrRestriction]
            byte* m_CurrentPtr;
            [NativeDisableUnsafePtrRestriction]
            byte* m_CurrentBlockEnd;

            int m_ForeachIndex;
            int m_ElementCount;

            [NativeDisableUnsafePtrRestriction]
            Block* m_FirstBlock;
            int m_FirstOffset;
            int m_NumberOfBlocks;

#pragma warning disable CS0649
            [NativeSetThreadIndex]
            int m_ThreadIndex;
#pragma warning restore CS0649

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle m_Safety;
            #pragma warning disable 414
            int m_Length;
            #pragma warning restore 414
            int m_MinIndex;
            int m_MaxIndex;

            [NativeDisableUnsafePtrRestriction] 
            void* m_PassByRefCheck;
#endif

            internal Writer(ref UnsafeBlockStream stream)
            {
                m_BlockStream = stream.m_Block;
                m_ForeachIndex = int.MinValue;
                m_ElementCount = -1;
                m_CurrentBlock = null;
                m_CurrentBlockEnd = null;
                m_CurrentPtr = null;
                m_FirstBlock = null;
                m_NumberOfBlocks = 0;
                m_FirstOffset = 0;
                m_ThreadIndex = 0;
                
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                m_Safety = stream.m_Safety;
                m_Length = int.MaxValue;
                m_MinIndex = int.MinValue;
                m_MaxIndex = int.MinValue;
                m_PassByRefCheck = null;
#endif
            }


            public int ForEachCount
            {
                get
                {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
                    return m_BlockStream->RangeCount;
                }
            }

            public void PatchMinMaxRange(int foreEachIndex)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                m_MinIndex = foreEachIndex;
                m_MaxIndex = foreEachIndex;
#endif                
            }

            public void BeginForEachIndex(int foreachIndex)
            {
                //@TODO: Check that no one writes to the same for each index multiple times...
                BeginForEachIndexChecks(foreachIndex);

                m_ForeachIndex = foreachIndex;
                m_ElementCount = 0;
                m_NumberOfBlocks = 0;
                m_FirstBlock = m_CurrentBlock;
                m_FirstOffset = (int)(m_CurrentPtr - (byte*)m_CurrentBlock);
            }

            public void EndForEachIndex()
            {
                EndForEachIndexChecks();

                m_BlockStream->Ranges[m_ForeachIndex].ElementCount = m_ElementCount;
                m_BlockStream->Ranges[m_ForeachIndex].OffsetInFirstBlock = m_FirstOffset;
                m_BlockStream->Ranges[m_ForeachIndex].Block = m_FirstBlock;

                m_BlockStream->Ranges[m_ForeachIndex].LastOffset = (int)(m_CurrentPtr - (byte*)m_CurrentBlock);
                m_BlockStream->Ranges[m_ForeachIndex].NumberOfBlocks = m_NumberOfBlocks;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                m_ForeachIndex = int.MinValue;
#endif
            }

            public void Write<T>(T value) where T : struct
            {
                ref T dst = ref Allocate<T>();
                dst = value;
            }

            public ref T Allocate<T>() where T : struct
            {
                IsUnmanagedAndThrow<T>();
                int size = UnsafeUtility.SizeOf<T>();
                return ref UnsafeUtilityEx.AsRef<T>(Allocate(size));
            }

            public byte* Allocate(int size)
            {
                AllocateChecks(size);
                
                byte* ptr = m_CurrentPtr;
                m_CurrentPtr += size;

                if (m_CurrentPtr > m_CurrentBlockEnd)
                {
                    Block* oldBlock = m_CurrentBlock;

                    m_CurrentBlock = m_BlockStream->Allocate(oldBlock, m_ThreadIndex);
                    m_CurrentPtr = m_CurrentBlock->Data;

                    if (m_FirstBlock == null)
                    {
                        m_FirstOffset = (int)(m_CurrentPtr - (byte*)m_CurrentBlock);
                        m_FirstBlock = m_CurrentBlock;
                    }
                    else
                    {
                        m_NumberOfBlocks++;
                    }

                    m_CurrentBlockEnd = (byte*)m_CurrentBlock + BlockStreamData.AllocationSize;
                    ptr = m_CurrentPtr;
                    m_CurrentPtr += size;
                }

                m_ElementCount++;

                return ptr;
            }

            [BurstDiscard]
            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            internal static void IsUnmanagedAndThrow<T>() where T : struct
            {
                if (!UnsafeUtility.IsUnmanaged<T>())
                {
                    throw new ArgumentException($"{typeof(T)} used in BlockStream must be unmanaged (contain no managed types).");
                }
            }


            [BurstDiscard]
            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            void BeginForEachIndexChecks(int foreachIndex)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                
                AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);

                if (m_PassByRefCheck == null)
                    m_PassByRefCheck = UnsafeUtility.AddressOf(ref this);

                if (foreachIndex < m_MinIndex || foreachIndex > m_MaxIndex)
                {
                    // When the code is not running through the job system no ParallelForRange patching will occur
                    // We can't grab m_BlockStream->RangeCount on creation of the writer because the RangeCount can be initialized
                    // in a job after creation of the writer
                    if (m_MinIndex == int.MinValue && m_MaxIndex == int.MinValue)
                    {
                        m_MinIndex = 0;
                        m_MaxIndex = m_BlockStream->RangeCount - 1;
                    }

                    if (foreachIndex < m_MinIndex || foreachIndex > m_MaxIndex)
                        throw new ArgumentException($"Index {foreachIndex} is out of restricted IJobParallelFor range [{m_MinIndex}...{m_MaxIndex}] in BlockStream.");
                }
                
                
                if (m_ForeachIndex != int.MinValue)
                    throw new ArgumentException($"BeginForEachIndex must always be balanced by a EndForEachIndex call");

                if (0 != m_BlockStream->Ranges[foreachIndex].ElementCount)
                    throw new ArgumentException($"BeginForEachIndex can only be called once for the same index ({foreachIndex}).");
                

                Assert.IsTrue(foreachIndex >= 0 && foreachIndex < m_BlockStream->RangeCount);
#endif
            }

            [BurstDiscard]
            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            void EndForEachIndexChecks()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);

                if (m_ForeachIndex == int.MinValue)
                    throw new System.ArgumentException("EndForEachIndex must always be called balanced by a BeginForEachIndex or AppendForEachIndex call");
#endif
            }

            [BurstDiscard]
            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            void AllocateChecks(int size)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);

                if (m_PassByRefCheck != UnsafeUtility.AddressOf(ref this))
                    throw new ArgumentException("UnsafeBlockStream.Writer must be passed by ref once it is in use");
                
                if( m_ForeachIndex == int.MinValue)
                    throw new ArgumentException("Allocate must be called within BeginForEachIndex / EndForEachIndex");
                if(size > BlockStreamData.AllocationSize - sizeof(void*))
                    throw new ArgumentException("Allocation size is too large");
#endif
            }
        }

        [NativeContainer]
        [NativeContainerIsReadOnly]
        public struct Reader
        {
            [NativeDisableUnsafePtrRestriction]
            BlockStreamData* m_BlockStream;
            [NativeDisableUnsafePtrRestriction]
            Block* m_CurrentBlock;
            [NativeDisableUnsafePtrRestriction]
            byte* m_CurrentPtr;
            [NativeDisableUnsafePtrRestriction]
            byte* m_CurrentBlockEnd;

            int m_RemainingItemCount;
            int m_LastBlockSize;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            int m_RemainingBlocks;
            internal AtomicSafetyHandle m_Safety;
#endif

            internal Reader(ref UnsafeBlockStream stream)
            {
                m_BlockStream = stream.m_Block;
                m_CurrentBlock = null;
                m_CurrentPtr = null;
                m_CurrentBlockEnd = null;
                m_RemainingItemCount = 0;
                m_LastBlockSize = 0;
                
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                m_RemainingBlocks = 0;
                m_Safety = stream.m_Safety;
#endif
            }

            
            /// <summary>
            /// Begin reading data at the iteration index
            /// </summary>
            /// <param name="foreachIndex"></param>
            /// <returns> The number of elements at this index</returns>
            public int BeginForEachIndex(int foreachIndex)
            {
                BeginForEachIndexChecks(foreachIndex);
                
                m_RemainingItemCount = m_BlockStream->Ranges[foreachIndex].ElementCount;
                m_LastBlockSize = m_BlockStream->Ranges[foreachIndex].LastOffset;

                m_CurrentBlock = m_BlockStream->Ranges[foreachIndex].Block;
                m_CurrentPtr = (byte*)m_CurrentBlock + m_BlockStream->Ranges[foreachIndex].OffsetInFirstBlock;
                m_CurrentBlockEnd = (byte*)m_CurrentBlock + BlockStreamData.AllocationSize;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                m_RemainingBlocks = m_BlockStream->Ranges[foreachIndex].NumberOfBlocks;
                if (m_RemainingBlocks == 0)
                    m_CurrentBlockEnd = (byte*)m_CurrentBlock + m_LastBlockSize;
#endif
                
                return m_RemainingItemCount;
            }
            
            /// <summary>
            /// EndForEachIndex() ensures that all data has been read for the active ForEach index.
            /// </summary>
            /// <returns> The number of elements at this index</returns>
            [BurstDiscard]
            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            public void EndForEachIndex()
            {
                if (m_RemainingItemCount != 0)
                    throw new System.ArgumentException("Not all elements (Count) have been read. If this is intentional, simply skip calling EndForEachIndex();");
                if (m_CurrentBlockEnd != m_CurrentPtr)
                    throw new System.ArgumentException("Not all data (Data Size) has been read. If this is intentional, simply skip calling EndForEachIndex();");
            }

            public int ForEachCount
            {
                get
                {
                    CheckAccess();
                    return m_BlockStream->RangeCount;
                }
            }

            public int RemainingItemCount => m_RemainingItemCount;

            public byte* ReadUnsafePtr(int size)
            {
                ReadChecks(size);
                
                m_RemainingItemCount--;

                byte* ptr = m_CurrentPtr;
                m_CurrentPtr += size;

                if (m_CurrentPtr > m_CurrentBlockEnd)
                {
                    m_CurrentBlock = m_CurrentBlock->Next;
                    m_CurrentPtr = m_CurrentBlock->Data;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    m_RemainingBlocks--;
                    if (m_RemainingBlocks < 0)
                        throw new System.ArgumentException("Reading out of bounds");
                    if (m_RemainingBlocks == 0 && size + sizeof(void*) > m_LastBlockSize)
                        throw new System.ArgumentException("Reading out of bounds");
                    if (m_RemainingBlocks <= 0)
                        m_CurrentBlockEnd = (byte*)m_CurrentBlock + m_LastBlockSize;
                    else
                        m_CurrentBlockEnd = (byte*)m_CurrentBlock + BlockStreamData.AllocationSize;
#else
                    m_CurrentBlockEnd = (byte*)m_CurrentBlock + BlockStreamData.AllocationSize;
#endif

                    ptr = m_CurrentPtr;
                    m_CurrentPtr += size;
                }

                return ptr;
            }

            public ref T Read<T>() where T : struct
            {
                int size = UnsafeUtility.SizeOf<T>();
                return ref UnsafeUtilityEx.AsRef<T>(ReadUnsafePtr(size));
            }

            public ref T Peek<T>() where T : struct
            {
                int size = UnsafeUtility.SizeOf<T>();
                ReadChecks(size);
                
                byte* ptr = m_CurrentPtr;
                if (ptr + size > m_CurrentBlockEnd)
                    ptr = m_CurrentBlock->Next->Data;

                return ref UnsafeUtilityEx.AsRef<T>(ptr);
            }
            
            public int ComputeItemCount()
            {
                CheckAccess();                
                
                int itemCount = 0;
                for (int i = 0; i != m_BlockStream->RangeCount; i++)
                    itemCount += m_BlockStream->Ranges[i].ElementCount;

                return itemCount;
            }

            
            [BurstDiscard]
            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            void CheckAccess()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            }
            
            [BurstDiscard]
            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            void ReadChecks(int size)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
                
                Assert.IsTrue(size <= BlockStreamData.AllocationSize - (sizeof(void*)));
                if (m_RemainingItemCount < 1)
                    throw new ArgumentException("There are no more items left to be read.");
#endif
            }

            [BurstDiscard]
            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            void BeginForEachIndexChecks(int forEachIndex)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
                
                if ((uint)forEachIndex >= (uint)m_BlockStream->RangeCount)
                    throw new System.ArgumentOutOfRangeException(nameof(forEachIndex), $"foreachIndex: {forEachIndex} must be between 0 and ForEachCount: {m_BlockStream->RangeCount}");
#endif
            }

        }

        [BurstCompile]
        struct DisposeJob : IJob
        {
            public UnsafeBlockStream UnsafeBlockStream;

            public void Execute()
            {
                UnsafeBlockStream.Deallocate();
            }
        }
        
        [BurstCompile]
        struct ConstructJobList<T> : IJob
            where T : struct
        {
            public UnsafeBlockStream UnsafeBlockStream;
            [ReadOnly]
            public NativeList<T> List;

            public void Execute()
            {
                UnsafeBlockStream.AllocateForEach(List.Length);
            }
        }
        
        [BurstCompile]
        struct ConstructJob : IJob
        {
            public UnsafeBlockStream UnsafeBlockStream;
            [ReadOnly]
            public NativeArray<int> Length;

            public void Execute()
            {
                UnsafeBlockStream.AllocateForEach(Length[0]);
            }
        }
    }
}
