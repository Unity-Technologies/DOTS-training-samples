using System;
using UnityEngine;

namespace UnityEditor.Build.Pipeline.Utilities
{
    interface ICachedData { }

    [Serializable]
    public class CachedInfo : ICachedData
    {
        public CacheEntry Asset { get; set; }
        public CacheEntry[] Dependencies { get; set; }
        public object[] Data { get; set; }
    }

    [Serializable]
    public struct CacheEntry : IEquatable<CacheEntry>
    {
        public enum EntryType
        {
            Asset,
            File,
            Data
        }

        public Hash128 Hash { get; internal set; }
        public GUID Guid { get; internal set; }
        public int Version { get; internal set; }
        public EntryType Type { get; internal set; }
        public string File { get; internal set; }

        public bool IsValid()
        {
            return Hash.isValid && !Guid.Empty();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is CacheEntry && Equals((CacheEntry)obj);
        }

        public static bool operator ==(CacheEntry x, CacheEntry y)
        {
            return x.Hash == y.Hash && x.Guid == y.Guid;
        }

        public static bool operator !=(CacheEntry x, CacheEntry y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Hash.GetHashCode();
                hashCode = (hashCode * 397) ^ Guid.GetHashCode();
                hashCode = (hashCode * 397) ^ Version;
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ (File != null ? File.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            if (Type == EntryType.File)
                return string.Format("{{{0}, {1}}}", File, Hash);
            return string.Format("{{{0}, {1}}}", Guid, Hash);
        }

        public bool Equals(CacheEntry other)
        {
            return Hash.Equals(other.Hash) && Guid.Equals(other.Guid) && Version == other.Version && Type == other.Type && string.Equals(File, other.File);
        }
    }
}
