using System;

namespace UnityEngine.Build.Pipeline
{
    /// <summary>
    /// Struct containing detailed information about a built asset bundle
    /// </summary>
    [Serializable]
    public struct BundleDetails : IEquatable<BundleDetails>
    {
        [SerializeField]
        string m_FileName;

        [SerializeField]
        uint m_Crc;

        [SerializeField]
        Hash128 m_Hash;

        [SerializeField]
        string[] m_Dependencies;

        /// <summary>
        /// Specific file name on disk of the asset bundle.
        /// </summary>
        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        /// <summary>
        /// Cyclic redundancy check of the content contained inside of the asset bundle.
        /// This value will not change between identical asset bundles with different compression options.
        /// </summary>
        public uint Crc
        {
            get { return m_Crc; }
            set { m_Crc = value; }
        }

        /// <summary>
        /// The hash version of the content contained inside of the asset bundle.
        /// This value will not change between identical asset bundles with different compression options.
        /// </summary>
        public Hash128 Hash
        {
            get { return m_Hash; }
            set { m_Hash = value; }
        }

        /// <summary>
        /// The array of all dependent asset bundles for this asset bundle.
        /// </summary>
        public string[] Dependencies
        {
            get { return m_Dependencies; }
            set { m_Dependencies = value; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is BundleDetails && Equals((BundleDetails)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (FileName != null ? FileName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)Crc;
                hashCode = (hashCode * 397) ^ Hash.GetHashCode();
                hashCode = (hashCode * 397) ^ (Dependencies != null ? Dependencies.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(BundleDetails a, BundleDetails b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BundleDetails a, BundleDetails b)
        {
            return !(a == b);
        }

        public bool Equals(BundleDetails other)
        {
            return string.Equals(FileName, other.FileName) && Crc == other.Crc && Hash.Equals(other.Hash) && Equals(Dependencies, other.Dependencies);
        }
    }
}