#if !UNITY_ZEROPLAYER && !UNITY_CSHARP_TINY
using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Burst
{
    /// <summary>
    /// Base interface for a function pointer.
    /// </summary>
    public interface IFunctionPointer
    {
        /// <summary>
        /// Converts a pointer to a function pointer.
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <returns>An instance of this interface.</returns>
        IFunctionPointer FromIntPtr(IntPtr ptr);
    }

    /// <summary>
    /// A function pointer that can be used from a burst jobs. It needs to be compiled through <see cref="BurstCompiler.CompileFunctionPointer{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of the delegate of this function pointer</typeparam>
    public struct FunctionPointer<T> : IFunctionPointer
    {
        [NativeDisableUnsafePtrRestriction]
        private readonly IntPtr _ptr;

        /// <summary>
        /// Creates a new instance of this function pointer with the following native pointer.
        /// </summary>
        /// <param name="ptr"></param>
        public FunctionPointer(IntPtr ptr)
        {
            _ptr = ptr;
        }

        /// <summary>
        /// Invoke this function pointer. Must be called from a Burst Jobs.
        /// </summary>
        public T Invoke => (T)(object)Marshal.GetDelegateForFunctionPointer(_ptr, typeof(T));

        IFunctionPointer IFunctionPointer.FromIntPtr(IntPtr ptr)
        {
            return new FunctionPointer<T>(ptr);
        }
    }
}
#endif