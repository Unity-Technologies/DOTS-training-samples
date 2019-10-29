using System;
using System.ComponentModel;

namespace Unity.Entities
{
    public struct ExcludeComponent<T>
    {
    }

    public partial struct ComponentType : IEquatable<ComponentType>
    {
        public enum AccessMode
        {
            ReadWrite,
            ReadOnly,
            Exclude
        }

        public int TypeIndex;
        public AccessMode AccessModeType;

        public bool IsBuffer => (TypeIndex & TypeManager.BufferComponentTypeFlag) != 0;
        public bool IsSystemStateComponent => (TypeIndex & TypeManager.SystemStateTypeFlag) != 0;
        public bool IsSystemStateSharedComponent => (TypeIndex & TypeManager.SystemStateSharedComponentTypeFlag) == TypeManager.SystemStateSharedComponentTypeFlag;
        public bool IsSharedComponent => (TypeIndex & TypeManager.SharedComponentTypeFlag) != 0;
        public bool IsZeroSized => (TypeIndex & TypeManager.ZeroSizeInChunkTypeFlag) != 0;
        public bool IsChunkComponent => (TypeIndex & TypeManager.ChunkComponentTypeFlag) != 0;
        public bool HasEntityReferences => (TypeIndex & TypeManager.HasNoEntityReferencesFlag) == 0;

        public bool IgnoreDuplicateAdd => TypeManager.IgnoreDuplicateAdd(TypeIndex);

        [EditorBrowsable(EditorBrowsableState.Never)]
        #if UNITY_2019_3_OR_NEWER
        [Obsolete("Create<T> has been renamed. Use ReadWrite<T> instead. (RemovedAfter 2019-08-25) (UnityUpgradable) -> ReadWrite<T>()", false)]
        #else
        [Obsolete("Create<T> has been renamed. Use ReadWrite<T> instead. (RemovedAfter 2019-08-25)", false)]
        #endif
        public static ComponentType Create<T>() => ReadWrite<T>();


        public static ComponentType ReadWrite<T>()
        {
            return FromTypeIndex(TypeManager.GetTypeIndex<T>());
        }

        public static ComponentType FromTypeIndex(int typeIndex)
        {
            ComponentType type;
            type.TypeIndex = typeIndex;
            type.AccessModeType = AccessMode.ReadWrite;
            return type;
        }

        public static ComponentType ReadOnly(Type type)
        {
            ComponentType t = FromTypeIndex(TypeManager.GetTypeIndex(type));
            t.AccessModeType = AccessMode.ReadOnly;
            return t;
        }

        public static ComponentType ReadOnly(int typeIndex)
        {
            ComponentType t = FromTypeIndex(typeIndex);
            t.AccessModeType = AccessMode.ReadOnly;
            return t;
        }

        public static ComponentType ReadOnly<T>()
        {
            ComponentType t = ReadWrite<T>();
//            ComponentType t = Create<T>();
            t.AccessModeType = AccessMode.ReadOnly;
            return t;
        }

        public static ComponentType ChunkComponent(Type type)
        {
            var typeIndex = TypeManager.MakeChunkComponentTypeIndex(TypeManager.GetTypeIndex(type));
            return FromTypeIndex(typeIndex);
        }

        public static ComponentType ChunkComponent<T>()
        {
            var typeIndex = TypeManager.MakeChunkComponentTypeIndex(TypeManager.GetTypeIndex<T>());
            return FromTypeIndex(typeIndex);
        }

        public static ComponentType ChunkComponentReadOnly<T>()
        {
            var typeIndex = TypeManager.MakeChunkComponentTypeIndex(TypeManager.GetTypeIndex<T>());
            return ReadOnly(typeIndex);
        }


        public static ComponentType Exclude(Type type)
        {
            ComponentType t = FromTypeIndex(TypeManager.GetTypeIndex(type));
            t.AccessModeType = AccessMode.Exclude;
            return t;
        }

        public static ComponentType Exclude<T>()
        {
            ComponentType t = ReadWrite<T>();
            t.AccessModeType = AccessMode.Exclude;
            return t;
        }

        public ComponentType(Type type, AccessMode accessModeType = AccessMode.ReadWrite)
        {
            TypeIndex = TypeManager.GetTypeIndex(type);
            AccessModeType = accessModeType;
        }

        public Type GetManagedType()
        {
            return TypeManager.GetType(TypeIndex);
        }

        public static implicit operator ComponentType(Type type)
        {
            return new ComponentType(type, AccessMode.ReadWrite);
        }

        public static bool operator <(ComponentType lhs, ComponentType rhs)
        {
            if (lhs.TypeIndex == rhs.TypeIndex)
                return lhs.AccessModeType < rhs.AccessModeType;

            return lhs.TypeIndex < rhs.TypeIndex;
        }

        public static bool operator >(ComponentType lhs, ComponentType rhs)
        {
            return rhs < lhs;
        }

        public static bool operator ==(ComponentType lhs, ComponentType rhs)
        {
            return lhs.TypeIndex == rhs.TypeIndex && lhs.AccessModeType == rhs.AccessModeType;
        }

        public static bool operator !=(ComponentType lhs, ComponentType rhs)
        {
            return lhs.TypeIndex != rhs.TypeIndex || lhs.AccessModeType != rhs.AccessModeType;
        }

        internal static unsafe bool CompareArray(ComponentType* type1, int typeCount1, ComponentType* type2,
            int typeCount2)
        {
            if (typeCount1 != typeCount2)
                return false;
            for (var i = 0; i < typeCount1; ++i)
                if (type1[i] != type2[i])
                    return false;
            return true;
        }


#if ENABLE_UNITY_COLLECTIONS_CHECKS
        public override string ToString()
        {
#if NET_DOTS
            var name = TypeManager.GetTypeInfo(TypeIndex).StableTypeHash.ToString();
#else
            var name = GetManagedType().Name;
            if (IsBuffer)
                return $"{name} [B]";
            if (AccessModeType == AccessMode.Exclude)
                return $"{name} [S]";
            if (AccessModeType == AccessMode.ReadOnly)
                return $"{name} [RO]";
            if (TypeIndex == 0)
                return "None";
#endif
            return name;
        }
#endif

        public bool Equals(ComponentType other)
        {
            return TypeIndex == other.TypeIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is ComponentType && (ComponentType) obj == this;
        }

        public override int GetHashCode()
        {
            return (TypeIndex * 5813);
        }
    }


    [Obsolete("SubtractiveComponent has been renamed. Use ExcludeComponent instead. (RemovedAfter 2019-08-25) (UnityUpgradable) -> ExcludeComponent<T>", true)]
    [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
    public struct SubtractiveComponent<T>
    {
    }

    public partial struct ComponentType
    {
        [Obsolete("ComponentType.Subtractive has been renamed. Use Exclude instead. (RemovedAfter 2019-08-25) (UnityUpgradable) -> Exclude(*)", true)]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType Subtractive(Type type)
        {
            return Exclude(type);
        }

        [Obsolete("ComponentType.Subtractive has been renamed. Use ExcludeComponent instead. (RemovedAfter 2019-08-25) (UnityUpgradable) -> Exclude<T>()", true)]
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType Subtractive<T>()
        {
            return Exclude<T>();
        }
    }

}
