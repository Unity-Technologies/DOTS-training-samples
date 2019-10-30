using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities.Serialization;

[assembly: InternalsVisibleTo("Unity.Entities.Tests")]

namespace Unity.Entities
{
    public static class FastEquality
    {
#if !NET_DOTS
        internal static TypeInfo CreateTypeInfo<T>() where T : struct
        {
            if (TypeUsesDelegates(typeof(T)))
                return CreateManagedTypeInfo(typeof(T));
            else
                return CreateTypeInfoBlittable(typeof(T));
        }

        internal static TypeInfo CreateTypeInfo(Type type)
        {
            if (TypeUsesDelegates(type))
                return CreateManagedTypeInfo(type);
            else
                return CreateTypeInfoBlittable(type);
        }

        private struct Dummy : IEquatable<Dummy>
        {
            public bool Equals(Dummy other)
            {
                return true;
            }
            public override int GetHashCode()
            {
                return 0;
            }
        }

        private struct CompareImpl<T> where T : struct, IEquatable<T>
        {
            public static unsafe bool CompareFunc(void* lhs, void* rhs)
            {
                return UnsafeUtilityEx.AsRef<T>(lhs).Equals(UnsafeUtilityEx.AsRef<T>(rhs));
            }
        }

        private struct GetHashCodeImpl<T> where T : struct, IEquatable<T>
        {
            public static unsafe int GetHashCodeFunc(void* lhs)
            {
                return UnsafeUtilityEx.AsRef<T>(lhs).GetHashCode();
            }
        }

        private struct ManagedCompareImpl<T> where T : IEquatable<T>
        {
            public static unsafe bool CompareFunc(object lhs, object rhs)
            {
                return ((T)lhs).Equals((T)rhs);
            }
        }

        private struct ManagedGetHashCodeImpl<T>
        {
            public static unsafe int GetHashCodeFunc(object val)
            {
                return ((T)val).GetHashCode();
            }
        }

        private unsafe static TypeInfo CreateManagedTypeInfo(Type t)
        {
            // ISharedComponentData Type must implement IEquatable<T> 
            if (typeof(ISharedComponentData).IsAssignableFrom(t))
            {
                if (!typeof(IEquatable<>).MakeGenericType(t).IsAssignableFrom(t))
                {
                    throw new ArgumentException($"type {t} is a ISharedComponentData and has managed references, you must implement IEquatable<T>");
                }

                // Type must override GetHashCode()
                var ghcMethod = t.GetMethod(nameof(GetHashCode));
                if (ghcMethod.DeclaringType != t)
                {
                    throw new ArgumentException($"type {t} is a/has managed references or implements IEquatable<T>, you must also override GetHashCode()");
                }
            }

            MethodInfo equalsFn = null;
            MethodInfo getHashFn = null;

            if (t.IsClass) 
            {
                if (typeof(IEquatable<>).MakeGenericType(t).IsAssignableFrom(t))
                    equalsFn = typeof(ManagedCompareImpl<>).MakeGenericType(t).GetMethod(nameof(ManagedCompareImpl<Dummy>.CompareFunc));

                var ghcMethod = t.GetMethod(nameof(GetHashCode));
                if (ghcMethod.DeclaringType == t)
                    getHashFn = typeof(ManagedGetHashCodeImpl<>).MakeGenericType(t).GetMethod(nameof(ManagedGetHashCodeImpl<Dummy>.GetHashCodeFunc));

                return new TypeInfo
                {
                    Layouts = null,
                    EqualFn = equalsFn != null ? Delegate.CreateDelegate(typeof(TypeInfo.ManagedCompareEqualDelegate), equalsFn) : null,
                    GetHashFn = getHashFn != null ? Delegate.CreateDelegate(typeof(TypeInfo.ManagedGetHashCodeDelegate), getHashFn) : null,
                    Hash = 1, // non-zero so out TypeInfo isn't equal to TypeInfo.Null
                };
            }
            else
            {
                equalsFn = typeof(CompareImpl<>).MakeGenericType(t).GetMethod(nameof(CompareImpl<Dummy>.CompareFunc));
                getHashFn = typeof(GetHashCodeImpl<>).MakeGenericType(t).GetMethod(nameof(GetHashCodeImpl<Dummy>.GetHashCodeFunc));

                return new TypeInfo
                {
                    Layouts = null,
                    EqualFn = Delegate.CreateDelegate(typeof(TypeInfo.CompareEqualDelegate), equalsFn),
                    GetHashFn = Delegate.CreateDelegate(typeof(TypeInfo.GetHashCodeDelegate), getHashFn),
                    Hash = 0,
                };
            }
        }

