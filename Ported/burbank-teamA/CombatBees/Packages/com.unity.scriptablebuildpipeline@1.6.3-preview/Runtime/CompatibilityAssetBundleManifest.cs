using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.Build.Pipeline
{
    [Serializable]
    public class CompatibilityAssetBundleManifest : ScriptableObject, ISerializationCallbackReceiver
    {
        Dictionary<string, BundleDetails> m_Details;

        [SerializeField]
        List<string> m_Keys;

        [SerializeField]
        List<BundleDetails> m_Values;

        public void SetResults(Dictionary<string, BundleDetails> results)
        {
            m_Details = new Dictionary<string, BundleDetails>(results);
        }

        public string[] GetAllAssetBundles()
        {
            string[] bundles = m_Details.Keys.ToArray();
            Array.Sort(bundles);
            return bundles;
        }

        public string[] GetAllAssetBundlesWithVariant()
        {
            return new string[0];
        }

        public Hash128 GetAssetBundleHash(string assetBundleName)
        {
            BundleDetails details;
            if (m_Details.TryGetValue(assetBundleName, out details))
                return details.Hash;
            return new Hash128();
        }

        public uint GetAssetBundleCrc(string assetBundleName)
        {
            BundleDetails details;
            if (m_Details.TryGetValue(assetBundleName, out details))
                return details.Crc;
            return 0;
        }

        public string[] GetDirectDependencies(string assetBundleName)
        {
            return GetAllDependencies(assetBundleName);
        }

        public string[] GetAllDependencies(string assetBundleName)
        {
            BundleDetails details;
            if (m_Details.TryGetValue(assetBundleName, out details))
                return details.Dependencies.ToArray();
            return new string[0];
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ManifestFileVersion: 1\n");
            builder.Append("CompatibilityAssetBundleManifest:\n");

            if (m_Details != null && m_Details.Count > 0)
            {
                builder.Append("  AssetBundleInfos:\n");
                int infoCount = 0;
                foreach (var details in m_Details)
                {
                    builder.AppendFormat("    Info_{0}:\n", infoCount++);
                    builder.AppendFormat("      Name: {0}\n", details.Key);
                    builder.AppendFormat("      Hash: {0}\n", details.Value.Hash);
                    builder.AppendFormat("      CRC: {0}\n", details.Value.Crc);
                    int dependencyCount = 0;
                    if (details.Value.Dependencies != null && details.Value.Dependencies.Length > 0)
                    {
                        builder.Append("      Dependencies: {}\n");
                        foreach (var dependency in details.Value.Dependencies)
                            builder.AppendFormat("        Dependency_{0}: {1}\n", dependencyCount++, dependency);
                    }
                    else
                        builder.Append("      Dependencies: {}\n");
                }
            }
            else
                builder.Append("  AssetBundleInfos: {}\n");

            return builder.ToString();
        }

        public void OnBeforeSerialize()
        {
            m_Keys = new List<string>();
            m_Values = new List<BundleDetails>();

            foreach (var pair in m_Details)
            {
                m_Keys.Add(pair.Key);
                m_Values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            m_Details = new Dictionary<string, BundleDetails>();
            for (int i = 0; i != Math.Min(m_Keys.Count, m_Values.Count); i++)
                m_Details.Add(m_Keys[i], m_Values[i]);
        }
    }
}