using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;

namespace Burst.Compiler.IL.Tests.Helpers
{
    // Only used to allow to call the delegate with a NativeArrayRaw
    // As we can't use a generic with pinvoke
    internal unsafe struct NativeArrayRaw : IDisposable
    {
        // MUST BE IN SYNC WITH NativeArray
        internal void* m_Buffer;
        internal int m_Length;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal int                      m_MinIndex;
        internal int                      m_MaxIndex;
        internal AtomicSafetyHandle       m_Safety;
        internal DisposeSentinel          m_DisposeSentinel;
#endif
        Allocator m_AllocatorLabel;

        public NativeArrayRaw(void* mBuffer, int mLength)
        {
            m_Buffer = mBuffer;
            m_Length = mLength;
            m_AllocatorLabel = Allocator.Persistent;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_MinIndex = 0;
            m_MaxIndex = m_Length -1;
            m_Safety = AtomicSafetyHandle.Create();
            m_DisposeSentinel = null;
#endif
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif
            if (m_Buffer != (void*)0)
            {
                UnsafeUtility.Free((void*)m_Buffer, m_AllocatorLabel);
                m_Buffer = (void*)0;
                m_Length = 0;
            }
        }
    }
}