        private static TypeInfo CreateTypeInfoBlittable(Type type)
        {
            var begin = 0;
            var end = 0;
            var hash = 0;

            var layouts = new List<Layout>();

            CreateLayoutRecurse(type, 0, layouts, ref begin, ref end, ref hash);

            if (begin != end)
                layouts.Add(new Layout {offset = begin, count = end - begin, Aligned4 = false});

            var layoutsArray = layouts.ToArray();

            for (var i = 0; i != layoutsArray.Length; i++)
                if (layoutsArray[i].count % 4 == 0 && layoutsArray[i].offset % 4 == 0)
                {
                    layoutsArray[i].count /= 4;
                    layoutsArray[i].Aligned4 = true;
                }

            return new TypeInfo { Layouts = layoutsArray, Hash = hash };
        }
#endif

        public struct Layout
        {
            public int offset;
            public int count;
            public bool Aligned4;

            public override string ToString()
            {
                return $"offset: {offset} count: {count} Aligned4: {Aligned4}";
            }
        }

        public struct TypeInfo
        {
#if !NET_DOTS
            public unsafe delegate bool CompareEqualDelegate(void* lhs, void* rhs);
            public unsafe delegate int GetHashCodeDelegate(void* obj);
            public unsafe delegate bool ManagedCompareEqualDelegate(object lhs, object rhs);
            public unsafe delegate int ManagedGetHashCodeDelegate(object obj);
#endif

            public Layout[] Layouts;
            public int Hash;
#if !NET_DOTS
            public Delegate EqualFn;
            public Delegate GetHashFn;
#endif

            public static TypeInfo Null => new TypeInfo();
        }

        private unsafe struct PointerSize
        {
            private void* pter;
        }
#if !NET_DOTS
        struct FieldData
        {
            public int Offset;
            public FieldInfo Field;
        }

        static FixedBufferAttribute GetFixedBufferAttribute(FieldInfo field)
        {
            foreach (var attribute in field.GetCustomAttributes(typeof(FixedBufferAttribute)))
            {
                return (FixedBufferAttribute)attribute;
            }

            return null;
        }

        static void CombineHash(ref int hash, params int[] values)
        {
            foreach (var value in values)
            {
                hash *= FNV_32_PRIME;
                hash ^= value;
            }
        }

        private static void CreateLayoutRecurse(Type type, int baseOffset, List<Layout> layouts, ref int begin,
            ref int end, ref int typeHash)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var fieldsWithOffset = new FieldData[fields.Length];
            for (int i = 0; i != fields.Length; i++)
            {
                fieldsWithOffset[i].Offset = UnsafeUtility.GetFieldOffset(fields[i]);
                fieldsWithOffset[i].Field = fields[i];
            }

            Array.Sort(fieldsWithOffset, (a, b) => a.Offset - b.Offset);

            foreach (var fieldWithOffset in fieldsWithOffset)
            {
                var field = fieldWithOffset.Field;
                var fixedBuffer = GetFixedBufferAttribute(field);
                var offset = baseOffset + fieldWithOffset.Offset;

                if (fixedBuffer != null)
                {
                    var stride = UnsafeUtility.SizeOf(fixedBuffer.ElementType);
                    for (int i = 0; i < fixedBuffer.Length; ++i)
                    {
                        CreateLayoutRecurse(fixedBuffer.ElementType, offset + i * stride, layouts, ref begin, ref end, ref typeHash);
                    }
                }
                else if (field.FieldType.IsPrimitive || field.FieldType.IsPointer || field.FieldType.IsClass || field.FieldType.IsEnum)
                {
                    CombineHash(ref typeHash, offset, (int)Type.GetTypeCode(field.FieldType));

                    var sizeOf = -1;
                    if (field.FieldType.IsPointer || field.FieldType.IsClass)
                        sizeOf = UnsafeUtility.SizeOf<PointerSize>();
                    else if (field.FieldType.IsEnum)
                    {
                        //@TODO: Workaround IL2CPP bug
                        // sizeOf = UnsafeUtility.SizeOf(field.FieldType);
                        sizeOf = UnsafeUtility.SizeOf(field.FieldType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0].FieldType);
                    }
                    else
                        sizeOf = UnsafeUtility.SizeOf(field.FieldType);

                    if (end != offset)
                    {
                        layouts.Add(new Layout {offset = begin, count = end - begin, Aligned4 = false });
                        begin = offset;
                        end = offset + sizeOf;
                    }
                    else
                    {
                        end += sizeOf;
                    }
                }
                else
                {
                    CreateLayoutRecurse(field.FieldType, offset, layouts, ref begin, ref end, ref typeHash);
                }
            }
        }
