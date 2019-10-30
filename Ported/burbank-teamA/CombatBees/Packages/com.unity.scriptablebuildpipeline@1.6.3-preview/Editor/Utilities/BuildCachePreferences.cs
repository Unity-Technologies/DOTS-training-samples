using UnityEngine;
using System.Threading;
using System.Collections.Generic;

namespace UnityEditor.Build.Pipeline.Utilities
{
    static class BuildCachePreferences
    {
        internal class GUIScope : GUI.Scope
        {
            float m_LabelWidth;
            public GUIScope(float layoutMaxWidth)
            {
                m_LabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 200;
                GUILayout.BeginHorizontal(GUILayout.MaxWidth(layoutMaxWidth));
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                GUILayout.Space(15);
            }

            public GUIScope() : this(500)
            {
            }

            protected override void CloseScope()
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = m_LabelWidth;
            }
        }

        internal class BuildCacheProperties
        {
            public static readonly GUIContent maxCacheSize = EditorGUIUtility.TrTextContent("Maximum Cache Size (GB)", "The size of the Build Cache folder will be kept below this maximum value when possible.");
            public static readonly GUIContent purgeCache = EditorGUIUtility.TrTextContent("Purge Cache");
            public static readonly GUIContent pruneCache = EditorGUIUtility.TrTextContent("Prune Cache");
            public static readonly GUIContent cacheSizeIs = EditorGUIUtility.TrTextContent("Cache size is");
            public static readonly GUIContent pleaseWait = EditorGUIUtility.TrTextContent("Please wait...");
            public static bool startedCalculation = false;
            public static long currentCacheSize = -1;
        }

#if UNITY_2019_1_OR_NEWER
        [SettingsProvider]
        static SettingsProvider CreateBuildCacheProvider()
        {
            var provider = new SettingsProvider("Preferences/Build Cache", SettingsScope.User, SettingsProvider.GetSearchKeywordsFromGUIContentProperties<BuildCacheProperties>());
            provider.guiHandler = sarchContext => OnGUI();
            return provider;
        }
#else
        [PreferenceItem("Build Cache")]
#endif
        static void OnGUI()
        {
            using (new GUIScope())
                DrawProperties();
        }

        static void DrawProperties()
        {
            // Show Gigabytes to the user.
            const int kMinSizeInGigabytes = 1;
            const int kMaxSizeInGigabytes = 200;

            // Write size in GigaBytes.
            int maximumSize = EditorPrefs.GetInt("BuildCache.maximumSize", 200);
            int newMaximumSize = EditorGUILayout.IntSlider(BuildCacheProperties.maxCacheSize, maximumSize, kMinSizeInGigabytes, kMaxSizeInGigabytes);
            if (maximumSize != newMaximumSize)
                EditorPrefs.SetInt("BuildCache.maximumSize", newMaximumSize);

            GUILayout.BeginHorizontal(GUILayout.MaxWidth(500));
            if (GUILayout.Button(BuildCacheProperties.purgeCache, GUILayout.Width(120)))
            {
                BuildCache.PurgeCache(true);
                BuildCacheProperties.startedCalculation = false;
            }

            if (GUILayout.Button(BuildCacheProperties.pruneCache, GUILayout.Width(120)))
            {
                BuildCache.PruneCache();
                BuildCacheProperties.startedCalculation = false;
            }
            GUILayout.EndHorizontal();

            // Current cache size
            if (!BuildCacheProperties.startedCalculation)
            {
                BuildCacheProperties.startedCalculation = true;
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    BuildCache.ComputeCacheSizeAndFolders(out BuildCacheProperties.currentCacheSize, out List<BuildCache.CacheFolder> cacheFolders);
                });
            }

            if (BuildCacheProperties.currentCacheSize >= 0)
                GUILayout.Label(BuildCacheProperties.cacheSizeIs.text + " " + EditorUtility.FormatBytes(BuildCacheProperties.currentCacheSize));
            else
                GUILayout.Label(BuildCacheProperties.cacheSizeIs.text + " is being calculated...");
        }
    }
}
