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
            this.length = stride * 3;

            array = new NativeArray<T>(length, allocator, clearMemory);
        }

        public T this[int x, int y, int z]
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return array[x+stride * (y + stride * z)]; }
            
            
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            set { array[x+stride * (y + stride * z)] = value; }
        }

        public T this[int3 index]
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get { return array[index.x+stride * (index.y + stride * index.z)]; }
            
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            set { array[index.x+stride * (index.y + stride * index.z)] = value; }
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