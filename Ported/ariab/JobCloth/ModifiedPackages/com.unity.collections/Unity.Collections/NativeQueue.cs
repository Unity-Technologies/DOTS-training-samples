using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using System.Threading;

namespace Unity.Collections
{
    unsafe struct NativeQueueBlockHeader
    {
        public void* nextBlock;
        public int itemsInBlock;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct NativeQueueBlockPoolData
    {
        internal IntPtr firstBlock;
        internal int allocatedBlocks;
        internal int MaxBlocks;
        internal const int BlockSize = 16*1024;
        internal int allocLock;

        public byte* AllocateBlock()
        {
            // There can only ever be a single thread allocating an entry from the free list since it needs to
            // access the content of the block (the next pointer) before doing the CAS.
            // If there was no lock thread A could read the next pointer, thread B could quickly allocate
            // the same block then free it with another next pointer before thread A performs the CAS which
            // leads to an invalid free list potentially causing memory corruption.
            // Having multiple threads freeing data concurrently to each other while another thread is allocating
            // is no problems since there is only ever a single thread modifying global data in that case.
            while (Interlocked.CompareExchange(ref allocLock, 1, 0) != 0)
            {
            }

            byte* checkBlock = (byte*)firstBlock;
            byte* block;
            do
            {
                block = checkBlock;
                if (block == null)
                {
                    Interlocked.Exchange(ref allocLock, 0);
                    Interlocked.Increment(ref allocatedBlocks);
                    block = (byte*)UnsafeUtility.Malloc(BlockSize, 16, Allocator.Persistent);
                    return block;
                }

                checkBlock = (byte*)Interlocked.CompareExchange(ref firstBlock,
                    (IntPtr) ((NativeQueueBlockHeader*) block)->nextBlock, (IntPtr) block);
            } while (checkBlock != block);
            Interlocked.Exchange(ref allocLock, 0);
            return block;
        }
        public void FreeBlock(byte* block)
        {
            if (allocatedBlocks > MaxBlocks)
            {
                if (Interlocked.Decrement(ref allocatedBlocks) + 1 > MaxBlocks)
                {
                    UnsafeUtility.Free(block, Allocator.Persistent);
                    return;
                }
                Interlocked.Increment(ref allocatedBlocks);
            }

            byte* checkBlock = (byte*) firstBlock;
            byte* nextPtr;
            do
            {
                nextPtr = checkBlock;
                ((NativeQueueBlockHeader*)block)->nextBlock = checkBlock;
                checkBlock = (byte*)Interlocked.CompareExchange(ref firstBlock, (IntPtr) block,
                    (IntPtr) checkBlock);
            } while (checkBlock != nextPtr);
        }
    }
    internal unsafe static class NativeQueueBlockPool
    {
        static NativeQueueBlockPoolData data;
        public static NativeQueueBlockPoolData* QueueBlockPool
        {
            get
            {
                if (data.allocatedBlocks == 0)
                {
                    data.allocatedBlocks = data.MaxBlocks = 256;
                    data.allocLock = 0;
                    // Allocate MaxBlocks items
                    byte* prev = null;
                    for (int i = 0; i < data.MaxBlocks; ++i)
                    {
                        NativeQueueBlockHeader* block = (NativeQueueBlockHeader*)UnsafeUtility.Malloc(NativeQueueBlockPoolData.BlockSize, 16, Allocator.Persistent);
                        block->nextBlock = prev;
                        prev = (byte*)block;
                    }
                    data.firstBlock = (IntPtr)prev;

#if !NET_DOTS
                    AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
#endif
                }
                return (NativeQueueBlockPoolData*)UnsafeUtility.AddressOf<NativeQueueBlockPoolData>(ref data);
            }
        }
#if !NET_DOTS
        static void OnDomainUnload(object sender, EventArgs e)
        {
            while (data.firstBlock != IntPtr.Zero)
            {
                NativeQueueBlockHeader* block = (NativeQueueBlockHeader*)data.firstBlock;
                data.firstBlock = (IntPtr)(block->nextBlock);
                UnsafeUtility.Free(block, Allocator.Persistent);
                --data.allocatedBlocks;
            }
        }
#endif

    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct NativeQueueData
    {
        public void*  m_FirstBlock;
        public IntPtr m_LastBlock;
        public int    m_ItemsPerBlock;
        public int    m_CurrentReadIndexInBlock;
        public byte*  m_CurrentWriteBlockTLS;

        internal void* GetCurrentWriteBlockTLS(int threadIndex)
        {
            var data = (void**)&m_CurrentWriteBlockTLS[threadIndex * JobsUtility.CacheLineSize];
            return *data;
        }

        internal void SetCurrentWriteBlockTLS(int threadIndex, void* currentWriteBlock)
        {
            var data = (void**)&m_CurrentWriteBlockTLS[threadIndex * JobsUtility.CacheLineSize];
            *data = currentWriteBlock;
        }

        public static byte* AllocateWriteBlockMT<T>(NativeQueueData* data, NativeQueueBlockPoolData* pool, int threadIndex) where T : struct
        {
            int tlsIdx = threadIndex;

            NativeQueueBlockHeader* currentWriteBlock = (NativeQueueBlockHeader*)data->GetCurrentWriteBlockTLS(tlsIdx);
            if (currentWriteBlock != null
            &&  currentWriteBlock->itemsInBlock == data->m_ItemsPerBlock)
            {
                currentWriteBlock = null;
            }

            if (currentWriteBlock == null)
            {
                currentWriteBlock = (NativeQueueBlockHeader*)pool->AllocateBlock();
                currentWriteBlock->nextBlock = null;
                currentWriteBlock->itemsInBlock = 0;
                NativeQueueBlockHeader* prevLast = (NativeQueueBlockHeader*)Interlocked.Exchange(ref data->m_LastBlock, (IntPtr)currentWriteBlock);

                if (prevLast == null)
                {
                    data->m_FirstBlock = currentWriteBlock;
                }
                else
                {
                    prevLast->nextBlock = currentWriteBlock;
                }

                data->SetCurrentWriteBlockTLS(tlsIdx, currentWriteBlock);
            }

            return (byte*)currentWriteBlock;
        }

        internal static int Align(int size, int alignmentPowerOfTwo)
        {
            return (size + alignmentPowerOfTwo - 1) & ~(alignmentPowerOfTwo - 1);
        }

        public unsafe static void AllocateQueue<T>(Allocator label, out NativeQueueData* outBuf) where T : struct
        {
            var queueDataSize = Align(UnsafeUtility.SizeOf<NativeQueueData>(), JobsUtility.CacheLineSize);

            var data = (NativeQueueData*)UnsafeUtility.Malloc(
                  queueDataSize
                + JobsUtility.CacheLineSize * JobsUtility.MaxJobThreadCount
                , JobsUtility.CacheLineSize
                , label
                );

            data->m_CurrentWriteBlockTLS = (((byte*)data) + queueDataSize);

            data->m_FirstBlock = null;
            data->m_LastBlock = IntPtr.Zero;
            data->m_ItemsPerBlock = (NativeQueueBlockPoolData.BlockSize - UnsafeUtility.SizeOf<NativeQueueBlockHeader>()) / UnsafeUtility.SizeOf<T>();

            data->m_CurrentReadIndexInBlock = 0;
            for (int tls = 0; tls < JobsUtility.MaxJobThreadCount; ++tls)
            {
                data->SetCurrentWriteBlockTLS(tls, null);
            }

            outBuf = data;
        }

        public unsafe static void DeallocateQueue(NativeQueueData* data, NativeQueueBlockPoolData* pool, Allocator allocation)
        {
            NativeQueueBlockHeader* firstBlock = (NativeQueueBlockHeader*)data->m_FirstBlock;
            while (firstBlock != null)
            {
                NativeQueueBlockHeader* next = (NativeQueueBlockHeader*)firstBlock->nextBlock;
                pool->FreeBlock((byte*)firstBlock);
                firstBlock = next;
            }
            UnsafeUtility.Free(data, allocation);
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    unsafe public struct NativeQueue<T> where T : struct
    {
	    [NativeDisableUnsafePtrRestriction]
        NativeQueueData* m_Buffer;

        [NativeDisableUnsafePtrRestriction]
        NativeQueueBlockPoolData* m_QueuePool;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle m_Safety;
	    [NativeSetClassTypeToNullOnSchedule]
        DisposeSentinel m_DisposeSentinel;
#endif

        Allocator m_AllocatorLabel;

        public unsafe NativeQueue(Allocator label)
        {
            CollectionHelper.CheckIsUnmanaged<T>();

            m_QueuePool = NativeQueueBlockPool.QueueBlockPool;
            m_AllocatorLabel = label;

			NativeQueueData.AllocateQueue<T>(label, out m_Buffer);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
			DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 0, label);
#endif
		}

		unsafe public int Count
		{
			get
			{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
				AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                int count = 0;
                for (NativeQueueBlockHeader* block = (NativeQueueBlockHeader*)m_Buffer->m_FirstBlock; block != null; block = (NativeQueueBlockHeader*)block->nextBlock)
                    count += block->itemsInBlock;
                return count - m_Buffer->m_CurrentReadIndexInBlock;
			}
		}

        static public int PersistentMemoryBlockCount
        {
            get {return NativeQueueBlockPool.QueueBlockPool->MaxBlocks;}
            set {Interlocked.Exchange(ref NativeQueueBlockPool.QueueBlockPool->MaxBlocks, value);}
        }
        static public int MemoryBlockSize
        {
            get {return NativeQueueBlockPoolData.BlockSize;}
        }

		unsafe public T Peek()
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif

            byte* firstBlock = (byte*)m_Buffer->m_FirstBlock;
            if (firstBlock == null)
                throw new InvalidOperationException("Trying to peek from an empty queue");
			return UnsafeUtility.ReadArrayElement<T>(firstBlock + UnsafeUtility.SizeOf<NativeQueueBlockHeader>(), m_Buffer->m_CurrentReadIndexInBlock);
		}
		unsafe public void Enqueue(T entry)
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif

			byte* writeBlock = NativeQueueData.AllocateWriteBlockMT<T>(m_Buffer, m_QueuePool, 0);
			UnsafeUtility.WriteArrayElement(writeBlock + UnsafeUtility.SizeOf<NativeQueueBlockHeader>(), ((NativeQueueBlockHeader*)writeBlock)->itemsInBlock, entry);
			++((NativeQueueBlockHeader*)writeBlock)->itemsInBlock;
		}

