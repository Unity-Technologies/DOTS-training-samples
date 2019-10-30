#if !UNITY_ZEROPLAYER && !UNITY_CSHARP_TINY && UNITY_2019_3_OR_NEWER
using System;
using System.Collections.Generic;
#if BURST_UNITY_MOCK
using System.Runtime.CompilerServices;
#endif
using Unity.Burst.LowLevel;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Burst
{
    /// <summary>
    /// A structure that allows to share mutable static data between C# and HPC#.
    /// </summary>
    /// <typeparam name="T">Type of the data to share (must not contain any reference types)</typeparam>
    public readonly unsafe struct SharedStatic<T> where T : struct
    {
        private readonly void* _buffer;

        private SharedStatic(void* buffer)
        {
            _buffer = buffer;
        }

        /// <summary>
        /// Get a writable reference to the shared data.
        /// </summary>
        public ref T Data
        {
            get
            {
                return ref Unsafe.AsRef<T>(_buffer);
            }
        }

        /// <summary>
        /// Get a direct unsafe pointer to the shared data.
        /// </summary>
        public void* UnsafeDataPointer
        {
            get { return _buffer; }
        }

        /// <summary>
        /// Creates a shared static data for the specified context (usable from both C# and HPC#)
        /// </summary>
        /// <typeparam name="TContext">A type class that uniquely identifies the this shared data.</typeparam>
        /// <param name="alignment">Optional alignment</param>
        /// <returns>A shared static for the specified context</returns>
        public static SharedStatic<T> GetOrCreate<TContext>(uint alignment = 0)
        {
            return new SharedStatic<T>(SharedStatic.GetOrCreateSharedStaticInternal(
                BurstRuntime.GetHashCode64<TContext>(), 0, (uint)UnsafeUtility.SizeOf<T>(), alignment == 0 ? 4 : alignment));
        }

        /// <summary>
        /// Creates a shared static data for the specified context and sub-context (usable from both C# and HPC#)
        /// </summary>
        /// <typeparam name="TContext">A type class that uniquely identifies the this shared data.</typeparam>
        /// <typeparam name="TSubContext">A type class that uniquely identifies this shared data within a sub-context of the primary context</typeparam>
        /// <param name="alignment">Optional alignment</param>
        /// <returns>A shared static for the specified context</returns>
        public static SharedStatic<T> GetOrCreate<TContext, TSubContext>(uint alignment = 0)
        {
            return new SharedStatic<T>(SharedStatic.GetOrCreateSharedStaticInternal(
                BurstRuntime.GetHashCode64<TContext>(), BurstRuntime.GetHashCode64<TSubContext>(),
                (uint)UnsafeUtility.SizeOf<T>(), alignment == 0 ? 4 : alignment));
        }

        /// <summary>
        /// Creates a shared static data for the specified context (reflection based, only usable from C#, but not from HPC#)
        /// </summary>
        /// <param name="contextType">A type class that uniquely identifies the this shared data</param>
        /// <param name="alignment">Optional alignment</param>
        /// <returns>A shared static for the specified context</returns>
        public static SharedStatic<T> GetOrCreate(Type contextType, uint alignment = 0)
        {
            return new SharedStatic<T>(SharedStatic.GetOrCreateSharedStaticInternal(
                BurstRuntime.GetHashCode64(contextType), 0, (uint)UnsafeUtility.SizeOf<T>(), alignment == 0 ? 4 : alignment));
        }

        /// <summary>
        /// Creates a shared static data for the specified context and sub-context (usable from both C# and HPC#)
        /// </summary>
        /// <param name="contextType">A type class that uniquely identifies the this shared data</param>
        /// <param name="subContextType">A type class that uniquely identifies this shared data within a sub-context of the primary context</param>
        /// <param name="alignment">Optional alignment</param>
        /// <returns>A shared static for the specified context</returns>
        public static SharedStatic<T> GetOrCreate(Type contextType, Type subContextType, uint alignment = 0)
        {
            return new SharedStatic<T>(SharedStatic.GetOrCreateSharedStaticInternal(
                BurstRuntime.GetHashCode64(contextType), BurstRuntime.GetHashCode64(subContextType),
                (uint)UnsafeUtility.SizeOf<T>(), alignment == 0 ? (uint)4 : alignment));
        }
    }

    internal static class SharedStatic
    {
        private static readonly Dictionary<long, Type> HashToType = new Dictionary<long, Type>();

        public static unsafe void* GetOrCreateSharedStaticInternal(Type typeContext, Type subTypeContext, uint sizeOf,
            uint alignment)
        {
            return GetOrCreateSharedStaticInternal(GetSafeHashCode64(typeContext), GetSafeHashCode64(subTypeContext),
                sizeOf, alignment);

        }
            
        public static unsafe void* GetOrCreateSharedStaticInternal(long getHashCode64, long getSubHashCode64, uint sizeOf, uint alignment)
        {
            if (sizeOf == 0) throw new ArgumentException("sizeOf must be > 0", nameof(sizeOf));
            var hash128 = new Hash128((ulong) getHashCode64, (ulong) getSubHashCode64);
            var result = BurstCompilerService.GetOrCreateSharedMemory(ref hash128, sizeOf, alignment);
            if (result == null)
            {
                throw new InvalidOperationException("Unable to create a SharedStatic for this key. It is likely that the same key was used to allocate a shared memory with a smaller size while the new size requested is bigger");
            }
            return result;
        }

        private static long GetSafeHashCode64(Type type)
        {
            var hash = BurstRuntime.GetHashCode64(type);
            lock (HashToType)
            {
                Type existingType;
                if (HashToType.TryGetValue(hash, out existingType))
                {
                    if (existingType != type)
                    {
                        var message = $"The type `{type}` has a hash conflict with `{existingType}`";
#if !BURST_UNITY_MOCK
                        Debug.LogError(message);
#endif
                        throw new InvalidOperationException(message);
                    }
                }
                else
                {
                    HashToType.Add(hash, type);
                }
            }
            return hash;
        }
    }
}
#endif