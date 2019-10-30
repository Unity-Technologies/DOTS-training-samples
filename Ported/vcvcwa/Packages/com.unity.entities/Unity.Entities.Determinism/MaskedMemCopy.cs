using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Mathematics;

namespace Unity.Entities.Determinism
{
    internal static class MaskedMemCopy
    {
        public static NativeArray<T> CreateMaskedBuffer<T>(NativeArray<T> data, NativeArray<byte> mask, Allocator allocator ) where T : struct
        {
            var result = NativeArrayUtility.CreateCopy(data, allocator);
            ApplyMask(result, mask);
            return result;
        }

        public static void ApplyMask<T>(NativeArray<T> target, NativeArray<byte> mask) where T : struct
        {
            ApplyMask(NativeViewUtility.GetWriteView(target), NativeViewUtility.GetReadView(mask));
        }

        internal static void ApplyMask(NativeView target, NativeView mask)
        {
            CheckMaskAndThrow(mask);
            
            unsafe
            {
                var targetLength = target.LengthInBytes;
                var targetBlockCount = targetLength >> 4;
                var maskBlockCount = mask.LengthInBytes >> 4;

                var targetBlocks = (uint4*) target.Ptr;
                var mskBlocks = (uint4*) mask.Ptr;

                var remainders = targetLength & 0xf;
                var dstOffset = target.Ptr + (targetLength - remainders);
                
                var mskPtr = (byte*)mskBlocks;
                if (1 == maskBlockCount) // for types with common size 1, 2, 4, 8 and 16
                {
                    var maskBlock = *mskBlocks;
                    for (int i = 0; i < targetBlockCount; i++)
                    {
                        var targetBlock = *targetBlocks;
                        *(targetBlocks++) = targetBlock & maskBlock;
                    }
                }
                else
                {
                    var n = 0;
                    for (int i = 0; i < targetBlockCount; i++)
                    {
                        var maskBlock = *((uint4*)mskPtr);
                        var targetBlock = *targetBlocks;

                        *(targetBlocks++) = targetBlock & maskBlock;
                        
                        n = math.select(0, ++n, n < maskBlockCount);
                        mskPtr = (byte*)(mskBlocks + n);
                    }
                }
                
                for (int i = 0; i < remainders; i++)
                {
                    var masked = (byte) ((*dstOffset) & (*(mskPtr + i)));
                    *(dstOffset++) = masked;
                }
            }    
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void CheckMaskAndThrow(NativeView mask)
        {
            if (mask.IsEmpty)
            {
                throw new ArgumentException("Provided mask does not contain data.");
            }

            if ((mask.LengthInBytes & 0xf) != 0)
            {
                throw new ArgumentException("Mask size must be a multiple of 16.");
            }
        }
    }

}