#endif
        private const int FNV_32_PRIME = 0x01000193;

        //@TODO: Encode type in hashcode...

#if !NET_DOTS
        public static unsafe int ManagedGetHashCode(object lhs, TypeInfo typeInfo)
        {
			var fn = (TypeInfo.ManagedGetHashCodeDelegate)typeInfo.GetHashFn;
			if(fn != null)
				return fn(lhs);

            int hash = 0;
            using (var buffer = new UnsafeAppendBuffer(16, 16, Allocator.Temp))
            {
                var writer = new PropertiesBinaryWriter(&buffer);
                BoxedProperties.WriteBoxedType(lhs, writer);

                var remainder = buffer.Size & (sizeof(int) - 1);
                var alignedSize = buffer.Size - remainder;
                var bufferPtrAtEndOfAlignedData = buffer.Ptr + alignedSize;
                for (int i = 0; i < alignedSize; i += sizeof(int))
                {
                    hash *= FNV_32_PRIME;
                    hash ^= *(int*)(buffer.Ptr + i);
                }
                for (var i = 0; i < remainder; i++)
                {
                    hash *= FNV_32_PRIME;
                    hash ^= *(byte*)(bufferPtrAtEndOfAlignedData + i);
                }
                foreach(var obj in writer.GetObjectTable())
                {
                    hash *= FNV_32_PRIME;
                    hash ^= obj.GetHashCode();
                }
            }

            return hash;
        }
#endif

        public static unsafe int GetHashCode<T>(T lhs, TypeInfo typeInfo) where T : struct
        {
            return GetHashCode(UnsafeUtility.AddressOf(ref lhs), typeInfo);
        }

        public static unsafe int GetHashCode<T>(ref T lhs, TypeInfo typeInfo) where T : struct
        {
            return GetHashCode(UnsafeUtility.AddressOf(ref lhs), typeInfo);
        }

        public static unsafe int GetHashCode(void* dataPtr, TypeInfo typeInfo)
        {
#if !NET_DOTS
            if (typeInfo.GetHashFn != null)
            {
                TypeInfo.GetHashCodeDelegate fn = (TypeInfo.GetHashCodeDelegate)typeInfo.GetHashFn;
                return fn(dataPtr);
            }
#endif

            var layout = typeInfo.Layouts;
            var data = (byte*) dataPtr;
            uint hash = 0;

            for (var k = 0; k != layout.Length; k++)
                if (layout[k].Aligned4)
                {
                    var dataInt = (uint*) (data + layout[k].offset);
                    var count = layout[k].count;
                    for (var i = 0; i != count; i++)
                    {
                        hash *= FNV_32_PRIME;
                        hash ^= dataInt[i];
                    }
                }
                else
                {
                    var dataByte = data + layout[k].offset;
                    var count = layout[k].count;
                    for (var i = 0; i != count; i++)
                    {
                        hash *= FNV_32_PRIME;
                        hash ^= dataByte[i];
                    }
                }

            return (int) hash;
        }