		unsafe public T Dequeue()
		{
			T item;
			if (!TryDequeue(out item))
				throw new InvalidOperationException("Trying to dequeue from an empty queue");
			return item;
		}
		unsafe public bool TryDequeue(out T item)
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif

            byte* firstBlock = (byte*)m_Buffer->m_FirstBlock;
            if (firstBlock == null)
            {
                item = default(T);
                return false;
            }
			item = UnsafeUtility.ReadArrayElement<T>(firstBlock + UnsafeUtility.SizeOf<NativeQueueBlockHeader>(), m_Buffer->m_CurrentReadIndexInBlock);
            ++m_Buffer->m_CurrentReadIndexInBlock;
            if (m_Buffer->m_CurrentReadIndexInBlock >= ((NativeQueueBlockHeader*)firstBlock)->itemsInBlock)
            {
                m_Buffer->m_CurrentReadIndexInBlock = 0;
                m_Buffer->m_FirstBlock = ((NativeQueueBlockHeader*)firstBlock)->nextBlock;
                if (m_Buffer->m_FirstBlock == null)
                {
                    m_Buffer->m_LastBlock = IntPtr.Zero;
                }

                for (int tls = 0; tls < JobsUtility.MaxJobThreadCount; ++tls)
                {
                    if (m_Buffer->GetCurrentWriteBlockTLS(tls) == firstBlock)
                    {
                        m_Buffer->SetCurrentWriteBlockTLS(tls, null);
                    }
                }
                m_QueuePool->FreeBlock(firstBlock);
            }
            return true;
		}

