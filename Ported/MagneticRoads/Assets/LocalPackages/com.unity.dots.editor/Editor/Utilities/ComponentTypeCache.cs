using System;
using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    static class ComponentTypeCache
    {
        static readonly SortedSet<IndexedType> m_ComponentIndexedTypes = new SortedSet<IndexedType>();
        static readonly HashSet<NameType> m_ComponentNameTypes = new HashSet<NameType>();

        static ComponentTypeCache()
        {
            m_ComponentIndexedTypes.Clear();
            TypeManager.Initialize();
            foreach (var typeInfo in TypeManager.GetAllTypes())
            {
                if (typeInfo.Type == null)
                    continue;

                m_ComponentIndexedTypes.Add(new IndexedType(typeInfo.Type.Name.ToLowerInvariant().GetHashCode(), typeInfo.Type));
                m_ComponentIndexedTypes.Add(new IndexedType(typeInfo.Type.FullName.ToLowerInvariant().GetHashCode(), typeInfo.Type));
                m_ComponentNameTypes.Add(new NameType(typeInfo.Type.Name.ToLowerInvariant(), typeInfo.Type));
                m_ComponentNameTypes.Add(new NameType(typeInfo.Type.FullName.ToLowerInvariant(), typeInfo.Type));
            }
        }

        public struct IndexedType : IEquatable<IndexedType>, IComparable<IndexedType>
        {
            readonly int m_Hash;
            public readonly Type Type;

            public IndexedType(int hash, Type type)
            {
                m_Hash = hash;
                Type = type;
            }

            public static implicit operator IndexedType(int i)
                => new IndexedType(i, null);

            public bool Equals(IndexedType other)
                => m_Hash == other.m_Hash && Type == other.Type;

            public override bool Equals(object obj)
                => obj is IndexedType other && Equals(other);

            public override int GetHashCode() => m_Hash;

            public int CompareTo(IndexedType other)
            {
                var comparison = m_Hash.CompareTo(other.m_Hash);
                return comparison != 0 ? comparison : string.Compare(Type?.AssemblyQualifiedName ?? string.Empty, other.Type?.AssemblyQualifiedName ?? string.Empty, StringComparison.Ordinal);
            }
        }

        public struct NameType : IEquatable<NameType>
        {
            public readonly string Name;
            public readonly Type Type;

            public NameType(string name, Type type)
            {
                Name = name;
                Type = type;
            }

            public bool Equals(NameType other)
                => Name == other.Name && Type == other.Type;

            public override bool Equals(object obj)
                => obj is NameType other && Equals(other);

            public override int GetHashCode() => Name.GetHashCode();
        }

        public static IEnumerable<Type> GetExactMatchingTypes(string str)
        {
            var typeHash = str.ToLowerInvariant().GetHashCode();
            var indexedTypes = m_ComponentIndexedTypes.GetViewBetween(typeHash, typeHash + 1);
            foreach (var indexedType in indexedTypes)
            {
                yield return indexedType.Type;
            }
        }

        public static IEnumerable<Type> GetFuzzyMatchingTypes(string str)
        {
            var strLower = str.ToLowerInvariant();

            using (var hashPool = PooledHashSet<Type>.Make())
            {
                foreach (var nameType in m_ComponentNameTypes)
                {
                    if (nameType.Name.IndexOf(strLower) >= 0)
                    {
                        if (hashPool.Set.Add(nameType.Type))
                            yield return nameType.Type;
                    }
                }
            }
        }
    }
}
