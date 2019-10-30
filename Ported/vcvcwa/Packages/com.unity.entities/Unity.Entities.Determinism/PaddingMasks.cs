using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities.Determinism
{
    internal struct PaddingMasks : IDisposable
    {
        public NativeArray<byte> GetTypeMask<T>() => GetTypeMask(TypeManager.GetTypeIndex<T>());
        public NativeArray<byte> GetTypeMask(TypeManager.TypeInfo type) => GetTypeMask(type.TypeIndex);
        public NativeArray<byte> GetTypeMask(ComponentType type) => GetTypeMask(type.TypeIndex);

        public bool IsMasked<T>() where T : struct => IsMasked(TypeManager.GetTypeIndex<T>());
        public bool IsMasked(TypeManager.TypeInfo type) => IsMasked(type.TypeIndex);
        public bool IsMasked(ComponentType type) => IsMasked(type.TypeIndex);

        public bool IsCreated => (!NativeArrayUtility.IsInvalidOrEmpty(MaskBuffer)) 
                              && (!NativeArrayUtility.IsInvalidOrEmpty(MaskViews));

        internal NativeView GetMaskView(ComponentType type) => GetMaskView(type.TypeIndex);
        internal NativeView GetMaskView<T>() where T : struct => GetMaskView(TypeManager.GetTypeIndex<T>());
        internal NativeView GetMaskView(TypeManager.TypeInfo type) => GetMaskView(type.TypeIndex);
        
        internal NativeView GetMaskView(int typeIndex)
        {
            AssertMaskCreated();
            return MaskViews[typeIndex & TypeManager.ClearFlagsMask];
        }
        
        internal NativeArray<byte> GetTypeMask(int typeIndex)
        {
            var view = GetMaskView(typeIndex);
            unsafe
            {
                var result = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>
                (
                    view.Ptr,
                    view.LengthInBytes,
                    Allocator.None
                );
            #if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref result, NativeArrayUnsafeUtility.GetAtomicSafetyHandle(MaskBuffer));
            #endif
                return result;
            }
        }

        internal bool IsMasked(int index)
        {
            AssertMaskCreated();
            return MaskViews[index & TypeManager.ClearFlagsMask].LengthInBytes > 0;
        }

        public void Dispose()
        {
            if( MaskBuffer.IsCreated ) MaskBuffer.Dispose();
            if( MaskViews.IsCreated )  MaskViews.Dispose();
        }
        
        internal NativeArray<byte> MaskBuffer;
        internal NativeArray<NativeView> MaskViews;

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void AssertMaskCreated()
        {
            if (!IsCreated)
            {
                throw new InvalidOperationException("Padding masks do not have any content.");
            }
        }

    }
    
    internal static class PaddingMaskBuilder
    {
        const byte kMaskedByte = 0xff;
        const int kAlignment = 16;

        struct MaskDescriptor
        {
            public int MaskOffset;
            public int LengthInBytes;
        }
        
        internal static PaddingMasks BuildPaddingMasks(TypeManager.TypeInfo[] types, Allocator allocator)
        {
            var typeCount = types.Length;
            if (0 == types.Length)
            {
                return default;
            }
            
            var descriptors = new NativeArray<MaskDescriptor>(typeCount, Allocator.Temp);
            var totalMaskLength = 0;
            
            try
            {
                foreach (var type in types)
                {
                    if (type.IsZeroSized)
                    {
                        continue;
                    }

                    var index = type.TypeIndex & TypeManager.ClearFlagsMask;

                    var requiredMaskLength = GetAlignedTypeMaskSize(type.ElementSize, kAlignment);
                    descriptors[index] = new MaskDescriptor
                    {
                        MaskOffset = totalMaskLength,
                        LengthInBytes = requiredMaskLength
                    };

                    totalMaskLength += requiredMaskLength;
                }
            }
            catch (Exception e)
            {
                if (descriptors.IsCreated)
                {
                    descriptors.Dispose();
                }
            #if !NET_DOTS
                UnityEngine.Debug.LogException(e);
            #endif
                throw;
            }

            PaddingMasks masks = default;
            try
            {
                masks = new PaddingMasks
                {
                    MaskBuffer = new NativeArray<byte>(totalMaskLength, allocator),
                    MaskViews = new NativeArray<NativeView>(typeCount, allocator)
                };

                unsafe
                {
                    using (descriptors)
                    {
                        PatchMaskViewPointerOffsets(masks.MaskViews, descriptors, (byte*)masks.MaskBuffer.GetUnsafeReadOnlyPtr());
                    }
                }
                
                foreach (var type in types)
                {
                    if (type.IsZeroSized)
                    {
                        continue;
                    }

                    WriteTypeMask(type, masks.GetMaskView(type), kAlignment);
                }

                return masks;
            }
            catch (Exception e)
            {
                masks.Dispose();
            #if !NET_DOTS
                UnityEngine.Debug.LogException(e);
            #endif
                throw;
            }
        }

        static unsafe void PatchMaskViewPointerOffsets(NativeArray<NativeView> masksMaskViews, NativeArray<MaskDescriptor> descriptors, byte* basePtr)
        {
            for (int i = 0; i < masksMaskViews.Length; ++i)
            {
                var descriptor = descriptors[i];
                masksMaskViews[i] = new NativeView(basePtr + descriptor.MaskOffset, descriptor.LengthInBytes);
            }
        }

        static void WriteTypeMask(TypeManager.TypeInfo type, NativeView mask, int alignment)
        {
            CheckTypeAndThrow(type);

            var typeSize = type.ElementSize;
            var maskSize = GetAlignedTypeMaskSize(typeSize, alignment);

            var typeInfo = TypeManager.GetFastEqualityTypeInfo(type);
            var layout = typeInfo.Layouts ?? FastEquality.CreateTypeInfo(type.Type).Layouts ?? GetFullMaskedLayout(typeSize);

            foreach (var field in layout)
            {
                var fieldOffset = field.offset;
                var length = field.Aligned4 ? field.count * 4 : field.count;

                for (int i = 0; i < length; i++)
                {
                    NativeViewUtility.WriteByte(mask, fieldOffset + i, kMaskedByte);
                }
            }

            for (int i = typeSize; i < maskSize; i++)
            {
                NativeViewUtility.WriteByte(mask, i, NativeViewUtility.ReadByte(mask, i % typeSize));
            }
        }

        static FastEquality.Layout[] GetFullMaskedLayout(int typeSize)
        {
            return new[]
            {
                new FastEquality.Layout
                {
                    offset = 0,
                    Aligned4 = 0 == ( typeSize & 3 ),
                    count = typeSize >> (0 == (typeSize & 3) ? 2 : 0 )
                }
            };
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void CheckTypeAndThrow(TypeManager.TypeInfo type)
        {
            var index = type.TypeIndex & TypeManager.ClearFlagsMask;

            if (index < 1)
            {
                throw new ArgumentException("Invalid type index, index must be > 0");
            }

            if ((0 == type.ElementSize) || (type.IsZeroSized) || TypeManager.IsZeroSized(index))
            {
                throw new ArgumentException("Can not create mask for zero-sized type.");
            }
        }

        static int GetAlignedTypeMaskSize(int typeSize, int alignment)
        {
            var lowestBitInSize = typeSize & (-typeSize);
            return lowestBitInSize >= alignment ? typeSize : (typeSize / lowestBitInSize) * alignment;
        }
    }
}