		unsafe public void Clear()
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            NativeQueueBlockHeader* firstBlock = (NativeQueueBlockHeader*)m_Buffer->m_FirstBlock;
            while (firstBlock != null)
            {
                NativeQueueBlockHeader* next = (NativeQueueBlockHeader*)firstBlock->nextBlock;
                m_QueuePool->FreeBlock((byte*)firstBlock);
                firstBlock = next;
            }
            m_Buffer->m_FirstBlock = null;
            m_Buffer->m_LastBlock = IntPtr.Zero;
            m_Buffer->m_CurrentReadIndexInBlock = 0;
            for (int tls = 0; tls < JobsUtility.MaxJobThreadCount; ++tls)
            {
                m_Buffer->SetCurrentWriteBlockTLS(tls, null);
            }
		}

		public bool IsCreated
		{
			get { return m_Buffer != null; }
		}

		public void Dispose()
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif

			NativeQueueData.DeallocateQueue(m_Buffer, m_QueuePool, m_AllocatorLabel);
			m_Buffer = null;
		}

        public Concurrent ToConcurrent()
        {
            NativeQueue<T>.Concurrent concurrent;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
			concurrent.m_Safety = m_Safety;
			AtomicSafetyHandle.UseSecondaryVersion(ref concurrent.m_Safety);
#endif

            concurrent.m_Buffer = m_Buffer;
            concurrent.m_QueuePool = m_QueuePool;
            concurrent.m_ThreadIndex = 0;
            return concurrent;
        }

		[NativeContainer]
		[NativeContainerIsAtomicWriteOnly]
		unsafe public struct Concurrent
		{
			[NativeDisableUnsafePtrRestriction]
			internal NativeQueueData* 	m_Buffer;
			[NativeDisableUnsafePtrRestriction]
			internal NativeQueueBlockPoolData* 	m_QueuePool;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
			internal AtomicSafetyHandle m_Safety;
#endif

		    [NativeSetThreadIndex]
			internal int m_ThreadIndex;

			unsafe public void Enqueue(T entry)
			{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
				AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
                byte* writeBlock = NativeQueueData.AllocateWriteBlockMT<T>(m_Buffer, m_QueuePool, m_ThreadIndex);
                UnsafeUtility.WriteArrayElement(writeBlock + UnsafeUtility.SizeOf<NativeQueueBlockHeader>(), ((NativeQueueBlockHeader*)writeBlock)->itemsInBlock, entry);
                ++((NativeQueueBlockHeader*)writeBlock)->itemsInBlock;
			}
		}
	}
}
