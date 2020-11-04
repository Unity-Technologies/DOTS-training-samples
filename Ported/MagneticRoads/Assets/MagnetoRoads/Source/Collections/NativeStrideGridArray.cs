using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Magneto.Collections
{
    public struct NativeStrideGridArray<T> : IDisposable where T : struct
    {
        public NativeArray<T> array;
        public int stride;
        public int length;
        
        public NativeStrideGridArray ( int stride, Allocator allocator, NativeArrayOptions clearMemory )
        {
            this.stride = stride;
            this.length = stride * stride * stride;

            array = new NativeArray<T>(this.length, allocator, clearMemory);
        }

        public T this[int x, int y, int z]
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => array[(z * stride * stride) + (y * stride) + x];


            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            set => array[(z * stride * stride) + (y * stride) + x] = value;
        }
       

        public T this[int3 index]
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => array[(index.z * stride * stride) + (index.y * stride) + index.x];

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            set => array[(index.z * stride * stride) + (index.y * stride) + index.x] = value;
        }
        
        public void Dispose()
        {
            if (array.IsCreated)
            {
                array.Dispose();


                array = default;
            }
        }
    }














}