#if !NET_DOTS
        public static unsafe bool ManagedEquals(object lhs, object rhs, TypeInfo typeInfo)
        {
			var fn = (TypeInfo.ManagedCompareEqualDelegate) typeInfo.EqualFn;
			
			if(fn != null)
				return fn(lhs, rhs);
				
            using (var bufferLHS = new UnsafeAppendBuffer(512, 16, Allocator.Temp))
            using (var bufferRHS = new UnsafeAppendBuffer(512, 16, Allocator.Temp))
            {
                var writerLHS = new PropertiesBinaryWriter(&bufferLHS);
                BoxedProperties.WriteBoxedType(lhs, writerLHS);
                var writerRHS = new PropertiesBinaryWriter(&bufferRHS);
                BoxedProperties.WriteBoxedType(rhs, writerRHS);

                if (UnsafeUtility.MemCmp(bufferLHS.Ptr, bufferRHS.Ptr, bufferLHS.Size) != 0)
                    return false;

                var objectTableLHS = writerLHS.GetObjectTable();
                var objectTableRHS = writerRHS.GetObjectTable(); 
                Assertions.Assert.AreEqual(objectTableLHS.Length, objectTableRHS.Length);

                for (int i = 0; i < objectTableLHS.Length; ++i)
                    if (!objectTableLHS[i].Equals(objectTableRHS[i]))
                        return false;
            }

            return true;
        }
#endif

        public static unsafe bool Equals<T>(T lhs, T rhs, TypeInfo typeInfo) where T : struct
        {
            return Equals(UnsafeUtility.AddressOf(ref lhs), UnsafeUtility.AddressOf(ref rhs), typeInfo);
        }

        public static unsafe bool Equals<T>(ref T lhs, ref T rhs, TypeInfo typeInfo) where T : struct
        {
            return Equals(UnsafeUtility.AddressOf(ref lhs), UnsafeUtility.AddressOf(ref rhs), typeInfo);
        }

        public static unsafe bool Equals(void* lhsPtr, void* rhsPtr, TypeInfo typeInfo)
        {
#if !NET_DOTS
            if (typeInfo.EqualFn != null)
            {
                var fn = (TypeInfo.CompareEqualDelegate) typeInfo.EqualFn;
                return fn(lhsPtr, rhsPtr);
            }
#endif

            var layout = typeInfo.Layouts;
            var lhs = (byte*) lhsPtr;
            var rhs = (byte*) rhsPtr;

            var same = true;

            for (var k = 0; k != layout.Length; k++)
                if (layout[k].Aligned4)
                {
                    var offset = layout[k].offset;
                    var lhsInt = (uint*) (lhs + offset);
                    var rhsInt = (uint*) (rhs + offset);
                    var count = layout[k].count;
                    for (var i = 0; i != count; i++)
                        same &= lhsInt[i] == rhsInt[i];
                }
                else
                {
                    var offset = layout[k].offset;
                    var lhsByte = lhs + offset;
                    var rhsByte = rhs + offset;
                    var count = layout[k].count;
                    for (var i = 0; i != count; i++)
                        same &= lhsByte[i] == rhsByte[i];
                }

            return same;
        }

#if !NET_DOTS
        private static bool TypeUsesDelegates(Type t)
        {
#if !UNITY_DISABLE_MANAGED_COMPONENTS
            // We have custom delegates to allow for class IComponentData comparisons
            // but any other non-value type should be ignored
            if (t.IsClass && typeof(IComponentData).IsAssignableFrom(t))
                return true;
#endif
            if (!t.IsValueType)
                return false;

            // Things with managed references must use delegate comparison.
            if (!UnsafeUtility.IsUnmanaged(t))
                return true;

            return typeof(IEquatable<>).MakeGenericType(t).IsAssignableFrom(t);
        }

        public static void AddExtraAOTTypes(Type type, HashSet<String> output)
        {
            if (!TypeUsesDelegates(type))
                return;

            if (type.IsClass)
            {
                if (typeof(IEquatable<>).MakeGenericType(type).IsAssignableFrom(type))
                    output.Add(typeof(ManagedCompareImpl<>).MakeGenericType(type).ToString());

                var ghcMethod = type.GetMethod(nameof(GetHashCode));
                if (ghcMethod.DeclaringType == type)
                    output.Add(typeof(ManagedGetHashCodeImpl<>).MakeGenericType(type).ToString());
            }
            else
            {
                output.Add(typeof(CompareImpl<>).MakeGenericType(type).ToString());
                output.Add(typeof(GetHashCodeImpl<>).MakeGenericType(type).ToString());
            }
        }
#endif
        }
    }
